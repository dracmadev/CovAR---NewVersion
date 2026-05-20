using UnityEngine;
using System.IO;

[System.Serializable]
public class St_Company
{
    [Space(0)]
    [Header("COMPANY DATA")]
    public E_CompanyType _CompanyType;  
    public Sprite _CompanyLogo;
    public string _CompanyKey;
    public string _CompanyLink;
    public St_Product[] _ProductsArray;
}
