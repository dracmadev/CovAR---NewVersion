using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class CustomSplineInstantiate : MonoBehaviour
{
    [Header("CUSTOM SPLINE INSTANTIATE:")]
    [Space()]
    [Header("Components:")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject lamaPrefab;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float _sliderValue = 0f;
    [SerializeField] private float _lamasSize = 0.05f;
    [SerializeField] private float _currentMaxDistance = 15f;
    [SerializeField] private int _initKnotIndex = 9;
    [SerializeField] private int _endtKnotIndex = 11;

    [Header("Limits & Animation")]
    [SerializeField] private float _animationDuration = 2f;    // Temps en segons que triga l'animació inicial

    private GameObject[] _LamasArray;
    private int _MAXLamas;
    private bool _isAnimatingInitial = false;

    private float _lastSliderValue = 0f;


    ARObjectManager _ARObjectManager;

    void Update()
    {
        if (_LamasArray != null && !_isAnimatingInitial)
        {
            
            if (Mathf.Approximately(_sliderValue, _lastSliderValue)) return;

            _lastSliderValue = _sliderValue;
            UpdateBlind();
        }
    }

    public void InitializeSlatPool()
    {
        _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();

        if (splineContainer == null || splineContainer.Spline == null) return;

        if (_LamasArray != null)
        {
            foreach (var go in _LamasArray) { if (go != null) Destroy(go); }
        }

        float totalLength = splineContainer.CalculateLength();
        _MAXLamas = Mathf.FloorToInt(totalLength / _lamasSize);

        _LamasArray = new GameObject[_MAXLamas];
        for (int i = 0; i < _MAXLamas; i++)
        {
            _LamasArray[i] = Instantiate(lamaPrefab, transform);
            _LamasArray[i].SetActive(false);
        }
    }

    //////////////////////////////////////////////////////////////// GETTER ////////////////////////////////////////////////////////////////////////
    
    private float GetDistanceToKnot(int knotIndex)
    {
        if (splineContainer == null || splineContainer.Spline == null) return 0f;

        float distance = 0f;
        
        for (int j = 0; j < knotIndex; j++)
        {
            if (j < splineContainer.Spline.Count - 1)
            {
                distance += splineContainer.Spline.GetCurveLength(j);
            }
        }
        return distance;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    ///////////////////////////////////////////////////////////////// BEHAVIOUR //////////////////////////////////////////////////////////////////////

    public void UpdateBlind()
    {
        if (_isAnimatingInitial) return;
        if (splineContainer == null || splineContainer.Spline == null || _LamasArray == null) return;

        float totalSplineLength = splineContainer.CalculateLength();

        // 1. Distancia entre Knot _initKnotIndex i _endtKnotIndex
        float initPos = GetDistanceToKnot(_initKnotIndex);
        float maxPosSpline = GetDistanceToKnot(_endtKnotIndex);

        // 2. Calcul espai real
        float usefullLenght = maxPosSpline - initPos;
        float currentUsefullLenght = Mathf.Min(_currentMaxDistance, usefullLenght);

        // 3. Calcul distancies (de  _initKnotIndex a Current lenght i total)
        float currentlenghtFromInitToCurrentSliderValue = _sliderValue * currentUsefullLenght;
        float fullSplineDistance = initPos + currentlenghtFromInitToCurrentSliderValue;

        // 4. Calcul lamas
        int currentNumActiveLamas = Mathf.CeilToInt(fullSplineDistance / _lamasSize);
        currentNumActiveLamas = Mathf.Clamp(currentNumActiveLamas, 0, _MAXLamas);

        for (int i = 0; i < _MAXLamas; i++)
        {
            if (i < currentNumActiveLamas)
            {
                _LamasArray[i].SetActive(true);

                float lamasPosition = fullSplineDistance - (i * _lamasSize);
                if (lamasPosition < 0f) lamasPosition = 0f;

                float t = lamasPosition / totalSplineLength;
                t = Mathf.Clamp01(t);

                _LamasArray[i].transform.position = splineContainer.EvaluatePosition(t);

                Vector3 direction = splineContainer.EvaluateTangent(t);
                Vector3 up = splineContainer.EvaluateUpVector(t);

                if (direction.sqrMagnitude > 0.0001f)
                {
                    _LamasArray[i].transform.rotation = Quaternion.LookRotation(direction, up);
                }
                else
                {
                    Vector3 fallbackDirection = splineContainer.EvaluateTangent(Mathf.Clamp01(t + 0.01f));
                    if (fallbackDirection.sqrMagnitude > 0.0001f)
                        _LamasArray[i].transform.rotation = Quaternion.LookRotation(fallbackDirection, up);
                    else
                        _LamasArray[i].transform.rotation = Quaternion.identity;
                }
            }
            else
            {
                _LamasArray[i].SetActive(false);
            }
        }
    }

    
    public void PlayInitialOpenAnimation()
    {
        InitializeSlatPool();

        if (!_isAnimatingInitial)
        {
            InitAnimation();
        }
    }

    public void InitAnimation()
    {
        _isAnimatingInitial = true;

        float totalSplineLength = splineContainer.CalculateLength();
        float lengthOfAnim = GetDistanceToKnot(_initKnotIndex);

        float animatedDistance = 0f;
        
        DOTween.Kill(this);

      
        DOTween.To(() => animatedDistance, x => animatedDistance = x, lengthOfAnim, _animationDuration)
            .SetEase(Ease.OutCubic) 
            .SetTarget(this)        
            .OnUpdate(() =>
            {
                UpdateBlindForAnimation(animatedDistance, totalSplineLength);
            })
            .OnComplete(() =>
            {
                UpdateBlindForAnimation(lengthOfAnim, totalSplineLength);

                _isAnimatingInitial = false;
                _sliderValue = 0f;
                _lastSliderValue = 0f; 
                UpdateBlind();
            });
    }

    private void UpdateBlindForAnimation(float lamasLenght, float totalSplineLength)
    {
        int currentNumActiveLamas = Mathf.CeilToInt(lamasLenght / _lamasSize);
        if (lamasLenght > 0f && currentNumActiveLamas == 0) currentNumActiveLamas = 1;
        currentNumActiveLamas = Mathf.Clamp(currentNumActiveLamas, 0, _MAXLamas);

        for (int i = 0; i < _MAXLamas; i++)
        {
            if (i < currentNumActiveLamas)
            {
                _LamasArray[i].SetActive(true);

                float lamasPosition = lamasLenght - (i * _lamasSize);
                if (lamasPosition < 0f) lamasPosition = 0f;

                float t = lamasPosition / totalSplineLength;
                t = Mathf.Clamp01(t);

                _LamasArray[i].transform.position = splineContainer.EvaluatePosition(t);

                Vector3 tangent = splineContainer.EvaluateTangent(t);
                Vector3 up = splineContainer.EvaluateUpVector(t);

                if (tangent.sqrMagnitude > 0.0001f)
                {
                    _LamasArray[i].transform.rotation = Quaternion.LookRotation(tangent, up);
                }
            }
            else
            {
                _LamasArray[i].SetActive(false);
            }
        }
    }

    private void OnValidate()
    {
        if (splineContainer != null && splineContainer.Spline != null && _LamasArray != null && _LamasArray.Length > 0 && !_isAnimatingInitial)
        {
            UpdateBlind();
        }
    }

    public void SetSliderValueFromUI(float normalizedValue)
    {
        _sliderValue = Mathf.Clamp01(normalizedValue);
        _lastSliderValue = _sliderValue; 
        UpdateBlind();
    }

    public void SetupMaxLamasDistance(float maxDistanceInMeters)
    {
        _currentMaxDistance = maxDistanceInMeters;
        UpdateBlind();
    }

    public float GetCurrentPosition()
    {
        if (_isAnimatingInitial) return 0f;
        if (splineContainer == null || splineContainer.Spline == null || _LamasArray == null || _LamasArray.Length == 0) return 0f;

        if (_sliderValue <= 0.001f) return 0f;

        float initLenght = GetDistanceToKnot(_initKnotIndex);
        float absolutSplineLenght = GetDistanceToKnot(_endtKnotIndex);

        float usefullLamasLenght = absolutSplineLenght - initLenght;
        float currentUsefullLamasLenght = Mathf.Min(_currentMaxDistance, usefullLamasLenght);

        float currentlenghtFromInitToCurrentSliderValue = _sliderValue * currentUsefullLamasLenght;

        float currentLenghtRounded = Mathf.Round(currentlenghtFromInitToCurrentSliderValue / _lamasSize) * _lamasSize;

        return Mathf.Max(0f, currentLenghtRounded);
    }

    public void SetLamasLenghtByNum(float newLenght, float maxLamasLenghtDistance, GameObject lenghtSliderRef, GameObject feedbackLenghtText)
    {
        if (splineContainer == null || splineContainer.Spline == null) return;
        if (_LamasArray == null || _LamasArray.Length == 0) InitializeSlatPool();

        // 1. Calculem distancies
        float initLenght = GetDistanceToKnot(_initKnotIndex);
        float absolutSplineLenght = GetDistanceToKnot(_endtKnotIndex);
        float usefullLamasLenght = absolutSplineLenght - initLenght;
        float currentUsefullLamasLenght = Mathf.Min(maxLamasLenghtDistance, usefullLamasLenght);

        // Forcem que la llargada 
        newLenght = Mathf.Clamp(newLenght, 0f, currentUsefullLamasLenght);
        float lenghtRounded = Mathf.Round(newLenght / _lamasSize) * _lamasSize;

        // 2. Assignem les variables
        _currentMaxDistance = currentUsefullLamasLenght;
        _sliderValue = _currentMaxDistance > 0f ? Mathf.Clamp01(lenghtRounded / _currentMaxDistance) : 0f;

        // 3. Forcem el redibuix de les lames
        UpdateBlind();

        // 4. MODIFIQUEM EL SLIDER DE LA INTERFÍCIE (UI)
        if (lenghtSliderRef != null)
        {
            var uiBehaviour = lenghtSliderRef.GetComponent<UIBehaviourComponent>();
            if (uiBehaviour != null)
            {
                var sliderData = uiBehaviour.GetSliderData();
                sliderData.sliderMin = 0f;
                sliderData.sliderMax = currentUsefullLamasLenght;
                sliderData.sliderResult = newLenght;

                UnityEngine.UI.Slider unitySlider = lenghtSliderRef.GetComponent<UnityEngine.UI.Slider>();
                if (unitySlider != null)
                {
                    float normalizedValue = newLenght / currentUsefullLamasLenght;
                    unitySlider.value = Mathf.Clamp01(normalizedValue);
                }
            }
        }

        // 5. ACTUALITZACIÓ DEL TEXT DE LA UI
        if (feedbackLenghtText != null)
        {
            feedbackLenghtText.GetComponent<TMPro.TMP_Text>().text = newLenght.ToString("F2") + "m";
        }

        // 6. Actualitzem Manager
        if (_ARObjectManager != null)
        {
            _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght = newLenght;
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}