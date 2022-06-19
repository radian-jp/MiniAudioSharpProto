using MiniAudioSharp.Native;

namespace MiniAudioSharp
{
    public class MiniAudioDecoder : MiniAudioMemory<ma_decoder>
    {
        internal unsafe MiniAudioDecoder(ma_decoder* pDecoder) : base(pDecoder) {}

        public static MiniAudioDecoder FromFile(string filePath)
        {
            unsafe
            {
                ma_decoder* pDecoder = MiniAudio.Helper.ma_helper_create_empty_decoder();
                ma_result result = OperatingSystem.IsWindows() ?
                    MiniAudio.ma_decoder_init_file_w(filePath, null, pDecoder) :
                    MiniAudio.ma_decoder_init_file(filePath, null, pDecoder);
                
                if (result != ma_result.MA_SUCCESS)
                {
                    MiniAudio.ma_free(pDecoder, null);
                    throw new MiniAudioException(result);
                }

                return new MiniAudioDecoder(pDecoder);
            }
        }

        protected override void Dispose(bool disposing)
        {
            var pointer = Interlocked.Exchange(ref Pointer, IntPtr.Zero);
            if (pointer != IntPtr.Zero)
            {
                unsafe
                {
                    MiniAudio.ma_decoder_uninit((ma_decoder*)pointer);
                    MiniAudio.ma_free((void*)pointer, null);
                }
            }
        }
    }
}
