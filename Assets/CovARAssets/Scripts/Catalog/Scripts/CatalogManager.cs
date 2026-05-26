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
    private ARObjectManager _ARObjectManager;

   
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
    
    /////////////////////////////////////////////////////////// SETTERS

}
