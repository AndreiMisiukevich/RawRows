using System;
namespace FifteenInRow
{
    public interface IAudioService
    {
        void Play(string resource, bool isLoop);
        void Stop(string resource);
    }
}
