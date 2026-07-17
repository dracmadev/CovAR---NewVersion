using UnityEngine;

public class APBlueprintManager : MonoBehaviour
{
    [Header("ASTRALPOOL BLUEPRINT MANAGER")]
    [Space()]
    [Header("BLOCKS REFERENCES:")]
    [SerializeField] private APTopInformationBlock _APTopInformationBlock;
    [SerializeField] private APBasicInfoBlock _APBasicInfoBlock;
    [SerializeField] private APRelatedProductsBlock _APRelatedProductsBlock;
    [SerializeField] private BlueprintPopUpBehaviour _BlueprintPopUpBlock;
    [Header("Other references:")]
    [SerializeField] private ARObjectManager _ARObjectManager;
    [SerializeField] private HUDManagerScript _HUDManagerScript;
    [SerializeField] private GeolocalizationManager _GeolocalizationManager;

    [Header("Order Data:")]
    [SerializeField] private St_OrderData _OrderData;
    private WebRequestManager _WebRequestManager;
    string LastPanelBeforeBlueprintPopUp;

    void Start()
    {
        InitAPCatalog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////////////

    void InitAPCatalog()
    {
        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        if (GameObject.FindGameObjectWithTag("HUD"))
        {
            _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
        }
        if (GameObject.FindGameObjectWithTag("GeolocalizationManager"))
        {
            _GeolocalizationManager = GameObject.FindGameObjectWithTag("GeolocalizationManager").GetComponent<GeolocalizationManager>();
        }
        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        _OrderData._CurrentDate = System.DateTime.Now.ToString("dd/MM/yyyy");
        _OrderData._Country = _GeolocalizationManager.GetCountry();
        _OrderData._PostCode = _GeolocalizationManager.GetPostCode();


        InitBlocks();

    }

    void InitBlocks()
    {
        if (_BlueprintPopUpBlock != null)
        {
            _OrderData._CustomerName = _BlueprintPopUpBlock.GetCustomerName();
            _OrderData._CustomerMail = _BlueprintPopUpBlock.GetCustomerMail();
            LastPanelBeforeBlueprintPopUp = _BlueprintPopUpBlock.GetLastPanelBeforeBlueprintPopUp();
        }


        if (_APTopInformationBlock != null)
        {
            string location = _GeolocalizationManager.GetGeolocText();

            _APTopInformationBlock.InitTopInformationBlock(_OrderData, location);
        }


        if (_APRelatedProductsBlock != null)
        {
            _APRelatedProductsBlock.InitRelatedProduct(_ARObjectManager);
        }

        if(_APBasicInfoBlock != null)
        {
            _APBasicInfoBlock.InitBasicInfo(_ARObjectManager);
        }

       
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////

    public St_OrderData GetOrderData()
    {
        return _OrderData;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////

    public void OnCickReturnToLastSubPanelFromBlueprint()
    {
        if(_HUDManagerScript != null)
        {
            _HUDManagerScript.OnShowCompanyMark(true);
            _HUDManagerScript.TravelToPanel(LastPanelBeforeBlueprintPopUp);
        }
    }

    public void OnClickTravelToAPBlueprint()
    {
       InitAPCatalog();

        if (_HUDManagerScript != null)
        {
            _HUDManagerScript.OnShowCompanyMark(false);
            _HUDManagerScript.TravelToPanel("AstralpoolBlueprintPanel");
        }
    }


    public void OnClickDownloadAndSaveBlueprit()
    {
        if(_APBasicInfoBlock != null)
        {
            _APBasicInfoBlock.OnUpdateDimensions();
        }

        if(_APRelatedProductsBlock != null)
        {
            _APRelatedProductsBlock.OnUpdateRelatedProductsOnAstralpoolData();
        }

        if(_WebRequestManager != null)
        {
            _WebRequestManager.OnSaveProjectAndOrderDataFromAstralpool();
        }
    }
  

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
