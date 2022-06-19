using MiniAudioSharp.Native;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MiniAudioSharp
{
    public class MiniAudioDeviceConfig : MiniAudioMemory<ma_device_config>
    {
        internal unsafe MiniAudioDeviceConfig(ma_device_config* pConfig) : base(pConfig) { }

        public unsafe static MiniAudioDeviceConfig CreatePlayback(MiniAudioDecoder decoder, MiniAudioDataCallback dataCallback, void* pUserData)
            => new MiniAudioDeviceConfig(
                    MiniAudio.Helper.ma_helper_create_playback_config(
                        decoder,
                        dataCallback,
                        pUserData
                    )
                );
    }
}
