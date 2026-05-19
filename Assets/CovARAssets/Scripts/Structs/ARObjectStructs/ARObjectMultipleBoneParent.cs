using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_ARObjectMultipleBoneParent
{
    [HideInInspector] public ARObjectPartScript partScript;

    [Header("Bone Parents Related?")]
    public St_ARObjectFeedbackPartData[] _ARObjectBoneParents;
    [Header("Which direction?")]
    public E_ARObjectReSizeDirections _ARObjectReSizeDirection;

    //[Header("Values: (Automatic)")]
    //public float currentValue;
    //public float minValue;
    //public float maxValue;
    //public float currentShelfWidth;
    //[HideInInspector] public Vector3 startingPosition;
}