using System.Reflection;
using System.Runtime.InteropServices;

namespace MiniAudioSharp.Native
{
    internal class NativeTypeNameAttribute : Attribute
    {
        public NativeTypeNameAttribute(string name) { }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void MiniAudioDataCallback(ma_device* pDevice, void* pOutput, void* pInput, uint frameCount);

    public unsafe static partial class MiniAudio
    {
        private enum OSType : int
        {
            Unknown = 0,
            Windows,
            Linux,
            MACOS,
            IOS,
            Android,
            Browser,
        }

        private static string _DirMiniAudioModules = "MiniAudioModules";
        private static string _PathMiniAudioModule = "";

        public static void Initialize()
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
        }

        public static void Initialize(string dirMiniAudioModules)
        {
            _DirMiniAudioModules = dirMiniAudioModules;
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName != "miniaudio")
                return IntPtr.Zero;

            if ( !String.IsNullOrEmpty(_PathMiniAudioModule) )
                return NativeLibrary.Load(_PathMiniAudioModule, assembly, searchPath);

            var arch = "32";
            if( Environment.Is64BitProcess )
                arch = "64";

            var os = OSType.Unknown;
            if (OperatingSystem.IsWindows())
                os = OSType.Windows;
            else if (OperatingSystem.IsLinux())
                os = OSType.Linux;
            else if (OperatingSystem.IsMacOS())
                os = OSType.MACOS;
            else if (OperatingSystem.IsAndroid())
                os = OSType.Android;
            else if (OperatingSystem.IsIOS())
                os = OSType.IOS;

            switch (os)
            {
                case OSType.Windows:
                    _PathMiniAudioModule = Path.Combine(_DirMiniAudioModules, os.ToString(), $"miniaudio{arch}.dll");
                    break;

                case OSType.Linux:
                case OSType.Android:
                    _PathMiniAudioModule = Path.Combine(_DirMiniAudioModules, os.ToString(), $"libminiaudio{arch}.so");
                    break;

                case OSType.IOS:
                case OSType.MACOS:
                    _PathMiniAudioModule = Path.Combine(_DirMiniAudioModules, os.ToString(), $"libminiaudio{arch}.dylib");
                    break;

                default:
                    throw new NotSupportedException();
            }

            return NativeLibrary.Load(_PathMiniAudioModule, assembly, searchPath);  
        }

        public class Helper
        {
            [DllImport("miniaudio", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern ma_device* ma_helper_create_empty_device();

            [DllImport("miniaudio", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern ma_device_config* ma_helper_create_empty_device_config();

            [DllImport("miniaudio", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern ma_decoder* ma_helper_create_empty_decoder();

            [DllImport("miniaudio", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            internal static extern ma_device_config* ma_helper_create_playback_config(ma_decoder* pDecoder, MiniAudioDataCallback dataCallback, void* userData);
        }

    }

}
