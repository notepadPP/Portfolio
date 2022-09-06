namespace Common
{
    public interface IDestroyable
    {
        bool IsDestroyed { get; }
        void Destroy();
    }
}