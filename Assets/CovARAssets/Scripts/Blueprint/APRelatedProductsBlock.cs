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
    [Header("RELATED PRODUCTS PREFABS LIST:")]
    [SerializeField] private GameObject[] _RelatedProductList;
    [Header("UI COMPONENTS:")]
    [SerializeField] private GameObject _RelatedProductsContainer;




    ARObjectManager _ARObjectManager;
    APBlueprintManager _APBlueprintManager;

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
    
    public void InitRelatedProduct(ARObjectManager objManager, APBlueprintManager blueprintManager)
    {
        _ARObjectManager = objManager;
        _APBlueprintManager = blueprintManager;

        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        InitCreateRelatedProducts();

        InitModelSpecificBehaviours();


    }

   

    void InitCreateRelatedProducts()
    {
        if (_ARObjectManager == null || _RelatedProductsContainer == null)
            return;

        GameObject currentObject = _ARObjectManager.GetCurrentARObject();
        if (currentObject == null) return;

        ARObjectScript arObj = currentObject.GetComponent<ARObjectScript>();
        if (arObj == null) return;

        if (_RelatedProductList == null) return;

        var relatedProductsList = arObj.GetARObjectRelatedProductsList();

        // Eliminem els productes creats anteriorment.
        foreach (Transform child in _RelatedProductsContainer.transform)
        {
            if (child.GetComponent<BlueprintRelatedProductScript>() == null)
                continue;

            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        // Si no hi ha productes, deixem el contenidor net.
        if (relatedProductsList == null) return;

        foreach (GameObject prefab in _RelatedProductList)
        {
            if (prefab == null) continue;

            BlueprintRelatedProductScript prefabScript = prefab.GetComponent<BlueprintRelatedProductScript>();

            if (prefabScript == null) continue;

            bool shouldCreate = false;

            foreach (St_APRelatedProduct product in relatedProductsList)
            {
                if (product._RelatedProductType == prefabScript.GetRelatedProductType())
                {
                    shouldCreate = true;
                    break;
                }
            }

            if (!shouldCreate) continue;

            GameObject instance = Instantiate(prefab, _RelatedProductsContainer.transform, false);

            BlueprintRelatedProductScript instanceScript = instance.GetComponent<BlueprintRelatedProductScript>();

            instanceScript.SetCanvasGroupState(true);
            instanceScript.SetAPBlueprintManagerVariable(_APBlueprintManager);
            instance.SetActive(true);
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
        if (GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP() != null)
        {
            TMP_Dropdown submergedDropdown = GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP();
            
            submergedDropdown.onValueChanged.RemoveListener(delegate { OnValueChangedInSubmergedType(); });
            submergedDropdown.onValueChanged.AddListener(delegate { OnValueChangedInSubmergedType(); });
        }
       
       
    }

    void InitPoolMaterialBehaviour()
    {
        if(GetRelatedProductByType(E_APRelatedproducts.SecuritySystem_Astralpool) != null)
        {
            GetRelatedProductByType(E_APRelatedproducts.SecuritySystem_Astralpool).GetButtonRP().interactable = true;
        }
           

        if(GetRelatedProductByType(E_APRelatedproducts.PoolMaterial_Astralpool) != null)
        {
            TMP_Dropdown poolMaterialDropdown = GetRelatedProductByType(E_APRelatedproducts.PoolMaterial_Astralpool).GetDropdown01RP();
            
            poolMaterialDropdown.onValueChanged.RemoveListener(delegate { OnValueChangedInPoolMaterial(); });
            poolMaterialDropdown.onValueChanged.AddListener(delegate { OnValueChangedInPoolMaterial(); });

            Debug.Log("Pool Material Dropdown Initialized");
        }

       
    }
    
    void InitCoverConnectBehaviour(int option)
    {
        if (GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool) != null)
        {
            BlueprintRelatedProductScript coverConect = GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool);
            
            if (option == 0)
            {

                coverConect.GetDropdown01RP().gameObject.SetActive(true);
                coverConect.GetDropdown02RP().gameObject.SetActive(false);
            }
            else if (option == 1)
            {
                coverConect.GetDropdown01RP().gameObject.SetActive(false);
                coverConect.GetDropdown02RP().gameObject.SetActive(true);
            }
        }
    }

    void ForceSecuritySystemToOneOption(int option)
    {
        if (GetRelatedProductByType(E_APRelatedproducts.SecuritySystem_Astralpool) != null)
        {
            BlueprintRelatedProductScript securitySystem = GetRelatedProductByType(E_APRelatedproducts.SecuritySystem_Astralpool);
            securitySystem.GetLocalizedTMPTextRP().LocalizationKey = securitySystem.GetSecuritySystemsArray()[option]._SSKey;
            securitySystem.GetLocalizedTMPTextRP().Localize();

            securitySystem.GetButtonRP().interactable = false;
           
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////

    BlueprintRelatedProductScript GetRelatedProductByType(E_APRelatedproducts type)
    {
        if (_RelatedProductsContainer == null)
            return null;

        foreach (Transform child in _RelatedProductsContainer.transform)
        {
            // Ignora els prefabs antics pendents de destrucció.
            if (!child.gameObject.activeSelf)
                continue;

            BlueprintRelatedProductScript script =
                child.GetComponent<BlueprintRelatedProductScript>();

            if (script != null && script.GetRelatedProductType() == type)
                return script;
        }

        return null;
    }


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
        if (GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP() != null)
        {

            int selectedIndex = GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP().value;

            if (selectedIndex == 0)
            {
                if(GetRelatedProductByType(E_APRelatedproducts.Cover_Astralpool) != null)
                {
                    GetRelatedProductByType(E_APRelatedproducts.Cover_Astralpool).SetCanvasGroupState(true);
                    //SetCanvasGroupActive(_CGConnectBlock, true);
                }
               
               
                if (GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool) != null)
                {
                   Debug.Log("SET COVER CONNECT ACTIVE");
                    GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool).SetCanvasGroupState(true);
                }

                if(GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool) != null)
                {
                    GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool).GetSubGameObjRP().SetActive(false);
                    GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool).GetDropdown01RP().gameObject.SetActive(true);

                    //_BeamText.SetActive(false);
                    //_BeamDropdown.gameObject.SetActive(true);
                }
               
            }
            else if (selectedIndex == 1)
            {

                if (GetRelatedProductByType(E_APRelatedproducts.Cover_Astralpool) != null)
                {
                    GetRelatedProductByType(E_APRelatedproducts.Cover_Astralpool).SetCanvasGroupState(false);
                    // SetCanvasGroupActive(_CGConnectBlock, false);
                }


                if (GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool) != null)
                {
                   Debug.Log("SET COVER CONNECT INACTIVE");
                    GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool).SetCanvasGroupState(false);
                }
                
                if (GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool) != null)
                {
                    GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool).GetSubGameObjRP().SetActive(true);
                    GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool).GetDropdown01RP().gameObject.SetActive(false);

                    //_BeamText.SetActive(true);
                    //_BeamDropdown.gameObject.SetActive(false);
                }



            }
            
        }
       
    }

    void OnValueChangedInPoolMaterial()
    {
        if (GetRelatedProductByType(E_APRelatedproducts.PoolMaterial_Astralpool) != null)
        {
            Debug.Log("Pool Material Dropdown Value Changed");
            int selectedIndex = GetRelatedProductByType(E_APRelatedproducts.PoolMaterial_Astralpool).GetDropdown01RP().value;

            BlueprintRelatedProductScript securitySystem = GetRelatedProductByType(E_APRelatedproducts.SecuritySystem_Astralpool);

            if (selectedIndex == 0)
            {
                // _SecuritySystemCG.interactable = true;
                securitySystem.GetButtonRP().interactable = true;
            }
            else if (selectedIndex == 1)
            {
                //_SecuritySystemCG.interactable = true;
                securitySystem.GetButtonRP().interactable = true;
            }
            else if (selectedIndex == 2)
            {
                securitySystem.GetButtonRP().interactable = false;
                _APBlueprintManager.SetCurrentSecuritySystemIndex(1);
                //securitySystem.GetLocalizedTMPTextRP().LocalizationKey = securitySystem.GetSecuritySystemsArray()[1]._SSKey;
                //securitySystem.GetLocalizedTMPTextRP().Localize();

            }
            else if (selectedIndex == 3)
            {
                //_SecuritySystemCG.interactable = true;
                securitySystem.GetButtonRP().interactable = true;
            }
            else if (selectedIndex == 4)
            {
                securitySystem.GetButtonRP().interactable = false;
                _APBlueprintManager.SetCurrentSecuritySystemIndex(2);

                //securitySystem.GetLocalizedTMPTextRP().LocalizationKey = securitySystem.GetSecuritySystemsArray()[2]._SSKey;
                //securitySystem.GetLocalizedTMPTextRP().Localize();

            }

        }

    }


    public void OnUpdateRelatedProductsOnAstralpoolData()
    {
        if(_WebRequestManager != null)
        {
            //LED LIGHTS/////////////////////////////////////
            if(GetRelatedProductByType(E_APRelatedproducts.LedLights_Astralpool) != null)
            {
                if (GetRelatedProductByType(E_APRelatedproducts.LedLights_Astralpool).GetToggleRP() != null)
                {
                    _WebRequestManager.SetLedLights(GetRelatedProductByType(E_APRelatedproducts.LedLights_Astralpool).GetToggleRP().isOn.ToString());
                }
                else
                {
                    _WebRequestManager.SetLedLights("None");
                }

            }
            else
            {
                _WebRequestManager.SetLedLights("None");
            }
            //////////////////////////////////////////////////

            //ELECTROLISIS/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.Electrolisis_Astralpool) != null)
            {
                if (GetRelatedProductByType(E_APRelatedproducts.Electrolisis_Astralpool).GetToggleRP() != null)
                {
                    _WebRequestManager.SetElectrosisContact(GetRelatedProductByType(E_APRelatedproducts.Electrolisis_Astralpool).GetToggleRP().isOn.ToString());
                }
                else
                {
                    _WebRequestManager.SetElectrosisContact("None");
                }
            }
            else
            {
                _WebRequestManager.SetElectrosisContact("None");
            }
            //////////////////////////////////////////////////

            //COVER CONNECT/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool) != null)
            {
                if(_ARObjectManager.GetCurrentPoolTypeData() != E_PoolType.AP_Rousillon)
                {
                    TMP_Dropdown groundCoverConectDropdown = GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool).GetDropdown01RP();

                  _WebRequestManager.SetCoverConnect(groundCoverConectDropdown.options[groundCoverConectDropdown.value].text);
                }
                else
                {
                    TMP_Dropdown submergedCoverConectDropdown = GetRelatedProductByType(E_APRelatedproducts.CoverConnect_Astralpool).GetDropdown02RP();

                    _WebRequestManager.SetCoverConnect(submergedCoverConectDropdown.options[submergedCoverConectDropdown.value].text);
                }     
            }
            else
            {
                _WebRequestManager.SetElectrosisContact("None");
            }
            //////////////////////////////////////////////////

            //POOL MATERIAL/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.PoolMaterial_Astralpool) != null)
            {
                TMP_Dropdown poolMatDropdown = GetRelatedProductByType(E_APRelatedproducts.PoolMaterial_Astralpool).GetDropdown01RP();

                _WebRequestManager.SetPoolMaterial(poolMatDropdown.options[poolMatDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetPoolMaterial("None");
            }
            //////////////////////////////////////////////////

            //SECURITY SYSTEM/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.SecuritySystem_Astralpool) != null)
            {
                LocalizedTMPText _SecuritySystemText = GetRelatedProductByType(E_APRelatedproducts.SecuritySystem_Astralpool).GetLocalizedTMPTextRP();
                
                _WebRequestManager.SetSecuritySystem(_SecuritySystemText.GetLocalizedText());
            }
            else
            {
                _WebRequestManager.SetSecuritySystem("None");
            }
            //////////////////////////////////////////////////


            //SUBMERGED MODEL/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP() != null)
            {
                TMP_Dropdown _SubmergedModelDropdown = GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP();

                _WebRequestManager.SetSubmergedModel(_SubmergedModelDropdown.options[_SubmergedModelDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetSubmergedModel("None");
            }
            //////////////////////////////////////////////////

            //MECANICS/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.Mecanics_Astralpool) != null)
            {
                TMP_Dropdown _MecanicsDropdown = GetRelatedProductByType(E_APRelatedproducts.Mecanics_Astralpool).GetDropdown01RP();

                _WebRequestManager.SetMecanicsType(_MecanicsDropdown.options[_MecanicsDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetMecanicsType("None");
            }
            //////////////////////////////////////////////

            //BEAM/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool) != null)
            {
                if (_ARObjectManager.GetCurrentPoolTypeData() == E_PoolType.AP_Rousillon)
                {
                    if (GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP() != null)
                    {
                        TMP_Dropdown _SubmergedModelDropdown = GetRelatedProductByType(E_APRelatedproducts.SubmergedModel_Astralpool).GetDropdown01RP();

                        if (_SubmergedModelDropdown.value == 0)
                        {
                            TMP_Dropdown _BeamDropdown = GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool).GetDropdown01RP();

                            _WebRequestManager.SetBeamType(_BeamDropdown.options[_BeamDropdown.value].text);
                        }
                        else
                        {
                            GameObject _BeamText = GetRelatedProductByType(E_APRelatedproducts.Beam_Astralpool).GetSubGameObjRP();

                            _WebRequestManager.SetBeamType(_BeamText.GetComponent<TMP_Text>().text);
                        }
                    }
                   
                }
               
            }
            else
            {
                _WebRequestManager.SetBeamType("None");
            }
            //////////////////////////////////////////////

            //COVER/////////////////////////////////////
            if (GetRelatedProductByType(E_APRelatedproducts.Cover_Astralpool) != null)
            {
                TMP_Dropdown _CoverDropdown = GetRelatedProductByType(E_APRelatedproducts.Cover_Astralpool).GetDropdown01RP();
                _WebRequestManager.SetCoverType(_CoverDropdown.options[_CoverDropdown.value].text);
            }
            else
            {
                _WebRequestManager.SetCoverType("None");
            }
            ////////////////////////////////////////////
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
