using UnityEditor;
using UnityEngine;

namespace Framework.Tween.Editor
{
    [CustomEditor(typeof(TweenAlpha))]
    public class TweenAlphaEditor : TweenBaseEditor
    {
        TweenAlpha alpha = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            alpha = tween as TweenAlpha;
            alpha?.FindCanvasGroup();
        }
        protected override void DrawTweenInspector()
        {
            CanvasGroup canvasGroup = alpha?.FindCanvasGroup() ?? null;
            if(canvasGroup != null)
            {
                DrawProperty("startAlpha", canvasGroup.alpha);
                DrawProperty("endAlpha", canvasGroup.alpha);

            }
        }
    }

}
