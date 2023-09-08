using UnityEditor;

namespace Framework.Tween.Editor
{
    [CustomEditor(typeof(TweenPosition))]
    public class TweenPositionEditor : TweenBaseEditor
    {
        protected override void DrawTweenInspector()
        {
            if (targetTransform != null)
            {
                DrawProperty("start", targetTransform.anchoredPosition3D);
                DrawProperty("end", targetTransform.anchoredPosition3D);
            }
        }
    }

}
