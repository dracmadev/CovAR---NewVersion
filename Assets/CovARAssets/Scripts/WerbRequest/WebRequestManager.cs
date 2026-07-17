using Assets.SimpleLocalization.Scripts;
using System.Collections;
using System.Drawing;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class WebRequestManager : MonoBehaviour
{
    [Header("WEB REQUEST MANAGER")]
    [Space()]
    [SerializeField] private bool bIsOnARScene = false;
    [Header("WEB REQUEST LINKS")]
    [SerializeField] private St_WebRequestLinkStruct _WebRequestLinkStruct;
    [Header("Current company")]
    [SerializeField] static private E_CompanyType _CurrentAppCompany = E_CompanyType.Astralpool;
    [Header("Order Data:")]
    [SerializeField] private St_OrderData _OrderData;
    [Header("ASTRALPOOL object data:")]
    [SerializeField] private St_AstralpoolProductData _AstralpoolObjectData;


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

    /////////////////////////////////////////////////////////// GETTERS //////////////////////////////////////////////////////////////////////

    public E_CompanyType GetCurrentCompany()
    {
        return _CurrentAppCompany;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////// SETTERS //////////////////////////////////////////////////////////////////////

    //ORDER SETTERS
    public void SetUserID(int id)
    {
        _OrderData._UserID = id;
    }
    public void SetStockID(int id)
    {
        _OrderData._StockID = id;
    }
    public void SetProductID(int id)
    {
        _OrderData._ProductID = id;
    }
    public void SetBasicOrderData(St_OrderData orderData)
    {
        _OrderData._CustomerName = orderData._CustomerName;
        _OrderData._CustomerMail = orderData._CustomerMail;
        _OrderData._PostCode = orderData._PostCode;
        _OrderData._Country = orderData._Country;
        _OrderData._CurrentDate = orderData._CurrentDate;
    }

    public void SetModelBasicInfo(string modelName, string modelColor)
    {
        _AstralpoolObjectData._ModelName = modelName;
        _AstralpoolObjectData._ModelColor = modelColor;
    }
   
    public void SetLamasBasicInfo(string lamasMat, string lamasColor)
    {
        _AstralpoolObjectData._LamasMaterial = lamasMat;
        _AstralpoolObjectData._LamasColor = lamasColor;
    }
    public void SetTopCladdingColor(string topCladdingColor)
    {
        _AstralpoolObjectData._TopCladdingColor = topCladdingColor;
    }
    public void SetSidesCladdingColor(string sides)
    {
        _AstralpoolObjectData._SidesCladdingColor = sides;
    }
    public void SetDimensions(string dimensions)
    {
        _AstralpoolObjectData._Dimensions = dimensions;
    }
    public void SetLedLights(string ledLights)
    {
        _AstralpoolObjectData._LedLights = ledLights;
    }
    public void SetElectrosisContact(string electrosisContact)
    {
        _AstralpoolObjectData._ElectrosisContact = electrosisContact;
    }
    public void SetCoverConnect(string coverConnect)
    {
        _AstralpoolObjectData._CoverConnect = coverConnect;
    }
    public void SetPoolMaterial(string poolMaterial)
    {
        _AstralpoolObjectData._PoolMaterial = poolMaterial;
    }
    public void SetSecuritySystem(string securitySystem)
    {
        _AstralpoolObjectData._SecuritySystem = securitySystem;
    }
    public void SetSubmergedModel(string submergedModel)
    {
        _AstralpoolObjectData._SubmergedModel = submergedModel;
    }
    public void SetMecanicsType(string mecanicsType)
    {
        _AstralpoolObjectData._MecanicsType = mecanicsType;
    }
    public void SetBeamType(string beamType)
    {
        _AstralpoolObjectData._BeamType = beamType;
    }
    public void SetCoverType(string coverType)
    {
        _AstralpoolObjectData._CoverType = coverType;
    }
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                    Debug.Log("[MISSATGE DE PHP] AstralPool Login: " + www.downloadHandler.text);
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

    IEnumerator WR_LoginBacPoolSystemCoroutine(string email, string password, HelperTextBehaviourScript _HelperText)
    {
        if (email != "" || password != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("correo", email);
            form.AddField("loginPass", password);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._LoginLinkURL_BacPoolSystem, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
                else
                {
                    Debug.Log("[MISSATGE DE PHP] AstralPool Login: " + www.downloadHandler.text);
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
            StartCoroutine(WR_LoginBacPoolSystemCoroutine(email, password, _HelperText));
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
                    Debug.Log("[MISSATGE DE PHP] GetCompanyByLeadsCoroutine: " + www.downloadHandler.text);
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
                        _CurrentAppCompany = E_CompanyType.None;
                    }
                    else
                    {
                        _HelperText.ShowLoginIncorrectMessage("Error");
                        _CurrentAppCompany = E_CompanyType.None;
                    }
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// CREATE USER //////////////////////////////////////////////////////////////////////
    
    public void WR_CreateUser(string name, string surname, string email, string password, string country, string phone, string company, HelperTextBehaviourScript _HelperText, Button returnButton, Button createUserButton)
    {
        returnButton.interactable = false;
        createUserButton.gameObject.SetActive(false);

        StartCoroutine(WR_CreateUserDependingOnLeads(name, surname, email, password, country, phone, company, _HelperText, returnButton, createUserButton));
    }

    IEnumerator WR_CreateUserDependingOnLeads(string name, string surname, string email, string password, string country, string phone, string company, HelperTextBehaviourScript _HelperText, Button returnButton, Button createUserButton)
    {
        StartCoroutine(WR_GetCompanyByLeadsCoroutine(email, _HelperText));
        yield return new WaitForSeconds(2f);

        if (_CurrentAppCompany == E_CompanyType.Astralpool)
        {
            Debug.Log("Lead from -> Astralpool");
            StartCoroutine(WR_CreateUserAstralpool(name, surname, email, password, country, phone, company, _HelperText, returnButton, createUserButton));
        }
        else if (_CurrentAppCompany == E_CompanyType.BACPoolSystems)
        {
            Debug.Log("Lead from -> BACPoolSystems");
            StartCoroutine(WR_CreateUserBacPoolSystem(name, surname, email, password, country, phone, company, _HelperText, returnButton, createUserButton));
        }
        else if (_CurrentAppCompany == E_CompanyType.None)
        {
            Debug.Log("Lead from -> None");
            returnButton.interactable = true;
            createUserButton.gameObject.SetActive(true);
        }
    }

    IEnumerator WR_CreateUserAstralpool(string name, string surname, string email, string password, string country, string phone, string company, HelperTextBehaviourScript _HelperText, Button returnButton, Button createUserButton)
    {
        if (name != "" && surname != "" && email != "" && password != "" && country != ""  && company != "")
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
                    returnButton.interactable = true;
                    createUserButton.gameObject.SetActive(true);
                }
                else
                {

                    Debug.Log("[MISSATGE DE PHP] CreateUser: " + www.downloadHandler.text);
                    string echo = www.downloadHandler.text;
                   
                    if (echo == "UserNotInLeads")
                    {
                        _HelperText.ShowLoginIncorrectMessage("NotInLeads");
                        returnButton.interactable = true;
                        createUserButton.gameObject.SetActive(true);
                    }
                    else if (echo == "correo is already taken")
                    {
                        _HelperText.ShowLoginIncorrectMessage("EmailAlreadyOnBBDD");
                        returnButton.interactable = true;
                        createUserButton.gameObject.SetActive(true);
                    }
                    else if (echo == "Creating user...New record created successfully")
                    {
                        returnButton.interactable = true;
                        createUserButton.gameObject.SetActive(true);
                        _HUDManagerScript.TravelToPanel("LoginPanel");

                    }

                }
            }
        }
    }

    IEnumerator WR_CreateUserBacPoolSystem(string name, string surname, string email, string password, string country, string phone, string company, HelperTextBehaviourScript _HelperText, Button returnButton, Button createUserButton)
    {
        if (name != "" && surname != "" && email != "" && password != "" && country != "" && company != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("nombre", name);
            form.AddField("apellidos", surname);
            form.AddField("correo", email);
            form.AddField("login", password);
            form.AddField("country", country);
            form.AddField("telefono", phone);
            form.AddField("empresa", company);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._CreateUserURL_BacPoolSystem, form))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    _HelperText.ShowLoginIncorrectMessage("Error");
                    returnButton.interactable = true;
                    createUserButton.gameObject.SetActive(true);
                }
                else
                {

                    Debug.Log("[MISSATGE DE PHP] CreateUser: " + www.downloadHandler.text);
                    string echo = www.downloadHandler.text;

                    if (echo == "UserNotInLeads")
                    {
                        _HelperText.ShowLoginIncorrectMessage("NotInLeads");
                        returnButton.interactable = true;
                        createUserButton.gameObject.SetActive(true);
                    }
                    else if (echo == "correo is already taken")
                    {
                        _HelperText.ShowLoginIncorrectMessage("EmailAlreadyOnBBDD");
                        returnButton.interactable = true;
                        createUserButton.gameObject.SetActive(true);
                    }
                    else if (echo == "Creating user...New record created successfully")
                    {
                        returnButton.interactable = true;
                        createUserButton.gameObject.SetActive(true);
                        _HUDManagerScript.TravelToPanel("LoginPanel");

                    }

                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// SET AND SEND PASSWORD ////////////////////////////////////////////////////////////////////

    public void WR_SetAndSendPassword(string email, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(SetAndSendPasswordCoroutine(email, _HelperText));
    }

    IEnumerator SetAndSendPasswordCoroutine(string email, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(WR_GetCompanyByLeadsCoroutine(email, _HelperText));
        yield return new WaitForSeconds(2f);

        if (_CurrentAppCompany == E_CompanyType.Astralpool)
        {
            Debug.Log("Lead from -> Astralpool");
            WR_SetAndSentAstralpoolPassword(email, _HelperText);
        }
        else if (_CurrentAppCompany == E_CompanyType.BACPoolSystems)
        {
            Debug.Log("Lead from -> BACPoolSystems");
            WR_SetAndSentBacPoolPassword(email, _HelperText);

        }
        else if (_CurrentAppCompany == E_CompanyType.None)
        {
            Debug.Log("Lead from -> None"); 
        }
    }


    public void WR_SetAndSentAstralpoolPassword(string email, HelperTextBehaviourScript _HelperText)
    {
        if (!string.IsNullOrEmpty(email))
        {
            //Generar nova contrasenya
            string newPassword = GeneratePassword(8);
            Debug.Log("[SEND MAIL] -> new password: " + newPassword);

            //Localize texts
            string emailTitle = LocalizationManager.Localize("Login.ReSetPassword.Mail.Title");
            string emailBody = LocalizationManager.Localize("Login.ReSetPassword.Mail.Body") + " " + newPassword;

            Debug.Log("[SEND MAIL] -> Mail prepared: " + email);

            //Set new password on Nominalia
            StartCoroutine(SendAndResetPasswordAstralpoolCoroutine(email, newPassword, emailTitle, emailBody, _HelperText));
        }
    }

    public IEnumerator SendAndResetPasswordAstralpoolCoroutine(string email, string newPass, string title, string body, HelperTextBehaviourScript _HelperText)
    {

        WWWForm form = new WWWForm();
        form.AddField("correo", email);
        form.AddField("newPass", newPass);
        form.AddField("subject", title);
        form.AddField("body", body);

        using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._SetAndSendNewPasswordURL_Astralpool, form))
        {
            www.timeout = 15;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                _HelperText.ShowLoginIncorrectMessage("Error");
            }
            else
            {
                Debug.Log("[MISSATGE DE PHP]  SendAndResetPassword: " + www.downloadHandler.text);

                if (www.downloadHandler.text == "MailSended")
                {
                    _HelperText.ShowLoginCorrectMessage("CorreoEnviado");
                }
                else
                {
                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
            }
        }
    }

    public void WR_SetAndSentBacPoolPassword(string email, HelperTextBehaviourScript _HelperText)
    {
        if (!string.IsNullOrEmpty(email))
        {
            //Generar nova contrasenya
            string newPassword = GeneratePassword(8);
            Debug.Log("[SEND MAIL] -> new password: " + newPassword);

            //Localize texts
            string emailTitle = LocalizationManager.Localize("Login.ReSetPassword.Mail.Title");
            string emailBody = LocalizationManager.Localize("Login.ReSetPassword.Mail.Body") + " " + newPassword;

            Debug.Log("[SEND MAIL] -> Mail prepared: " + email);

            //Set new password on Nominalia
            StartCoroutine(SendAndResetPasswordBacPoolCoroutine(email, newPassword, emailTitle, emailBody, _HelperText));
        }
    }

    public IEnumerator SendAndResetPasswordBacPoolCoroutine(string email, string newPass, string title, string body, HelperTextBehaviourScript _HelperText)
    {
        WWWForm form = new WWWForm();
        form.AddField("correo", email);
        form.AddField("newPass", newPass);
        form.AddField("subject", title);
        form.AddField("body", body);

        using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._SetAndSendNewPasswordURL_BacPoolSystem, form))
        {
            www.timeout = 15;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                _HelperText.ShowLoginIncorrectMessage("Error");
            }
            else
            {
                Debug.Log("[MISSATGE DE PHP]  SendAndResetPassword: " + www.downloadHandler.text);

                if (www.downloadHandler.text == "MailSended")
                {
                    _HelperText.ShowLoginCorrectMessage("CorreoEnviado");
                }
                else
                {
                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
            }
        }
    }


    public static string GeneratePassword(int length = 8)
    {
        char[] letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] numbers = "0123456789".ToCharArray();
        char[] symbols = "!@#$%&*?-_+=".ToCharArray();

        string allChars = new string(letters) + new string(numbers) + new string(symbols);

        StringBuilder password = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int rand = Random.Range(0, allChars.Length);
            password.Append(allChars[rand]);
        }

        return password.ToString();
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////// SET PASSWORD /////////////////////////////////////////////////////////////////////////

    public void WR_OnClickSetPassword(string email, string oldPass, string newPass, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(SetPasswordCoroutine(email, oldPass, newPass, _HelperText));
    }

    IEnumerator SetPasswordCoroutine(string email, string oldPass, string newPass, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(WR_GetCompanyByLeadsCoroutine(email, _HelperText));
        yield return new WaitForSeconds(2f);

        if (_CurrentAppCompany == E_CompanyType.Astralpool)
        {
            Debug.Log("Lead from -> Astralpool");
            StartCoroutine(SetPasswordAstralpoolCoroutine(email, oldPass, newPass, _HelperText));
        }
        else if (_CurrentAppCompany == E_CompanyType.BACPoolSystems)
        {
            Debug.Log("Lead from -> BACPoolSystems");
            StartCoroutine(SetPasswordBacPoolCoroutine(email, oldPass, newPass, _HelperText));
        }
        else if (_CurrentAppCompany == E_CompanyType.None)
        {
            Debug.Log("Lead from -> None");
        }
    }


    public IEnumerator SetPasswordAstralpoolCoroutine(string email, string oldPass, string newPass, HelperTextBehaviourScript _HelperText)
    {
        WWWForm form = new WWWForm();
        form.AddField("correo", email);
        form.AddField("loginPass", oldPass);
        form.AddField("newloginPass", newPass);

        using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._SetPasswordManuallyURL_Astralpool, form))
        {
            yield return www.SendWebRequest();

            // Netegem la resposta de PHP d'espais en blanc o salts de línia residuals
            string responseText = www.downloadHandler.text.Trim();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[ERROR XARXA/SERVIDOR] Result: {www.result} | Error: {www.error}");
                Debug.Log("[TEXT REBUT TOT I L'ERROR]: " + responseText);

                _HelperText.ShowLoginIncorrectMessage("Error");
            }
            else
            {
                Debug.Log("[MISSATGE DE PHP] SetPassword: " + responseText);

                if (responseText == "PasswordChanged")
                {
                    _HelperText.ShowLoginCorrectMessage("PasswordChangedCorrectly");
                }
                else if (responseText == "UserNotFound")
                {
                    _HelperText.ShowLoginIncorrectMessage("UserNotFound");
                }
                else
                {
                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
            }
        }
    }

    public IEnumerator SetPasswordBacPoolCoroutine(string email, string oldPass, string newPass, HelperTextBehaviourScript _HelperText)
    {
        WWWForm form = new WWWForm();
        form.AddField("correo", email);
        form.AddField("loginPass", oldPass);
        form.AddField("newloginPass", newPass);

        using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._SetPasswordManuallyURL_BacPoolSystem, form))
        {
            yield return www.SendWebRequest();

            // Netegem la resposta de PHP d'espais en blanc o salts de línia residuals
            string responseText = www.downloadHandler.text.Trim();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[ERROR XARXA/SERVIDOR] Result: {www.result} | Error: {www.error}");
                Debug.Log("[TEXT REBUT TOT I L'ERROR]: " + responseText);

                _HelperText.ShowLoginIncorrectMessage("Error");
            }
            else
            {
                Debug.Log("[MISSATGE DE PHP] SetPassword: " + responseText);

                if (responseText == "PasswordChanged")
                {
                    _HelperText.ShowLoginCorrectMessage("PasswordChangedCorrectly");
                }
                else if (responseText == "UserNotFound")
                {
                    _HelperText.ShowLoginIncorrectMessage("UserNotFound");
                }
                else
                {
                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////// DELETE USER /////////////////////////////////////////////////////////////////////////
    
    public void WR_OnDeleteUser(string email, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(DeleteUserCoroutine(email, _HelperText));
    }

    IEnumerator DeleteUserCoroutine(string email, HelperTextBehaviourScript _HelperText)
    {
        StartCoroutine(WR_GetCompanyByLeadsCoroutine(email, _HelperText));
        yield return new WaitForSeconds(2f);

        if (_CurrentAppCompany == E_CompanyType.Astralpool)
        {
            Debug.Log("Lead from -> Astralpool");
            StartCoroutine(DeleteUserAstralpoolCoroutine(email, _HelperText));
        }
        else if (_CurrentAppCompany == E_CompanyType.BACPoolSystems)
        {
            Debug.Log("Lead from -> BACPoolSystems");
            StartCoroutine(DeleteUserBacPoolCoroutine(email, _HelperText)); 
        }
        else if (_CurrentAppCompany == E_CompanyType.None)
        {
            Debug.Log("Lead from -> None");
        }
    }

    public IEnumerator DeleteUserAstralpoolCoroutine(string email, HelperTextBehaviourScript _HelperText)
    {
        if(email != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("email", email);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._DeleteUserURL_Astralpool, form))
            {
                yield return www.SendWebRequest();

                // Netegem la resposta de PHP d'espais en blanc o salts de línia residuals
                string responseText = www.downloadHandler.text.Trim();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[ERROR XARXA/SERVIDOR] Result: {www.result} | Error: {www.error}");

                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
                else
                {
                    Debug.Log("[MISSATGE DE PHP] Delete user: " + responseText);

                    if (responseText == "AccountDeleted")
                    {
                        _HelperText.ShowLoginCorrectMessage("UserDeletedCorrectly");
                    }
                    else if (responseText == "CamposIncorrectos")
                    {
                        _HelperText.ShowLoginIncorrectMessage("UserNotFound");
                    }
                    else
                    {
                        _HelperText.ShowLoginIncorrectMessage("Error");
                    }
                }
            }
        }
        
    }

    public IEnumerator DeleteUserBacPoolCoroutine(string email, HelperTextBehaviourScript _HelperText)
    {
        if (email != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("email", email);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._DeleteUserURL_BacPoolSystem, form))
            {
                yield return www.SendWebRequest();

                // Netegem la resposta de PHP d'espais en blanc o salts de línia residuals
                string responseText = www.downloadHandler.text.Trim();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[ERROR XARXA/SERVIDOR] Result: {www.result} | Error: {www.error}");

                    _HelperText.ShowLoginIncorrectMessage("Error");
                }
                else
                {
                    Debug.Log("[MISSATGE DE PHP] Delete user: " + responseText);

                    if (responseText == "AccountDeleted")
                    {
                        _HelperText.ShowLoginCorrectMessage("UserDeletedCorrectly");
                    }
                    else if (responseText == "CamposIncorrectos")
                    {
                        _HelperText.ShowLoginIncorrectMessage("UserNotFound");
                    }
                    else
                    {
                        _HelperText.ShowLoginIncorrectMessage("Error");
                    }
                }
            }
        }

    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////// GET USER ID ///////////////////////////////////////////////////////////////////////////////////


    public void OnSaveProjectAndOrderDataFromAstralpool()
    {
        StartCoroutine(OnSaveProjectAndOrderDataFromAstralpoolCoroutine(PlayerPrefs.GetString("UserEmail")));
    }

    IEnumerator OnSaveProjectAndOrderDataFromAstralpoolCoroutine(string email)
    {
        if(_CurrentAppCompany == E_CompanyType.Astralpool)
        {
            StartCoroutine(GetUserIDAstralpool(email));

            switch(_ARObjectManager.GetCovARObjectData()._CurrentModelProduct._ProductType)
            {
                case E_ProductType.GroundRollerCover: _OrderData._StockID = 1; break;
                case E_ProductType.SubmergedRollerCover: _OrderData._StockID = 2; break;
                case E_ProductType.BencheAndCladdings: _OrderData._StockID = 3;  break;
            }

            StartCoroutine(SaveProductDataFromAstralpool());

            yield return new WaitForSeconds(3f);
        }
        else if(_CurrentAppCompany == E_CompanyType.BACPoolSystems)
        {

        }
          

    }


    public IEnumerator GetUserIDAstralpool(string email)
    {
        if (email != "")
        {
            WWWForm form = new WWWForm();
            form.AddField("correo", email);

            using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._GetUserIDURL_Astralpool, form))
            {
                yield return www.SendWebRequest();

                // Netegem la resposta de PHP d'espais en blanc o salts de línia residuals
                string responseText = www.downloadHandler.text.Trim();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[MISSATGE DE PHP] Result: {www.result} | Error: {www.error}");

                }
                else
                {
                    Debug.Log("[MISSATGE DE PHP] GetUserID: " + responseText);

                    if (responseText != "0")
                    {
                        _OrderData._UserID = int.Parse(responseText);
                    }
                   
                }
            }
        }
    }

    public IEnumerator SaveProductDataFromAstralpool()
    {
       
        WWWForm form = new WWWForm();

        form.AddField("_StokID", _OrderData._StockID);
        form.AddField("_ModelName", _AstralpoolObjectData._ModelName);
        form.AddField("_ModelColor", _AstralpoolObjectData._ModelColor);
        form.AddField("_LamasMaterial", _AstralpoolObjectData._LamasMaterial);
        form.AddField("_LamasColor", _AstralpoolObjectData._LamasColor);
        form.AddField("_TopCladdingColor", _AstralpoolObjectData._TopCladdingColor);
        form.AddField("_SidesCladdingColor", _AstralpoolObjectData._SidesCladdingColor);
        form.AddField("_Dimensions", _AstralpoolObjectData._Dimensions);
        form.AddField("_LedLights", _AstralpoolObjectData._LedLights);
        form.AddField("_ElectrosisContact", _AstralpoolObjectData._ElectrosisContact);
        form.AddField("_CoverConnect", _AstralpoolObjectData._CoverConnect);
        form.AddField("_PoolMaterial", _AstralpoolObjectData._PoolMaterial);
        form.AddField("_SecuritySystem", _AstralpoolObjectData._SecuritySystem);
        form.AddField("_SubmergedModel", _AstralpoolObjectData._SubmergedModel);
        form.AddField("_MecanicsType", _AstralpoolObjectData._MecanicsType);
        form.AddField("_BeamType", _AstralpoolObjectData._BeamType);
        form.AddField("_CoverType", _AstralpoolObjectData._CoverType);

        using (UnityWebRequest www = UnityWebRequest.Post(_WebRequestLinkStruct._SaveProductDataURL_Astralpool, form))
        {
            yield return www.SendWebRequest();

            // Netegem la resposta de PHP d'espais en blanc o salts de línia residuals
            string responseText = www.downloadHandler.text.Trim();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[MISSATGE DE PHP] Result: {www.result} | Error: {www.error}");

            }
            else
            {
                Debug.Log("[MISSATGE DE PHP] SaveProductData: " + responseText);

                if (responseText != "0")
                {
                    _OrderData._ProductID = int.Parse(responseText);
                }

            }
        }
        
    }



    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
