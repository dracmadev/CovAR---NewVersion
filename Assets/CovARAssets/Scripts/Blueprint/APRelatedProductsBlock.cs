using Assets.SimpleLocalization.Scripts;
using System.Collections.Generic;
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
    [SerializeField] private Button _SecuritySystemDropdown;
    [SerializeField] private LocalizedTMPText _SecuritySystemText;
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

    private Dictionary<E_APRelatedproducts, CanvasGroup> _canvasGroupMap;

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

        InitDictionaty();
        ChooseWichRelatedProductsShow();


        _SubmergedModelDropdown.onValueChanged.AddListener(delegate { OnValueChangedInSubmergedType(); });

    }

    void InitDictionaty()
    {
        _canvasGroupMap = new Dictionary<E_APRelatedproducts, CanvasGroup>()
        {
            { E_APRelatedproducts.LedLights,       _CGLedLightBlock },
            { E_APRelatedproducts.Electrolisis,    _CGElectrolisisBlock },
            { E_APRelatedproducts.CoverConnect,    _CGCoverConnectBlock },
            { E_APRelatedproducts.PoolMaterial,    _CGPoolMaterialBlock },
            { E_APRelatedproducts.SecuritySystem,  _CGSecuritySystemBlock },
            { E_APRelatedproducts.SubmergedModel, _CGSubmergedModelBlock },
            { E_APRelatedproducts.Mecanics,        _CGMecanicsBlock },
            { E_APRelatedproducts.Beam,            _CGBeamBlock },
            { E_APRelatedproducts.Cover,           _CGConnectBlock }
        };
    }

    public void ChooseWichRelatedProductsShow()
    {
        if (_ARObjectManager == null || _ARObjectManager.GetCurrentARObject() == null) return;

        ARObjectScript arObj = _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>();
        if (arObj == null) return;

        // 1.Amaguem TOTS els CanvasGroups per defecte
        foreach (var kvp in _canvasGroupMap)
        {
            if (kvp.Value != null) SetCanvasGroupActive(kvp.Value, false);
        }

        // 2. Obtenim la llista d'enums de l'objecte actual
        var relatedProductsList = arObj.GetARObjectRelatedProductsList();

        if (relatedProductsList != null)
        {
            // 3. Activem NOMÉS els CanvasGroups  estan a la llista
            foreach (St_APRelatedProduct product in relatedProductsList)
            {
                
                E_APRelatedproducts currentEnum = product._RelatedProductType;

                if (_canvasGroupMap.TryGetValue(currentEnum, out CanvasGroup cg))
                {
                    if (cg != null) SetCanvasGroupActive(cg, true);
                }
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// SETTER /////////////////////////////////////////////////////////////////////////
    private void SetCanvasGroupActive(CanvasGroup cg, bool isActive)
    {
        cg.alpha = isActive ? 1f : 0f;
        cg.interactable = isActive;
        cg.blocksRaycasts = isActive;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////////////

    void OnValueChangedInSubmergedType()
    {
        if(_CGSubmergedModelBlock.alpha != 0)
        {
            int selectedIndex = _SubmergedModelDropdown.value;

            if (selectedIndex == 0)
            {
                SetCanvasGroupActive(_CGConnectBlock, true);
                SetCanvasGroupActive(_CGCoverConnectBlock, true);
            }
            else if (selectedIndex == 1)
            {
                SetCanvasGroupActive(_CGConnectBlock, false);
                SetCanvasGroupActive(_CGCoverConnectBlock, false);
            }
            
        }
       
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
