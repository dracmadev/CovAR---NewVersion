using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MaterialBlockBehaviour : MonoBehaviour
{
    [Header("MATERIAL BLOCK BEHAVIOUR")]
    [Space()]
    [Header("UI Components")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _viewportRT;
    [SerializeField] private RectTransform _contentRT;
    [Header("Max slots before ScrollView can move?:")]
    [SerializeField] private int _MaxMaterialSlotsBeforeScrollCanMove;

    [Header("Array of MaterialSlots:")]
    [SerializeField] private MaterialSlotBehaviour[] _MaterialSlotsArray;

    [Header("Current Selection data: (Inspector View)")]
    [SerializeField] private St_Material[] _IncomingMaterialsList;
    [SerializeField] private St_Material _currentMaterialSelected;

    private ARObjectManager _ARObjectManager;

    void Start()
    {
        //On CatalogManager
    }

    public void InitMaterialBlockBehaviour()
    {
        GameObject arManagerGO = GameObject.FindGameObjectWithTag("ARObjectManager");
        if (arManagerGO != null)
        {
            _ARObjectManager = arManagerGO.GetComponent<ARObjectManager>();
        }

        if (_MaterialSlotsArray == null || _MaterialSlotsArray.Length == 0)
        {
            _MaterialSlotsArray = GetComponentsInChildren<MaterialSlotBehaviour>(true);
        }
    }

    public void InitCatalogMaterialBlock(St_Material[] materialsList, St_Material currentSelected)
    {
        if (_MaterialSlotsArray == null || _MaterialSlotsArray.Length == 0)
        {
            _MaterialSlotsArray = GetComponentsInChildren<MaterialSlotBehaviour>(true);
        }

        _IncomingMaterialsList = materialsList;
        _currentMaterialSelected = currentSelected;

        DoInitOnMaterialSlots();
    }

    void DoInitOnMaterialSlots()
    {
        if (_MaterialSlotsArray == null || _MaterialSlotsArray.Length == 0) return;

        int activeCount = 0;

        for (int i = 0; i < _MaterialSlotsArray.Length; i++)
        {
            _MaterialSlotsArray[i].InitMaterialSlot(this);

            if (_IncomingMaterialsList != null && i < _IncomingMaterialsList.Length)
            {
                St_Material data = _IncomingMaterialsList[i];

                _MaterialSlotsArray[i].SetMaterialData(data);
                _MaterialSlotsArray[i].SetImageOfMaterialSlot(data._MaterialSprite);
                _MaterialSlotsArray[i].SetTextOfMaterialSlot(data._MaterialKey);
                _MaterialSlotsArray[i].SetStateOfMaterialSlot(true);

                activeCount++;
            }
            else
            {
                _MaterialSlotsArray[i].SetMaterialData(null);
                _MaterialSlotsArray[i].SetTextOfMaterialSlot("");
                _MaterialSlotsArray[i].SetStateOfMaterialSlot(false);
            }
        }

        // --- CONTROL DEL SCROLL (Igual que al ModelBlock) ---
        if (_scrollRect != null)
        {
            _scrollRect.horizontal = false;
            _scrollRect.vertical = (activeCount > _MaxMaterialSlotsBeforeScrollCanMove);

            if (activeCount <= _MaxMaterialSlotsBeforeScrollCanMove && _contentRT != null)
            {
                _contentRT.anchoredPosition = new Vector2(_contentRT.anchoredPosition.x, 0);
            }
        }

        if (_contentRT != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRT);
        }

        // --- ANIMACIÓ APAREIXER ESCALONADA ---
        float currentDelay = 0f;
        float delayBetweenSlots = 0.03f;

        for (int i = 0; i < _MaterialSlotsArray.Length; i++)
        {
            if (_MaterialSlotsArray[i].GetActiveState())
            {
                _MaterialSlotsArray[i].AnimateFadeIn(currentDelay);
                currentDelay += delayBetweenSlots;
            }
        }

        SelectCurrentMaterialVisual();
    }

    void SelectCurrentMaterialVisual()
    {
        if (_MaterialSlotsArray == null || _MaterialSlotsArray.Length == 0 || _currentMaterialSelected == null) return;

        foreach (MaterialSlotBehaviour slot in _MaterialSlotsArray)
        {
            if (slot.GetActiveState())
            {
                if (slot.GetCurrentMaterialData()._MaterialType == _currentMaterialSelected._MaterialType)
                {
                    OnSelectButton(slot);
                    break;
                }
            }
        }
    }

    public void OnSelectButton(MaterialSlotBehaviour selectedSlot)
    {
        if (_MaterialSlotsArray == null || _MaterialSlotsArray.Length == 0) return;

        foreach (MaterialSlotBehaviour slot in _MaterialSlotsArray)
        {
            if (slot.GetActiveState())
            {
                if (slot == selectedSlot)
                {
                    slot.SetSelectStateFromMaterialSlot(true);
                    _currentMaterialSelected = slot.GetCurrentMaterialData();

                    RectTransform slotRT = slot.GetComponent<RectTransform>();
                    SnapToMaterial(slotRT);
                }
                else
                {
                    slot.SetSelectStateFromMaterialSlot(false);
                }
            }
        }
    }

    public void SnapToMaterial(RectTransform targetItem)
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