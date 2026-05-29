using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class ARObjectManager : MonoBehaviour
{
    [Header("AR OBJECT MANAGER")]
    [Space(25)]
    [Header("Current Company Selected:")]
    [SerializeField] private E_CompanyType _CurrentCompanyRunning;

    [Header("First object to visualize:")]
    [SerializeField] private E_PoolType _CurrentPoolType;

    // Lista de Prefabs del manager nuevo
    [Header("List of ARObjects (Prefabs):")]
    [SerializeField] private List<GameObject> _PoolPrefabs;

    [Header("Current ARObject:")]
    [SerializeField] private GameObject _CurrentARObject;
    [Header("AR CAMERA reference: (Automatic)")]
    [SerializeField] private Camera _ARCamera;
    [Header("PlaneFinder reference: (Automatic)")]
    [SerializeField] private GameObject _PlaneFinder;
    [SerializeField] private bool bDontShowPlaneFinderAtStart;
    [Header("CovAR DATA:")]
    [SerializeField] private St_CovARObjectData _CovARObjectData;
    [Header("Current Company/Product/Model/Material Selected:")]
    [SerializeField] private St_Company _CurrentCompanySelected;
    [SerializeField] private St_Product _CurrentProductSelected;
    [SerializeField] private St_Model _CurrentModelSelected;
    [SerializeField] private St_Material _CurrentMaterialSelected;
    [Header("CovAR Catalogs:")]
    [SerializeField] private CovARCatalogScripteableObj _AstralpoolProductsCatalog;

    [Header("CovAR CURRENT Catalogs:")]
    [SerializeField] private CovARCatalogScripteableObj _CurrentProductCatalog;

    //Local variables
    HUDManagerScript _HUDManagerScrit;


    void Start()
    {
        InitARObjectManager();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////

    void InitARObjectManager()
    {
        _HUDManagerScrit = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();


        InitARCameraRef();
        //SetActiveARObject(_CurrentPoolType);
        InitPlaneFinder();

        if (bDontShowPlaneFinderAtStart)
        {
            SetActivePlaneFinder(false);
        }
        else
        {
            SetActivePlaneFinder(true);
        }

        InitCatalogBehaviour();
    }

    void InitARCameraRef()
    {
        _ARCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void InitPlaneFinder()
    {
        _PlaneFinder = GameObject.FindGameObjectWithTag("PlaneFinder");
    }

    // Lógica de estados de movimiento y rotación actualizada como el nuevo
    void UpdateARObjectSpecificState()
    {
        if (GetCurrentARObject() == null) return;

        switch (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState())
        {
            case E_ARObjectStates.DefaultState:
                _HUDManagerScrit.HideAllOfSubMenus();
                break;

            case E_ARObjectStates.MovingState:
                _HUDManagerScrit.HideAllOfSubMenus();
                break;

            case E_ARObjectStates.RotatingState:
                _HUDManagerScrit.ShowSpecificSubMenu("RotationSubPanel");
                _CurrentARObject.GetComponent<ARObjectScript>().InitRotationState();
                break;

            case E_ARObjectStates.SetLenghtState:
                _HUDManagerScrit.ShowSpecificSubMenu("SetARObjectLenghtSupPanel");
                _CurrentARObject.GetComponent<ARObjectScript>().InitSetLenghtState();
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// SETTERS ///////////////////////////////////////////////////////////////////

    // Instanciación dinámica real: guarda la posición/rotación anterior, destruye y genera el nuevo prefab
    public void SetActiveARObject(E_PoolType _ARObjectToActive)
    {
        if (_ARObjectToActive == E_PoolType.None) { return; }

        Vector3 lastPosition = Vector3.zero;
        Quaternion lastRotation = Quaternion.identity;

        if (_CurrentARObject != null)
        {
            lastPosition = _CurrentARObject.transform.position;
            lastRotation = _CurrentARObject.transform.rotation;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject prefab in _PoolPrefabs)
        {
            if (prefab.GetComponent<ARObjectScript>().GeCoverType() == _ARObjectToActive)
            {
                _CurrentARObject = Instantiate(prefab, transform);

                if (lastPosition != Vector3.zero)
                {
                    _CurrentARObject.transform.position = lastPosition;
                    _CurrentARObject.transform.rotation = lastRotation;
                }

                ARObjectScript newObjScript = _CurrentARObject.GetComponent<ARObjectScript>();

                newObjScript.InitARObjectScript();

                float maxLenghtOfNewModel = newObjScript.GetMaxARObjectLenghtDistance();

                if (_CovARObjectData._CurrentARObjectLenght > maxLenghtOfNewModel)
                {
                    _CovARObjectData._CurrentARObjectLenght = maxLenghtOfNewModel;
                }

                Debug.Log("Lenght: " + _CovARObjectData._CurrentARObjectLenght);
                newObjScript.SetARObjectLenghtByNum(_CovARObjectData._CurrentARObjectLenght);

                SetCurrentARObjectState("DefaultState");
                break;
            }
        }
    }

    // Comportamiento de "Toggle" (interruptor) para el movimiento y rotación del nuevo manager
    public void SetCurrentARObjectState(string StateName)
    {
        switch (StateName)
        {
            case "DefaultState":
                if (GetCurrentARObject() != null)
                {
                    GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    UpdateARObjectSpecificState();
                }
                break;

            case "MovingState":
                if (GetCurrentARObject() != null)
                {
                    // Si ya estaba moviéndose, vuelve a Default
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.MovingState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.MovingState);
                    }
                    _HUDManagerScrit.HideAllOfSubMenus();
                    UpdateARObjectSpecificState();
                }
                break;

            case "RotatingState":
                if (GetCurrentARObject() != null)
                {
                    // Si ya estaba rotando, vuelve a Default
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.RotatingState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                        _HUDManagerScrit.HideAllOfSubMenus();
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.RotatingState);
                    }
                    UpdateARObjectSpecificState();
                }
                break;

            case "SetLenghtState":
                if (GetCurrentARObject() != null)
                {
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.SetLenghtState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.SetLenghtState);
                    }
                    UpdateARObjectSpecificState();
                }
                break;
        }
    }

    public void SetActivePlaneFinder(bool active)
    {
        if (_PlaneFinder != null)
        {
            _PlaneFinder.SetActive(active);

            
        }

      
    }

    public void OnARObjectPosicioned()
    {
        SetActivePlaneFinder(false);
        _HUDManagerScrit.TravelToPanel("AP_ManipulateGroundRollerPanel");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// GETTERS /////////////////////////////////////////////////////////////////////

    public Camera GetARCamera()
    {
        return _ARCamera;
    }

    public GameObject GetCurrentARObject()
    {
        return _CurrentARObject;
    }

    public St_Company GetCurrentCompanyData()
    {
        return _CurrentCompanySelected;
    }

    public St_Product GetCurrentProductData()
    {
        return _CurrentProductSelected;
    }

    public  St_Model GetCurrentModelData()
    {
        return _CurrentModelSelected;
    }

    public St_Material GetCurrentMaterialData()
    {
        return _CurrentMaterialSelected;
    }

    public E_CompanyType GetCurrentCompanyRunning()
    {
        return _CurrentCompanyRunning;
    }

    public St_CovARObjectData GetCovARObjectData()
    {
        return _CovARObjectData;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// SETTER  //////////////////////////////////////////////////////////////////
    public void SetCurrentCompanyData(E_CompanyType company)
    {
        foreach(St_Company currentComp in _CurrentProductCatalog._CompanysArray)
        {
            if (currentComp._CompanyType == company)
            {
                _CovARObjectData._CurrentCompany = currentComp;
                _CurrentCompanySelected = currentComp;
                break;
            }
        }
       
        
    }

    public void SetCurrentProductData(St_Product product)
    {
        if(product != null)
        {
            switch(product._ProductType)
            {
                case E_ProductType.None: break;
                case E_ProductType.GroundRollerCover: _CovARObjectData._CurrentModelProduct = product; break;
                case E_ProductType.SubmergedRollerCover: _CovARObjectData._CurrentModelProduct = product; break;
                case E_ProductType.BencheAndCladdings: _CovARObjectData._CurrentCladdingProduct = product; break;
                case E_ProductType.FabricCover: _CovARObjectData._CurrentFabricCoverProduct = product; break;
                case E_ProductType.Lamas: _CovARObjectData._CurrentLamasProduct = product; break;

            }

           
            _CurrentProductSelected = product;
        }
    }

    public void SetCurrentModelData(St_Model model)
    {
        if (model != null)
        {
            if(_CurrentProductSelected != null)
            {
                switch (_CurrentProductSelected._ProductType)
                {
                    case E_ProductType.None: break;
                    case E_ProductType.GroundRollerCover: _CovARObjectData._CurrentModelModel = model; break;
                    case E_ProductType.SubmergedRollerCover: _CovARObjectData._CurrentModelModel = model; break;
                    case E_ProductType.BencheAndCladdings: 
                        
                        switch(model._ModelType)
                        {
                            case E_ModelType.AP_LeBancBigFoot: _CovARObjectData._CurrentModelModel = model;
                                _CovARObjectData._CurrentModelProduct = _CurrentProductSelected;  break;
                            case E_ModelType.AP_LeBancSmallFoot: _CovARObjectData._CurrentModelModel = model;
                                _CovARObjectData._CurrentModelProduct = _CurrentProductSelected; break;
                            case E_ModelType.AP_LeBancTopCladding: _CovARObjectData._CurrentTopCladdingModel = model; break;
                            case E_ModelType.AP_LeBancSidesCladding: _CovARObjectData._CurrentSidesCladdingModel = model; break;
                        }

                        break;
                    case E_ProductType.FabricCover: _CovARObjectData._CurrentFabricCoverModel = model; break;
                    case E_ProductType.Lamas: _CovARObjectData._CurrentLamasModel = model; break;
                }
            }

            _CurrentModelSelected = model;
        }
    }

    public void SetCurrentMaterialData(St_Material material)
    {
        if (material != null)
        {
            if(_CurrentModelSelected != null)
            {
                switch (_CurrentModelSelected._ModelType)
                {
                    case E_ModelType.None: break;
                    
                    ////////////////////////////////////////////////////////////// MODELS ///////////////////////////////////////////////////////////////
                    case E_ModelType.AP_Octeo: _CovARObjectData._CurrentModelMaterial = material; break;
                    case E_ModelType.AP_Sveltea: _CovARObjectData._CurrentModelMaterial = material; break;
                    case E_ModelType.AP_SvelteaManual: _CovARObjectData._CurrentModelMaterial = material; break;
                    case E_ModelType.AP_Coverly: _CovARObjectData._CurrentModelMaterial = material; break;
                    case E_ModelType.AP_Bellasun: _CovARObjectData._CurrentModelMaterial = material; break;
                    case E_ModelType.AP_Rousillon: _CovARObjectData._CurrentModelMaterial = material; break;
                    case E_ModelType.AP_LeBancSmallFoot: _CovARObjectData._CurrentModelMaterial = material; break;
                    case E_ModelType.AP_LeBancBigFoot: _CovARObjectData._CurrentModelMaterial = material; break;

                    //////////////////////////////////////////////////////////// CLADDINGS AND BENCHES ///////////////////////////////////////////////////
                    case E_ModelType.AP_LeBancTopCladding: _CovARObjectData._CurrentTopCladdingMaterial = material; break;
                    case E_ModelType.AP_LeBancSidesCladding: _CovARObjectData._CurrentSidesCladdingMaterial = material; break;

                    //////////////////////////////////////////////////////////// LAMAS  //////////////////////////////////////////////////////////////////
                    case E_ModelType.AP_LamasPolicarbonate: _CovARObjectData._CurrentLamasMaterial = material; break;
                    case E_ModelType.AP_LamasPVC: _CovARObjectData._CurrentLamasMaterial = material; break;


                }
            }

           _CurrentMaterialSelected = material;
        }
    }

    public void SetCurrentCompany(E_CompanyType company)
    {
        SetCurrentCompanyData(company);
    }

    public void SetCurrentProduct(E_ProductType product)
    {
        SetCurrentProductData(GetSpecificProductFromCurrentCompany(GetCurrentCompanyData(), product));
        
        //ADD MORE
    }

    public void SetCurrentModel(E_ModelType model, E_ModelGeneralType generalModel)
    {
       SetCurrentModelData(GetSpecificModelFromCurrentProduct(GetCurrentProductData(), model));


        //ADD MORE
    }

    public void SetCurrentMaterial(E_MaterialType material)
    {
        SetCurrentMaterialData(GetSpecificMaterialFromCurrentModel(GetCurrentModelData(), material));

        //ADD MORE
    }

    public void SetCurrentMaterialToNull()
    {
        _CovARObjectData._CurrentModelMaterial = null;
       
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////////////// CATALOG BEHAVIOUR //////////////////////////////////////////////////////////////////


    public void InitCatalogBehaviour()
    {
        if(_AstralpoolProductsCatalog != null)
        {
            _CurrentProductCatalog = _AstralpoolProductsCatalog;
        }

        //Init CurrentCatalogVariables
        SetCurrentCompanyData(_CurrentCompanyRunning);

        SetCurrentProductData(GetFirstProductFromCurrentCompany(GetCurrentCompanyData()));
        SetCurrentModelData(GetFirstModelFromCurrentProduct(GetCurrentProductData()));
        SetCurrentMaterialData(GetFirstMaterialFromCurrentModel(GetCurrentModelData()));

        //Init Lamas
        InitLamasCovARData();
    }

    public void InitLamasCovARData()
    {
        foreach(St_Product product in _CurrentCompanySelected._ProductsArray)
        {
            if (product._ProductType == E_ProductType.Lamas)
            {
               _CovARObjectData._CurrentLamasProduct = product;
               _CovARObjectData._CurrentLamasModel = GetFirstModelFromCurrentProduct(product);
                _CovARObjectData._CurrentLamasMaterial = GetFirstMaterialFromCurrentModel(_CovARObjectData._CurrentLamasModel);
            }
        }
    }

    public CovARCatalogScripteableObj GetCurrentProductCatalog()
    {
        return _CurrentProductCatalog;
    }

    


    public St_Company GetFirstCompanyFromCatalog(CovARCatalogScripteableObj currentProductCatalog)
    {
        if (currentProductCatalog == null || currentProductCatalog._CompanysArray == null || currentProductCatalog._CompanysArray.Length == 0)
        {
            return null;
        }
        return currentProductCatalog._CompanysArray[0];
    }

    public St_Product GetFirstProductFromCurrentCompany(St_Company currentCompany)
    {
        if (currentCompany == null || currentCompany._ProductsArray == null || currentCompany._ProductsArray.Length == 0)
        {
            return null;
        }
        return currentCompany._ProductsArray[0];
    }

    public St_Model GetFirstModelFromCurrentProduct(St_Product currentProduct)
    {
        if (currentProduct == null || currentProduct._ModelsArray == null || currentProduct._ModelsArray.Length == 0)
        {
            return null;
        }
        return currentProduct._ModelsArray[0];
    }

    public St_Material GetFirstMaterialFromCurrentModel(St_Model currentModel)
    {
        if (currentModel == null || currentModel._MaterialsArray == null || currentModel._MaterialsArray.Length == 0)
        {
            return null;
        }
        return currentModel._MaterialsArray[0];
    }

    public St_Company GetSpecificCompanyFromCatalog(CovARCatalogScripteableObj currentProductCatalog, E_CompanyType specificCompany)
    {
        if (currentProductCatalog == null || currentProductCatalog._CompanysArray == null) return null;

        foreach (St_Company company in currentProductCatalog._CompanysArray)
        {
            if (company._CompanyType == specificCompany)
            {
                return company;
            }
        }
        return null;
    }

    public St_Product GetSpecificProductFromCurrentCompany(St_Company currentCompany, E_ProductType specificProduct)
    {
        if (currentCompany == null || currentCompany._ProductsArray == null) return null;

        foreach (St_Product product in currentCompany._ProductsArray)
        {
            if (product._ProductType == specificProduct)
            {
                return product;
            }
        }

        return null;
    }

    public St_Model GetSpecificModelFromCurrentProduct(St_Product currentProduct, E_ModelType specificModel)
    {
        if (currentProduct == null || currentProduct._ModelsArray == null) return null;

        foreach (St_Model model in currentProduct._ModelsArray)
        {
            if (model._ModelType == specificModel)
            {
                return model;
            }
        }

        return null;
    }

    public St_Material GetSpecificMaterialFromCurrentModel(St_Model currentModel, E_MaterialType specificMaterial)
    {
        if (currentModel == null || currentModel._MaterialsArray == null) return null;

        foreach (St_Material material in currentModel._MaterialsArray)
        {
            if (material._MaterialType == specificMaterial)
            {
                return material;
            }
        }

        return null;
    }


    public void SetCovARFootMaterial(St_Material currentMatData)
    {
        ARObjectScript arScript = _CurrentARObject.GetComponent<ARObjectScript>();
        GameObject footRight = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.FootRight); //RIGHT FOOT
        footRight.TryGetComponent(out Renderer rendRight);
        GameObject footLeft = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.FootLeft); // LEFT FOOT
        footLeft.TryGetComponent(out Renderer rendLeft);

        if (arScript != null)
        {
            switch (_CurrentPoolType)
            {
                case E_PoolType.AP_Octeo:
                            
                    if (footRight != null && rendRight != null)
                    {
                        Material[] mats = rendRight.materials; 
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight.materials = mats; 
                    }

                    if (footLeft != null && rendLeft != null)
                    {
                        Material[] mats = rendLeft.materials; 
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft.materials = mats;
                    }

                    break;
                case E_PoolType.AP_Sveltea:

                    if (footRight != null && rendRight != null)
                    {
                        Material[] mats = rendRight.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight.materials = mats;
                    }

                    if (footLeft != null && rendLeft != null)
                    {
                        Material[] mats = rendLeft.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft.materials = mats;
                    }

                    break;
                case E_PoolType.AP_SvelteaManual:

                    if (footRight != null && rendRight != null)
                    {
                        Material[] mats = rendRight.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight.materials = mats;
                    }

                    if (footLeft != null && rendLeft != null)
                    {
                        Material[] mats = rendLeft.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft.materials = mats;
                    }

                    break;
                case E_PoolType.AP_Coverly: break;
                case E_PoolType.AP_Bellasun: break;
                case E_PoolType.AP_Lebanc_Small: break;
                case E_PoolType.AP_Lebanc_Big: break;
                case E_PoolType.AP_Rousillon: break;
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}