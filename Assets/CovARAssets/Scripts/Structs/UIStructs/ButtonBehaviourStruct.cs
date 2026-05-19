using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_ButtonBehaviourStruct
{
    [Space()]
    [Header("Button Components: (Automatic)")]
    public TMP_Text _buttonText;
    public Image _buttonImage;
    [Space()]
    [Header("IMAGE BUTTON BEHAVIOUR:")]
    [Space()]
    [Header("Show Text under the Image?")]
    public bool bShowText;
    [Space()]
    [Header("Change image when click?")]
    public bool bChangeImage;
    [Header("Button Components that change OnClick: ")]
    public Sprite _buttonImageChanged;
    [Space()]
    [Header("Button in mobile device mode:")]
    public bool bButtonInMobileMode;
    [Space()]
    [Header("Animation variables: (Not done yet!)")]
    public bool bHasAnimation;
    [Space()]
    [Header("Multilanguage variables:  (Not done yet!)")]
    public string key;
    [Space()]
    [Header("Have tutorial pop up when clicks")]
    public bool bHaveTutorialPopUp;


}
