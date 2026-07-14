using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class CatalogManager : MonoBehaviour
{
    [Header("CATALOG MANAGER")]
    [Space()]
    [Header("Current Catalog Company (Just to see)")]
    [SerializeField] private E_CompanyType _CurrentCompany;

    [Header("Catalog Blocks")]
    [SerializeField] private CompanyBlockBehaviour _CompanyBlock;
    [SerializeField] private ProductBlockBehaviour _ProductBlock;
    [SerializeField] private ModelBlockBehaviour _ModelBlock;
    [SerializeField] private MaterialBlockBehaviour _MaterialBlock;

    [Header("Panel Names depending on Product:")]
    [SerializeField] private string _GroundRollerManipulatePanelName;
    [SerializeField] private string _SubmergedRollerManipulatePanelName;
    [SerializeField] private string _BenchRollerManipulatePanelName;
    [SerializeField] private string _FabricCoverManipulatePanelName;

    private ARObjectManager _ARObjectManager;
    private HUDManagerScript _HUDManagerScript;

    void Start()
    {
        InitCatalog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////////////
    public void InitCatalog()
    {
       
        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();


        _ARObjectManager.InitCatalogBehaviour();

        _CurrentCompany = _ARObjectManager.GetCurrentCompanyRunning();

        InitCatalogBlocks();


    }



    void InitCatalogBlocks()
    {
        if (_CompanyBlock != null)
        {
            _CompanyBlock.InitCompanyBlock();
        }
        else
        {
            Debug.LogWarning("No s'ha pogut inicialitzar el Company Block perquè la referència és null.");
        }

        if (_ProductBlock != null)
        {
            _ProductBlock.InitProductBlockBehaviour();
        }
        else
        {
            Debug.LogWarning("No s'ha pogut inicialitzar el Product Block perquè la referència és null.");
        }
        if (_ModelBlock != null)
        {
            _ModelBlock.InitModelBlockBehaviour();
        }
        else
        {
            Debug.LogWarning("No s'ha pogut inicialitzar el Model Block perquè la referència és null.");
        }
        if (_MaterialBlock != null)
        {
            _MaterialBlock.InitMaterialBlockBehaviour();
        }
        else
        {
            Debug.LogWarning("No s'ha pogut inicialitzar el Material Block perquè la referència és null.");
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// SETTERS //////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////// GETTER ////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////

    public void OnClickCloseCatalog()
    {
        switch(_ARObjectManager.GetCovARObjectData()._CurrentModelProduct._ProductType)
        {
            case E_ProductType.None: break;
            case E_ProductType.GroundRollerCover: _HUDManagerScript.TravelToPanel(_GroundRollerManipulatePanelName); break;
            case E_ProductType.SubmergedRollerCover: _HUDManagerScript.TravelToPanel(_SubmergedRollerManipulatePanelName); break;
            case E_ProductType.BencheAndCladdings: _HUDManagerScript.TravelToPanel(_BenchRollerManipulatePanelName); break;
            case E_ProductType.FabricCover: _HUDManagerScript.TravelToPanel(_FabricCoverManipulatePanelName); break;
            

        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
