using Assets.SimpleLocalization.Scripts;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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
    [SerializeField] private TMP_Dropdown _CoverConnectDropdown_GroundRoller;
    [SerializeField] private TMP_Dropdown _CoverConnectDropdown_Submerged;
    [SerializeField] private CanvasGroup _CGCoverConnectBlock;
    [Header("POOL MATERIAL:")]
    [SerializeField] private TMP_Dropdown _PoolMaterialDropdown;
    [SerializeField] private CanvasGroup _CGPoolMaterialBlock;
    [Header("SECURITY SYSTEM:")]
    [SerializeField] private CanvasGroup _SecuritySystemCG;
    [SerializeField] private LocalizedTMPText _SecuritySystemText;
    [SerializeField] private CanvasGroup _CGSecuritySystemBlock;
    [SerializeField] private St_SecuritySystem[] _SecuritySystemsArray;
    [Header("SUBMERGED MODEL:")]
    [SerializeField] private TMP_Dropdown _SubmergedModelDropdown;
    [SerializeField] private CanvasGroup _CGSubmergedModelBlock;
    [Header("MECANICS:")]
    [SerializeField] private TMP_Dropdown _MecanicsDropdown;
    [SerializeField] private CanvasGroup _CGMecanicsBlock;
    [Header("BEAM:")]
    [SerializeField] private TMP_Dropdown _BeamDropdown;
    [SerializeField] private GameObject _BeamText;
    [SerializeField] private CanvasGroup _CGBeamBlock;
    [Header("COVER:")]
    [SerializeField] private TMP_Dropdown _CoverDropdown;
    [SerializeField] private CanvasGroup _CGConnectBlock;





    ARObjectManager _ARObjectManager;

    private Dictionary<E_APRelatedproducts, CanvasGroup> _canvasGroupMap;

    private WebRequestManager _WebRequestManager;



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

        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }


        InitDictionaty();
        ChooseWichRelatedProductsShow();


        InitModelSpecificBehaviours();


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

    void InitModelSpecificBehaviours()
    {
        switch(_ARObjectManager.GetCurrentPoolTypeData())
        {
            case E_PoolType.AP_Octeo: InitPoolMaterialBehaviour(); InitCoverConnectBehaviour(0); break;
                case E_PoolType.AP_Sveltea: InitPoolMaterialBehaviour(); InitCoverConnectBehaviour(0); break;
                case E_PoolType.AP_SvelteaManual: InitPoolMaterialBehaviour(); InitCoverConnectBehaviour(0); break;
                case E_PoolType.AP_Coverly: InitPoolMaterialBehaviour(); InitCoverConnectBehaviour(0); break;
                case E_PoolType.AP_Bellasun: InitCoverConnectBehaviour(0); ForceSecuritySystemToOneOption(2);  break;
                case E_PoolType.AP_Lebanc_Big: InitPoolMaterialBehaviour(); InitCoverConnectBehaviour(0); break;
                case E_PoolType.AP_Lebanc_Small: InitPoolMaterialBehaviour(); InitCoverConnectBehaviour(0); break;
                case E_PoolType.AP_Rousillon: InitPoolMaterialBehaviour(); InitSubmergedTypeBehaviour(); InitCoverConnectBehaviour(1); break;
        }
    }

    void InitSubmergedTypeBehaviour()
    {
        _SubmergedModelDropdown.onValueChanged.RemoveListener(delegate { OnValueChangedInSubmergedType(); });
        _SubmergedModelDropdown.onValueChanged.AddListener(delegate { OnValueChangedInSubmergedType(); });
    }

    void InitPoolMaterialBehaviour()
    {
        _SecuritySystemCG.interactable = true;

        _PoolMaterialDropdown.onValueChanged.RemoveListener(delegate { OnValueChangedInPoolMaterial(); });
        _PoolMaterialDropdown.onValueChanged.AddListener(delegate { OnValueChangedInPoolMaterial(); });
    }
    
    void InitCoverConnectBehaviour(int option)
    {
        if(option == 0)
        {
            _CoverConnectDropdown_GroundRoller.gameObject.SetActive(true);
            _CoverConnectDropdown_Submerged.gameObject.SetActive(false);
        }
        else if (option == 1)
        {
            _CoverConnectDropdown_GroundRoller.gameObject.SetActive(false);
            _CoverConnectDropdown_Submerged.gameObject.SetActive(true);
        }
    }

    void ForceSecuritySystemToOneOption(int option)
    {
        //Debug.Log("FORCE SECURITY SYSTEM TO OPTION: " + option);
        _SecuritySystemText.LocalizationKey = _SecuritySystemsArray[option]._SSKey;
        _SecuritySystemText.Localize();

        _SecuritySystemCG.interactable = false;
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

                _BeamText.SetActive(false);
                _BeamDropdown.gameObject.SetActive(true);
            }
            else if (selectedIndex == 1)
            {
                SetCanvasGroupActive(_CGConnectBlock, false);
                SetCanvasGroupActive(_CGCoverConnectBlock, false);

                _BeamText.SetActive(true);
                _BeamDropdown.gameObject.SetActive(false);
            }
            
        }
       
    }

    void OnValueChangedInPoolMaterial()
    {
        if (_CGPoolMaterialBlock.alpha != 0)
        {
            int selectedIndex = _PoolMaterialDropdown.value;

            if (selectedIndex == 0)
            {
                _SecuritySystemCG.interactable = true;
            }
            else if (selectedIndex == 1)
            {
                _SecuritySystemCG.interactable = true;
            }
            else if (selectedIndex == 2)
            {
                _SecuritySystemCG.interactable = false;
                _SecuritySystemText.LocalizationKey = _SecuritySystemsArray[1]._SSKey;
                _SecuritySystemText.Localize();
            }
            else if (selectedIndex == 3)
            {
                _SecuritySystemCG.interactable = true;
            }
            else if (selectedIndex == 4)
            {
                _SecuritySystemCG.interactable = false;
                _SecuritySystemText.LocalizationKey = _SecuritySystemsArray[2]._SSKey;
                _SecuritySystemText.Localize();
            }

        }

    }


    public void OnUpdateRelatedProductsOnAstralpoolData()
    {
        if(_WebRequestManager != null)
        {
            if(_CGLedLightBlock.alpha == 1)
            {
                _WebRequestManager.SetLedLights(_LedLightCheckMark.isOn.ToString());
            }
            else
            {
                _WebRequestManager.SetLedLights("None");
            }

            if (_CGElectrolisisBlock.alpha == 1)
            {
                _WebRequestManager.SetElectrosisContact(_ElectrolisisCheckMark.isOn.ToString());
            }
            else
            {
                _WebRequestManager.SetElectrosisContact("None");
            }

            if (_CGCoverConnectBlock.alpha == 1)
            {
                if(_ARObjectManager.GetCurrentPoolTypeData() != E_PoolType.AP_Rousillon)
                {
                    _WebRequestManager.SetCoverConnect(_CoverConnectDropdown_GroundRoller.options[_CoverConnectDropdown_GroundRoller.value].text);
                }
                else
                {
                    _WebRequestManager.SetCoverConnect(_CoverConnectDropdown_Submerged.options[_CoverConnectDropdown_Submerged.value].text);
                }
                   
            }
            else
            {
                _WebRequestManager.SetElectrosisContact("None");
            }

            if (_CGPoolMaterialBlock.alpha == 1)
            {
                _WebRequestManager.SetPoolMaterial(_PoolMaterialDropdown.options[_PoolMaterialDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetPoolMaterial("None");
            }

            if (_CGSecuritySystemBlock.alpha == 1)
            {
                _WebRequestManager.SetSecuritySystem(_SecuritySystemText.GetLocalizedText());
            }
            else
            {
                _WebRequestManager.SetSecuritySystem("None");
            }

            if (_CGSubmergedModelBlock.alpha == 1)
            {
                _WebRequestManager.SetSubmergedModel(_SubmergedModelDropdown.options[_SubmergedModelDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetSubmergedModel("None");
            }


            if (_CGMecanicsBlock.alpha == 1)
            {
                _WebRequestManager.SetMecanicsType(_MecanicsDropdown.options[_MecanicsDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetMecanicsType("None");
            }

            if (_CGBeamBlock.alpha == 1)
            {
                _WebRequestManager.SetBeamType(_BeamDropdown.options[_BeamDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetBeamType("None");
            }

            if (_CGConnectBlock.alpha == 1)
            {
                _WebRequestManager.SetCoverType(_CoverDropdown.options[_CoverDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetCoverType("None");
            }

        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
