
namespace Framework.Common
{

    abstract public class Element : UnityEngine.MonoBehaviour, Interface.IElement
    {
        public abstract void OnInitialize();
        public abstract void DoDestroy();
        public bool IsInitialized { get; private set; } = false;
        public bool IsDestroyed { get; private set; } = false;
        protected virtual bool IsDestoryGameObject { get; private set; } = true;

        public UnityEngine.RectTransform rectTransform { get; private set; } = null;

        public void Initialize()
        {
            if (IsInitialized)
                return;
            IsInitialized = true; 
            rectTransform = transform as UnityEngine.RectTransform;
            Interface.IElement[] objects = GetComponentsInChildren<Interface.IElement>(true);
            foreach (Interface.IElement obj in objects)
                obj.Initialize();

            try
            {
                OnInitialize();
                //SetActive(false);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return;
            }
        }
        public void Destroy()
        {
            if (IsDestroyed)
            {
                Debug.LogAssertion($"{name} has been destroyed");
                return;
            }

            IsDestroyed = true;

            //EventManager.RemoveAll(this);
            StopAllCoroutines();
#if UNITY_EDITOR
            ClearEditUpdate();
            DoEditDestroy();
#endif

            DoDestroy();
            if (UnityEngine.Application.isPlaying)
            {
                if (IsDestoryGameObject)
                    Destroy(gameObject);
                else
                    Destroy(this);
            }
            else
            {
                if (IsDestoryGameObject)
                    DestroyImmediate(gameObject);
                else
                    DestroyImmediate(this);

            }
        }

        void OnDestroy() { if (!IsDestroyed) Destroy(); }


#if UNITY_EDITOR
        [UnityEngine.SerializeField] internal protected bool doEditApply = false;

        void Reset()
        {
            OnEditInitialize();
        }

        void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += OnValidateCallback;


        }
        void OnValidateCallback()
        {
            if (this == null)
            {
                UnityEditor.EditorApplication.delayCall -= OnValidateCallback;
                return; // MissingRefException if managed in the editor - uses the overloaded Unity == operator.
            }

            if (doEditApply) OnEditComponenet();
            OnEdit(doEditApply, UnityEditor.EditorApplication.timeSinceStartup);
            doEditApply = false;
        }

        void OnEditUpdateCallback()
        {
            if (this == null)
            {
                ClearEditUpdate();
                return;
            }

            OnEditUpdate(UnityEditor.EditorApplication.timeSinceStartup);
        }
        public void SetEditUpdate()
        {
            if (UnityEngine.Application.isPlaying) return;
            UnityEditor.EditorApplication.update -= OnEditUpdateCallback;
            UnityEditor.EditorApplication.update += OnEditUpdateCallback;
        }

        public void ClearEditUpdate()
        {
            if (UnityEngine.Application.isPlaying) return;
            UnityEditor.EditorApplication.update -= OnEditUpdateCallback;
        }

        protected virtual void SetEditQueuePlayerLoopUpdate()
        {
            if (!UnityEngine.Application.isPlaying)
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        }

        protected virtual void OnEditInitialize()
        {
        }

        protected virtual void OnEditComponenet()
        {
        }

        protected virtual void OnEdit(bool isApply, double time)
        {
            if(isApply)
                rectTransform = transform as UnityEngine.RectTransform;
        }

        protected virtual void OnEditUpdate(double time) => SetEditQueuePlayerLoopUpdate();

        protected virtual void DoEditDestroy()
        {
        }
#endif // UNITY_EDITOR

    }

}