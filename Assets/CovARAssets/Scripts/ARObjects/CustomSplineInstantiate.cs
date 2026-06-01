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

    [Header("Limits & Animation")]
    [Range(0f, 1f)]
    [SerializeField] private float _minPathThreshold = 0.54f; // El teu nou "0" real (la boca del calaix)
    [SerializeField] private float _animationDuration = 2f;    // Temps en segons que triga l'animació inicial

    private GameObject[] _LamasArray;
    private int _MAXLamas;
    private bool _isAnimatingInitial = false;
    private float _animationProgress = 0f;

    void Start()
    {
        InitializeSlatPool();
       
    }

    void Update()
    {
        UpdateBlind();
    }

    void InitializeSlatPool()
    {
        float totalLength = splineContainer.CalculateLength();
        _MAXLamas = Mathf.FloorToInt(totalLength / _lamasSize);

        _LamasArray = new GameObject[_MAXLamas];
        for (int i = 0; i < _MAXLamas; i++)
        {
            _LamasArray[i] = Instantiate(lamaPrefab, transform);
            _LamasArray[i].SetActive(false);
        }
    }

    void UpdateBlind()
    {
        if (splineContainer == null || _LamasArray == null || _LamasArray.Length == 0) return;

        float totalLength = splineContainer.CalculateLength();
        float targetProgress = 0f;

        if (_isAnimatingInitial)
        {
            // Durant l'animació inicial, anem de 0 a 0.54 directament
            targetProgress = _animationProgress * _minPathThreshold;
        }
        else
        {
            // LÒGICA DE MAPEIG: Tradueix el slider (0 a 1) per a que vagi de (0.54 a 1)
            // Fórmula: Min + (Slider * (Max - Min))
            targetProgress = _minPathThreshold + (_sliderValue * (1f - _minPathThreshold));
        }

        // Calculem quants metres realment equival aquest percentatge de camí
        float distanciaActualPersiana = totalLength * targetProgress;
        int lamesAActivar = Mathf.CeilToInt(distanciaActualPersiana / _lamasSize);

        for (int i = 0; i < _MAXLamas; i++)
        {
            if (i < lamesAActivar)
            {
                _LamasArray[i].SetActive(true);

                float distanciaEnMetres = distanciaActualPersiana - (i * _lamasSize);
                if (distanciaEnMetres < 0f) distanciaEnMetres = 0f;

                float t = distanciaEnMetres / totalLength;

                Vector3 position = splineContainer.EvaluatePosition(t);
                Vector3 direction = splineContainer.EvaluateTangent(t);
                Vector3 up = splineContainer.EvaluateUpVector(t);

                _LamasArray[i].transform.position = position;
                _LamasArray[i].transform.rotation = Quaternion.LookRotation(direction, up);
            }
            else
            {
                _LamasArray[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Funció pública per activar l'animació de sortida des del calaix (0 fins a 0.54)
    /// </summary>
    public void PlayInitialOpenAnimation()
    {
        if (!_isAnimatingInitial)
        {
            StartCoroutine(AnimateToThresholdRoutine());
        }
    }

    private IEnumerator AnimateToThresholdRoutine()
    {
        _isAnimatingInitial = true;
        _animationProgress = 0f;
        _sliderValue = 0f; // Ens assegurem que el slider comenci a 0

        float elapsedTime = 0f;

        while (elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;
            // Fem una progressió suau lineal
            _animationProgress = Mathf.Clamp01(elapsedTime / _animationDuration);
            yield return null;
        }

        _animationProgress = 1f;
        _isAnimatingInitial = false; // L'animació ha acabat, ara el slider torna a tenir el control
    }

    private void OnValidate()
    {
        // En l'editor (sense Play), previsualitzem el comportament del slider mapejat
        if (_LamasArray != null && _LamasArray.Length > 0 && !_isAnimatingInitial)
        {
            UpdateBlind();
        }
    }
}