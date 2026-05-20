using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CustomEditor(typeof(UIBehaviourComponent))]
public class UIBehaviourComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var uiComponentProp = serializedObject.FindProperty("UI_Component");
        EditorGUILayout.PropertyField(uiComponentProp);

        E_UIComponents uiComponent = (E_UIComponents)uiComponentProp.enumValueIndex;

        switch (uiComponent)
        {
            case E_UIComponents.Default:
                break;

            case E_UIComponents.Panel:
            case E_UIComponents.DefaultPopUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myPanel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("St_DefaultPopUpBehaviourData"), true);
                break;

            case E_UIComponents.Button:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myButton"));

                SerializedProperty buttonTypeProp = serializedObject.FindProperty("buttonType");
                EditorGUILayout.PropertyField(buttonTypeProp);
                E_ButtonType buttonType = (E_ButtonType)buttonTypeProp.enumValueIndex;

                SerializedProperty buttonStructProp = serializedObject.FindProperty("St_ButtonBehaviourData");
                SerializedProperty listOfButtonsToHide = serializedObject.FindProperty("_ButtonsToHideArray");
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Button Behaviour Settings", EditorStyles.boldLabel);

                // Mostrar només els camps rellevants segons tipus
                switch (buttonType)
                {
                    case E_ButtonType.ImageButton:
                        DrawField(buttonStructProp, "_buttonText");
                        DrawField(buttonStructProp, "_buttonImage");
                        DrawField(buttonStructProp, "bShowText");
                        DrawField(buttonStructProp, "bChangeImage");
                        DrawField(buttonStructProp, "_buttonImageChanged");
                        DrawField(buttonStructProp, "bButtonInMobileMode");
                        DrawField(buttonStructProp, "bHasAnimation"); 
                        DrawField(buttonStructProp, "key");
                        DrawField(buttonStructProp, "bHaveTutorialPopUp");
                        EditorGUILayout.PropertyField(listOfButtonsToHide, false);
                        break;

                    case E_ButtonType.TextButton:
                        DrawField(buttonStructProp, "key");
                        EditorGUILayout.PropertyField(listOfButtonsToHide, false);
                        break;

                    case E_ButtonType.TextButtonWithoutBackground:
                        DrawField(buttonStructProp, "key");
                        EditorGUILayout.PropertyField(listOfButtonsToHide, false);
                        break;

                    case E_ButtonType.SpriteButton:
                        EditorGUILayout.PropertyField(listOfButtonsToHide, false);
                        break;
                    case E_ButtonType.HideUIButton:
                        EditorGUILayout.PropertyField(listOfButtonsToHide, true);
                        break;
                }

                // Mostrar St_TutorialPopUpData si cal
                var bHaveTutorialProp = buttonStructProp.FindPropertyRelative("bHaveTutorialPopUp");
                if (bHaveTutorialProp != null && bHaveTutorialProp.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("St_TutorialPopUpData"), true);
                }

               //Premium State
                SerializedProperty bPremium = serializedObject.FindProperty("bPremiumBehaviour");

                EditorGUILayout.PropertyField(bPremium, true);

                if (bPremium != null && bPremium.boolValue)
                {
                    // Si és true, busquem i dibuixem l'estructura de dades
                    SerializedProperty uiPremium = serializedObject.FindProperty("_UIPremiumBehaviour");
                    if (uiPremium != null)
                    {
                        EditorGUILayout.PropertyField(uiPremium, true);
                    }
                }
                //Premium State
                SerializedProperty bFeatureNotAviable = serializedObject.FindProperty("bFeatureNotAviable");

                EditorGUILayout.PropertyField(bFeatureNotAviable, true);
                break;

            case E_UIComponents.Slider:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mySlider"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("St_SliderBehaviourData"), true);
                break;

            case E_UIComponents.TextBox:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myInputField"));
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("inputFieldType"), true);
                SerializedProperty InputTextStructProp = serializedObject.FindProperty("St_InputFieldBehaviourData");

                SerializedProperty InputTextTypeProp = serializedObject.FindProperty("inputFieldType");
                EditorGUILayout.PropertyField(InputTextTypeProp);
                E_InputFieldType inputType = (E_InputFieldType)InputTextTypeProp.enumValueIndex;
                SerializedProperty inputTextStructProp = serializedObject.FindProperty("St_InputFieldBehaviourData");

                switch(inputType)
                {
                    case E_InputFieldType.Default:DrawField(inputTextStructProp, "bRemeberMe");
                                                  DrawField(inputTextStructProp, "playerPrefName");
                                                  DrawField(inputTextStructProp, "key"); 
                                                  DrawField(inputTextStructProp, "_eyeGameObj"); break;
                    case E_InputFieldType.IntInRange: DrawField(inputTextStructProp, "_min");
                                                      DrawField(inputTextStructProp, "_max"); 
                                                      DrawField(inputTextStructProp, "bRemeberMe");
                                                      DrawField(inputTextStructProp, "playerPrefName");
                                                      DrawField(inputTextStructProp, "key");
                                                       break;
                    case E_InputFieldType.Email:DrawField(inputTextStructProp, "bRemeberMe");
                                                DrawField(inputTextStructProp, "playerPrefName");
                                                DrawField(inputTextStructProp, "key");
                                                break;
                    case E_InputFieldType.MaxNumberLenght: DrawField(inputTextStructProp, "_maxLenght"); 
                                                           DrawField(inputTextStructProp, "bRemeberMe");
                                                           DrawField(inputTextStructProp, "playerPrefName");
                                                           DrawField(inputTextStructProp, "key"); 
                                                           break;
                    case E_InputFieldType.Password: DrawField(inputTextStructProp, "bRemeberMe");
                                                    DrawField(inputTextStructProp, "playerPrefName");
                                                    DrawField(inputTextStructProp, "_eyeGameObj");
                                                    DrawField(inputTextStructProp, "_eyeOpen");
                                                    DrawField(inputTextStructProp, "_eyeClosed");
                                                    DrawField(inputTextStructProp, "_eyeClosedColor");
                                                    DrawField(inputTextStructProp, "_eyeOpenColor");
                                                    break;
                }
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("St_InputFieldBehaviourData"), true);
                break;

            case E_UIComponents.Text:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myText"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("St_TextBehaviourData"), true);
                break;

            case E_UIComponents.Dropdown:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myDropdown"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("St_DropdownBehaviourData"), true);
                break;
            case E_UIComponents.Toggle:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("myButton"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("St_ToggleBehaviourScript"), true);
               // EditorGUILayout.PropertyField(serializedObject.FindProperty("St_DefaultPopUpBehaviourData"), false); 
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawField(SerializedProperty parent, string relativeName)
    {
        SerializedProperty prop = parent.FindPropertyRelative(relativeName);
        if (prop != null)
        {
            EditorGUILayout.PropertyField(prop, true);
        }
    }
}