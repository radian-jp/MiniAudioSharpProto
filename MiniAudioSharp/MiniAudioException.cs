using MiniAudioSharp.Native;

namespace MiniAudioSharp
{
    public class MiniAudioException : Exception
    {
        internal MiniAudioException(ma_result result) : base(result.ToString()) { }
    }
}
