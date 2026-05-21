using UnityEngine;
using System.IO;

[System.Serializable]
public class St_CovARObjectData
{
    [Space(0)]
    [Header("CovAR OBJECT DATA")]
    public St_Company _CurrentCompany;
    public St_Product _CurrentProduct;
    public St_Model _CurrentModel;
    public St_Material _CurrentMaterial;
}
    