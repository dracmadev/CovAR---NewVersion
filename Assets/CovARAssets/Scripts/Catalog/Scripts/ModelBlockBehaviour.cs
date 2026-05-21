using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ModelBlockBehaviour : MonoBehaviour
{
    [Header("MODEL BLOCK BEHAVIOUR")]
    [Space()]
    [Header("UI Components")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _viewportRT;
    [SerializeField] private RectTransform _contentRT;
    [Header("Max slots before ScrollView can move?:")]
    [SerializeField] private int _MaxModelSlotsBeforeScrollCanMove;


    [Header("Array of ModelSlots:")]
    [SerializeField] private ModelSlotBehaviour[] _ModelSlotsArray;

    [Header("Current Selection data: (Just to see)")]
    [SerializeField] private St_Model[] _IncomingModelsModelsList; 
    [SerializeField] private St_Model _currentModelSelected;      

    private ARObjectManager _ARObjectManager;

    void Start()
    {
        InitModelBlockBehaviour();
    }

    void InitModelBlockBehaviour()
    {
        GameObject arManagerGO = GameObject.FindGameObjectWithTag("ARObjectManager");
        if (arManagerGO != null)
        {
            _ARObjectManager = arManagerGO.GetComponent<ARObjectManager>();

            if(_ARObjectManager.GetCurrentProductCatalog() == null)
            {
                _ARObjectManager.InitCatalogBehaviour();
            }
        }

        if (_ModelSlotsArray == null || _ModelSlotsArray.Length == 0)
        {
            _ModelSlotsArray = GetComponentsInChildren<ModelSlotBehaviour>(true);
        }
      
        if (_ARObjectManager != null && _ARObjectManager.GetCurrentProductData() != null)
        {
            St_Model[] modelsArray = _ARObjectManager.GetCurrentProductData()._ModelsArray;

            InitCatalogModelBlock(modelsArray, _ARObjectManager.GetFirstModelFromCurrentProduct(_ARObjectManager.GetCurrentProductData()));
        }
        else
        {
            Debug.LogWarning("No current product data found in ARObjectManager o el Manager és null.");
        }
    }

    ///////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////////////

    public void InitCatalogModelBlock(St_Model[] modelsList, St_Model currentSelected)
    {

        if (_ModelSlotsArray == null || _ModelSlotsArray.Length == 0)
        {
            _ModelSlotsArray = GetComponentsInChildren<ModelSlotBehaviour>(true);
        }

        _IncomingModelsModelsList = modelsList;
        _currentModelSelected = currentSelected;

        DoInitOnModelSlots();
    }

    void DoInitOnModelSlots()
    {
        if (_ModelSlotsArray == null || _ModelSlotsArray.Length == 0) return;

        int activeCount = 0; 

        for (int i = 0; i < _ModelSlotsArray.Length; i++)
        {
            _ModelSlotsArray[i].InitModelSlot(this);

            if (_IncomingModelsModelsList != null && i < _IncomingModelsModelsList.Length)
            {
                St_Model data = _IncomingModelsModelsList[i];

                _ModelSlotsArray[i].SetModelData(data);
                _ModelSlotsArray[i].SetImageOfModelSlot(data._ModelLogo); 
                _ModelSlotsArray[i].SetStateOfModelSlot(true);            

                activeCount++;
            }
            else
            {
                _ModelSlotsArray[i].SetModelData(null);
                _ModelSlotsArray[i].SetStateOfModelSlot(false);
            }
        }

        // --- CONTROL DEL SCROLL ---
        if (_scrollRect != null)
        {
            _scrollRect.horizontal = (activeCount > _MaxModelSlotsBeforeScrollCanMove);

            if (activeCount <= _MaxModelSlotsBeforeScrollCanMove && _contentRT != null)
            {
                _contentRT.anchoredPosition = new Vector2(0, _contentRT.anchoredPosition.y);
            }
        }

        if (_contentRT != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRT);
        }

        SelectCurrentModelVisual();
    }

    void SelectCurrentModelVisual()
    {
        if (_ModelSlotsArray == null || _ModelSlotsArray.Length == 0 || _currentModelSelected == null) return;

        foreach (ModelSlotBehaviour slot in _ModelSlotsArray)
        {
            if (slot.GetActiveState())
            {
            
                if (slot.GetCurrentModelData()._ModelType == _currentModelSelected._ModelType)
                {
                    OnSelectButton(slot);
                    break; 
                }
            }
        }
    }

    public void OnSelectButton(ModelSlotBehaviour selectedSlot)
    {
        if (_ModelSlotsArray == null || _ModelSlotsArray.Length == 0) return;

        foreach (ModelSlotBehaviour slot in _ModelSlotsArray)
        {
            if (slot.GetActiveState())
            {
                if (slot == selectedSlot)
                {
                    slot.SetSelectStateFromModelSlot(true);
                    _currentModelSelected = slot.GetCurrentModelData();

                    RectTransform slotRT = slot.GetComponent<RectTransform>();
                    SnapToModel(slotRT);
                }
                else
                {
                    slot.SetSelectStateFromModelSlot(false);
                }
            }
        }
    }

    public void SnapToModel(RectTransform targetItem)
    {
        if (_scrollRect == null || _contentRT == null || _viewportRT == null) return;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRT);

        _contentRT.DOKill();

        if (!_scrollRect.horizontal)
        {
            _contentRT.anchoredPosition = new Vector2(0, _contentRT.anchoredPosition.y);
            return;
        }

        float itemCenterX = targetItem.anchoredPosition.x;

        float targetX = -itemCenterX + (_viewportRT.rect.width / 2);

        float minScroll = -(_contentRT.rect.width - _viewportRT.rect.width);
        float maxScroll = 0;
        targetX = Mathf.Clamp(targetX, minScroll, maxScroll);

        _contentRT.DOAnchorPosX(targetX, 0.4f).SetEase(Ease.OutCubic);
    }
}