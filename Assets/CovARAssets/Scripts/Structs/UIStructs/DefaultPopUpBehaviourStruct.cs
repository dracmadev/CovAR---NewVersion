using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


[System.Serializable]
public class St_DefaultPopUpData
{
    [Header("Pop up Components: (Automatic)")]
    public TMP_Text _PopUpTitle;
    public TMP_Text _PopUpDescription;
    public Image _PopUpImage;
    public List<Button> _PopUpButtonsList;
    [Header("Animator Component:")]
    public Animator _PopUpAnimator;


    [Header("Default Pop Up Data: (Automatic, loaded from button)")]
    public string popUpTitle;
    public St_BasicPopUpData[] popUpContentList;

    // Constructor per inicialitzar els camps
    public St_DefaultPopUpData(string title, St_BasicPopUpData[] contentList)
    {
        this.popUpTitle = title;
        this.popUpContentList = contentList;
    }
}

