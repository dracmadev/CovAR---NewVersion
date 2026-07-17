using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_OrderDataStruct
{
    [Header("Order Data:")]
    public int _UserID;
    public int _StockID;
    public int  _ProductID;
    public string _CustomerName;
    public string _CustomerEmail;
    public string _PostCode;
    public string _Country;
    public string _OrderDate;

}