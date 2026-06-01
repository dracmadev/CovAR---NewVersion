using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleLocalization.Scripts;

public class MaterialSlotBehaviour : MonoBehaviour
{
    [Header("MATERIAL SLOT BEHAVIOUR")]
    [Space()]
    [Header("Current Material: (Inspector View)")]
    [SerializeField] private St_Material _CurrentMaterialData;

    [Header("UI Components")]
    [SerializeField] private Button _MaterialSlotButton;
    [SerializeField] private Image _MaterialSlotImage;
    [SerializeField] private CanvasGroup _MaterialSlotCG;
    [SerializeField] private CanvasGroup _MaterialSelectedTickCG;
    [SerializeField] private TMP_Text _MaterialNameText;

    private MaterialBlockBehaviour _MaterialBlockBehaviour;
    private ARObjectManager _ARObjectManager;
    private bool bImActive;

    public void InitMaterialSlot(MaterialBlockBehaviour materialBlock)
    {
        _MaterialBlockBehaviour = materialBlock;

        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        _MaterialSlotButton.onClick.RemoveListener(OnClickSelectMaterial);
        _MaterialSlotButton.onClick.AddListener(OnClickSelectMaterial);
    }

    public void SetMaterialData(St_Material materialData)
    {
        _CurrentMaterialData = materialData;
    }

    public void SetStateOfMaterialSlot(bool active)
    {
        if (_MaterialSlotCG == null) _MaterialSlotCG = this.GetComponent<CanvasGroup>();

        bImActive = active;

        if (_MaterialSlotCG != null) _MaterialSlotCG.DOKill();

        if (active)
        {
            _MaterialSlotCG.alpha = 0f;
            _MaterialSlotCG.interactable = true;
            _MaterialSlotCG.blocksRaycasts = true;
        }
        else
        {
            _MaterialSlotCG.alpha = 0f;
            _MaterialSlotCG.interactable = false;
            _MaterialSlotCG.blocksRaycasts = false;

            if (_MaterialSelectedTickCG != null) _MaterialSelectedTickCG.alpha = 0f;
        }
    }

    public void AnimateFadeIn(float delayTime)
    {
        if (_MaterialSlotCG == null) return;

        _MaterialSlotCG.DOFade(1f, 0.3f)
                    .SetDelay(delayTime)
                    .SetEase(Ease.OutCubic);
    }

    public void SetSelectStateFromMaterialSlot(bool isSelected)
    {
        if (_MaterialSelectedTickCG != null)
        {
            _MaterialSelectedTickCG.alpha = isSelected ? 1f : 0f;
        }
    }

    public void SetImageOfMaterialSlot(Sprite sp)
    {
        if (_MaterialSlotImage != null)
        {
            _MaterialSlotImage.sprite = sp;
        }
    }

    public void OnClickSelectMaterial()
    {
        if (_MaterialBlockBehaviour != null)
        {
            _MaterialBlockBehaviour.OnSelectButton(this);
        }

        if (_ARObjectManager != null && _CurrentMaterialData != null)
        {
            SetMaterialBehaviour();
        }
    }

    public void SetTextOfMaterialSlot(string translationKey)
    {
        if (_MaterialNameText != null)
        {
            if (!string.IsNullOrEmpty(translationKey))
            {
                _MaterialNameText.text = LocalizationManager.Localize(translationKey);
            }
            else
            {
                _MaterialNameText.text = "";
            }
        }
    }

    public bool GetActiveState() => bImActive;
    public St_Material GetCurrentMaterialData() => _CurrentMaterialData;
  
    public void SetMaterialBehaviour()
    {
        _ARObjectManager.SetCurrentMaterial(_CurrentMaterialData._MaterialType);

        switch (_ARObjectManager.GetCurrentProductData()._ProductType)
        {
            case E_ProductType.None: break;
            case E_ProductType.GroundRollerCover:
                _ARObjectManager.SetCovARFootMaterial(_CurrentMaterialData);
                break;
            case E_ProductType.SubmergedRollerCover:
                _ARObjectManager.SetCovARSubmergedMaterial(_CurrentMaterialData);
                break;
            case E_ProductType.BencheAndCladdings:
                
                switch(_ARObjectManager.GetCurrentModelData()._ModelType)
                {
                    case E_ModelType.AP_LeBancBigFoot: _ARObjectManager.SetCovARFootMaterial(_CurrentMaterialData); break;
                    case E_ModelType.AP_LeBancSmallFoot: _ARObjectManager.SetCovARFootMaterial(_CurrentMaterialData); break;
                    case E_ModelType.AP_LeBancTopCladding: break;
                    case E_ModelType.AP_LeBancSidesCladding: break;
                }

                break;
            case E_ProductType.FabricCover: break;
            case E_ProductType.Lamas: break;
        }
    }
}