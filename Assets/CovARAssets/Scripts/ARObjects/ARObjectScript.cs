using UnityEngine;
using System.Collections.Generic;
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
    [SerializeField] private List<St_ARObjectPart> _ARObjectPartsList = new();

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

    //////////////////////////////////////////////////////////////////////// INIT ////////////////////////////////////////////////////////////

    void InitARObjectScript()
    {
        _ARObjectPartsList = new List<St_ARObjectPart>(); // Inicialitzem
        SetARObjectPartsList(this.gameObject);
        _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        _HUDManagerScrit = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
    }

    void TraverseAndGetTheObjectPart(Transform parent)
    {
        foreach (Transform child in parent)
        {
            var partScript = child.GetComponent<ARObjectPartScript>();
            if (partScript != null)
            {
                // Crear còpia nova de les dades que té el script
                St_ARObjectPart copiedPart = new St_ARObjectPart
                {
                    _ARObjectPartType = partScript.GetARObjectPartData()._ARObjectPartType,
                    _ARObjectPartReference = partScript.gameObject
                };

                _ARObjectPartsList.Add(copiedPart);
            }

            // Recursivitat
            TraverseAndGetTheObjectPart(child);
        }
    }

    void SetARObjectPartsList(GameObject arObject)
    {
        TraverseAndGetTheObjectPart(arObject.transform);
    }

   

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////// GETTER //////////////////////////////////////////////////////////////////////////

    public E_PoolType GetPoolType()
    {
        return _PoolType;
    }
    public E_ARObjectStates GetARObjectCurrentState()
    {
        return _ARObjectState;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////// SETTER ////////////////////////////////////////////////////////////////////////////

    public void SetARObjectState(E_ARObjectStates newState)
    {
        _ARObjectState = newState;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////// STATE MACHINE //////////////////////////////////////////////////////////////////////////

    void ARObjectBehaviour()
    {
        switch(_ARObjectState)
        {
            case E_ARObjectStates.DefaultState: break;
            case E_ARObjectStates.MovingState: ARObjectMovingXZAxisBehaviour(); break;
            case E_ARObjectStates.RotatingState: RotateYAxisARObject();  break;
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

        float speed = 200f;
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed * Time.deltaTime);
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

        if(_HUDManagerScrit.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel").Count != 0)
        {
            foreach (GameObject slider in _HUDManagerScrit.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel"))
            {
                if(slider.GetComponent<UIBehaviourComponent>().GetUIComponentType() == E_UIComponents.Slider)
                {
                    myRotationSlider = slider;
                }
            }
                
        }
       
        return myRotationSlider;
    }



    void RotateYAxisARObject()
    {
        if(GetRotationSlider())
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
