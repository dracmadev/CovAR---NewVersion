using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ARObjectScript : MonoBehaviour
{
    [Header("AR OBJECT SCRIPT")]
    [Space(25)]
    [Header("Type of ARObject:")]
    [SerializeField] private E_PoolType _PoolType;

    [Header("Current ARObject state: (Exposed for debug)")]
    [SerializeField] private E_ARObjectStates _ARObjectState;

    [Header("ARObject parts: (Automatic)")]
    [SerializeField] private List<ARObjectPartScript> _ARObjectSpecificPartsList = new();
    [Header("ARObject Bones: (Automatic)")]
    [SerializeField] private List<ARObjectPartScript> _ARObjectBonesList = new();

    [Header("ARObjectManager reference: ")]
    [SerializeField] private ARObjectManager _ARObjectManager;


    //LOCAL VARIABLES
    //MOVMENT
    bool isDragging;
    //HUD
    HUDManagerScript _HUDManagerScript;

    //ROTATION
    GameObject _RotationSliderRef;
    private float _initialYRotation;
    private bool _hasSavedInitialRotation = false;


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
        _ARObjectSpecificPartsList = new List<ARObjectPartScript>(); // Inicialitzem
        _ARObjectBonesList = new List<ARObjectPartScript>(); // Inicialitzem

        SetARObjectPartsList();

        _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
    }

    void SetARObjectPartsList()
    {
        // Busquem els components en els fills de manera optimitzada al Start
        ARObjectPartScript[] allPartsInChilds = GetComponentsInChildren<ARObjectPartScript>(true);

        // Netegem les llistes 
        _ARObjectSpecificPartsList.Clear();
        _ARObjectBonesList.Clear();

        foreach (ARObjectPartScript part in allPartsInChilds)
        {
            if (part.GetARGeneralObjectPart() == E_ARObjectGeneralParts.PoolSpecificPart)
            {
                AddObjectToList(part.gameObject, _ARObjectSpecificPartsList);
            }
            else if (part.GetARGeneralObjectPart() == E_ARObjectGeneralParts.Bones)
            {
                AddObjectToList(part.gameObject, _ARObjectBonesList);
            }
        }
    }

    public void AddObjectToList(GameObject objToRegister, List<ARObjectPartScript> list, bool checkIfUnique = true)
    {
        var partScripts = objToRegister.GetComponentsInChildren<ARObjectPartScript>();

        foreach (var partScript in partScripts)
        {
            ProcessARObjectPart(partScript, checkIfUnique, list);
        }
    }

    private void ProcessARObjectPart(ARObjectPartScript partScript, bool checkIfUnique, List<ARObjectPartScript> list)
    {
        if (partScript == null) return;

      //Comprovem si existeix 
        bool alreadyExists = list.Exists(item => item.GetARObjectPartData()._ARObjectPartReference == partScript.gameObject);

        if (!checkIfUnique || !alreadyExists)
        {
            list.Add(partScript);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////// GETTER //////////////////////////////////////////////////////////////////////////

    public E_PoolType GeCoverType()
    {
        return _PoolType;
    }
    public E_ARObjectStates GetARObjectCurrentState()
    {
        return _ARObjectState;
    }

    public GameObject GetBoneObjBasedOnDirection(E_ARObjectComponentsDirection direction)
    {
        foreach (ARObjectPartScript bone in _ARObjectBonesList)
        {
            if (bone.GetBoneData()._BoneDirection == direction)
            {
                return bone.GetARObjectPartData()._ARObjectPartReference;
            }
        }
        return null;
    }

    public GameObject GetARObjectSpecificPartBasedOnType(E_ARObjectParts part)
    {
        foreach (ARObjectPartScript specificPart in _ARObjectSpecificPartsList)
        {
            if (specificPart.GetARObjectPartData()._ARObjectPartType == part)
            {
                return specificPart.GetARObjectPartData()._ARObjectPartReference;
            }
        }
        return null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////// SETTER //////////////////////////////////////////////////////////////////////////

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

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * 25f);
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
    
    public void InitRotationState()
    {
        AssignRotationButtonsEvents();
    }
    
    
    GameObject GetRotationSlider()
    {
        if (_RotationSliderRef == null)
        {
            if (_HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel").Count != 0)
            {
                foreach (GameObject slider in _HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel"))
                {
                    if (slider.GetComponent<UIBehaviourComponent>().GetUIComponentType() == E_UIComponents.Slider)
                    {
                        _RotationSliderRef = slider;
                    }
                }
            }
        }

        return _RotationSliderRef;
    }


    Button GetRotationMinusButton()
    {
        if (_HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel").Count != 0)
        {
            foreach (GameObject obj in _HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel"))
            {
                if (obj.GetComponent<UIBehaviourComponent>().GetUIComponentType() == E_UIComponents.Button && obj.name.Contains("Minus"))
                {
                    return obj.GetComponent<Button>();
                }
            }
        }
        return null;
    }

    Button GetRotationPlusButton()
    {
        if (_HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel").Count != 0)
        {
            foreach (GameObject obj in _HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel("RotationSubPanel"))
            {
                if (obj.GetComponent<UIBehaviourComponent>().GetUIComponentType() == E_UIComponents.Button && obj.name.Contains("Plus"))
                {
                    return obj.GetComponent<Button>();
                }
            }
        }
        return null;
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

    public void MinusRotateARObject()
    {
        GameObject sliderObj = GetRotationSlider();
        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float newRot = sliderData.sliderResult - 5f;
            sliderData.sliderResult = Mathf.Clamp(newRot, sliderData.sliderMin, sliderData.sliderMax);

            this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x, sliderData.sliderResult, this.transform.localEulerAngles.z);

            UpdateRotationSlider();
        }
    }

    public void PlusRotateARObject()
    {
        GameObject sliderObj = GetRotationSlider();
        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float newRot = sliderData.sliderResult + 5f;
            sliderData.sliderResult = Mathf.Clamp(newRot, sliderData.sliderMin, sliderData.sliderMax);

            this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x, sliderData.sliderResult, this.transform.localEulerAngles.z);

            UpdateRotationSlider();
        }
    }

    void UpdateRotationSlider()
    {
        GameObject sliderObj = GetRotationSlider();
        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float normalizedStartingSliderValue = (sliderData.sliderResult - sliderData.sliderMin) /
                                                 (sliderData.sliderMax - sliderData.sliderMin);

            sliderObj.GetComponent<UnityEngine.UI.Slider>().value = normalizedStartingSliderValue;
        }
    }

    void AssignRotationButtonsEvents()
    {
        Button minusBtn = GetRotationMinusButton(); // MINUS
        if (minusBtn != null)
        {
            minusBtn.onClick.RemoveListener(MinusRotateARObject);
            minusBtn.onClick.AddListener(MinusRotateARObject);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Minus per assignar l'esdeveniment.");
        }

        Button plusBtn = GetRotationPlusButton(); //PLUS
        if (plusBtn != null)
        {
            plusBtn.onClick.RemoveListener(PlusRotateARObject);
            plusBtn.onClick.AddListener(PlusRotateARObject);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Plus per assignar l'esdeveniment.");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}