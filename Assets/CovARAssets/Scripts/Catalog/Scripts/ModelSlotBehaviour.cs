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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}