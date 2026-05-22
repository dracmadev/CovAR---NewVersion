using UnityEngine;
using System.IO;

[System.Serializable]
public class St_CovARObjectData
{
    [Space(0)]
    [Header("CovAR OBJECT DATA")]
    [Space(0)]
    [Header("COMPANY:")]
    public St_Company _CurrentCompany;
    [Header("MODEL DATA:")]
    public St_Product _CurrentModelProduct;
    public St_Model _CurrentModelModel;
    public St_Material _CurrentModelMaterial;
    [Header("CLADDING DATA (TOP):")]
    public St_Product _CurrentTopCladdingProduct;
    public St_Model _CurrentTopCladdingModel;
    public St_Material _CurrentTopCladdingMaterial;
    [Header("CLADDING DATA (SIDES):")]
    public St_Product _CurrentSidesCladdingProduct;
    public St_Model _CurrentSidesCladdingModel;
    public St_Material _CurrentSidesCladdingMaterial;
    [Header("LAMAS DATA:")]
    public St_Product _CurrentLamasProduct;
    public St_Model _CurrentLamasModel;
    public St_Material _CurrentLamasMaterial;
}
    