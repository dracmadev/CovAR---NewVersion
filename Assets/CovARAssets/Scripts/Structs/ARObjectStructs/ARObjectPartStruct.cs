using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_ARObjectPart
{
    [Header("Type of ARObject part:")]
    public E_ARObjectParts _ARObjectPartType;
    [Header("ARObject part reference: (Automatic)")]
    public GameObject _ARObjectPartReference;
}