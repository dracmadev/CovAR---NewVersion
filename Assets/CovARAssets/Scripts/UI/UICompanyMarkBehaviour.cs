using DG.Tweening;
using UnityEngine;

public class UICompanyMarkBehaviour : MonoBehaviour
{
    [Header("Company Mark Settings")]
    [Space()]
    [Header("Anim Settings:")]
    [SerializeField] private float _animDuration = 0.3f;
    [SerializeField] private float _YOffset = 500f;
    [SerializeField] private Ease _animEase = Ease.InOutSine;

    [Header("Behaviour settings:")]
    [SerializeField] private bool _IsOnScreen = false;


    private RectTransform _rectTransform;
    private Vector2 _originalLocalPosition;
    private Vector2 _hiddenLocalPosition;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        _originalLocalPosition = _rectTransform.anchoredPosition;

        _hiddenLocalPosition = new Vector2(_originalLocalPosition.x, _originalLocalPosition.y + _YOffset);

        if (!_IsOnScreen)
        {
            _rectTransform.anchoredPosition = _hiddenLocalPosition;
        }
    }


    /////////////////////////////////////////////////////////////////// ANIM BEHAVIOUR ////////////////////////////////////////////////////////////////////////

    public void OnCompanyMarkBehaviour(bool show)
    {
        if (show)
        {
            OnShowCompanyMark();
        }
        else
        {
            OnHideCompanyMark();
        }
    }

    void OnShowCompanyMark()
    {
        _IsOnScreen = true;

        _rectTransform.DOKill();

        _rectTransform.DOAnchorPos(_originalLocalPosition, _animDuration)
            .SetEase(_animEase);
    }

    void OnHideCompanyMark()
    {
        _IsOnScreen = false;

        _rectTransform.DOKill();

        _rectTransform.DOAnchorPos(_hiddenLocalPosition, _animDuration)
            .SetEase(_animEase);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}