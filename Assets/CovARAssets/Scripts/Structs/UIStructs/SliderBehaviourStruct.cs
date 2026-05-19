using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public class St_SliderBehaviourStruct
{
    [Header("SLIDER BEHAVIOUR:")]
    [Space()]
    [Header("Minimum & Maximum values:")]
    public float sliderMin;
    public float sliderMax;
    [Header("Value in return: (Don't Touch!)")]
    public float sliderResult;
}
