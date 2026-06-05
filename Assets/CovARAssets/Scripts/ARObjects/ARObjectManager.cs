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
    bool bARObjectPlaced = false;

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

        InitPlaneFinder();

        if (bDontShowPlaneFinderAtStart)
        {
            SetActivePlaneFinder(false);
        }
        else
        {
            SetActivePlaneFinder(true);
        }

       // InitCatalogBehaviour();
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

            case E_ARObjectStates.SetLamasLenghtState:
                _HUDManagerScrit.ShowSpecificSubMenu("SetLamasLenghtSubPanel");
                _CurrentARObject.GetComponent<ARObjectScript>().InitSetLamasLenghtState();
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// SETTERS ///////////////////////////////////////////////////////////////////

    // Instanciación dinámica
    public void SetActiveARObject(E_PoolType _ARObjectToActive)
    {
        if (_ARObjectToActive == E_PoolType.None) { return; }

        _CurrentPoolType = _ARObjectToActive;

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

                newObjScript.SetLamasLenghtByNum(_CovARObjectData._CurrentLamasLenght);


                newObjScript.SetARObjectLenghtByNum(_CovARObjectData._CurrentARObjectLenght);
                
                SetLamasMaterial(_CovARObjectData._CurrentLamasMaterial);

                SetCurrentARObjectState("DefaultState");

                break;
            }
        }
    }

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

            case "SetLamasLenghtState":
                if (GetCurrentARObject() != null)
                {
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.SetLamasLenghtState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.SetLamasLenghtState);
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
        SetActiveARObject(_CurrentPoolType);
        _HUDManagerScrit.TravelToPanel("AP_ManipulateGroundRollerPanel");
        bARObjectPlaced = true;
        _CurrentARObject.GetComponent<ARObjectScript>().GetCustomSplineInstantiateScript().PlayInitialOpenAnimation();
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

    public bool GetIsARObjectPlaced()
    {
        return bARObjectPlaced;
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
                    case E_ProductType.GroundRollerCover: _CovARObjectData._CurrentModelModel = model; 
                                                       // _CovARObjectData._CurrentTopCladdingMaterial._MaterialType = E_MaterialType.None;
                                                       // _CovARObjectData._CurrentSidesCladdingMaterial._MaterialType = E_MaterialType.None; 
                                                       break;
                    case E_ProductType.SubmergedRollerCover: _CovARObjectData._CurrentModelModel = model;
                        // _CovARObjectData._CurrentTopCladdingMaterial._MaterialType = E_MaterialType.None;
                        // _CovARObjectData._CurrentSidesCladdingMaterial._MaterialType = E_MaterialType.None; 
                        break;
                    case E_ProductType.BencheAndCladdings: 
                        
                        switch(model._ModelType)
                        {
                            case E_ModelType.AP_LeBancBigFoot: _CovARObjectData._CurrentModelModel = model;
                                _CovARObjectData._CurrentModelProduct = _CurrentProductSelected;
                                /*
                                _CovARObjectData._CurrentTopCladdingMaterial = GetFirstMaterialFromCurrentModel(_CovARObjectData._CurrentModelModel);
                                _CovARObjectData._CurrentSidesCladdingMaterial = GetFirstMaterialFromCurrentModel(_CovARObjectData._CurrentModelModel);
                                */
                                break;
                            case E_ModelType.AP_LeBancSmallFoot: _CovARObjectData._CurrentModelModel = model;
                                _CovARObjectData._CurrentModelProduct = _CurrentProductSelected;
                                /*
                                _CovARObjectData._CurrentTopCladdingMaterial = GetFirstMaterialFromCurrentModel(_CovARObjectData._CurrentModelModel);
                                _CovARObjectData._CurrentSidesCladdingMaterial = GetFirstMaterialFromCurrentModel(_CovARObjectData._CurrentModelModel);
                                */
                                break;
                            case E_ModelType.AP_LeBancTopCladding: _CovARObjectData._CurrentTopCladdingModel = model; break;
                            case E_ModelType.AP_LeBancSidesCladding: _CovARObjectData._CurrentSidesCladdingModel = model; break;
                        }

                        break;
                    case E_ProductType.FabricCover: _CovARObjectData._CurrentFabricCoverModel = model;
                                                    _CovARObjectData._CurrentTopCladdingMaterial._MaterialType = E_MaterialType.None;
                                                    _CovARObjectData._CurrentSidesCladdingMaterial._MaterialType = E_MaterialType.None; break;  
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

        GameObject footLeft = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.FootLeft); // LEFT FOOT

        if (arScript != null)
        {
            switch (_CurrentPoolType)
            {
                case E_PoolType.AP_Octeo:

                    footRight.TryGetComponent(out Renderer rendRight_Octeo);

                    if (footRight != null && rendRight_Octeo != null)
                    {
                        Material[] mats = rendRight_Octeo.materials; 
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight_Octeo.materials = mats; 
                    }

                    footLeft.TryGetComponent(out Renderer rendLeft_Octeo);

                    if (footLeft != null && rendLeft_Octeo != null)
                    {
                        Material[] mats = rendLeft_Octeo.materials; 
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft_Octeo.materials = mats;
                    }

                    break;
                case E_PoolType.AP_Sveltea:

                    footRight.TryGetComponent(out Renderer rendRight_Sverltea);

                    if (footRight != null && rendRight_Sverltea != null)
                    {
                        Material[] mats = rendRight_Sverltea.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight_Sverltea.materials = mats;
                    }

                    footLeft.TryGetComponent(out Renderer rendLeft_Sveltea);

                    if (footLeft != null && rendLeft_Sveltea != null)
                    {
                        Material[] mats = rendLeft_Sveltea.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft_Sveltea.materials = mats;
                    }

                    break;
                case E_PoolType.AP_SvelteaManual:

                    footRight.TryGetComponent(out Renderer rendRight_SverlteaM);

                    if (footRight != null && rendRight_SverlteaM != null)
                    {
                        Material[] mats = rendRight_SverlteaM.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight_SverlteaM.materials = mats;
                    }

                    footLeft.TryGetComponent(out Renderer rendLeft_SvelteaM);

                    if (footLeft != null && rendLeft_SvelteaM != null)
                    {
                        Material[] mats = rendLeft_SvelteaM.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft_SvelteaM.materials = mats;
                    }

                    break;
                case E_PoolType.AP_Coverly:

                    footRight.TryGetComponent(out Renderer rendRight_Coverly);

                    if (footRight != null && rendRight_Coverly != null)
                    {
                        Material[] mats = rendRight_Coverly.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight_Coverly.materials = mats;
                    }

                    footLeft.TryGetComponent(out Renderer rendLeft_Coverly);

                    if (footLeft != null && rendLeft_Coverly != null)
                    {
                        Material[] mats = rendLeft_Coverly.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft_Coverly.materials = mats;
                    }

                    break;
                case E_PoolType.AP_Bellasun: break;
                case E_PoolType.AP_Lebanc_Small:

                    footRight.TryGetComponent(out Renderer rendRight_LebancS);

                    if (footRight != null && rendRight_LebancS != null)
                    {
                        Material[] mats = rendRight_LebancS.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight_LebancS.materials = mats;
                    }

                    footLeft.TryGetComponent(out Renderer rendLeft_LebancS);

                    if (footLeft != null && rendLeft_LebancS != null)
                    {
                        Material[] mats = rendLeft_LebancS.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft_LebancS.materials = mats;
                    }

                    break;
                case E_PoolType.AP_Lebanc_Big:

                    footRight.TryGetComponent(out Renderer rendRight_LebancB);

                    if (footRight != null && rendRight_LebancB != null)
                    {
                        Material[] mats = rendRight_LebancB.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendRight_LebancB.materials = mats;
                    }

                    footLeft.TryGetComponent(out Renderer rendLeft_LebancB);

                    if (footLeft != null && rendLeft_LebancB != null)
                    {
                        Material[] mats = rendLeft_LebancB.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendLeft_LebancB.materials = mats;
                    }

                    break;
                case E_PoolType.AP_Rousillon:

                  
                    GameObject submergedTarima = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.UpCoverPlane); //TARIMA
                    submergedTarima.TryGetComponent(out Renderer renderSubmergedTarima);
                    GameObject axisLamas = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.CoverAxisLamas); // AXIS LAMAS
                    axisLamas.TryGetComponent(out Renderer renderSubmergedAxisLamas);


                    if (submergedTarima != null && renderSubmergedTarima != null)
                    {
                        Material[] mats = renderSubmergedTarima.materials;
                        mats[0] = currentMatData._Material01;
                        renderSubmergedTarima.materials = mats;
                    }

                    if (axisLamas != null && renderSubmergedAxisLamas != null)
                    {
                        Material[] mats = renderSubmergedAxisLamas.materials;
                        mats[0] = currentMatData._Material02;
                        renderSubmergedAxisLamas.materials = mats;
                    }

                    break;
            }
        }
    }

    public void SetCovARSubmergedMaterial(St_Material currentMatData)
    {
        ARObjectScript arScript = _CurrentARObject.GetComponent<ARObjectScript>();
        GameObject submergedTarima = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.UpCoverPlane); //TARIMA
        GameObject axisLamas = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.CoverAxis); // AXIS LAMAS

        if (arScript != null)
        {
            switch (_CurrentPoolType)
            {
                case E_PoolType.AP_Rousillon:

                    submergedTarima.TryGetComponent(out Renderer renderSubmergedTarima);
                   
                    axisLamas.TryGetComponent(out Renderer renderSubmergedAxisLamas);

                    if (submergedTarima != null && renderSubmergedTarima != null)
                    {
                        Material[] mats = renderSubmergedTarima.materials;
                        mats[0] = currentMatData._Material01;
                        renderSubmergedTarima.materials = mats;
                    }

                    if (axisLamas != null && renderSubmergedAxisLamas != null)
                    {
                        Material[] mats = renderSubmergedAxisLamas.materials;
                        mats[0] = currentMatData._Material02;
                        renderSubmergedAxisLamas.materials = mats;
                    }

                    break;
            }
        }
    }

    public void SetCovARCladdingsMaterial(St_Material currentMatData)
    {
        ARObjectScript arScript = _CurrentARObject.GetComponent<ARObjectScript>();

        GameObject topCladding = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.UpCoverPlane); //TOP CLADDING
        GameObject frontSideCladding = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.FrontCoverPlane); // Front CLADDING
        GameObject backSideCladding = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.BackCoverPlane); // Back CLADDING

        switch(_CurrentModelSelected._ModelType)
        {
            case E_ModelType.AP_LeBancTopCladding:
                if (topCladding != null)
                {
                    topCladding.TryGetComponent(out Renderer rendTopCladding);
                    if (rendTopCladding != null)
                    {
                        Material[] mats = rendTopCladding.materials;
                        mats[0] = currentMatData._Material01;
                        mats[1] = currentMatData._Material02;
                        rendTopCladding.materials = mats;
                    }
                }
                break;

            case E_ModelType.AP_LeBancSidesCladding:

                if(currentMatData._MaterialType == E_MaterialType.None)
                {
                    frontSideCladding.SetActive(false);
                    backSideCladding.SetActive(false);
                }
                else
                {
                    if (frontSideCladding != null)
                    {
                        frontSideCladding.SetActive(true);

                        frontSideCladding.TryGetComponent(out Renderer rendFrontSideCladding);
                        if (rendFrontSideCladding != null)
                        {
                            Material[] mats = rendFrontSideCladding.materials;
                            mats[0] = currentMatData._Material01;
                            mats[1] = currentMatData._Material02;
                            rendFrontSideCladding.materials = mats;
                        }
                    }
                    if (backSideCladding != null)
                    {
                        backSideCladding.SetActive(true);

                        backSideCladding.TryGetComponent(out Renderer rendBackSideCladding);
                        if (rendBackSideCladding != null)
                        {
                            Material[] mats = rendBackSideCladding.materials;
                            mats[0] = currentMatData._Material01;
                            mats[1] = currentMatData._Material02;
                            rendBackSideCladding.materials = mats;
                        }
                    }
                }
                break;
        }
    }

    public void SetLamasMaterial(St_Material currentMatData)
    {
        ARObjectScript arScript = _CurrentARObject.GetComponent<ARObjectScript>();

        GameObject lamasCoverAxis = arScript.GetARObjectSpecificPartBasedOnType(E_ARObjectParts.CoverAxisLamas);
        if (lamasCoverAxis != null && lamasCoverAxis.TryGetComponent(out Renderer renderLamasCoverAxis))
        {
            Material[] matsAxis = renderLamasCoverAxis.materials;
            matsAxis[0] = currentMatData._Material01;
            renderLamasCoverAxis.materials = matsAxis; 
        }

        List<GameObject> lamasList = arScript.GetARObjectSpecificPartListBasedOnType(E_ARObjectParts.Lama);


        foreach (GameObject lamaObj in lamasList)
        {
            if (lamaObj != null && lamaObj.TryGetComponent(out Renderer renderLama))
            {
                Material[] matsLama = renderLama.materials;
                matsLama[0] = currentMatData._Material01; 
                renderLama.materials = matsLama; 
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




}