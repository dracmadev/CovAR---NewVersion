using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetPasswordBehaviout : MonoBehaviour
{
    [Header("CREATE USER")]
    [Space()]
    [SerializeField] private TMP_InputField _EmailSetAndSendPassword;
    [SerializeField] private TMP_InputField _CurrentPassword;
    [SerializeField] private TMP_InputField _NewPassword;
    [SerializeField] private TMP_InputField _EmailSetPassword;
    [SerializeField] private Button _SendAndSetPassword;
    [SerializeField] private Button _UpdatePassword;
    [SerializeField] private HelperTextBehaviourScript _HelperText;

    private WebRequestManager _WebRequestManager;

    void Start()
    {
        InitSetPassword();
    }

    // Update is called once per frame
    void Update()
    {
        OnSetAndSendBehaviour();
        OnSetBehaviour();

        
    }

    //////////////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////

    void InitSetPassword()
    {
        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        _SendAndSetPassword.onClick.AddListener(OnClickSendAndSetPassword);
        _UpdatePassword.onClick.AddListener(OnClickSetPassword);

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////// BEHAVIOUR ///////////////////////////////////////////////////////////////

    void OnSetAndSendBehaviour()
    {
        if(_EmailSetAndSendPassword.text != "")
        {
            _SendAndSetPassword.interactable = true;
        }
        else
        {
            _SendAndSetPassword.interactable = false;
        }
    }

    void OnSetBehaviour()
    {
        if (_EmailSetPassword.text != "" && _CurrentPassword.text != "" && _NewPassword.text != "")
        {
            _UpdatePassword.interactable = true;
        }
        else
        {
            _UpdatePassword.interactable = false;
        }
    }

    void OnClickSendAndSetPassword()
    {
        if (_WebRequestManager != null)
        {
            _WebRequestManager.WR_SetAndSendPassword(_EmailSetAndSendPassword.text, _HelperText);
        }
    }

    void OnClickSetPassword()
    {
        if (_WebRequestManager != null)
        {
            if(!CheckIfThisBothCampsAreEqual(_CurrentPassword.text, _NewPassword.text))
            {
                if (CheckIfPasswordIsCorrect(_NewPassword.text))
                {
                    _WebRequestManager.WR_OnClickSetPassword(_EmailSetPassword.text, _CurrentPassword.text, _NewPassword.text, _HelperText);
                }
                else
                {
                    _HelperText.ShowLoginIncorrectMessage("NewPasswordNotCorrect");
                }
            }
            else
            {
                _HelperText.ShowLoginIncorrectMessage("SamePassword");
            }
            
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
