using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIAnimationComponent))]
public class EditorScript_UIComponenAnimation : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var uiComponentProp = serializedObject.FindProperty("UI_Component");
        EditorGUILayout.PropertyField(uiComponentProp);

        // Cast enum index a l'enum per llegibilitat
        E_UIComponents componentType = (E_UIComponents)uiComponentProp.enumValueIndex;

        // Mostra propietats segons el component escollit
        switch (componentType)
        {
            case E_UIComponents.Default:
                break;

            case E_UIComponents.Panel:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animationType"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ComponentStruct"), true);
                break;

            case E_UIComponents.Button:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myButton"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animationType"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ComponentStruct"), true);

                // Aquí entra la lògica del segon script

                var onClickAnimTypeProp = serializedObject.FindProperty("UI_ButtonOnClickAnimType");
                EditorGUILayout.PropertyField(onClickAnimTypeProp);

                E_ButtonOnClickAnimType animType = (E_ButtonOnClickAnimType)onClickAnimTypeProp.enumValueIndex;

                switch (animType)
                {
                    case E_ButtonOnClickAnimType.None:
                        break;

                    case E_ButtonOnClickAnimType.DefaultOnClick:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnClickDefaultStruct"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnSelectedData"), true); 
                        break;

                    case E_ButtonOnClickAnimType.GoLeft:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnClickGoSidesStruct"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnSelectedData"), true);
                        break;

                    case E_ButtonOnClickAnimType.GoRight:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnClickGoSidesStruct"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnSelectedData"), true);
                        break;

                    case E_ButtonOnClickAnimType.Swipe:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnClickSwipeStruct"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ButtonOnSelectedData"), true);
                        break;

                }
                break;

            case E_UIComponents.TextBox:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myInputField"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animationType"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ComponentStruct"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_InputFieldStruct"), true);
                break;
            case E_UIComponents.DefaultPopUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animationType"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ComponentStruct"), true);
                break;
            case E_UIComponents.Text:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myText"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animationType"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("UI_ComponentStruct"), true);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
