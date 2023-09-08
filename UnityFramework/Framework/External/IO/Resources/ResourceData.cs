namespace Framework.Common.IO.Resources
{
    public class ResourceData : Interface.IData
    {
        public bool IsAvailable => string.IsNullOrEmpty(Name) == false && Object != null;
        public ResourceData(string name, UnityEngine.Object obj, bool isPatchResource) => SetData(name, obj, isPatchResource);
        private void SetData(string name, UnityEngine.Object obj, bool isPatchResource)
        {
            Name = name;
            Object = obj;
            IsPatchResource = isPatchResource;
        }
        public string Name { get; private set; } = string.Empty;
        public UnityEngine.Object Object { get; private set; } = null;
        public bool IsPatchResource { get; private set; } = false;
    }
}
