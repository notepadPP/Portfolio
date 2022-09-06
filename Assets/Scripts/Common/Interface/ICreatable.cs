namespace Common
{
    public interface ICreatable
    {
        bool IsInitialized { get; }
        void Initialize();
    }
}

