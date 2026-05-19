using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ARObjectPartScript))]
public class EditorScript_ARObjectPart : Editor
{
    SerializedProperty _ARObjectGeneralPart;
    SerializedProperty _ARObjectPartData;
    SerializedProperty _ARObjectBoneData;
    SerializedProperty _ARObjectBoneParentData;
    SerializedProperty _ARObjectMultipleBoneParentData;
    SerializedProperty _CurrentARObject;

    void OnEnable()
    {
        _ARObjectGeneralPart = serializedObject.FindProperty("_ARObjectGeneralPart");
        _ARObjectPartData = serializedObject.FindProperty("_ARObjectPartData");
        _ARObjectBoneData = serializedObject.FindProperty("_ARObjectBoneData");
        _ARObjectBoneParentData = serializedObject.FindProperty("_ARObjectBoneParentData");
        _ARObjectMultipleBoneParentData = serializedObject.FindProperty("_ARObjectMultipleBoneParentData");
        _CurrentARObject = serializedObject.FindProperty("_CurrentARObject");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Mostrar l'enum
        EditorGUILayout.PropertyField(_ARObjectGeneralPart);

        EditorGUILayout.Space(10);

        // Mostrar structs segons el valor de l'enum
        var selectedPart = (E_ARObjectGeneralParts)_ARObjectGeneralPart.enumValueIndex;

        switch (selectedPart)
        {
            case E_ARObjectGeneralParts.PoolSpecificPart:
                EditorGUILayout.PropertyField(_ARObjectPartData, true);
                EditorGUILayout.PropertyField(_CurrentARObject);
                break;

            case E_ARObjectGeneralParts.BoneParent:
                EditorGUILayout.PropertyField(_ARObjectBoneParentData, true);
                EditorGUILayout.PropertyField(_CurrentARObject);
                break;

            case E_ARObjectGeneralParts.Bones:
                EditorGUILayout.PropertyField(_ARObjectBoneData, true);
                EditorGUILayout.PropertyField(_CurrentARObject);
                break;

            case E_ARObjectGeneralParts.MultipleBoneParent:
                EditorGUILayout.PropertyField(_ARObjectMultipleBoneParentData, true);
                EditorGUILayout.PropertyField(_CurrentARObject);
                break;

            case E_ARObjectGeneralParts.None:
            default:
                EditorGUILayout.HelpBox("Selecciona un tipus de part per mostrar la configuració.", MessageType.Info);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
