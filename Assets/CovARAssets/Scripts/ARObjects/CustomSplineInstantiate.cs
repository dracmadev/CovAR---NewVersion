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

    [SerializeField] private GameObject[] _LamasArray;
    [SerializeField] private int _MAXLamas;

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
        // 1. Calculate total length of the spline
        float totalLength = splineContainer.CalculateLength();

        // 2. Calculate max amount of slats that can fit the path
        _MAXLamas = Mathf.FloorToInt(totalLength / _lamasSize);

        // 3. Create an object pool to avoid Instantiate/Destroy at runtime
        _LamasArray = new GameObject[_MAXLamas];
        for (int i = 0; i < _MAXLamas; i++)
        {
            _LamasArray[i] = Instantiate(lamaPrefab, transform);
            _LamasArray[i].SetActive(false); // Start disabled
        }
    }

    void UpdateBlind()
    {
        // 1. Calculem quants metres de persiana s'han de veure (de 0 a la longitud total del camí)
        float longitudTotal = splineContainer.CalculateLength();
        float distanciaActualPersiana = longitudTotal * _sliderValue;

        // 2. Calculem quantes lames caben en aquesta distància actual
        int lamesAActivar = Mathf.CeilToInt(distanciaActualPersiana / _lamasSize);

        for (int i = 0; i < _MAXLamas; i++)
        {
            if (i < lamesAActivar)
            {
                _LamasArray[i].SetActive(true);

                // CLAU DE L'EFECTE: 
                // La primera lama (i=0) estarà al capdavant de tot (distanciaActualPersiana).
                // Les següents (i=1, i=2...) es col·loquen restants enrere, empenyent-se de forma contínua.
                float distanciaEnMetres = distanciaActualPersiana - (i * _lamasSize);

                // Evitem valors negatius estranys si ens passem una mica en el càlcul
                if (distanciaEnMetres < 0f) distanciaEnMetres = 0f;

                // Convertim a valor "t" normalitzat (0 a 1) per al spline
                float t = distanciaEnMetres / longitudTotal;

                // Obtenim posició i rotació exactes del spline
                Vector3 position = splineContainer.EvaluatePosition(t);
                Vector3 direction = splineContainer.EvaluateTangent(t);
                Vector3 up = splineContainer.EvaluateUpVector(t);

                // Apliquem transformacions
                _LamasArray[i].transform.position = position;
                _LamasArray[i].transform.rotation = Quaternion.LookRotation(direction, up);
            }
            else
            {
                // Desactivem les que encara no han sortit del "calaix"
                _LamasArray[i].SetActive(false);
            }
        }
    }

    // Allows previewing changes directly in the Editor without hitting Play
    private void OnValidate()
    {
        if (_LamasArray != null && _LamasArray.Length > 0)
        {
            UpdateBlind();
        }
    }
}