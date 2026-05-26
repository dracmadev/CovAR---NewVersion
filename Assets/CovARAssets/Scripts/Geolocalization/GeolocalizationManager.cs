using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Android;

public class GeolocalizationManager : MonoBehaviour
{
    // --- VARIABLES DE LOCALITZACIÓ I API ---
    [Header("GEO-LOCALIZATION VARIABLES:")]
    [Space]
    [Header("Api Key:")]
    [SerializeField] private string apiKey = "66a7b074b36ee374543186fqh82446e";
    [Space]
    [Header("Debug coordinates:")]
    [SerializeField] private bool bDebugCoordinates;
    [SerializeField] private string latitude = "41.783649873174234";
    [SerializeField] private string longitude = "1.2929005391566282";

    // Utilització de les classes JSON definides a l'altre fitxer
    [SerializeField] private JsonData jsonData;

    private const string urlGeoCode = "https://geocode.maps.co/reverse?";

    // --- VARIABLES DE TEXT ---
    [Space]
    [Header("Text settings:")]
    [SerializeField] private string geolocText;
    private GameObject _BlueprintGeolocText;

    // Variables per guardar les dades un cop obtingudes
    private string currentCountry = "Pais";
    private string currentCity = "Ciudad";
    private string currentPostCode = "XXXXX";

    bool bGeolocalizationFounded;


    void Start()
    {
        // 1. Demanar permisos
        RequestLocationPermission();

        // 2. Iniciar la geolocalització
        InitGeolocalizationScript();
    }
    private void RequestLocationPermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
#elif UNITY_IOS
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Ubicació desactivada a iOS. L'usuari ha de donar permisos.");
        }
#endif
    }



    public void InitGeolocalizationScript()
    {
        if(!bGeolocalizationFounded)
        {
            StartCoroutine(GetLocationAndFindGeolocation());
        }
    }


    public string GetLocalizationText(JsonData json)
    {
        if (json.address == null) return "Error: Dades d'adreça no trobades.";

        currentPostCode = json.address.postcode;
        currentCountry = json.address.country; // Guardem el país

        // Lògica per trobar el nom de la població més específica
        string cityOrTown = "NotFound";

        if (json.address.city != null)
        {
            cityOrTown = json.address.city;
        }
        else if (json.address.town != null)
        {
            cityOrTown = json.address.town;
        }
        else if (json.address.village != null)
        {
            cityOrTown = json.address.village;
        }

        currentCity = cityOrTown; // Guardem la ciutat/poble

        return $"{currentCountry}, {currentCity}, {currentPostCode}";
    }

    IEnumerator GetLocationAndFindGeolocation()
    {
        if (!bDebugCoordinates)
        {
            if (!Input.location.isEnabledByUser)
            {
                geolocText = "N/A";
                yield break;
            }

            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                geolocText = "Calculating...";
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
            {
                geolocText = "N/A";
                Input.location.Stop();
                yield break;
            }

            // Utilitzar CultureInfo("en-US") per assegurar el punt com a separador decimal
            CultureInfo culture = new CultureInfo("en-US");
            latitude = Input.location.lastData.latitude.ToString(culture);
            longitude = Input.location.lastData.longitude.ToString(culture);

            Input.location.Stop();
        }

        geolocText = "Loading...";
        string requestUrl = $"{urlGeoCode}lat={latitude}&lon={longitude}&api_key={apiKey}&format=json";
        Debug.Log($"Solicitud enviada a: {requestUrl}");

        using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error en la solicitud: " + www.error);
                geolocText = $"Error ({latitude} / {longitude})";
                bGeolocalizationFounded = false;
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                jsonData = JsonUtility.FromJson<JsonData>(jsonResponse);
                geolocText = GetLocalizationText(jsonData);
                /*
                if (GameObject.FindGameObjectWithTag("Blueprint"))
                {
                    GameObject.FindGameObjectWithTag("Blueprint").GetComponent<BlueprintBehaviourScript>().SetGeolocText(geolocText);
                    GameObject.FindGameObjectWithTag("Blueprint").GetComponent<BlueprintBehaviourScript>().SetCountryFromOrderData(currentCountry);
                    GameObject.FindGameObjectWithTag("Blueprint").GetComponent<BlueprintBehaviourScript>().SetPostCodeFromOrderData(currentPostCode);
                }*/
                bGeolocalizationFounded = true;
            }
        }
    }


    public string GetPostCode()
    {
        if (bDebugCoordinates)
        {
            return "08013";
        }
        else
        {
            // Retorna el valor guardat o un valor per defecte si no s'ha carregat
            return currentPostCode != "XXXXX" ? currentPostCode : "00000";
        }
    }

    public string GetCountry()
    {
        if (bDebugCoordinates)
        {
            return "Spain";
        }
        else
        {
            // Retorna el valor guardat o un valor per defecte
            return currentCountry != "Pais" ? currentCountry : "Unknown";
        }
    }

    public string GetGeolocText()
    {
        return geolocText;
    }

   
}