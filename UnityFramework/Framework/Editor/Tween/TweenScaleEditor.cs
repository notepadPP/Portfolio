using UnityEditor;

namespace Framework.Tween.Editor
{
    [CustomEditor(typeof(TweenScale))]
    public class TweenScaleEditor : TweenBaseEditor
    {
        protected override void DrawTweenInspector()
        {
            if(targetTransform != null)
            {
                DrawProperty("start", targetTransform.localScale);
                DrawProperty("end", targetTransform.localScale);
            }
        }
    }

}
