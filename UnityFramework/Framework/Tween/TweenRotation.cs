using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Tween
{

    public class TweenRotation : TweenBase
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
                targetTransform.localEulerAngles = start + Length * animationCurve.Evaluate(1);
            else
                targetTransform.localEulerAngles = end;
        }
        public static void Play(RectTransform rectTransform, Vector3 start, Vector3 end, float time)
        {
            TweenRotation tween = rectTransform.gameObject.AddComponent<TweenRotation>();
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