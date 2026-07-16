using Assets.SimpleLocalization.Scripts;
using System.Collections;
using System.Text;
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
    [SerializeField] static private E_CompanyType _CurrentAppCompany = E_CompanyType.BACPoolSystems;

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
}
