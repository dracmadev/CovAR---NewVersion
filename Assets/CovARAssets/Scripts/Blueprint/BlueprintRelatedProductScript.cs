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
    [Header("State:")]
    [SerializeField] private bool _bIsActive;
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
    
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /////////////////////////////////////////////////////////////////// GETTERS //////////////////////////////////////////////////////////////////////
    public E_APRelatedproducts GetRelatedProductType()
    {
        return _RelatedProductType;
    }

    public bool GetRelatedProductCurrentState()
    {
        return _bIsActive;
    }


    public CanvasGroup GetCanvasGroupRP()
    {
        return _CanvasGroupRP;
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
    
    public void SetRelatedProductCurrentState(bool bNewState)
    {
        _bIsActive = bNewState;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
