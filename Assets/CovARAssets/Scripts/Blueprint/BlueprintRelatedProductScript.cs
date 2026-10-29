using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintRelatedProductScript : MonoBehaviour
{
    [Header("RELATED PRODUCT SCRIPT")]
    [Space()]
    [Header("Type of Related Product:")]
    [SerializeField] private E_APRelatedproducts _RelatedProductType;
    [Header("UI Components:")]
    [SerializeField] private CanvasGroup _CanvasGroupRP;
    [SerializeField] private GameObject _SubGameObjRP;
    [SerializeField] private Button _ButtonRP;
    [SerializeField] private Toggle _ToggleRP;
    [SerializeField] private TMP_Dropdown _Dropdown01RP;
    [SerializeField] private TMP_Dropdown _Dropdown02RP;
    [SerializeField] private LocalizedTMPText _LocalizeTextRP;


    [Header("Other components:")]
    [SerializeField] private St_SecuritySystem[] _SecuritySystemsArray;
    APBlueprintManager _APBlueprintManager;
    void Start()
    {
        InitSecuritySystemPrefab();


    }

    // Update is called once per frame
    void Update()
    {
        UpdateSecutirySystemPrefab();
    }

    /////////////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////////

    void InitSecuritySystemPrefab()
    {
        if(_RelatedProductType == E_APRelatedproducts.SecuritySystem_Astralpool)
        {
            if(_ButtonRP != null && _APBlueprintManager != null)
            {
                _ButtonRP.onClick.RemoveListener(_APBlueprintManager.GetPopUpSecuritySystemGO().OnClickOpenSecuritySystemPopUp);
                _ButtonRP.onClick.AddListener(_APBlueprintManager.GetPopUpSecuritySystemGO().OnClickOpenSecuritySystemPopUp);
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////// UPDATE //////////////////////////////////////////////////////////////////

    void UpdateSecutirySystemPrefab()
    {
        if (_RelatedProductType == E_APRelatedproducts.SecuritySystem_Astralpool)
        {
            if (_LocalizeTextRP != null && _APBlueprintManager != null)
            {
               _LocalizeTextRP.LocalizationKey = _SecuritySystemsArray[_APBlueprintManager.GetCurrentSecuritySystemIndex()]._SSKey;
                _LocalizeTextRP.Localize();
              // Debug.Log("Localization Index: " + _APBlueprintManager.GetCurrentSecuritySystemIndex() + " | Localized Text: " + _LocalizeTextRP.GetLocalizedText());
            }
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////// GETTERS //////////////////////////////////////////////////////////////////////
    public E_APRelatedproducts GetRelatedProductType()
    {
        return _RelatedProductType;
    }


    public GameObject GetSubGameObjRP()
    {
        return _SubGameObjRP;
    }

    public TMP_Dropdown GetDropdown01RP()
    {
        return _Dropdown01RP;
    }
    public TMP_Dropdown GetDropdown02RP()
    {
        return _Dropdown02RP;
    }

    public Button GetButtonRP()
    {
        return _ButtonRP;
    }

    public Toggle GetToggleRP()
    {
        return _ToggleRP;
    }

    public LocalizedTMPText GetLocalizedTMPTextRP()
    {
        return _LocalizeTextRP;
    }

    public St_SecuritySystem[] GetSecuritySystemsArray()
    {
        return _SecuritySystemsArray;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////// SETTERS /////////////////////////////////////////////////////////////////
    
    public void SetCanvasGroupState(bool bNewState)
    {
        if (_CanvasGroupRP != null)
        {
            if(bNewState)
            {
                _CanvasGroupRP.alpha = 1f;
                _CanvasGroupRP.interactable = true;
                _CanvasGroupRP.blocksRaycasts = true;

            }
            else
            {
                _CanvasGroupRP.alpha = 0f;
                _CanvasGroupRP.interactable = false;
                _CanvasGroupRP.blocksRaycasts = false;
            }
        }
    }

    public void SetAPBlueprintManagerVariable(APBlueprintManager blueprintManager)
    {
       _APBlueprintManager = blueprintManager;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
