using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ARObjectScript : MonoBehaviour
{
    [Header("AR OBJECT SCRIPT")]
    [Space(25)]
    [Header("Type of ARObject:")]
    [SerializeField] private E_PoolType _PoolType;

    [Header("Current ARObject state: (Exposed for debug)")]
    [SerializeField] private E_ARObjectStates _ARObjectState;

    [Header("ARObject parts: (Automatic)")]
    [SerializeField] private List<St_ARObjectPart> _ARObjectSpecificPartsList = new();

    [Header("ARObjectManager reference: ")]
    [SerializeField] private ARObjectManager _ARObjectManager;


    //LOCAL VARIABLES
    //MOVMENT
    bool isDragging;
    //HUD
    HUDManagerScript _HUDManagerScrit;

    void Start()
    {
        InitARObjectScript();
    }

    private void Update()
    {
        ARObjectBehaviour();
    }

    ////////////////////////////////////////////////////////////////////// INIT ////////////////////////////////////////////////////////////

    public void InitARObjectScript()
    {
        _ARObjectSpecificPartsList = new List<St_ARObjectPart>(); // Inicialitzem
        SetARObjectPartsList();

        _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        _HUDManagerScrit = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
    }

    void SetARObjectPartsList()
    {
        // Busquem els components en els fills de manera optimitzada al Start
        ARObjectPartScript[] allPartsInChilds = GetComponentsInChildren<ARObjectPartScript>(true);

        // Guardem tot usant LINQ a la llista
        _ARObjectSpecificPartsList = allPartsInChilds.Select(part => new St_ARObjectPart
        {
            _ARObjectPartType = part.GetARObjectPartData()._ARObjectPartType,
            _ARObjectPartReference = part.gameObject
        }).ToList();
    }

    // ==========================================
    // AFEGIT: FUNCIONS DINÀMIQUES DEL SCRIPT NOU
    // ==========================================

    public void AddObjectToARObjectPartsList(GameObject objToRegister, bool checkIfUnique = true)
    {
        var partScripts = objToRegister.GetComponentsInChildren<ARObjectPartScript>();

        foreach (var partScript in partScripts)
        {
            // Passem el bool a la funció de processament
            ProcessSinglePart(partScript, checkIfUnique);
        }
    }

    private void ProcessSinglePart(ARObjectPartScript partScript, bool checkIfUnique)
    {
        if (partScript == null) return;

        if (partScript.GetARGeneralObjectPart() == E_ARObjectGeneralParts.PoolSpecificPart)
        {
            // COMPROVACIÓ D'OBJECTE REPETIT
            // Busquem si algun element de la llista ja té exactament la mateixa referència de GameObject
            bool alreadyExists = _ARObjectSpecificPartsList.Exists(item => item._ARObjectPartReference == partScript.gameObject);

            if (!alreadyExists)
            {
                St_ARObjectPart newPart = new St_ARObjectPart
                {
                    _ARObjectPartType = partScript.GetARObjectPartData()._ARObjectPartType,
                    _ARObjectPartReference = partScript.gameObject
                };

                _ARObjectSpecificPartsList.Add(newPart);
            }
        }
    }

    public void RemoveObjectFromARPartsList(GameObject objToRemove)
    {
        // Utilitzem RemoveAll per netejar qualsevol entrada que coincideixi amb aquest GameObject
        int removedCount = _ARObjectSpecificPartsList.RemoveAll(item => item._ARObjectPartReference == objToRemove);

        if (removedCount > 0)
        {
            //Debug.Log($"S'han eliminat {removedCount} referències de l'objecte {objToRemove.name}");
        }
    }

    // ==========================================

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////// GETTER //////////////////////////////////////////////////////////////////////////

    public E_PoolType GetPoolType()
    {
        return _PoolType;
    }
    public E_ARObjectStates GetARObjectCurrentState()
    {
        return _ARObjectState;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////// SETTER ////////////////////////////////////////////////////////////////////////////

    public void SetARObjectState(E_ARObjectStates newState)
    {
        _ARObjectState = newState;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////// STATE MACHINE //////////////////////////////////////////////////////////////////////////

    void ARObjectBehaviour()
    {
        switch (_ARObjectState)
        {
            case E_ARObjectStates.DefaultState: break;
            case E_ARObjectStates.MovingState: ARObjectMovingXZAxisBehaviour(); break;
            case E_ARObjectStates.RotatingState: RotateYAxisARObject(); break;
            case E_ARObjectStates.ReSizeingState: break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////// MOVE AR OBJECT ///////////////////////////////////////////////////////////////////////////

    void ARObjectMovingXZAxisBehaviour()
    {
        if (!IsTouchingUI())
        {
            bool isPressed = false;
            Vector2 screenPos = Vector2.zero;

            if (Touchscreen.current?.primaryTouch.press.isPressed == true)
            {
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
                isPressed = true;
            }
            else if (Mouse.current?.leftButton.isPressed == true)
            {
                screenPos = Mouse.current.position.ReadValue();
                isPressed = true;
            }

            if (isPressed)
            {
                isDragging = true;
                MoveObjectXZToScreenPoint(new Vector3(screenPos.x, screenPos.y, 0));
            }
            else if (isDragging)
            {
                StopDragging();
            }
        }
    }

    void MoveObjectXZToScreenPoint(Vector3 screenPos)
    {
        float fixedY = this.transform.position.y;

        if (_ARObjectManager.GetARCamera() == null || this == null) return;

        float objectDistance = Vector3.Dot(
            this.transform.position - _ARObjectManager.GetARCamera().transform.position,
            _ARObjectManager.GetARCamera().transform.forward
        );

        Vector3 worldPos = _ARObjectManager.GetARCamera().ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, objectDistance));
        Vector3 targetPos = new Vector3(worldPos.x, fixedY, worldPos.z);

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * 15f);
    }

    void StopDragging()
    {
        isDragging = false;
    }

    bool IsTouchingUI()
    {
        Vector2 pointerPos = Vector2.zero;

        if (Touchscreen.current?.primaryTouch.press.isPressed == true)
        {
            pointerPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            pointerPos = Mouse.current.position.ReadValue();
        }

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = pointerPos
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////// ROTATE /////////////////////////////////////////////////////////////////////////////

    GameObject GetRotationSlider()
    {
        GameObject myRotationSlider = null;

        if (_HUDManagerScrit.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel").Count != 0)
        {
            foreach (GameObject slider in _HUDManagerScrit.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel"))
            {
                if (slider.GetComponent<UIBehaviourComponent>().GetUIComponentType() == E_UIComponents.Slider)
                {
                    myRotationSlider = slider;
                }
            }
        }

        return myRotationSlider;
    }

    void RotateYAxisARObject()
    {
        if (GetRotationSlider())
        {
            float _YValueRot = GetRotationSlider().GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult;
            this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x, _YValueRot, this.transform.localEulerAngles.z);
        }
        else
        {
            Debug.LogError("ARObjectScript -> Error alhora d'agafar el slider de rotació... (Alvaro)");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}