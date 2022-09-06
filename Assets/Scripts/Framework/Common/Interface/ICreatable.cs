namespace Framework.Common.Interface
{
    public interface ICreatable
    {
        bool IsInitialized { get; }
        void Initialize();
    }
}

