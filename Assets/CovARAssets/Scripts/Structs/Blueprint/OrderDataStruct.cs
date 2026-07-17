using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_OrderData
{
    [Header("Order data info:")]
    public string _CustomerName;
    public string _CustomerMail;
    public string _CurrentDate;
    public string _Country;
    public string _PostCode;
    public int _UserID;
    public int _ProductID;
    public int _StockID;
}
