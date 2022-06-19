using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using MiniAudioSharp;
using MiniAudioSharp.Native;

namespace MiniAudioTest
{
    internal class Program
    {
        static unsafe void data_callback(ma_device* pDevice, void* pOutput, void* pInput, uint frameCount)
        {
            MiniAudioDecoder decoder = Unsafe.AsRef<MiniAudioDecoder>(pDevice->pUserData);

            MiniAudio.ma_decoder_read_pcm_frames(decoder, pOutput, frameCount, null);
        }

        static unsafe int Main(string[] args)
        {
            if( args.Length == 0)
            {
                Console.WriteLine("No input file.");
                Console.ReadKey();
                return -1;
            }

            MiniAudio.Initialize();
            MiniAudioDecoder? decoder = null;
            try
            {
                decoder = MiniAudioDecoder.FromFile(args[0]);
                using (var deviceConfig = MiniAudioDeviceConfig.CreatePlayback(decoder, data_callback, Unsafe.AsPointer(ref decoder)))
                using (var device = new MiniAudioDevice(deviceConfig))
                {
                    if (MiniAudio.ma_device_start(device) != ma_result.MA_SUCCESS)
                    {
                        Console.WriteLine("Failed to start playback device.");
                        return -4;
                    }

                    Console.WriteLine("Press Enter to quit...");
                    Console.ReadKey();
                }
            }
            finally
            {
                decoder?.Dispose();
            }

            return 0;
        }
    }
}