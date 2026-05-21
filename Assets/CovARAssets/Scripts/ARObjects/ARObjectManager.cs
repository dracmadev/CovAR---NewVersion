using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ARObjectManager : MonoBehaviour
{
    [Header("AR OBJECT MANAGER")]
    [Space(25)]
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
        SetActiveARObject(_CurrentPoolType);
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
                break;

            case E_ARObjectStates.ReSizeingState:
                _HUDManagerScrit.HideAllOfSubMenus();
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// SETTERS ///////////////////////////////////////////////////////////////////

    // Instanciación dinámica real: guarda la posición/rotación anterior, destruye y genera el nuevo prefab
    public void SetActiveARObject(E_PoolType _ARObjectToActive)
    {
        Vector3 lastPosition = Vector3.zero;
        Quaternion lastRotation = Quaternion.identity;

        // Si ya había un objeto antes, guardamos dónde estaba para que el nuevo aparezca exactamente ahí
        if (_CurrentARObject != null)
        {
            lastPosition = _CurrentARObject.transform.position;
            lastRotation = _CurrentARObject.transform.rotation;
        }

        // Neteja de memòria: Destruimos todos los hijos actuales
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Buscamos el nuevo prefab en la lista e instanciamos
        foreach (GameObject prefab in _PoolPrefabs)
        {
            if (prefab.GetComponent<ARObjectScript>().GetPoolType() == _ARObjectToActive)
            {
                _CurrentARObject = Instantiate(prefab, transform);

                // Si ya había una posición guardada, se la aplicamos a la nueva piscina
                if (lastPosition != Vector3.zero)
                {
                    _CurrentARObject.transform.position = lastPosition;
                    _CurrentARObject.transform.rotation = lastRotation;
                }

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

            case "ReSizeingState":
                if (GetCurrentARObject() != null)
                {
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.ReSizeingState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.ReSizeingState);
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
        return _CovARObjectData._CurrentCompany;
    }

    public St_Product GetCurrentProductData()
    {
        return _CovARObjectData._CurrentProduct;
    }

    public  St_Model GetCurrentModelData()
    {
        return _CovARObjectData._CurrentModel;
    }

    public St_Material GetCurrentMaterialData()
    {
        return _CovARObjectData._CurrentMaterial;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// SETTER  //////////////////////////////////////////////////////////////////
    public void SetCurrentCompanyData(St_Company company)
    {
        if (company != null)
        {
            _CovARObjectData._CurrentCompany = company;
        }
    }

    public void SetCurrentProductData(St_Product product)
    {
        if(product != null)
        {
            _CovARObjectData._CurrentProduct = product;
        }
    }

    public void SetCurrentModelData(St_Model model)
    {
        if (model != null)
        {
            _CovARObjectData._CurrentModel = model;
        }
    }

    public void SetCurrentMaterialData(St_Material material)
    {
        if (material != null)
        {
            _CovARObjectData._CurrentMaterial = material;
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////////////// CATALOG BEHAVIOUR //////////////////////////////////////////////////////////////////


    public void InitCatalogBehaviour()
    {
        if(_AstralpoolProductsCatalog != null)
        {
            _CurrentProductCatalog = _AstralpoolProductsCatalog;
        }

        SetCurrentCompanyData(GetFirstCompanyFromCatalog(_CurrentProductCatalog));
        SetCurrentProductData(GetFirstProductFromCurrentCompany(GetCurrentCompanyData()));
        SetCurrentModelData(GetFirstModelFromCurrentProduct(GetCurrentProductData()));
        SetCurrentMaterialData(GetFirstMaterialFromCurrentModel(GetCurrentModelData()));
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}