using UnityEngine;

namespace Framework.Common.Interface
{
    public interface IElement
    {
        public RectTransform rectTransform { get; }
        public bool IsInitialized { get; }
        public bool IsDestroyed { get; }
        public void OnInitialize();
        public void DoDestroy();
        public void Initialize();
        public void Destroy();
    }

}