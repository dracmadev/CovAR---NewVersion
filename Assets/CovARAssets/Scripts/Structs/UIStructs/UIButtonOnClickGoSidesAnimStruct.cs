using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public class St_ButtonGoSidesAnimation
{
    [Header("Go sides Animation variables:")]
    public float moveAmount;
    public float animationDuration;

    public St_ButtonGoSidesAnimation(float moveAmount, float animationDuration)
    {
        this.moveAmount = moveAmount;
        this.animationDuration = animationDuration;
    }
}