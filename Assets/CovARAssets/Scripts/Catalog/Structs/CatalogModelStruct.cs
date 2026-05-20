using UnityEngine;
using System.IO;

[System.Serializable]
public class St_Model
{
    [Space(0)]
    [Header("MODEL DATA")]
    public E_ModelType _ModelType;
    public Sprite _ModelLogo;
    public string _ModelKey;
    public St_Material[] _MaterialsArray;

}
