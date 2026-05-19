using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_UIPanelStruct
{
    [Header("UI PANEL:")]
    [Header("Asign a name to this Panel (NO SPACES):")]
    public string _PanelName;
    [Header("Panel reference: (not-Automatic)")]
    public GameObject _PanelRef;


    public St_UIPanelStruct(string panelName, GameObject panelRef)
    {
        this._PanelName = panelName;
        this._PanelRef = panelRef;
    }
}
