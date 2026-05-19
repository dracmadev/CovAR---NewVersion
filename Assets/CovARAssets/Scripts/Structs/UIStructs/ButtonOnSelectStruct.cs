using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_ButtonOnSelectData
{
    [Header("Selected state variables:")]
    public bool bActiveSelectedState;
    public bool bStartSelected;
    public bool DeselectWhenYouClick;
    public E_SelectedButtonMode _SelectButtonMode;
    [Header("MODE -> SetPositionByOffset:")]
    public float offsetPositionOnXAxisForSelectedMode;
    [Header("MODE -> ChangeColor:")]
    public Color _OnSelectedColor;
    public Color _UnSelectedColor;
    [Header("MODE -> SetSprite:")]
    public Sprite _OnSelectedSprite;
    public Sprite _UnSelectedSprite;

    // Constructor amb paràmetres
    public St_ButtonOnSelectData(bool activeSelectedState, float offsetX)
    {
        bActiveSelectedState = activeSelectedState;
        offsetPositionOnXAxisForSelectedMode = offsetX;
    }

}