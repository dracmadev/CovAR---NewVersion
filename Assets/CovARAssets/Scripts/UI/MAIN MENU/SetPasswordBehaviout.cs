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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
