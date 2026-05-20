using UnityEngine;

[CreateAssetMenu(fileName = "NCatalogDataBaseSO", menuName = "CovAR SO/Catalog DataBase")]
public class CovARCatalogScripteableObj : ScriptableObject
{
    [Header("CATALOG DATA BASE: ")]
    public St_Company[] _CompanysArray;
}
