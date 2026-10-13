using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleMainMenuLoginBehaviour : MonoBehaviour
{
    [Header("LOGIN")]
    [Space()]
    [SerializeField] private TMP_InputField _EmailName;
    [SerializeField] private Button _LoginButton;
    [SerializeField] private TMP_Dropdown _CountryDropdown;
    [SerializeField] private Button _AcceptCountry;
    [SerializeField] private CanvasGroup _LoginCG;
    [SerializeField] private CanvasGroup _SetCountryCG;
    [SerializeField] private HelperTextBehaviourScript _HelperText;


    private WebRequestManager _WebRequestManager;

    void Start()
    {
        InitLoginBehaviour();
    }

    // Update is called once per frame
    void Update()
    {
        LoginButtonBehaviour();
    }

    //////////////////////////////////////////////////////// INIT /////////////////////////////////////////////////////////////

    void InitLoginBehaviour()
    {
        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        _LoginButton.onClick.AddListener(OnClickLoginBehaviour);

        _AcceptCountry.onClick.AddListener(OnClickSetCountryBehaviour);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// BEHAVIOUR ///////////////////////////////////////////////////////////

    void LoginButtonBehaviour()
    {
        if (_EmailName.text != "")
        {
            _LoginButton.interactable = true;
        }
        else
        {
            _LoginButton.interactable = false;
        }

    }

    void OnClickLoginBehaviour()
    {
        if (_WebRequestManager != null)
        {
            _WebRequestManager.WR_Login(_EmailName.text, "", _HelperText, _LoginButton);
        }

    }

    void OnClickSetCountryBehaviour()
    {
        if (_WebRequestManager != null)
        {
            _WebRequestManager.WR_SetCountry(GetDropdownValueAsString(), _HelperText);
        }
    }

   

    string GetDropdownValueAsString()
    {
        string answer = "None";

        switch (_CountryDropdown.value)
        {
            case 0: answer = "Spain"; break;
            case 1: answer = "Portugal"; break;
            case 2: answer = "France"; break;
        }
        return answer;
    }


    public void SetLoginCGState(bool newState)
    {
        _LoginCG.DOKill();

        _LoginCG.interactable = newState;
        _LoginCG.blocksRaycasts = newState;

        _LoginCG.DOFade(newState ? 1f : 0f, 0.2f).SetEase(Ease.OutSine);
    }

    public void SetCountryCGState(bool newState)
    {
        _SetCountryCG.DOKill();

        _SetCountryCG.interactable = newState;
        _SetCountryCG.blocksRaycasts = newState;

        _SetCountryCG.DOFade(newState ? 1f : 0f, 0.2f).SetEase(Ease.OutSine);
    }


   
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
