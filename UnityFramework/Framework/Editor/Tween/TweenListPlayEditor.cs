using UnityEditor;

namespace Framework.Tween.Editor
{
    [CustomEditor(typeof(TweenListPlay))]
    public class TweenListPlayEditor : TweenBaseEditor
    {
        protected override bool isDrawAnimationCurve => false;
        protected override bool isDrawTargetTransform => false;
        protected override bool isDrawisAutoPlay => true;
        protected override bool isDrawTimer => false;
        protected override void DrawTweenInspector()
        {
            DrawProperty("List");
        }
    }

}
