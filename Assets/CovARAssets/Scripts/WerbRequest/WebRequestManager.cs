using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestManager : MonoBehaviour
{
    [Header("WEB REQUEST MANAGER")]
    [Space()]
    [SerializeField] private bool bIsOnARScene = false;
    [Header("WEB REQUEST LINKS")]
    [SerializeField] private St_WebRequestLinkStruct _WebRequestLinkStruct;
    [Header("Current company")]
    [SerializeField] static private E_CompanyType _CurrentAppCompany;

    HUDManagerScript _HUDManagerScript;
    ARObjectManager _ARObjectManager;


    void Start()
    {
        InitWebRequestManager(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///////////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////
    
    void InitWebRequestManager()
    {
        if(GameObject.FindGameObjectWithTag("HUD"))
        {
            _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
        }

        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

    }




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    ////////////////////////////////////////////////////////////// LOGIN ////////////////////////////////////////////////////////////////////
    
    public void WR_Login(string email, string password, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(WR_LoginAstralpoolCoroutine(email, password, _HelperText));
    }

    IEnumerator WR_LoginAstralpoolCoroutine(string email, string password, HelperTextBehaviourScript _HelperText)
    {
        if (email != "" || password != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("correo", email);
            form.AddField("loginPass", password);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._LoginLinkURL_Astralpool, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
                else
                {
                    Debug.Log("[MISSATGE DE PHP]  " + www.downloadHandler.text);
                    string echo = www.downloadHandler.text;

                    PlayerPrefs.SetString("UserEmail", email);
                    PlayerPrefs.SetString("UserPassword", password);
                    PlayerPrefs.Save();

                    if (echo.StartsWith("loginCorrecto"))
                    {
                        _HUDManagerScript.OnClickMoveToScene("Scene_ARScene");
                    }
                    else if (echo == "loginIncorrecto")
                    {
                        _HelperText.ShowLoginIncorrectMessage("loginIncorrecto");   
                    }
                    else
                    {
                        _HelperText.ShowLoginIncorrectMessage("Error");
                    }
                }
            }
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
