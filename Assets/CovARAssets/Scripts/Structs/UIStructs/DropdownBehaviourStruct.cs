using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]

public class St_DropdownBehaviourStruct
{
    [Header("DROPDOWN BEHAVIOUR")]
    [Space()]
    [Header("Create dropdown options manually?")]
    public bool bOptionsManually;
    [Header("Enumeration to assign:")]
    public string enumTypeName;
    [Header("Default value to show:")]
    public int defaultValue;
    [Header("Multilanguage variables")]
    public string key;
}
