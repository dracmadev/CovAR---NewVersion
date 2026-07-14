using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateUserBehaviour : MonoBehaviour
{
    [Header("CREATE USER")]
    [Space()]
    [SerializeField] private TMP_Dropdown _UserCountry;
    [SerializeField] private TMP_InputField _UserName;
    [SerializeField] private TMP_InputField _UserSurname;
    [SerializeField] private TMP_InputField _UserPhone;
    [SerializeField] private TMP_InputField _UserCompany;
    [SerializeField] private TMP_InputField _EmailName;
    [SerializeField] private TMP_InputField _EmailRepName;
    [SerializeField] private TMP_InputField _PasswordName;
    [SerializeField] private TMP_InputField _PasswordRepName;
    [SerializeField] private Button _CreateUserButton;
    [SerializeField] private Button _ReturnButton;
    [SerializeField] private HelperTextBehaviourScript _HelperText;

    private WebRequestManager _WebRequestManager;

    void Start()
    {
        InitCreateUser();
    }

    // Update is called once per frame
    void Update()
    {
        CreateButtonUpdateBehaviour();
    }

    //////////////////////////////////////////////////////////////////////// INIT ////////////////////////////////////////////////////////////////////////
    
    void InitCreateUser()
    {
        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        _CreateUserButton.onClick.AddListener(OnClickCreateUser);
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////////////
    
    void CreateButtonUpdateBehaviour()
    {
        if(_UserName.text != "" && _UserSurname.text != "" && _UserCompany.text != "" && _EmailName.text != "" && _EmailRepName.text != "" && _PasswordName.text != "" && _PasswordRepName.text != "")
        {
            _CreateUserButton.interactable = true;
        }
        else
        {
            _CreateUserButton.interactable = false;
        }
    }

    bool CheckIfPasswordIsCorrect(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
    }

    bool CheckIfThisBothCampsAreEqual(string camp1, string camp2)
    {
        return camp1 == camp2;
    }


    void OnClickCreateUser()
    {
        if(CheckIfThisBothCampsAreEqual(_EmailName.text, _EmailRepName.text))
        {
            if(CheckIfThisBothCampsAreEqual(_PasswordName.text, _PasswordRepName.text))
            {
                if(CheckIfPasswordIsCorrect(_PasswordName.text))
                {
                    string cleanEmail = _EmailName.text.Trim();
                    string country = _UserCountry.options[_UserCountry.value].text;

                    _WebRequestManager.WR_CreateUser(_UserName.text, _UserSurname.text, cleanEmail, _PasswordName.text, country, _UserPhone.text, _UserCompany.text, _HelperText, _ReturnButton, _CreateUserButton);
                }
                else
                {
                    _HelperText.ShowLoginIncorrectMessage("NoCumpleRequisitos");
                }
            }
            else
            {
                _HelperText.ShowLoginIncorrectMessage("PasswordNoCoinciden");
            }
        }
        else
        {
            // Show error message
            _HelperText.ShowLoginIncorrectMessage("EmailNoCoinciden");
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
