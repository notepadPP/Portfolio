using Framework.Common.Editor;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Tween.Editor
{
    [CustomEditor(typeof(TweenBase))]
    public abstract class TweenBaseEditor : ElementInspectorEditor
    {
        protected override bool isApply => false;
        protected virtual bool isDrawisAutoPlay { get; private set; } = true;
        protected virtual bool isDrawisLoop { get; private set; } = true;
        protected virtual bool isDrawAnimationCurve { get; private set; } = true;
        protected virtual bool isDrawTargetTransform { get; private set; } = true;
        protected virtual bool isDrawTimer { get; private set; } = true;
        protected TweenBase tween = null;
        protected RectTransform targetTransform = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            tween = target as TweenBase;
            targetTransform = GetPropertyData<RectTransform>("targetTransform");
        }
        protected override void DrawInspector()
        {
            StopAndPlay();
            DrawCurrentTimeSlider();
            DefaultInspectorGUI();
            DrawTweenInspector();
            targetTransform = GetPropertyData<RectTransform>("targetTransform");
        }
        protected abstract void DrawTweenInspector();


        private void DrawCurrentTimeSlider()
        {
            Type type = typeof(TweenBase);
            FieldInfo info = type.GetField("time", BindingFlags.NonPublic | BindingFlags.Instance);
            float time = (float)info.GetValue(target);
            SerializedProperty timerProperty = serializedObject.FindProperty("timer");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Slider("Progress Time", time, 0, timerProperty.floatValue);
            EditorGUI.EndDisabledGroup();
        }
        private void StopAndPlay()
        {
            TweenBase tween = target as TweenBase;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset"))
            {
                tween.Rewind();
            }
            if (GUILayout.Button("Stop"))
            {
                tween.Stop();
            }
            if (GUILayout.Button("Play"))
            {
                tween.Play();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DefaultInspectorGUI()
        {
            if (isDrawisAutoPlay) DrawProperty("isAutoPlay");
            if (isDrawisLoop) DrawProperty("isLoop");
            if (isDrawTargetTransform) DrawProperty("targetTransform");
            if (isDrawAnimationCurve) DrawProperty("animationCurve");
            if (isDrawTimer) DrawProperty("timer");
        }
    }

}
