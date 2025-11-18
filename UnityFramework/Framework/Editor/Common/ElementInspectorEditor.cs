using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Framework.Common.Editor
{
    [CustomEditor(typeof(Common.Element))]
    public abstract class ElementInspectorEditor : UnityEditor.Editor
    {
        //static protected readonly string[] filters = new string[]
        //{
        //    "System",
        //    "UnityEngine",
        //    "UnityEditor",
        //    "Unity",
        //    "Mono",
        //    "Bee",

        //    "mscorlib",
        //};
        static protected readonly GUILayoutOption ButtonWidth = GUILayout.Width(40);
        protected virtual bool isApply { get; private set; } = true;
        protected List<Type> TypeEnums = GetEnums("Mjjigae");
        protected virtual void OnEnable()
        {
            serializedObject.Update();
            SerializedProperty property = serializedObject.FindProperty("doEditApply");
            property.boolValue = true;
            serializedObject.ApplyModifiedProperties();

        }
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            ScriptInspector();
            if (isApply) DrawProperty("doEditApply");
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
        }


        protected abstract void DrawInspector();
        protected void ScriptInspector()
        {
            EditorGUI.BeginDisabledGroup(true);
            DrawProperty("m_Script");
            EditorGUI.EndDisabledGroup();
        }
        protected T GetPropertyData<T>(string PropertyName) where T : UnityEngine.Object
        {
            SerializedProperty property = serializedObject.FindProperty(PropertyName);
            if (property != null)
            {
                return property.objectReferenceValue as T;
            }
            return null;
        }
        protected void DrawProperty(string PropertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(PropertyName);
            if (property != null) EditorGUILayout.PropertyField(property);
        }
        protected void DrawProperty(string PropertyName, float value)
        {
            SerializedProperty property = serializedObject.FindProperty(PropertyName);
            EditorGUILayout.BeginHorizontal();
            if (property != null) EditorGUILayout.PropertyField(property);
            if (GUILayout.Button("Snap", ButtonWidth))
                if (property != null) property.floatValue = value;
            EditorGUILayout.EndHorizontal();
        }
        protected void DrawProperty(string PropertyName, Vector3 value)
        {
            SerializedProperty property = serializedObject.FindProperty(PropertyName);
            EditorGUILayout.BeginHorizontal();
            if (property != null) EditorGUILayout.PropertyField(property);
            if (GUILayout.Button("Snap", ButtonWidth))
                if (property != null) property.vector3Value = value;
            EditorGUILayout.EndHorizontal();
        }

        protected int DrawPropertyEnum(string propertyName, List<string> enumNames)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            int Index = enumNames.FindIndex(obj => obj == property.stringValue);
            if (Index < 0) Index = 0;
            int newIndex = EditorGUILayout.Popup(propertyName, Index, enumNames.ToArray());
            property.stringValue = enumNames[newIndex];
            return newIndex;
        }
        static protected List<Type> GetEnums(string nameSpace)
        {
            List<Type> list = new List<Type>();
            Assembly[] asms = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in asms)
            {
                if (assembly.FullName.Contains("Assembly-CSharp") == false) continue;
                list.AddRange(assembly.GetTypes().Where(type => type.IsEnum));
            }
            return list;
        }
    }
}