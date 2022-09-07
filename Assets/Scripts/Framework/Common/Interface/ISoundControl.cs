using System;

namespace Framework.Common.Interface
{
    public interface ISoundControl
    {
        bool IsMute { set; get; }
        float Volume { set; get; }
        void Play<T>(T id, float volume = 1) where T : unmanaged, Enum;
        void Play<T>(T id, float volume, bool loop) where T : unmanaged, Enum;
        void Stop<T>(T id) where T : unmanaged, Enum;
        void StopAll();
    }
}