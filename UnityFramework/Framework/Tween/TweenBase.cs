using System;
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;

namespace Framework.Tween
{
    public abstract class TweenBase : Common.Element
    {
        public virtual float Timer => timer;
        [SerializeField] private bool isAutoPlay = false;
        [SerializeField] private bool isLoop = false;
        [SerializeField] protected RectTransform targetTransform;
        [SerializeField] protected float timer = 0;

        [SerializeField] protected AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        protected override bool IsDestoryGameObject => false;
        protected float time = 0;

#if UNITY_EDITOR
        EditorCoroutine editorCoroutine = null;
#else
        Coroutine routine = null;
#endif

        public override void OnInitialize()
        {

        }
        public override void DoDestroy()
        {
        }
        protected virtual void OnEnable()
        {
            if (isAutoPlay)
                Play();
        }
        public void Rewind()
        {
            time = 0;
            OnReset();
        }
        public void Stop()
        {
            OnStop();
#if UNITY_EDITOR
            if (editorCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(editorCoroutine);
                editorCoroutine = null;
            }
#else
            if(routine != null)
            {
                MonoBehaviour monoBehaviour = targetTransform.GetComponent<MonoBehaviour>();
                monoBehaviour.StopCoroutine(routine);
                routine = null;
            }
#endif

        }
        public void Play() => Play(null);
        public void Play(Action<TweenBase> Completed)
        {
            Stop();
            Rewind();

            OnStart();
#if UNITY_EDITOR
            editorCoroutine = EditorCoroutineUtility.StartCoroutine(coPlay(Completed), this);
#else
            MonoBehaviour monoBehaviour = targetTransform.GetComponent<MonoBehaviour>();
            routine = monoBehaviour.StartCoroutine(coPlay(Completed));
#endif
        }
        IEnumerator coPlay(Action<TweenBase> Completed = null)
        {
            float percent = 0;

            while (percent < 1.0f)
            {
                time += Time.deltaTime;
                percent = time / timer;
                OnUpdate(percent);
                if (isLoop == true && percent >= 1.0f)
                {
                    time = 0;
                }
                yield return null;
            }
            OnUpdateFinish();
            Completed?.Invoke(this);

        }
        protected abstract void OnReset();
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void OnUpdate(float percent);
        protected abstract void OnUpdateFinish();

    }
}
