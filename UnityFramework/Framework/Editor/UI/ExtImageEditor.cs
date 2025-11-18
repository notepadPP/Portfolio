using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExtImage))]
public class ExtImageEditor : UnityEditor.Editor
{
    private SerializedProperty spriteProp;
    private SerializedProperty colorProp;
    private SerializedProperty materialProp;
    private SerializedProperty flipProp;

    private SerializedProperty useSpriteMeshProp;
    private SerializedProperty preserveAspectProp;
    private SerializedProperty typeProp;
    private SerializedProperty fillDirectionProp;
    private SerializedProperty fillAmountProp;
    private SerializedProperty pixelsPerUnitMultiplierProp;
    private SerializedProperty fillCenterProp;

    private GUIContent spriteLabel;
    private ExtImage image;
    protected void OnEnable()
    {
        spriteProp = serializedObject.FindProperty("m_Sprite");
        colorProp = serializedObject.FindProperty("m_Color");
        materialProp = serializedObject.FindProperty("m_Material");
        flipProp = serializedObject.FindProperty("flip");
        typeProp = serializedObject.FindProperty("m_Type");
        useSpriteMeshProp = serializedObject.FindProperty("m_UseSpriteMesh");
        preserveAspectProp = serializedObject.FindProperty("m_PreserveAspect");


        fillDirectionProp = serializedObject.FindProperty("m_FillDirection");
        fillAmountProp = serializedObject.FindProperty("m_FillAmount");
        pixelsPerUnitMultiplierProp = serializedObject.FindProperty("m_PixelsPerUnitMultiplier");
        fillCenterProp = serializedObject.FindProperty("m_FillCenter");

        spriteLabel = new GUIContent("Source Image");
        image = target as ExtImage;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(spriteProp, spriteLabel);
        EditorGUILayout.PropertyField(colorProp);
        EditorGUILayout.PropertyField(materialProp);
        EditorGUILayout.PropertyField(flipProp);
        DrawPropertiesExcluding(serializedObject, "m_Script", "m_Sprite", "m_Color", "m_UseSpriteMesh", "m_PreserveAspect", "m_Material", "m_OnCullStateChanged", "m_Type", "m_FillDirection", "m_FillAmount", "m_PixelsPerUnitMultiplier", "m_FillCenter", "flip");
        EditorGUILayout.PropertyField(typeProp);
        EditorGUI.indentLevel++;
        switch (image.type)
        {
            case ExtImageType.Simple:
                EditorGUILayout.PropertyField(useSpriteMeshProp);
                EditorGUILayout.PropertyField(preserveAspectProp);
                break;
            case ExtImageType.Sliced:

                EditorGUILayout.PropertyField(fillDirectionProp);
                EditorGUILayout.PropertyField(fillAmountProp);
                EditorGUILayout.PropertyField(pixelsPerUnitMultiplierProp);
                EditorGUILayout.PropertyField(fillCenterProp);
                break;
            case ExtImageType.Tiled:
                EditorGUILayout.PropertyField(pixelsPerUnitMultiplierProp);
                break;
            // case ExtImageType.Filled:
                // EditorGUILayout.PropertyField(fillDirectionProp);
                // EditorGUILayout.PropertyField(fillAmountProp);
                // 
                // EditorGUILayout.PropertyField(preserveAspectProp);
                // break;
        }
        EditorGUI.indentLevel--;


        serializedObject.ApplyModifiedProperties();
    }
}
