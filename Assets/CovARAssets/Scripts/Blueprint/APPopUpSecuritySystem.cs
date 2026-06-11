using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class APPopUpSecuritySystem : MonoBehaviour
{
    [Header("Security System Pop-Up")]
    [Space()]
    [SerializeField] private CanvasGroup _CGSecuritySystemPopUp;
    [SerializeField] private Image _CurrentSecuritySystemImage;
    [SerializeField] private LocalizedTMPText _CurrentSecuritySystemText;
    [SerializeField] private LocalizedTMPText _CurrentSecuritySystemLocalizedText;
    [SerializeField] private Button _GoLeftButton;
    [SerializeField] private Button _GoRightButton;
    [SerializeField] private Button _AcceptButton;
    [Header("List of security systems:")]
    [SerializeField] private St_SecuritySystem[] _SecuritySystemsArray;

    int currentIndex = 0;

    /////////////////////////////////////////////////////////////////// DEFAULT FUNCTIONS //////////////////////////////////////////////////////////////////////////
    void Start()
    {
        InitSecuritySystemPopUp();
    }

    void Update()
    {

    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////////////

    public void InitSecuritySystemPopUp()
    {
        UpdateSecuritySystem();

        _GoLeftButton.onClick.RemoveListener(OnClickGoLeft);
        _GoLeftButton.onClick.AddListener(OnClickGoLeft);

        _GoRightButton.onClick.RemoveListener(OnClickGoRight);
        _GoRightButton.onClick.AddListener(OnClickGoRight);

        _AcceptButton.onClick.RemoveListener(OnClickCloseSecuritySystemPopUp);
        _AcceptButton.onClick.AddListener(OnClickCloseSecuritySystemPopUp);

        HideSecuritySystemAnim();
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// SETTER /////////////////////////////////////////////////////////////////////////




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// BEHAVIOUR ///////////////////////////////////////////////////////////////////////

    public void OnClickOpenSecuritySystemPopUp()
    {
        ShowSecurityStstemAnim();
    }

    public void OnClickCloseSecuritySystemPopUp()
    {
        HideSecuritySystemAnim();
    }

    void ShowSecurityStstemAnim()
    {
        _CGSecuritySystemPopUp.alpha = 0f;

        _CGSecuritySystemPopUp.interactable = true;
        _CGSecuritySystemPopUp.blocksRaycasts = true;

        _CGSecuritySystemPopUp.DOFade(1f, 0.3f).SetEase(Ease.OutSine);  
    }

    void HideSecuritySystemAnim()
    {
        _CGSecuritySystemPopUp.interactable = false;
        _CGSecuritySystemPopUp.blocksRaycasts = false;

        _CGSecuritySystemPopUp.DOFade(0f, 0.3f).SetEase(Ease.OutSine);
    }

    public void OnClickGoRight()
    {
        if (currentIndex < _SecuritySystemsArray.Length - 1)
        {
            currentIndex++; 
        }
        else
        {
            currentIndex = 0;
        }

        UpdateSecuritySystem();
    }
    public void OnClickGoLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = _SecuritySystemsArray.Length - 1;
        }

        UpdateSecuritySystem();
    }

    void UpdateSecuritySystem()
    {
        _CurrentSecuritySystemImage.sprite = _SecuritySystemsArray[currentIndex]._SSImage;
        _CurrentSecuritySystemText.LocalizationKey = _SecuritySystemsArray[currentIndex]._SSKey;
        _CurrentSecuritySystemText.Localize();

        _CurrentSecuritySystemLocalizedText.LocalizationKey = _SecuritySystemsArray[currentIndex]._SSKey;
        _CurrentSecuritySystemLocalizedText.Localize();
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
