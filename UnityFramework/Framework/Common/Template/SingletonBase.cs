using Framework.Common.Interface;
using System.Collections;
using UnityEngine;

namespace Framework.Common.Template
{
    public abstract class SingletonBase : IElement
    {
        public bool IsInitialized { get; private set; } = false;

        public bool IsDestroyed { get; private set; } = false;
        public Transform transform { get; private set; } = null;

        public RectTransform rectTransform { get; private set; } = null;

        protected MonoBehaviour behaviour { get; private set; } = null;
        public void SetMonoBehaviour(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
            transform = behaviour.transform;
            rectTransform = transform as RectTransform;
        }
        public Coroutine StartCoroutine(IEnumerator routine) => behaviour.StartCoroutine(routine);
        public void StopCoroutine(IEnumerator routine) => behaviour.StopCoroutine(routine);
        public void StopCoroutine(Coroutine routine) => behaviour.StopCoroutine(routine);
        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            try
            {
                OnInitialize();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return;
            }
        }
        public void Destroy()
        {
            if (IsDestroyed) return;
            IsDestroyed = true;

            DoDestroy();
        }
        public abstract void DoDestroy();
        public abstract void OnInitialize();
    }
}