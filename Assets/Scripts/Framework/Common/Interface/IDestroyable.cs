namespace Framework.Common.Interface
{
    public interface IDestroyable
    {
        bool IsDestroyed { get; }
        void Destroy();
    }
}