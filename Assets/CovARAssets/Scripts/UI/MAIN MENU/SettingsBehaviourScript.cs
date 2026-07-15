using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsBehaviourScript : MonoBehaviour
{
    [Header("SETTINGS")]
    [Space()]
    [SerializeField] private TMP_InputField _EmailDeleteUser;
    [SerializeField] private Button _DeleteUserButton;
    [SerializeField] private HelperTextBehaviourScript _HelperText;
    private WebRequestManager _WebRequestManager;
    void Start()
    {
        InitDeleteUser();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ////////////////////////////////////////////////////////////////////////// INIT /////////////////////////////////////////////////////////////////////////

    void InitDeleteUser()
    {
        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        _DeleteUserButton.onClick.AddListener(OnClickDeleteUser);

    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////////////////////////////
    
    void OnClickDeleteUser()
    {
        if(_WebRequestManager != null)
        {
            _WebRequestManager.WR_OnDeleteUser(_EmailDeleteUser.text, _HelperText);
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
