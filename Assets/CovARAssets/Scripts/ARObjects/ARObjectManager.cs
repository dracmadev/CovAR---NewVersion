using UnityEngine;
using System.Collections.Generic;

public class ARObjectManager : MonoBehaviour
{
    [Header("AR OBJECT MANAGER")]
    [Space(25)]
    [Header("First object to visualize:")]
    [SerializeField] private E_PoolType _CurrentPoolType;

    // Lista de Prefabs del manager nuevo
    [Header("List of ARObjects (Prefabs):")]
    [SerializeField] private List<GameObject> _PoolPrefabs;

    [Header("Current ARObject:")]
    [SerializeField] private GameObject _CurrentARObject;
    [Header("AR CAMERA reference: (Automatic)")]
    [SerializeField] private Camera _ARCamera;
    [Header("PlaneFinder reference: (Automatic)")]
    [SerializeField] private GameObject _PlaneFinder;
    [SerializeField] private bool bDontShowPlaneFinderAtStart;

    //Local variables
    HUDManagerScript _HUDManagerScrit;


    void Start()
    {
        InitARObjectManager();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////

    void InitARObjectManager()
    {
        _HUDManagerScrit = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();


        InitARCameraRef();
        SetActiveARObject(_CurrentPoolType);
        InitPlaneFinder();

        if (bDontShowPlaneFinderAtStart)
        {
            SetActivePlaneFinder(false);
        }
        else
        {
            SetActivePlaneFinder(true);
        }

        
    }

    void InitARCameraRef()
    {
        _ARCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void InitPlaneFinder()
    {
        _PlaneFinder = GameObject.FindGameObjectWithTag("PlaneFinder");
    }

    // Lógica de estados de movimiento y rotación actualizada como el nuevo
    void UpdateARObjectSpecificState()
    {
        if (GetCurrentARObject() == null) return;

        switch (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState())
        {
            case E_ARObjectStates.DefaultState:
                _HUDManagerScrit.HideAllOfSubMenus();
                break;

            case E_ARObjectStates.MovingState:
                _HUDManagerScrit.HideAllOfSubMenus();
                break;

            case E_ARObjectStates.RotatingState:
                _HUDManagerScrit.ShowSpecificSubMenu("RotationSubPanel");
                break;

            case E_ARObjectStates.ReSizeingState:
                _HUDManagerScrit.HideAllOfSubMenus();
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// SETTERS ///////////////////////////////////////////////////////////////////

    // Instanciación dinámica real: guarda la posición/rotación anterior, destruye y genera el nuevo prefab
    public void SetActiveARObject(E_PoolType _ARObjectToActive)
    {
        Vector3 lastPosition = Vector3.zero;
        Quaternion lastRotation = Quaternion.identity;

        // Si ya había un objeto antes, guardamos dónde estaba para que el nuevo aparezca exactamente ahí
        if (_CurrentARObject != null)
        {
            lastPosition = _CurrentARObject.transform.position;
            lastRotation = _CurrentARObject.transform.rotation;
        }

        // Neteja de memòria: Destruimos todos los hijos actuales
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Buscamos el nuevo prefab en la lista e instanciamos
        foreach (GameObject prefab in _PoolPrefabs)
        {
            if (prefab.GetComponent<ARObjectScript>().GetPoolType() == _ARObjectToActive)
            {
                _CurrentARObject = Instantiate(prefab, transform);

                // Si ya había una posición guardada, se la aplicamos a la nueva piscina
                if (lastPosition != Vector3.zero)
                {
                    _CurrentARObject.transform.position = lastPosition;
                    _CurrentARObject.transform.rotation = lastRotation;
                }

                SetCurrentARObjectState("DefaultState");
                break;
            }
        }
    }

    // Comportamiento de "Toggle" (interruptor) para el movimiento y rotación del nuevo manager
    public void SetCurrentARObjectState(string StateName)
    {
        switch (StateName)
        {
            case "DefaultState":
                if (GetCurrentARObject() != null)
                {
                    GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    UpdateARObjectSpecificState();
                }
                break;

            case "MovingState":
                if (GetCurrentARObject() != null)
                {
                    // Si ya estaba moviéndose, vuelve a Default
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.MovingState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.MovingState);
                    }
                    _HUDManagerScrit.HideAllOfSubMenus();
                    UpdateARObjectSpecificState();
                }
                break;

            case "RotatingState":
                if (GetCurrentARObject() != null)
                {
                    // Si ya estaba rotando, vuelve a Default
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.RotatingState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                        _HUDManagerScrit.HideAllOfSubMenus();
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.RotatingState);
                    }
                    UpdateARObjectSpecificState();
                }
                break;

            case "ReSizeingState":
                if (GetCurrentARObject() != null)
                {
                    if (GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() == E_ARObjectStates.ReSizeingState)
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.DefaultState);
                    }
                    else
                    {
                        GetCurrentARObject().GetComponent<ARObjectScript>().SetARObjectState(E_ARObjectStates.ReSizeingState);
                    }
                    UpdateARObjectSpecificState();
                }
                break;
        }
    }

    public void SetActivePlaneFinder(bool active)
    {
        if (_PlaneFinder != null)
        {
            _PlaneFinder.SetActive(active);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// GETTERS /////////////////////////////////////////////////////////////////////

    public Camera GetARCamera()
    {
        return _ARCamera;
    }

    public GameObject GetCurrentARObject()
    {
        return _CurrentARObject;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}