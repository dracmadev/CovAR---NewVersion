using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_WebRequestLinkStruct
{
    [Header("MAIN MENU LINKS:")]
    [Header("[ASTRALPOOL]")]
    public string _LoginLinkURL_Astralpool;
    public string _CreateUserURL_Astralpool;
    public string _SetAndSendNewPasswordURL_Astralpool;
    public string _SetPasswordManuallyURL_Astralpool;
    public string _DeleteUserURL_Astralpool;
    [Header("[BACPoolSystem]")]
    public string _LoginLinkURL_BacPoolSystem;
    public string _CreateUserURL_BacPoolSystem;
    public string _SetAndSendNewPasswordURL_BacPoolSystem;
    public string _SetPasswordManuallyURL_BacPoolSystem;
    public string _DeleteUserURL_BacPoolSystem;
    [Header("[GENERAL] -> CREATE USER LINK:")]
    public string _GetUserCompanyURL;
    
    [Header("AR SCENE LINKS:")]
    [Header("[ASTRALPOOL]")]
    public string _GetUserIDURL_Astralpool;
    public string _SaveOrderDataURL_Astralpool;
    public string _SaveProductDataURL_Astralpool;
    [Header("[BACPoolSystem]")]
    public string _GetUserIDURL_BacPoolSystem;
    public string _SaveOrderDataURL_BacPoolSystem;
    public string _SaveProductDataURL_BacPoolSystem;
}
