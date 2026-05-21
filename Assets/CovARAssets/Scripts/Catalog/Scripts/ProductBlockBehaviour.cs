using UnityEngine;
using TMPro; 
using Assets.SimpleLocalization.Scripts; 
using System.Collections.Generic;

public class ProductBlockBehaviour : MonoBehaviour
{
    [Header("PRODUCT BLOCK BEHAVIOUR")]
    [Space()]
    [Header("UI Components")]
    [SerializeField] private TMP_Dropdown _productDropdown; 
    [SerializeField] private ModelBlockBehaviour _modelBlockUI; 

    [Header("Current Selection data: (Just to see)")]
    [SerializeField] private St_Product[] _IncomingProductList;
    [SerializeField] private St_Product _currentProductSelected;

    private ARObjectManager _ARObjectManager;

    void Start()
    {
        InitProductBlockBehaviour();
    }

    void InitProductBlockBehaviour()
    {
        
        GameObject arManagerGO = GameObject.FindGameObjectWithTag("ARObjectManager");
        if (arManagerGO != null)
        {
            _ARObjectManager = arManagerGO.GetComponent<ARObjectManager>();

            if (_ARObjectManager.GetCurrentProductCatalog() == null)
            {
                _ARObjectManager.InitCatalogBehaviour();
            }
        }

        if (_ARObjectManager != null && _ARObjectManager.GetCurrentCompanyData() != null)
        {
            _IncomingProductList = _ARObjectManager.GetCurrentCompanyData()._ProductsArray;

            SetupProductDropdown();
        }
        else
        {
            Debug.LogWarning("No s'ha pogut carregar la llista de productes perquè l'ARObjectManager o la Companyia és null.");
        }
    }

    void SetupProductDropdown()
    {
        if (_productDropdown == null || _IncomingProductList == null || _IncomingProductList.Length == 0) return;

        // Netejem les opcions que vinguin per defecte al Dropdown d'Unity
        _productDropdown.ClearOptions();

        // Creem la llista d'opcions temporals compatible amb TMP_Dropdown
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (St_Product product in _IncomingProductList)
        {
            // Traduïm la lletra fent servir la clau del producte amb el teu SimpleLocalization
            string translatedName = LocalizationManager.Localize(product._ProductKey);
            Sprite productIcon = product._ProductSprite;

            // 🛠️ SOLUCIÓ: Creem l'opció buida i n'assignem els valors directament
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = translatedName;
            option.image = productIcon;

            newOptions.Add(option);
        }

        // Afegim totes les opcions de cop al Dropdown
        _productDropdown.AddOptions(newOptions);

        // Escoltació de canvis: primer netegem per seguretat i despres lliguem el mètode
        _productDropdown.onValueChanged.RemoveListener(OnDropdownProductChanged);
        _productDropdown.onValueChanged.AddListener(OnDropdownProductChanged);

        // Per defecte, seleccionem el primer producte de la llista al frame 1
        if (_IncomingProductList.Length > 0)
        {
            _productDropdown.value = 0;
            OnDropdownProductChanged(0);
        }
    }

    public void OnDropdownProductChanged(int index)
    {
        if (_IncomingProductList == null || index >= _IncomingProductList.Length) return;

        // 1. Actualitzem quin és el producte seleccionat actualment
        _currentProductSelected = _IncomingProductList[index];

        // 2. Avisem al teu ARObjectManager global perquè sàpiga quin és el producte actiu a l'aplicació
        if (_ARObjectManager != null)
        {
            _ARObjectManager.SetCurrentProductData(_currentProductSelected);
        }

        // 3. ACTUALITZEM ELS MODELS! Cridem la funció pública del teu script anterior
        if (_modelBlockUI != null && _currentProductSelected._ModelsArray != null)
        {
            St_Model[] modelsDeAquestProducte = _currentProductSelected._ModelsArray;

            // Agafem el primer model per defecte d'aquest nou producte per passar-lo com a seleccionat
            St_Model primerModel = null;
            if (modelsDeAquestProducte.Length > 0)
            {
                primerModel = modelsDeAquestProducte[0];
            }

            // Invoquem la inicialització dinàmica dels teus slots de models
            _modelBlockUI.InitCatalogModelBlock(modelsDeAquestProducte, primerModel);
        }
    }
}