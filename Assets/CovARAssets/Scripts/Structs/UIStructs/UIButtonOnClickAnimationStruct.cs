using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public struct St_ButtonOnClickAnimation
{
    [Header("OnClick Default Animation variables:")]
    public float scaleUpFactor;
    public float animationDuration;
    public Ease easeType;
   
   


    // Constructor per inicialitzar els camps
  public St_ButtonOnClickAnimation(float scaleUpFactor, float animationDuration, Ease easeType)
{
    this.scaleUpFactor = scaleUpFactor;
    this.animationDuration = animationDuration;
    this.easeType = easeType;
    
    
}
}
