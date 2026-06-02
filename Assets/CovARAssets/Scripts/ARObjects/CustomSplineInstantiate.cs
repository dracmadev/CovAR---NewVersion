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

    [Header("Limits & Animation")]
    [SerializeField] private float _animationDuration = 2f;    // Temps en segons que triga l'animació inicial

    private GameObject[] _LamasArray;
    private int _MAXLamas;
    private bool _isAnimatingInitial = false;

    void Update()
    {
        if (_LamasArray != null && !_isAnimatingInitial)
        {
            UpdateBlind();
        }
    }

    public void InitializeSlatPool()
    {
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

    // =================================================================================================
    // MÈTODE AUXILIAR: CALCULA ELS METRES REALS FINALS FINS A UN KNOT CONCRET SUMANT CORBES
    // =================================================================================================
    private float GetDistanceToKnot(int knotIndex)
    {
        if (splineContainer == null || splineContainer.Spline == null) return 0f;

        float distance = 0f;
        // Una corba (Curve) uneix el Knot 'j' amb el Knot 'j+1'. 
        // Sumem les longituds de totes les corbes prèvies fins a arribar al Knot demanat.
        for (int j = 0; j < knotIndex; j++)
        {
            if (j < splineContainer.Spline.Count - 1)
            {
                distance += splineContainer.Spline.GetCurveLength(j);
            }
        }
        return distance;
    }

    // =================================================================================================
    // UPDATE BLIND: CONTROLAT PEL SLIDER (Del Knot 9 real al Knot 11 real)
    // =================================================================================================
    public void UpdateBlind()
    {
        if (_isAnimatingInitial) return;
        if (splineContainer == null || splineContainer.Spline == null || _LamasArray == null) return;

        float totalSplineLength = splineContainer.CalculateLength();

        // 1. Busquem els metres geomètrics de la sortida (Knot 9) i el topall (Knot 11)
        float iniciPersianaMetres = GetDistanceToKnot(9);
        float finalAbsolutSplineMetres = GetDistanceToKnot(11);

        // 2. Espai útil real de la piscina
        float recorridoMaximGeometric = finalAbsolutSplineMetres - iniciPersianaMetres;
        float metresUtilsPiscina = Mathf.Min(_currentMaxDistance, recorridoMaximGeometric);

        // 3. ATENCIÓ: Eliminem l'arrodoniment brusc del càlcul 3D! 
        // Deixem que l'extensió sigui totalment fluida i contínua, com a l'animació inicial.
        float extensioSolicitadaUI = _sliderValue * metresUtilsPiscina;

        // El cap avançat de la persiana és fluid
        float distanciaCapPersiana = iniciPersianaMetres + extensioSolicitadaUI;

        // 4. Quantes lames necessitem actives (calculat amb la distància fluida)
        int lamesAActivar = Mathf.CeilToInt(distanciaCapPersiana / _lamasSize);
        lamesAActivar = Mathf.Clamp(lamesAActivar, 0, _MAXLamas);

        for (int i = 0; i < _MAXLamas; i++)
        {
            if (i < lamesAActivar)
            {
                _LamasArray[i].SetActive(true);

                // CLAVAT A L'ANIMACIÓ INICIAL: La fórmula exacta que saps que funciona bé
                float posicioLamaEnMetres = distanciaCapPersiana - (i * _lamasSize);
                if (posicioLamaEnMetres < 0f) posicioLamaEnMetres = 0f;

                float t = posicioLamaEnMetres / totalSplineLength;
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

    // =================================================================================================
    // ANIMACIÓ INICIAL: DES DEL KNOT 0 FINS A LA SORTIDA REAL DEL KNOT 9
    // =================================================================================================
    public void PlayInitialOpenAnimation()
    {
        InitializeSlatPool();

        if (!_isAnimatingInitial)
        {
            StartCoroutine(AnimateToThresholdRoutine());
        }
    }

    private IEnumerator AnimateToThresholdRoutine()
    {
        _isAnimatingInitial = true;

        float totalSplineLength = splineContainer.CalculateLength();

        // Obtenim de forma geomètrica pura quants metres reals fa el tram curt des de l'inici fins al Knot 9
        float posicioKnot9EnMetres = GetDistanceToKnot(9);

        float tempsTranscorregut = 0f;

        while (tempsTranscorregut < _animationDuration)
        {
            tempsTranscorregut += Time.deltaTime;
            float percentatgeAnimacio = tempsTranscorregut / _animationDuration;

            // Avança la persiana exclusivament pel tram que separa la recollida de la boca del calaix
            float distanciaCapPersiana = Mathf.Lerp(0f, posicioKnot9EnMetres, percentatgeAnimacio);

            UpdateBlindForAnimation(distanciaCapPersiana, totalSplineLength);

            yield return null;
        }

        UpdateBlindForAnimation(posicioKnot9EnMetres, totalSplineLength);

        _isAnimatingInitial = false;

        // Passem el relleu al Slider, definint que el seu 0 és la línia del Knot 9 on ha acabat l'animació
        _sliderValue = 0f;
        UpdateBlind();
    }

    private void UpdateBlindForAnimation(float distanciaCapPersiana, float totalSplineLength)
    {
        int lamesAActivar = Mathf.CeilToInt(distanciaCapPersiana / _lamasSize);
        if (distanciaCapPersiana > 0f && lamesAActivar == 0) lamesAActivar = 1;
        lamesAActivar = Mathf.Clamp(lamesAActivar, 0, _MAXLamas);

        for (int i = 0; i < _MAXLamas; i++)
        {
            if (i < lamesAActivar)
            {
                _LamasArray[i].SetActive(true);

                float posicioLamaEnMetres = distanciaCapPersiana - (i * _lamasSize);
                if (posicioLamaEnMetres < 0f) posicioLamaEnMetres = 0f;

                float t = posicioLamaEnMetres / totalSplineLength;
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

        // Si el slider està a zero absolut, evitem qualsevol recompte i tornem 0 clavat
        if (_sliderValue <= 0.001f) return 0f;

        // Busquem el límit geomètric i apliquem la mateixa lògica d'extensió arrodonida
        float iniciPersianaMetres = GetDistanceToKnot(9);
        float finalAbsolutSplineMetres = GetDistanceToKnot(11);

        float recorridoMaximGeometric = finalAbsolutSplineMetres - iniciPersianaMetres;
        float metresUtilsPiscina = Mathf.Min(_currentMaxDistance, recorridoMaximGeometric);

        float extensioSolicitadaUI = _sliderValue * metresUtilsPiscina;

        // Tornem directament el valor arrodonit de 0.05 en 0.05 que s'està aplicant a l'escena 3D
        float extensioArrodonidaUI = Mathf.Round(extensioSolicitadaUI / _lamasSize) * _lamasSize;

        return Mathf.Max(0f, extensioArrodonidaUI);
    }
}