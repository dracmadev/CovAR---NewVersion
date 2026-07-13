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
    [SerializeField] static private E_CompanyType _CurrentAppCompany = E_CompanyType.None;

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
        StartCoroutine(WR_LoginCoroutine(email, password, _HelperText));
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

    IEnumerator WR_LoginCoroutine(string email, string password, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(WR_GetCompanyByLeadsCoroutine(email, _HelperText));
        yield return new WaitForSeconds(2f);

        if (_CurrentAppCompany == E_CompanyType.Astralpool)
        {
            Debug.Log("Login user from -> Astralpool");
            StartCoroutine(WR_LoginAstralpoolCoroutine(email, password, _HelperText));
        }
        else if (_CurrentAppCompany == E_CompanyType.BACPoolSystems)
        {
            Debug.Log("Login user from -> BACPoolSystems");
            //StartCoroutine(WR_LoginBacPoolSystemsCoroutine(email, password, _HelperText));
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////// CHEK COMPANY BY LEADS ////////////////////////////////////////////////////////////

    IEnumerator WR_GetCompanyByLeadsCoroutine(string email, HelperTextBehaviourScript _HelperText)
    {
        if (email != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("correo", email);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._GetUserCompanyURL, form))
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

                    if (echo.StartsWith("Astralpool"))
                    {
                        _CurrentAppCompany = E_CompanyType.Astralpool;
                    }
                    else if (echo == "BACPoolSystems")
                    {
                        _CurrentAppCompany = E_CompanyType.BACPoolSystems;
                    }
                    else if (echo == "NotInLeads")
                    {
                        _HelperText.ShowLoginIncorrectMessage("NotInLeads");
                    }
                    else
                    {
                        _HelperText.ShowLoginIncorrectMessage("Error");
                    }
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// CREATE USER //////////////////////////////////////////////////////////////////////
    
    public void WR_CreateUser(string name, string surname, string email, string password, string country, string phone, string company, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(WR_CreateUserDependingOnLeads(name, surname, email, password, country, phone, company, _HelperText));
    }

    IEnumerator WR_CreateUserDependingOnLeads(string name, string surname, string email, string password, string country, string phone, string company, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(WR_GetCompanyByLeadsCoroutine(email, _HelperText));
        yield return new WaitForSeconds(2f);

        if (_CurrentAppCompany == E_CompanyType.Astralpool)
        {
            Debug.Log("Lead from -> Astralpool");
            StartCoroutine(WR_CreateUserAstralpool(name, surname, email, password, country, phone, company, _HelperText));
        }
        else if (_CurrentAppCompany == E_CompanyType.BACPoolSystems)
        {
            Debug.Log("Lead from -> BACPoolSystems");
        }
    }

    IEnumerator WR_CreateUserAstralpool(string name, string surname, string email, string password, string country, string phone, string company, HelperTextBehaviourScript _HelperText)
    {
        if (name != "" && surname != "" && email != "" && password != "" && country != "" && phone != "" && company != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("nombre", name);
            form.AddField("apellidos", surname);
            form.AddField("correo", email);
            form.AddField("login", password);
            form.AddField("country", country);
            form.AddField("telefono", phone);
            form.AddField("empresa", company);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._CreateUserURL_Astralpool, form))
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
                   
                    if (echo.StartsWith("New record created successfully"))
                    {
                       
                    }
                    else if (echo == "UserNotInLeads")
                    {
                        _HelperText.ShowLoginIncorrectMessage("NotInLeads");
                    }
                    else
                    {
                        _HelperText.ShowLoginIncorrectMessage("Error");
                    }
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
