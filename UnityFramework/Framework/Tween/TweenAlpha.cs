using UnityEngine;

namespace Framework.Tween
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenAlpha : TweenBase
    {
        [SerializeField, Range(0, 1)] float startAlpha = 1;
        [SerializeField, Range(0, 1)] float endAlpha = 1;
        private CanvasGroup canvasGroup = null;
        private float length;
        public override void OnInitialize()
        {
            base.OnInitialize();
            FindCanvasGroup();
        }
        protected override void OnStart()
        {
            length = endAlpha - startAlpha;
            if (canvasGroup != null)
            {
                if(animationCurve != null)
                    canvasGroup.alpha = startAlpha + length * animationCurve.Evaluate(0);
                else
                    canvasGroup.alpha = startAlpha;
            }
        }

        protected override void OnUpdate(float percent)
        {
            if (canvasGroup != null)
            {
                if (animationCurve != null)
                    canvasGroup.alpha = startAlpha + length * animationCurve.Evaluate(percent);
                else
                    canvasGroup.alpha = startAlpha + (endAlpha - startAlpha) * percent;
            }
        }

        protected override void OnUpdateFinish()
        {
            if (canvasGroup != null)
            {
                if (animationCurve != null)
                    canvasGroup.alpha = startAlpha + length * animationCurve.Evaluate(1);
                else
                    canvasGroup.alpha = endAlpha;
            }
        }
        public CanvasGroup FindCanvasGroup()
        {
            if (targetTransform == null) return null;
            canvasGroup = targetTransform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = targetTransform.gameObject.AddComponent<CanvasGroup>();
            return canvasGroup;
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
