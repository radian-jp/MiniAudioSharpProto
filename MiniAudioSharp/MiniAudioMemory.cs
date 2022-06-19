using MiniAudioSharp.Native;

namespace MiniAudioSharp
{
    public class MiniAudioMemory<T> : IDisposable where T : unmanaged
    {
        public IntPtr Pointer;
        protected Action<IntPtr>? DisposeAction { get; set; }

        public ref T AsRef()
        {
            unsafe
            {
                return ref new Span<T>((T*)Pointer, 1)[0];
            }
        }

        public unsafe MiniAudioMemory()
        {
        }

        public unsafe MiniAudioMemory(T* pointer) : this(pointer, (ptr) => MiniAudio.ma_free((void*)ptr, null))
        {
        }

        public unsafe MiniAudioMemory(T* pointer, Action<IntPtr>? disposeAction)
        {
            Pointer = (IntPtr)pointer;
            DisposeAction = disposeAction;
        }

        protected virtual void Dispose(bool disposing)
        {
            var pointer = Interlocked.Exchange(ref Pointer, IntPtr.Zero);
            if (pointer != IntPtr.Zero)
            {
                DisposeAction?.Invoke(pointer);
            }
        }

        ~MiniAudioMemory()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public unsafe static implicit operator T*(MiniAudioMemory<T> mem) => (T*)mem.Pointer;
    }
}
