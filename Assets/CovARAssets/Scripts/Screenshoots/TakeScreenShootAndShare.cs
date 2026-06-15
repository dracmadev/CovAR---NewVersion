using System.Collections;
using System.IO;
using UnityEngine;
using System;
using Assets.SimpleLocalization.Scripts;
using UnityEngine;
//using Vuforia;

public class TakeScreenShootAndShare : MonoBehaviour
{
    [Header("TAKE SCREENSHOOT & SHARE")]

    private bool bHideHUDForScreenshoot;

    private HUDManagerScript _HUDManagerScript;
    private GeolocalizationManager _GeolocalizationManager;

    // Control per saber si estem en el procés de compartir i gestionar la tornada
    private bool _isSharingProcessActive = false;



    private void Start()
    {
        InitTakeScreenshootAndShare();
    }

    //////////////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////////////////////

    void InitTakeScreenshootAndShare()
    {
        if (GameObject.FindGameObjectWithTag("HUD"))
        {
            _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
        }
        else
        {
            Debug.LogError("The HUD MANAGER needs a CanvasGroup because TakeScreenShootAndShare need It To work! (Alvaro)");
        }

        if (GameObject.FindGameObjectWithTag("GeolocalizationManager"))
        {
            _GeolocalizationManager = GameObject.FindGameObjectWithTag("GeolocalizationManager").GetComponent<GeolocalizationManager>();
        }
        else
        {
            Debug.LogError("No object GeolocalizationManager on Scene. TakeScreenShootAndShare need It To work! (Alvaro)");
        }
    }

    /////////////////////////////////////////////////////////// BEHAVIOUR //////////////////////////////////////////////////////////////////////

    public void OnClickTakeScreenShootAndShare()
    {
        bHideHUDForScreenshoot = false;
        StartCoroutine(CaptureAndShareCoroutine());
    }

    public void OnClickTakePhotoAndShare()
    {

        bHideHUDForScreenshoot = true;
        StartCoroutine(CaptureAndShareCoroutine());

    }

    private IEnumerator CaptureAndShareCoroutine()
    {
        _isSharingProcessActive = true;
        yield return new WaitForSeconds(0.25f);

        if(bHideHUDForScreenshoot)
        {
            _HUDManagerScript.HideAllHUD();
        }

        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filename = "";
        string SubjectName = "";
        string DescripName = "";

        if (bHideHUDForScreenshoot)
        {
            SubjectName = LocalizationManager.Localize("ScreenshootAndShare.Photo.Title");
            DescripName = LocalizationManager.Localize("ScreenshootAndShare.Photo.Descrip"); 
            filename = "Photo_" + _GeolocalizationManager.GetCountry() + "_" + _GeolocalizationManager.GetPostCode() + "_" + GetCurrentDate() + ".png";
        }
        else
        {
           
              
            SubjectName = LocalizationManager.Localize("ScreenshootAndShare.Blueprint.Title");
            DescripName = LocalizationManager.Localize("ScreenshootAndShare.Blueprint.Descrip");
            
            filename = "Screenshoot_" + _GeolocalizationManager.GetCountry() + "_" + _GeolocalizationManager.GetPostCode() + "_" + GetCurrentDate() + ".png";
        }

        string filePath = Path.Combine(Application.temporaryCachePath, filename);
        File.WriteAllBytes(filePath, ss.EncodeToPNG());
        Destroy(ss);

        // Desa a la galeria
       
        NativeGallery.SaveImageToGallery(filePath, "CovAR", filename);
        

        // Comparteix amb Callback
        new NativeShare()
            .AddFile(filePath)
            .SetText(DescripName)
            .SetSubject(SubjectName)
            .SetCallback((result, shareTarget) => {
                Debug.Log("Share result: " + result);
                _isSharingProcessActive = false;
                ReactivateVuforia();
            })
            .Share();

        if (bHideHUDForScreenshoot)
        {
            _HUDManagerScript.ShowAllHUD();
        }
    }

    // Funció que crida la corrutina de reinici
    private void ReactivateVuforia()
    {
        StopCoroutine(nameof(ForceVuforiaRestart));
        StartCoroutine(nameof(ForceVuforiaRestart));
    }

    private IEnumerator ForceVuforiaRestart()
    {
        Debug.Log("♻️ Iniciant recuperació de càmera (Mètode Ultra-Compatible)...");

        // 1. Espera perquè el sistema operatiu retorni el control de la lent
        yield return new WaitForSeconds(0.5f);

        // 2. Refrescar el fons de vídeo sense citar classes internes de Vuforia
        // Utilitzem UnityEngine.Object per evitar l'error d'ambigüitat CS0104
        MonoBehaviour[] allComponents = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour comp in allComponents)
        {
            // Si el component és vàlid i el seu nom conté "VideoBackground"
            if (comp != null && comp.GetType().Name.Contains("VideoBackground"))
            {
                try
                {
                    comp.enabled = false;
                    // No cal fer un yield per cada component, ho fem un sol cop al final del bucle
                    comp.enabled = true;
                    Debug.Log("✅ Refrescat correctament: " + comp.GetType().Name);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("No s'ha pogut refrescar " + comp.GetType().Name + ": " + e.Message);
                }
            }
        }

        // 3. Temps extra per deixar que la textura de la càmera es renderitzi
        yield return new WaitForSeconds(0.3f);

        Debug.Log("✅ Procés de recuperació de visibilitat finalitzat.");
    }

    // Backup per si el Callback de NativeShare no es dispara en alguns dispositius
    private void OnApplicationPause(bool paused)
    {
        if (!paused && _isSharingProcessActive)
        {
            Debug.Log("📲 Aplicació recuperada de pausa (Focus)");
            _isSharingProcessActive = false;
            ReactivateVuforia();
        }
    }

    string GetCurrentDate()
    {
        return DateTime.Now.ToString("dd-MM-yyyy");
    }







}