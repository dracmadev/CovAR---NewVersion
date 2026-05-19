using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_ARObjectBonesData
{
    [HideInInspector] public ARObjectPartScript partScript;

    [Header("BONE OF THE AR OBJECT:")]
    [Space()]
    [Header("Side to move:")]
    public E_SidesInLetters _BoneSide;
    public E_ARObjectComponentsDirection _BoneDirection;

    public float sideLength;
}