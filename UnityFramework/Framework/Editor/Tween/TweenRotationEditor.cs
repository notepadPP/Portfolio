using UnityEditor;

namespace Framework.Tween.Editor
{
    [CustomEditor(typeof(TweenRotation))]
    public class TweenRotationEditor : TweenBaseEditor
    {
        protected override void DrawTweenInspector()
        {
            if (targetTransform != null)
            {
                DrawProperty("start", targetTransform.localEulerAngles);
                DrawProperty("end", targetTransform.localEulerAngles);

            }
        }
    }

}
