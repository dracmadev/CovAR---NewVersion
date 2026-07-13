using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLoginBehaviour : MonoBehaviour
{
    [Header("LOGIN")]
    [Space()]
    [SerializeField] private TMP_InputField _EmailName;
    [SerializeField] private TMP_InputField _PasswordName;
    [SerializeField] private Button _LoginButton;
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
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// BEHAVIOUR ///////////////////////////////////////////////////////////

    void LoginButtonBehaviour()
    {
        if (_EmailName.text != "" && _PasswordName.text != "")
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
            _WebRequestManager.WR_Login(_EmailName.text, _PasswordName.text, _HelperText);
        }

    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
