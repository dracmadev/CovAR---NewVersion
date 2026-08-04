using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModelSlotBehaviour : MonoBehaviour
{
    [Header("MODEL SLOT BEHAVIOUR")]
    [Space()]
    [Header("Current Model: (Just to see in Inspector)")]
    [SerializeField] private St_Model _CurrentModelData;

    [Header("UI Components")]
    [SerializeField] private Button _ModelSlotButton;
    [SerializeField] private Image _ModelSlotImage;
    [SerializeField] private CanvasGroup _ModelSlotCG;
    [SerializeField] private CanvasGroup _ModelSelectedTickCG;
    [SerializeField] private TMP_Text _ModelNameText;

    

    private ModelBlockBehaviour _ModelBlockBehaviour;
    private ARObjectManager _ARObjectManager;

    private bool bImActive;

    //////////////////////////////////////////////////////////////////////////////// INIT ////////////////////////////////////////////////////////////////////////////////

    public void InitModelSlot(ModelBlockBehaviour modelBlock)
    {
        _ModelBlockBehaviour = modelBlock;

        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        _ModelSlotButton.onClick.RemoveListener(OnClickSelectModel);
        _ModelSlotButton.onClick.AddListener(OnClickSelectModel);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////// BEHAVIOUR ///////////////////////////////////////////////////////////////////////////////////////////////

    public void SetModelData(St_Model modelData)
    {
        _CurrentModelData = modelData;
    }

    public void SetStateOfModelSlot(bool active)
    {
        if (_ModelSlotCG == null) _ModelSlotCG = this.GetComponent<CanvasGroup>();

        bImActive = active;

        if (_ModelSlotCG != null) _ModelSlotCG.DOKill();

        if (active)
        {
            _ModelSlotCG.alpha = 0f;
            _ModelSlotCG.interactable = true;
            _ModelSlotCG.blocksRaycasts = true;
        }
        else
        {
            _ModelSlotCG.alpha = 0f;
            _ModelSlotCG.interactable = false;
            _ModelSlotCG.blocksRaycasts = false;

            if (_ModelSelectedTickCG != null) _ModelSelectedTickCG.alpha = 0f;
        }
    }

    public void AnimateFadeIn(float delayTime)
    {
        if (_ModelSlotCG == null) return;

        _ModelSlotCG.DOFade(1f, 0.3f)
                    .SetDelay(delayTime)
                    .SetEase(Ease.OutCubic);
    }

    public void SetSelectStateFromModelSlot(bool isSelected)
    {
        if (isSelected)
        {
            _ModelSelectedTickCG.alpha = 1f;
        }
        else
        {
            _ModelSelectedTickCG.alpha = 0f;
        }
    }

    public void SetImageOfModelSlot(Sprite sp)
    {
        if (_ModelSlotImage != null)
        {
            _ModelSlotImage.sprite = sp;
        }
    }

    public void OnClickSelectModel()
    {
        if (_ModelBlockBehaviour != null)
        {
            _ModelBlockBehaviour.OnSelectButton(this);
        }

        _ARObjectManager.SetCurrentModel(_CurrentModelData._ModelType, _CurrentModelData._ModelGeneralType);
        _ARObjectManager.SetCurrentMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(_CurrentModelData)._MaterialType);

        if (_ARObjectManager.GetIsARObjectPlaced())
        {
          
            _ARObjectManager.SetActiveARObject(GetPoolTypeBasedOnModelType());
        }
        
       
    }


    public void SetTextOfModelSlot(string translationKey)
    {
        if (_ModelNameText != null)
        {
            if (!string.IsNullOrEmpty(translationKey))
            {
                _ModelNameText.text = Assets.SimpleLocalization.Scripts.LocalizationManager.Localize(translationKey);
            }
            else
            {
                _ModelNameText.text = "";
            }
        }
    }

    public void OnUpdateMaterials(MaterialBlockBehaviour _MaterialBlockBehaviour)
    {
        if( _MaterialBlockBehaviour != null)
        {
            _MaterialBlockBehaviour.InitCatalogMaterialBlock(_CurrentModelData._MaterialsArray, _ARObjectManager.GetFirstMaterialFromCurrentModel(_CurrentModelData));
        }
        
    }

    ///////////////////////////////////////////////////////////////////////////////// GETTERS /////////////////////////////////////////////////////////////////////////////////

    public bool GetActiveState()
    {
        return bImActive;
    }

    public St_Model GetCurrentModelData()
    {
        return _CurrentModelData;
    }

     E_PoolType GetPoolTypeBasedOnModelType()
     {
        E_PoolType returnVar = E_PoolType.None;

        switch (_CurrentModelData._ModelType)
        {
            case E_ModelType.None: returnVar = E_PoolType.None;  break;

            case E_ModelType.AP_Octeo: returnVar = E_PoolType.AP_Octeo; break;
            case E_ModelType.AP_Sveltea: returnVar = E_PoolType.AP_Sveltea; break;
            case E_ModelType.AP_SvelteaManual: returnVar = E_PoolType.AP_SvelteaManual; break;
            case E_ModelType.AP_Coverly: returnVar = E_PoolType.AP_Coverly; break;
            case E_ModelType.AP_Bellasun: returnVar = E_PoolType.AP_Bellasun; break;

            case E_ModelType.AP_Rousillon: returnVar = E_PoolType.AP_Rousillon; break;

            case E_ModelType.AP_LeBancSmallFoot: returnVar = E_PoolType.AP_Lebanc_Small; break;
            case E_ModelType.AP_LeBancBigFoot: returnVar = E_PoolType.AP_Lebanc_Big; break;
            case E_ModelType.AP_LeBancTopCladding: returnVar = E_PoolType.None; break;
            case E_ModelType.AP_LeBancSidesCladding: returnVar = E_PoolType.None; break;

            case E_ModelType.AP_LamasPolicarbonate: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.AP_LamasPVC: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;

            case E_ModelType.BAC_FabricCover_Multiversal: returnVar = E_PoolType.BAC_FabricCover_Multiversal; _ARObjectManager.SetFabricCoverMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_FabricCover_Novatrend: returnVar = E_PoolType.BAC_FabricCover_Novatrend; _ARObjectManager.SetFabricCoverMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_FabricCover_SwissTop: returnVar = E_PoolType.BAC_FabricCover_SwissTop; _ARObjectManager.SetFabricCoverMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;

            case E_ModelType.BAC_SlattedRollfix: returnVar = E_PoolType.BAC_SlattedRollfix; break;
            case E_ModelType.BAC_SlattedRonda: returnVar = E_PoolType.BAC_SlattedRonda; break;
            case E_ModelType.BAC_SlattedLido: returnVar = E_PoolType.BAC_SlattedLido; break;
            case E_ModelType.BAC_SlattedAcoe: returnVar = E_PoolType.BAC_SlattedAcoe; break;
            case E_ModelType.BAC_SlattedDavos: returnVar = E_PoolType.BAC_SlattedDavos; break;

            case E_ModelType.BAC_IsolaRollfix: returnVar = E_PoolType.BAC_IsolaRollfix; _ARObjectManager.SetIsolaCoverMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_IsolaAcoe: returnVar = E_PoolType.BAC_IsolaAcoe; _ARObjectManager.SetIsolaCoverMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_IsolaRonda: returnVar = E_PoolType.BAC_IsolaRonda; _ARObjectManager.SetIsolaCoverMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_IsolaRondalino: returnVar = E_PoolType.BAC_IsolaRondalino; _ARObjectManager.SetIsolaCoverMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            
            case E_ModelType.BAC_CladdingAcoe: returnVar = E_PoolType.BAC_CladdingAcoe; break;
            case E_ModelType.BAC_CladdingQuadro: returnVar = E_PoolType.BAC_CladdingQuadro; break;
            case E_ModelType.BAC_CladdingLido: returnVar = E_PoolType.BAC_CladdingLido; break;
            case E_ModelType.BAC_CladdingDavos: returnVar = E_PoolType.BAC_CladdingDavos; break; 


            case E_ModelType.BAC_Isola: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_Whirloop: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;

            case E_ModelType.BAC_LamasDuotherm: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_LamasTwist: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_RollmaticPVC: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_RollmaticPolycarbonate: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;
            case E_ModelType.BAC_RollmaticPolycarbonatePro: returnVar = E_PoolType.None; _ARObjectManager.SetLamasMaterial(_ARObjectManager.GetFirstMaterialFromCurrentModel(GetCurrentModelData())); break;

        }

        return returnVar;
     }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}