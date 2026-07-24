using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStepManager : MonoBehaviour
{
    [Header("TUTORIAL STEP MANAGER")]
    [Space()]
    [Header("Tutorial Steps data:")]
    [SerializeField] private int currentTutorialInfoNumber = 0;
    [SerializeField] private E_TutorialActions currentTutorialState = E_TutorialActions.Default;
    [SerializeField] private St_TutorialInfoData[] _StepInfoData;
    [Header("Tutorial UI components:")]
    [SerializeField] private LocalizedTMPText _TitleText;
    [SerializeField] private LocalizedTMPText _DescripText;
    [SerializeField] private Button _ContinueButton;
    [SerializeField] private Button _PreviousButton;
    [SerializeField] private Button _CancelTutorialButton;
    [SerializeField] private GameObject _TutorialInfoPanel;
    [SerializeField] private GameObject _NoInteractPanel;
    [SerializeField] private GameObject _LeftRadialFeedback;
    [SerializeField] private GameObject _RightRadialFeedback;
    [SerializeField] private GameObject _ProductRadialFeedback;
    [SerializeField] private GameObject _ModelRadialFeedback;
    [SerializeField] private GameObject _MaterialRadialFeedback;
    
    [Header("InGame UI components:")]
    [SerializeField] private GameObject _MoveObjButton;
    [SerializeField] private GameObject _RotateObjButton;
    [SerializeField] private GameObject _SetObjLenghtButton;
    [SerializeField] private GameObject _SetLamasLenghtButton;
    [SerializeField] private GameObject _OpenBlueprintButton;
    [SerializeField] private GameObject _CatalogButton;
    [SerializeField] private GameObject _TakePhotoButton;
    [SerializeField] private GameObject _CreatePoolButton;
    [SerializeField] private GameObject _HideLeftButtons;
    [SerializeField] private GameObject _HideRightButtons;
    [SerializeField] private GameObject _SettingsButton;

    HUDManagerScript _HUDManagerScript;
    ARObjectManager _ARObjectManager;

    private RectTransform _rectTransform;
    private Vector2 _initialPosition;
    float _xOffset = 0.0f;
    float _yOffset = 0.0f;

    Vector3 _InitCurrentObjectPosition;
    Quaternion _InitCurrentObjectRotation;
    float _InitObjectLenght;
    float _InitLamasLenght;

    St_Product _InitProductData;
    St_Model _InitModelData;
    St_Material _InitMaterialData;

    void Start()
    {
        InitTutorialStepManager();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdateTutorialState();
    }

    ///////////////////////////////////////////////////////////////////////////////////// INIT /////////////////////////////////////////////////////////////////////////////////////////////

    private void InitTutorialStepManager()
    {
        if (GameObject.FindGameObjectWithTag("HUD"))
        {
            _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
        }

        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        if (_TutorialInfoPanel != null)
        {
            _rectTransform = _TutorialInfoPanel.GetComponent<RectTransform>();
            _initialPosition = _rectTransform.anchoredPosition;
        }

        _ContinueButton.onClick.RemoveListener(OnClickContinueButton);
        _ContinueButton.onClick.AddListener(OnClickContinueButton);

        _PreviousButton.onClick.RemoveListener(OnClickPreviousButton);
        _PreviousButton.onClick.AddListener(OnClickPreviousButton);

        currentTutorialInfoNumber = 0;
        SetTutorialInfo(currentTutorialInfoNumber);

    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////////// SETTER /////////////////////////////////////////////////////////////////////////////////////////////
    void SetTitleText(string key)
    {
        _TitleText.LocalizationKey = key;
        _TitleText.Localize();
    }
    void SetDescripText(string key)
    {
        _DescripText.LocalizationKey = key;
        _DescripText.Localize();
    }

    void SetTutorialInfo(int index)
    {
        if (_StepInfoData.Length != 0 && (_StepInfoData.Length - 1) >= index)
        {
            SetTitleText(_StepInfoData[index]._TitleKey);
            SetDescripText(_StepInfoData[index]._DescripKey);

            _xOffset = _StepInfoData[index]._xOffset;
            _yOffset = _StepInfoData[index]._yOffset;

            OnAnimTutorialInfoPanel(_xOffset, _yOffset);

            _NoInteractPanel.SetActive(_StepInfoData[index].bNoInteractPanelIsActive);

            _ContinueButton.gameObject.SetActive(_StepInfoData[index].bContinueButtonIsActive);

            _LeftRadialFeedback.SetActive(_StepInfoData[index].bLeftFeedbackObj);
            _RightRadialFeedback.SetActive(_StepInfoData[index].bRightFeedbackObj);
            _ProductRadialFeedback.SetActive(_StepInfoData[index].bCatalogProductFeedbackObj);
            _ModelRadialFeedback.SetActive(_StepInfoData[index].bCatalogModelFeedbackObj);
            _MaterialRadialFeedback.SetActive(_StepInfoData[index].bCatalogMaterialFeedbackObj);

            _InitProductData = _ARObjectManager.GetCurrentProductData();
            _InitModelData = _ARObjectManager.GetCurrentModelData();
            _InitMaterialData = _ARObjectManager.GetCurrentMaterialData();

            currentTutorialState = _StepInfoData[index].currentStepState;
            SetTutorialCurrentState();
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////////////////////////////////

    public void SetTutorialCurrentState()
    {
        switch (currentTutorialState)
        {
            case E_TutorialActions.Default:

                if (_HUDManagerScript != null)
                {
                    _HUDManagerScript.DeselectAllUIFromSpecificPanel(_HUDManagerScript.GetCurrentPanelOnScreenObj());
                    _HUDManagerScript.HideAllOfSubMenus();
                }
                    
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    _ARObjectManager.DesactiveFeedbackDecal();

                    if (_ARObjectManager.GetCurrentARObject() != null)
                    {
                        _InitCurrentObjectPosition = _ARObjectManager.GetCurrentARObject().transform.position;
                        _InitCurrentObjectRotation = _ARObjectManager.GetCurrentARObject().transform.rotation;
                        _InitObjectLenght = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;
                        _InitLamasLenght = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght;
                    }
                }

                SetButtonsState("All", true);
 
                break;

            case E_TutorialActions.OnInitTutorial:

                if (_ARObjectManager != null)
                {
                    _ARObjectManager.SetActivePlaneFinder(false);
                }
                _PreviousButton.gameObject.SetActive(false);
                break;

            case E_TutorialActions.OnPlaceObject:
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.SetActivePlaneFinder(true);
                }
                _PreviousButton.gameObject.SetActive(true);
                break;

            case E_TutorialActions.OnMoveObject:

                if (_HUDManagerScript != null)
                {
                    _HUDManagerScript.DeselectAllUIFromSpecificPanel(_HUDManagerScript.GetCurrentPanelOnScreenObj());
                    _HUDManagerScript.HideAllOfSubMenus();
                }
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    _ARObjectManager.DesactiveFeedbackDecal();
                }
                SetButtonsState("All", false);
                SetButtonsState("MoveObjButton", true);
                break;

            case E_TutorialActions.OnRotateObject:

                if (_HUDManagerScript != null)
                {
                    _HUDManagerScript.DeselectAllUIFromSpecificPanel(_HUDManagerScript.GetCurrentPanelOnScreenObj());
                    _HUDManagerScript.HideAllOfSubMenus();
                }
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    _ARObjectManager.DesactiveFeedbackDecal();
                    _InitCurrentObjectRotation = _ARObjectManager.GetCurrentARObject().transform.rotation;
                }
                SetButtonsState("All", false);
                SetButtonsState("RotateObjButton", true);
                break;

            case E_TutorialActions.OnSetObjectLenght:

                if (_HUDManagerScript != null)
                {
                    _HUDManagerScript.DeselectAllUIFromSpecificPanel(_HUDManagerScript.GetCurrentPanelOnScreenObj());
                    _HUDManagerScript.HideAllOfSubMenus();
                }
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    _ARObjectManager.DesactiveFeedbackDecal();
                    _InitObjectLenght = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;
                }
                SetButtonsState("All", false);
                SetButtonsState("SetObjLenghtButton", true);
                break;

            case E_TutorialActions.OnSetLamasLenght:

                if (_HUDManagerScript != null)
                {
                    _HUDManagerScript.DeselectAllUIFromSpecificPanel(_HUDManagerScript.GetCurrentPanelOnScreenObj());
                    _HUDManagerScript.HideAllOfSubMenus();
                }
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.DesactiveFeedbackDecal();
                    _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    _InitLamasLenght = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght;
                }
                SetButtonsState("All", false);
                SetButtonsState("SetLamasLenghtButton", true);
                break;

            case E_TutorialActions.OnShowCatalog:

                if (_HUDManagerScript != null)
                {
                    _HUDManagerScript.DeselectAllUIFromSpecificPanel(_HUDManagerScript.GetCurrentPanelOnScreenObj());
                    _HUDManagerScript.HideAllOfSubMenus();

                    if (_HUDManagerScript.GetCurrentPanelOnScreenName() == "CatalogPanel")
                    {
                        _HUDManagerScript.TravelToPanel("AP_ManipulateGroundRollerPanel");
                    }
                }
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.DesactiveFeedbackDecal();
                    _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                }
                SetButtonsState("All", false);
                SetButtonsState("CatalogButton", true);

                _CatalogButton.GetComponent<Button>().onClick.RemoveListener(OnClickContinueButton);
                _CatalogButton.GetComponent<Button>().onClick.AddListener(OnClickContinueButton);

                break;
            case E_TutorialActions.OnShowCatalogProductPart:
                _CatalogButton.GetComponent<Button>().onClick.RemoveListener(OnClickContinueButton);
                _InitProductData = _ARObjectManager.GetCurrentProductData();
                break;
            case E_TutorialActions.OnShowCatalogModelPart:
                _InitModelData = _ARObjectManager.GetCurrentModelData();
                break;
            case E_TutorialActions.OnShowCatalogMaterialPart:
                _InitMaterialData = _ARObjectManager.GetCurrentMaterialData();
                if (_HUDManagerScript != null)
                {
                    if (_HUDManagerScript.GetCurrentPanelOnScreenName() != "CatalogPanel")
                    {
                        _HUDManagerScript.TravelToPanel("CatalogPanel");
                    }
                }
                SetButtonsState("All", false);
                break;

            case E_TutorialActions.OnTakePhoto:
                SetButtonsState("All", false);
                SetButtonsState("TakePhotoButton", true);
                break;
            case E_TutorialActions.OnCreatePool:
                if (_HUDManagerScript != null)
                {
                    if (_HUDManagerScript.GetCurrentPanelOnScreenName() == "CatalogPanel")
                    {
                        _HUDManagerScript.TravelToPanel("AP_ManipulateGroundRollerPanel");
                    }
                }
                SetButtonsState("All", false);
                SetButtonsState("CreatePoolButton", true);
                break;

            case E_TutorialActions.OnShowBlueprintPopUp:
                if (_HUDManagerScript != null)
                {
                    if (_HUDManagerScript.GetCurrentPanelOnScreenName() == "BlueprintPopUpPanel")
                    {
                        _HUDManagerScript.TravelToLastPanel();
                    }
                }
                SetButtonsState("All", false);
                SetButtonsState("OpenBlueprintButton", true);


                break;

            case E_TutorialActions.OnShowBlueprint:



                break;

        }
    }

    public void OnUpdateTutorialState()
    {
        switch (currentTutorialState)
        {
            case E_TutorialActions.Default:

                break;

            case E_TutorialActions.OnInitTutorial:
                if (_ARObjectManager != null)
                {
                    _ARObjectManager.SetActivePlaneFinder(false);
                }
                break;
            case E_TutorialActions.OnPlaceObject: 
                if(_ARObjectManager != null)
                {
                    if(_ARObjectManager.GetIsARObjectPlaced())
                    {
                        OnClickContinueButton();
                    }
                }
                break;
            case E_TutorialActions.OnMoveObject:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.MovingState)
                    {
                       if(_ARObjectManager.GetCurrentARObject().transform.position != _InitCurrentObjectPosition)
                       {
                            _ContinueButton.gameObject.SetActive(true);
                       }
                    }
                }
                break;
            case E_TutorialActions.OnRotateObject:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.RotatingState)
                    {
                        if (_ARObjectManager.GetCurrentARObject().transform.rotation != _InitCurrentObjectRotation)
                        {
                            _ContinueButton.gameObject.SetActive(true);
                        }
                    }
                }
                break;
            case E_TutorialActions.OnSetObjectLenght:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.SetLenghtState)
                    {
                        if (_ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght != _InitObjectLenght)
                        {
                            _ContinueButton.gameObject.SetActive(true);
                        }
                    }
                }
                break;
            case E_TutorialActions.OnSetLamasLenght:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.SetLamasLenghtState)
                    {
                        if (_ARObjectManager.GetCovARObjectData()._CurrentLamasLenght != _InitLamasLenght)
                        {
                            _ContinueButton.gameObject.SetActive(true);
                        }
                    }
                }
                break;
            case E_TutorialActions.OnShowCatalog:
                

                break;
            case E_TutorialActions.OnShowCatalogProductPart:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetCurrentProductData() != _InitProductData)
                    {
                        _ContinueButton.gameObject.SetActive(true);
                    }
                }
                break;
            case E_TutorialActions.OnShowCatalogModelPart:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetCurrentModelData() != _InitModelData)
                    {
                        _ContinueButton.gameObject.SetActive(true);
                    }
                }
                break;
            case E_TutorialActions.OnShowCatalogMaterialPart:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetCurrentMaterialData() != _InitMaterialData)
                    {
                        _ContinueButton.gameObject.SetActive(true);
                    }
                }
                break;
            case E_TutorialActions.OnTakePhoto: break;
            case E_TutorialActions.OnCreatePool:
                if (_ARObjectManager != null)
                {
                    if (_ARObjectManager.GetPoolActiveState())
                    {
                        _ContinueButton.gameObject.SetActive(true);
                    }
                }
                break;
           
            case E_TutorialActions.OnShowBlueprintPopUp:
                if (_HUDManagerScript != null)
                {
                    if (_HUDManagerScript.GetCurrentPanelOnScreenName() == "BlueprintPopUpPanel")
                    {
                        OnClickContinueButton();
                    }
                }
                break;
            case E_TutorialActions.OnShowBlueprint:
                if (_HUDManagerScript != null)
                {
                    if (_HUDManagerScript.GetCurrentPanelOnScreenName() == "AstralpoolSettingsPanel")
                    {
                        OnClickContinueButton();
                    }
                }
                break;
        }
    }


    void OnClickContinueButton()
    {

        if (currentTutorialInfoNumber < _StepInfoData.Length - 1)
        {
            currentTutorialInfoNumber++;
            SetTutorialInfo(currentTutorialInfoNumber);
           
        }
        else
        {

        }
    }

    void OnClickPreviousButton()
    {
        if (currentTutorialInfoNumber > 0)
        {
            currentTutorialInfoNumber-=1;
            SetTutorialInfo(currentTutorialInfoNumber);
            if (_HUDManagerScript != null)
            {
                _HUDManagerScript.DeselectAllUIFromSpecificPanel(_HUDManagerScript.GetCurrentPanelOnScreenObj());
                _HUDManagerScript.HideAllOfSubMenus();
            }
        }
        else
        {
 
        }
    }
    
    void SetButtonsState(string button, bool state)
    {
       switch(button)
       { 
            case "MoveObjButton": _MoveObjButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "RotateObjButton": _RotateObjButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "SetObjLenghtButton": _SetObjLenghtButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "SetLamasLenghtButton": _SetLamasLenghtButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "OpenBlueprintButton": _OpenBlueprintButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "CatalogButton": _CatalogButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "TakePhotoButton": _TakePhotoButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "CreatePoolButton": _CreatePoolButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "HideLeftButtons": _HideLeftButtons.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "HideRightButtons": _HideRightButtons.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "SettingsButton": _SettingsButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
            case "All": _MoveObjButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _RotateObjButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _SetObjLenghtButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _SetLamasLenghtButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _OpenBlueprintButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _CatalogButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _TakePhotoButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _CreatePoolButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _HideLeftButtons.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state);
                _HideRightButtons.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); 
                _SettingsButton.GetComponent<UIBehaviourComponent>().InitNotAviableFeatureBehaviour(state); break;
       }
    }

    public void OnAnimTutorialInfoPanel(float xOffset, float yOffset)
    {
        if (_rectTransform == null) return;

        Vector2 targetPosition = _initialPosition + new Vector2(xOffset, yOffset);

        Sequence animSequence = DOTween.Sequence();

        if (_rectTransform.anchoredPosition != targetPosition)
        {
            animSequence.Append(
                _rectTransform.DOAnchorPos(targetPosition, 0.3f).SetEase(Ease.OutBack)
            );
        }

        animSequence.Append(
             _rectTransform.DOScale(1.1f, 0.25f).SetEase(Ease.InSine)
         );

        animSequence.Append(
            _rectTransform.DOScale(1.0f, 0.25f).SetEase(Ease.OutSine)
        );
    }
}

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



