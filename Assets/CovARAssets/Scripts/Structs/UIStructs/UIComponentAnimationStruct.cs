using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public struct St_UIComponentAnimation
{
    [Space()]
    [Header("Common animation variables:")]
    public float animationDuration;
    public Ease appearEase;
    public Ease disapearEase;
    [Header("Appears from right/left variables:")]
    public float hiddenPositionOnXAxis;
    [Header("Appears from right/left sumator on MobileState:")]
    public float offsetPositionOnXAxisForMobileMode;
    [Header("Appears from up/down sumator on MobileState:")]
    public float offsetPositionOnYAxisForMobileMode;

    [Header("Appears from up/down variables:")]
    public float hiddenPositionOnYAxis;
    [Header("Don't make an animation at start:")]
    public bool bDontStartHide;



    // Constructor para inicializar todos los campos
    public St_UIComponentAnimation(
         
         float animationDuration,
         Ease appearEase,
         Ease disapearEase,
         float hiddenPositionOnXAxis,
         float offsetPositionOnXAxisForMobileMode,
         float offsetPositionOnYAxisForMobileMode,
         float hiddenPositionOnYAxis,
         bool bDontStartHide
     )
    {
        
        this.animationDuration = animationDuration;
        this.appearEase = appearEase;
        this.disapearEase = disapearEase;
        this.hiddenPositionOnXAxis = hiddenPositionOnXAxis;
        this.offsetPositionOnXAxisForMobileMode = offsetPositionOnXAxisForMobileMode;
        this.offsetPositionOnYAxisForMobileMode = offsetPositionOnYAxisForMobileMode;
        this.hiddenPositionOnYAxis = hiddenPositionOnYAxis;
        this.bDontStartHide = bDontStartHide;
       
    }

}
