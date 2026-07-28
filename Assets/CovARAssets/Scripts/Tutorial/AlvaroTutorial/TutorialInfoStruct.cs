using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_TutorialInfoData
{
    public string _TitleKey;
    public string _DescripKey;
    public float _xOffset;
    public float _yOffset;
    public Color _PanelColor;
    public bool bNoInteractPanelIsActive;
    public bool bContinueButtonIsActive;
    public bool bFinishButtonIsActive;
    public bool bLeftFeedbackObj;
    public bool bRightFeedbackObj;
    public bool bCatalogProductFeedbackObj;
    public bool bCatalogModelFeedbackObj;
    public bool bCatalogMaterialFeedbackObj;
    public E_TutorialActions currentStepState;


}