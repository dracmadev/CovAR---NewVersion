using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_WebRequestLinkStruct
{
    [Header("[ASTRALPOOL] -> LOGIN MENU SCENE LINKS:")]
    public string _LoginLinkURL_Astralpool;
    public string _SetAndSendNewPasswordURL_Astralpool;
    public string _SetPasswordManuallyURL_Astralpool;
    public string _DeleteUserURL_Astralpool;
    [Header("[BACPoolSystem] -> LOGIN MENU SCENE LINKS:")]
    public string _LoginLinkURL_BacPoolSystem;
    public string _SetAndSendNewPasswordURL_BacPoolSystem;
    public string _SetPasswordManuallyURL_BacPoolSystem;
    public string _DeleteUserURL_BacPoolSystem;
    [Header("[GENERAL] -> CREATE USER LINK:")]
    public string _SetUserCompanyURL;
    public string _CreateUserURL;
    [Header("AR SCENE LINKS:")]
    public string _SaveOrderDataURL;



}
