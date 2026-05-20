using UnityEngine;
using System.IO;

[System.Serializable]
public class St_Product
{
    [Space(0)]
    [Header("PRODUCT DATA")]
    public E_ProductType _ProductType;
    public Sprite _ProductSprite;
    public string _ProductKey;
    public St_Model[] _ModelsArray;

}
