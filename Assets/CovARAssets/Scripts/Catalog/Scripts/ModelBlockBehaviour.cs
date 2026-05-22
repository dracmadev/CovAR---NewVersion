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
        }

        if (_ModelSlotsArray == null || _ModelSlotsArray.Length == 0)
        {
            _ModelSlotsArray = GetComponentsInChildren<ModelSlotBehaviour>(true);
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

                _ModelSlotsArray[i].SetTextOfModelSlot(data._ModelKey);

                _ModelSlotsArray[i].SetStateOfModelSlot(true);

                activeCount++;
            }
            else
            {
                _ModelSlotsArray[i].SetModelData(null);

                _ModelSlotsArray[i].SetTextOfModelSlot("");

                _ModelSlotsArray[i].SetStateOfModelSlot(false);
            }
        }

        // --- CONTROL DEL SCROLL ---
        if (_scrollRect != null)
        {
           
            _scrollRect.horizontal = false;
            _scrollRect.vertical = (activeCount > _MaxModelSlotsBeforeScrollCanMove);

            if (activeCount <= _MaxModelSlotsBeforeScrollCanMove && _contentRT != null)
            {
                _contentRT.anchoredPosition = new Vector2(_contentRT.anchoredPosition.x, 0);
            }
        }

        if (_contentRT != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRT);
        }

        // --- AnimacioDeApareixer ---
        float currentDelay = 0f;
        float delayBetweenSlots = 0.03f; 

        for (int i = 0; i < _ModelSlotsArray.Length; i++)
        {
            if (_ModelSlotsArray[i].GetActiveState())
            {
                _ModelSlotsArray[i].AnimateFadeIn(currentDelay);
                currentDelay += delayBetweenSlots; 
            }
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

        if (!_scrollRect.vertical)
        {
            _contentRT.anchoredPosition = new Vector2(_contentRT.anchoredPosition.x, 0);
            return;
        }

        float itemCenterY = targetItem.anchoredPosition.y;

        float targetY = -itemCenterY - (_viewportRT.rect.height / 2);


        float maxScroll = _contentRT.rect.height - _viewportRT.rect.height;
        float minScroll = 0;

        targetY = Mathf.Clamp(targetY, minScroll, maxScroll);

        _contentRT.DOAnchorPosY(targetY, 0.4f).SetEase(Ease.OutCubic);
    }
 }