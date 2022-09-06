namespace Framework.Common
{
    public abstract class LifeCycle : Interface.ICreatable, Interface.IDestroyable
    {
        public bool IsInitialized { get; private set; }
        public bool IsDestroyed { get; private set; }

        public void Destroy()
        {
            IsDestroyed = true;
            OnDestroy();
        }

        public void Initialize()
        {
            IsInitialized = true;
            OnInitialize();
        }
        protected abstract void OnInitialize();
        protected abstract void OnDestroy();

    }

}
