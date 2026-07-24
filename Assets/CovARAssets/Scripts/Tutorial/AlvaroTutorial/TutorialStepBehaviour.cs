using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStepBehaviour : MonoBehaviour
{
    [Header("TUTORIAL STEP")]
    [Space()]
    [Header("Step base info:")]
    [SerializeField] private int _StepID;
    [SerializeField] private int currentTutorialInfoNumber = 0;
    [SerializeField] private St_TutorialInfoData[] _StepInfoData;
    [Header("InGame UI You need for this Step:")]
    [SerializeField] private Button _InGameButton01;
    [SerializeField] private Button _InGameButton02;
    [SerializeField] private Button _InGameButton03;
    [SerializeField] private Slider _InGameSlider;
    [Header("Tutorial Step UI:")]
    [SerializeField] private LocalizedTMPText _TitleText;
    [SerializeField] private LocalizedTMPText _DescripText;
    [SerializeField] private Button _ContinueButton;
    [SerializeField] private Button _PreviousButton;
    [SerializeField] private Button _CancelTutorialButton;
    [SerializeField] private GameObject _NoInteractPanel;

    TutorialStepManager _TutorialStepManager;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    ///////////////////////////////////////////////////////////////////////////////////// INIT /////////////////////////////////////////////////////////////////////////////////////////////
    
    public void InitStep(TutorialStepManager tutorialStepManager)
    {

        _TutorialStepManager = tutorialStepManager;

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
        if(_StepInfoData.Length != 0 && (_StepInfoData.Length -1) >= index)
        {
            SetTitleText(_StepInfoData[index]._TitleKey);
            SetDescripText(_StepInfoData[index]._DescripKey);
            _NoInteractPanel.SetActive(_StepInfoData[index].bNoInteractPanelIsActive);
            _ContinueButton.gameObject.SetActive(_StepInfoData[index].bContinueButtonIsActive);

            //_TutorialStepManager.SetStepCurrentState(_StepInfoData[index].currentStepState);
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////////////////////////

    public int GetCurrentStepNumber()
    {
        return currentTutorialInfoNumber;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////////////////////////////////

    void OnClickContinueButton()
    {
        if (currentTutorialInfoNumber < (_StepInfoData.Length - 1))
        {
            currentTutorialInfoNumber++;
            SetTutorialInfo(currentTutorialInfoNumber);
        }
        else if (currentTutorialInfoNumber == (_StepInfoData.Length - 1))
        {
            // Tutorial Step Finished
        }
    }

    void OnClickPreviousButton()
    {
        if(currentTutorialInfoNumber >= (_StepInfoData.Length -1))
        {
            currentTutorialInfoNumber--;
            SetTutorialInfo(currentTutorialInfoNumber);
        }
        else if (currentTutorialInfoNumber == 0)
        {
            // Tutorial Step Finished
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
