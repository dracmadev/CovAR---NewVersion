using UnityEngine;
using Vuforia;
using TMPro; // Si fas servir TextMeshPro per als missatges
using Assets.SimpleLocalization.Scripts;
using System;

public class ARTrackingController : MonoBehaviour
{

    private ObserverBehaviour _observerBehaviour;
    //Local variables
    private HUDManagerScript _HUDManagerScrit;
    private ARObjectManager _ARObjectManager;

    public TMP_Text debugText;
    void Start()
    {
        // Busquem l'Observer en aquest mateix objecte o els seus fills
        _observerBehaviour = GetComponent<ObserverBehaviour>();

        if (_observerBehaviour == null)
            _observerBehaviour = GetComponentInChildren<ObserverBehaviour>();

        if (_observerBehaviour != null)
        {
            // Ens subscrivim als canvis d'estat del tracking
            _observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
        else
        {
            Debug.LogError("ARTrackingController: No s'ha trobat cap ObserverBehaviour!");
        }

        _HUDManagerScrit = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
        _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
    }

    private void OnDestroy()
    {
        if (_observerBehaviour != null)
        {
            _observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (!_HUDManagerScrit.GetStopTracking())
        {
            Debug.Log($"<color=yellow>AR Status:</color> {targetStatus.Status} | <color=yellow>Info:</color> {targetStatus.StatusInfo}");

            switch (targetStatus.Status)
            {
                case Status.TRACKED:
                    // El model està perfectament fixat
                    Debug.Log("[TRACKING] -> MODEL PERFECTAMENT FIXAT. TRACKING CORRECTE!");


                    break;

                case Status.EXTENDED_TRACKED:
                    // El model es veu però la precisió és limitada (per sensors, no per càmera)
                    Debug.Log("[TRACKING] -> MODEL TRACKEJAT AMB PRECISIÓ LIMITADA...");


                    break;

                case Status.LIMITED:
                    // El sistema sap on és però no té prou referències visuals (massa fosc, terra pla sense textura)
                    Debug.Log("[TRACKING] -> NO ES DETECTA BE EL TERRA (textura) O L'ESPAI ES MASSA FOSC.");

                    break;

                case Status.NO_POSE:
                    /*
                    if (_HUDManagerScrit != null && _ARObjectManager != null)
                    {
                        Debug.Log("[TRACKING] -> NETEJANT ÀNCORA PER RE-POSICIONAR");

                        GameObject stage = GameObject.Find("Ground Plane Stage");
                        if (stage != null)
                        {
                            // 1. Desactivem l'objecte perquè no "floti" en l'aire mentre busquem
                            stage.SetActive(false);

                            // 2. DESTRUÏM l'àncora vella. Això és el que impedeix que el 
                            // OnContentPlaced torni a saltar.
                            var anchor = stage.GetComponent<Vuforia.AnchorBehaviour>();
                            if (anchor != null) DestroyImmediate(anchor);
                        }

                        _HUDManagerScrit.TravelToPanel("ReFindModelPanel");
                        _ARObjectManager.SetActivePlaneFinder(true);
                    }
                    */
                    break;
            }
        }
    }

    public void ForcePlaceContent(Vuforia.HitTestResult result)
    {
        Debug.Log("<color=yellow>Executant ForcePlaceContent manual...</color>");

        // 1. Busquem l'objecte Ground Plane Stage fins i tot si està desactivat
        GameObject stage = null;
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("GroundPlaneStage"))
            {
                stage = obj;
                break;
            }
        }

        if (stage != null && result != null)
        {
            // 2. NETEJA CRÍTICA: Eliminem l'àncora vella que bloqueja el renderitzat
            // Això soluciona l'error vermell de "No content to place"
            var oldAnchor = stage.GetComponent<Vuforia.AnchorBehaviour>();
            if (oldAnchor != null)
            {
                DestroyImmediate(oldAnchor);
                Debug.Log("<color=orange>Àncora vella eliminada per evitar conflictes.</color>");
            }

            // 3. ACTIVEM L'OBJECTE
            stage.SetActive(true);

            // 4. POSICIONEM MANUALMENT segons el clic detectat
            // Fem servir result.Position i result.Rotation que ens dona el Plane Finder
            stage.transform.position = result.Position;
            stage.transform.rotation = result.Rotation;
            stage.transform.localScale = Vector3.one;

            // 5. REVISIÓ DE FILLS: Ens assegurem que la piscina sigui visible
            // ***** NO!! això fa que sempre surti per defecte la corona desbordant doble al principi
            // Això recorre ARObjectManager i P_SquarePool
            foreach (Transform child in stage.GetComponentsInChildren<Transform>(true))
            {
                //child.gameObject.SetActive(true);

                // Si tenen renderitzadors (malla), els forcem a "On"
                var renderer = child.GetComponent<Renderer>();
                if (renderer != null) renderer.enabled = true;
            }

            Debug.Log("<color=green>[SUCCESS] Piscina posicionada i activada manualment!</color>");

            // 6. NETEJA DE INTERFÍCIE
            // Tanquem els hexàgons del Plane Finder
            if (_ARObjectManager != null)
            {
                _ARObjectManager.OnARObjectPosicioned();
            }

          
        }
        else
        {
            Debug.LogError("CRITICAL: No s'ha trobat l'objecte amb el Tag 'GroundPlaneStage' o el HitTestResult és nul.");
        }
    }

    public void LogToScreen(string message)
    {
        if (debugText != null) debugText.text = DateTime.Now.ToString("HH:mm:ss") + ": " + message;
    }

}