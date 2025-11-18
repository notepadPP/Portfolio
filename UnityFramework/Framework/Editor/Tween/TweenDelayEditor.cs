using UnityEditor;

namespace Framework.Tween.Editor
{
    [CustomEditor(typeof(TweenDelay))]
    public class TweenDelayEditor : TweenBaseEditor
    {
        protected override bool isDrawAnimationCurve => false;
        protected override bool isDrawisAutoPlay => false;
        protected override bool isDrawTargetTransform => false;
        protected override void DrawTweenInspector()
        {
        }
    }

}
