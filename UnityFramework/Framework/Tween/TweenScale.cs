using UnityEngine;

namespace Framework.Tween
{

    public class TweenScale : TweenBase
    {
        [SerializeField] Vector3 start;
        [SerializeField] Vector3 end;
        Vector3 Length = Vector3.zero;
        protected override void OnStart()
        {
            Length = end - start;
            if (animationCurve != null)
                targetTransform.localScale = start + Length * animationCurve.Evaluate(0);
            else
                targetTransform.localScale = start;
        }

        protected override void OnUpdate(float percent)
        {
            if (animationCurve != null)
                targetTransform.localScale = start + Length * animationCurve.Evaluate(percent);
            else
                targetTransform.localScale = Vector3.Lerp(start, end, percent);
        }

        protected override void OnUpdateFinish()
        {
            if (animationCurve != null)
                targetTransform.localScale = start + Length * animationCurve.Evaluate(1);
            else
                targetTransform.localScale = end;
        }
        public static void Play(RectTransform rectTransform, Vector3 start, Vector3 end, float time)
        {
            TweenScale tween = rectTransform.gameObject.AddComponent<TweenScale>();
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