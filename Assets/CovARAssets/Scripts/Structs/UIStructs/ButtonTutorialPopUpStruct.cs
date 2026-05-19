using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class St_ButtonTutorialPopUpData
{
    [Header("Tutorial Pop Up Variables:")]
    [Header("OnClick duration to active PopUp:")]
    public float onClickDuration;
    [Header("Default Pop Up Data:")]
    public string popUpTitle;
    public St_BasicPopUpData[] popUpContentList;
    [Header("Default Pop Up reference in Scene:")]
    public GameObject defaultPopUpRef;
    [Header("Use Animation instead of an Image?")]
    public bool bUseAnimation;
    public RuntimeAnimatorController _AnimCont;

    public St_ButtonTutorialPopUpData(float duration, string title, St_BasicPopUpData[] contentList)
    {
        this.onClickDuration = duration;
        this.popUpTitle = title;
        this.popUpContentList = contentList;
    }
}