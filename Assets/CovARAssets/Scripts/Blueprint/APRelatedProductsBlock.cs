using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class APRelatedProductsBlock : MonoBehaviour
{
    [Header("[ASTRALPOOL] RELATED PRODUCTS BLOCK")]
    [Space()]
    [Header("LED LIGHT:")]
    [SerializeField] private Toggle _LedLightCheckMark;
    [SerializeField] private CanvasGroup _CGLedLightBlock;
    [Header("ELECTROLISIS:")]
    [SerializeField] private Toggle _ElectrolisisCheckMark;
    [SerializeField] private CanvasGroup _CGElectrolisisBlock;
    [Header("COVER CONNECT:")]
    [SerializeField] private TMP_Dropdown _CoverConnectDropdown;
    [SerializeField] private CanvasGroup _CGCoverConnectBlock;
    [Header("POOL MATERIAL:")]
    [SerializeField] private TMP_Dropdown _PoolMaterialDropdown;
    [SerializeField] private CanvasGroup _CGPoolMaterialBlock;
    [Header("SECURITY SYSTEM:")]
    [SerializeField] private TMP_Dropdown _SecuritySystemDropdown;
    [SerializeField] private CanvasGroup _CGSecuritySystemBlock;
    [Header("SUBMERGED MODEL:")]
    [SerializeField] private TMP_Dropdown _SubmergedModelDropdown;
    [SerializeField] private CanvasGroup _CGSubmergedModelBlock;
    [Header("MECANICS:")]
    [SerializeField] private TMP_Dropdown _MecanicsDropdown;
    [SerializeField] private CanvasGroup _CGMecanicsBlock;
    [Header("BEAM:")]
    [SerializeField] private TMP_Dropdown _BeamDropdown;
    [SerializeField] private CanvasGroup _CGBeamBlock;
    [Header("COVER:")]
    [SerializeField] private TMP_Dropdown _CoverDropdown;
    [SerializeField] private CanvasGroup _CGConnectBlock;





    ARObjectManager _ARObjectManager;

    /////////////////////////////////////////////////////////////////// DEFAULT FUNCTIONS //////////////////////////////////////////////////////////////////////////

    void Start()
    {
        // On BlueprintManager
    }

  
    void Update()
    {
        
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    /////////////////////////////////////////////////////////////////////////////// INIT /////////////////////////////////////////////////////////////////////////
    
    public void InitRelatedProduct(ARObjectManager objManager)
    {
        _ARObjectManager = objManager;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
