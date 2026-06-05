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
    [Header("CLADDING DATA:")]
    public St_Product _CurrentCladdingProduct;
    public St_Model _CurrentTopCladdingModel;
    public St_Material _CurrentTopCladdingMaterial;
    public St_Model _CurrentSidesCladdingModel;
    public St_Material _CurrentSidesCladdingMaterial;
    [Header("LAMAS DATA:")]
    public St_Product _CurrentLamasProduct;
    public St_Model _CurrentLamasModel;
    public St_Material _CurrentLamasMaterial;
    [Header("FABRIC COVER DATA:")]
    public St_Product _CurrentFabricCoverProduct;
    public St_Model _CurrentFabricCoverModel;
    public St_Material _CurrentFabricCoverMaterial;
    [Space(25)]
    [Header("CURRENT AROBJECT LENGHT:")]
    public float _CurrentARObjectLenght = 1;
    public float _CurrentLamasLenght = 0;
   
    
}
    