using UnityEngine;
using System.IO;

[System.Serializable]
public class St_Material
{
    [Space()]
    [Header("MATERIAL DATA")]
    public E_MaterialType _MaterialType;
    public E_PartToApplyMaterial _PartToApplyMaterial;
    public Sprite _MaterialSprite;
    public string _MaterialKey;
    public Material _Material01;
    public Material _Material02;
    public Material _Material03;
    
}
