using MiniAudioSharp.Native;

namespace MiniAudioSharp
{
    public class MiniAudioDevice : MiniAudioMemory<ma_device>
    {
        internal unsafe MiniAudioDevice(ma_device* pDevice, Action<IntPtr>? disposeAction) : base(pDevice, disposeAction) { }

        public MiniAudioDevice(MiniAudioDeviceConfig deviceConfig) : base()
        {
            unsafe
            {
                var pDevice = MiniAudio.Helper.ma_helper_create_empty_device();
                var result = MiniAudio.ma_device_init(null, deviceConfig, pDevice);
                if (result != ma_result.MA_SUCCESS)
                {
                    MiniAudio.ma_free(pDevice, null);
                    throw new MiniAudioException(result);
                }
                Pointer = (IntPtr)pDevice;
            }

            DisposeAction = (ptr) =>
            {
                unsafe
                {
                    ma_device* pDevice = (ma_device*)ptr;
                    MiniAudio.ma_device_uninit(pDevice);
                    MiniAudio.ma_free(pDevice, null);
                }
            };
        }
    }
}
