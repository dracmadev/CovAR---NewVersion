using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class St_ButtonSwipeAnimation
{
    [Header("OnClick swipe Animation variables:")]
    public float animationDuration;
    public bool bDontDoFlipFlop;


    // Constructor per inicialitzar els camps
    public St_ButtonSwipeAnimation(float animationDuration)
    { 
        this.animationDuration = animationDuration;  
    }
}
