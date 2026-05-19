using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public class St_ToggleBehaviourScript
{
    [Header("TOGGLE BEHAVIOUR")]
    [Space()]
    [Header("SPRITES OF TOGGLE")]
    public Sprite _OnActiveToggle;
    public Sprite _UnActiveToggle;
    [Header("SPRITES OF TOGGLE")]
    public Color _OnActiveToggleColor;
    public Color _UnActiveToggleColor;
    [Header("CURRENT STATE")]
    public bool bCurrentToggleState;
    [Header("DEPENDS ON PlayerPref?")]
    public bool bDependsOnPlayerPref;
    public string _PlayerPref;

}
