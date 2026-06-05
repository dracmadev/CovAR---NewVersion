using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class APBasicInfoBlock : MonoBehaviour
{
    [Header("[ASTRALPOOL] BASIC INFO BLOCK")]
    [Space()]
    [Header("MODEL INFO:")]
    [SerializeField] private TMP_Text _CurrentModelText;
    [SerializeField] private Image _CurrentModelImage;
    [Header("LAMAS INFO:")]
    [SerializeField] private TMP_Text _CurrentLamasText;
    [SerializeField] private Image _CurrentLamasImage;
    [Header("COLORS:")]
    [SerializeField] private TMP_Text _CurrentModelColorText;
    [SerializeField] private TMP_Text _CurrentLamasColorText;
    [SerializeField] private TMP_Text _CurrentTopCladdingColorText;
    [SerializeField] private TMP_Text _CurrentSidesCladdingColorText;
    [SerializeField] private CanvasGroup _CanvasGroupTopCladdingBlock;
    [SerializeField] private CanvasGroup _CanvasGroupSidesCladdingBlock;
    [Header("DIMENSIONS:")]
    [SerializeField] private TMP_InputField _CurrentModelLenghtInputText;
    [SerializeField] private TMP_InputField _CurrentLamasLenghtInputText;
    [SerializeField] private Image _CurrentBlueprintModelImage;
    [Header("ANNOTATIONS:")]
    [SerializeField] private TMP_InputField _AnnotationsInputText;



    ARObjectManager _ARObjectManager;

    /////////////////////////////////////////////////////////////////// DEFAULT FUNCTIONS //////////////////////////////////////////////////////////////////////////
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////////////

    public void InitBasicInfo(ARObjectManager objManager)
    {
        _ARObjectManager = objManager;
        InitInfo();
    }

    void InitInfo()
    {
        if (_ARObjectManager == null) return;

        //CURRENT MODEL (Name, image, blueprint image & color)
        _CurrentModelText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey); 
        _CurrentModelImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialSprite;
        _CurrentBlueprintModelImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentModelModel._BlueprintModelImage;
        _CurrentModelColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey);

        //CURRENT LAMAS (Name, image)
        _CurrentLamasText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey);
        _CurrentLamasImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialSprite;
        _CurrentLamasColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey);

        //TOP CLADDING
        if(_ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialType != E_MaterialType.None)
        {
            _CanvasGroupTopCladdingBlock.alpha = 1f;
            _CurrentTopCladdingColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialKey);
        }
        else
        {
            _CanvasGroupTopCladdingBlock.alpha = 0f;
        }

        //SIDES CLADDING
        if (_ARObjectManager.GetCovARObjectData()._CurrentSidesCladdingMaterial._MaterialType != E_MaterialType.None)
        {
            _CanvasGroupSidesCladdingBlock.alpha = 1f;
            _CurrentSidesCladdingColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentSidesCladdingMaterial._MaterialKey);
        }
        else
        {
            _CanvasGroupSidesCladdingBlock.alpha = 0f;
        }

        //DIMENSIONS
        _CurrentModelLenghtInputText.text = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString();
        _CurrentLamasLenghtInputText.text = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString();


    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// SETTER /////////////////////////////////////////////////////////////////////////




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
