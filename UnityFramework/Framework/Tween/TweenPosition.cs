using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Tween
{

    public class TweenPosition : TweenBase
    {
        [SerializeField] Vector3 start;
        [SerializeField] Vector3 end;
        Vector3 Length = Vector3.zero;
        protected override void OnStart()
        {
            Length = end - start;
            if (targetTransform != null) targetTransform.anchoredPosition3D = start;
        }

        protected override void OnUpdate(float percent)
        {
            if (targetTransform != null)
            {
                if(animationCurve != null)
                    targetTransform.anchoredPosition3D = start+ Length * animationCurve.Evaluate(percent);
                else
                    targetTransform.anchoredPosition3D = Vector3.Lerp(start, end, percent);
            }
        }

        protected override void OnUpdateFinish()
        {
            if (targetTransform != null) targetTransform.anchoredPosition3D = end;
        }
        public static void Play(RectTransform rectTransform, Vector3 start, Vector3 end, float time)
        {
            TweenPosition tween = rectTransform.gameObject.AddComponent<TweenPosition>();
            tween.targetTransform = rectTransform;
            tween.start = start;
            tween.end = end;
            tween.timer = time;
            tween.animationCurve = null;
            tween.Play((result) => result.Destroy());
        }

        protected override void OnReset()
        {
            OnStart();
        }

        protected override void OnStop()
        {
        }
    }
}