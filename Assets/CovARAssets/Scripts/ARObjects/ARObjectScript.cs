using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField] private float _MaxARObjectLenghtDistance = 10f;
    [SerializeField] private float _MaxLamasLenghtDistance = 10f;

    [Header("Current ARObject state: (Exposed for debug)")]
    [SerializeField] private E_ARObjectStates _ARObjectState;

    [Header("ARObject parts: (Automatic)")]
    [SerializeField] private List<ARObjectPartScript> _ARObjectSpecificPartsList = new();
    [Header("ARObject Bones: (Automatic)")]
    [SerializeField] private List<ARObjectPartScript> _ARObjectBonesList = new();

    [Header("ARObjectManager reference: ")]
    [SerializeField] private ARObjectManager _ARObjectManager;
    [SerializeField] private CustomSplineInstantiate _CustomSplineInstantiateScript;


    //LOCAL VARIABLES
    //MOVMENT
    bool isDragging;
    //HUD
    HUDManagerScript _HUDManagerScript;

    //OBJ LENGHT
    private  GameObject _lenghtSliderRef;
    private List<GameObject> _boneRightObjList = new List<GameObject>();
    private GameObject _footRightObj;
    private GameObject _feedbackLenghtText;

    //LAMAS LENGHT
    public GameObject _lamasLenghtSliderRef;
    public GameObject _lamasFeedbackLenghtText;


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

    public List<GameObject> GetAllBoneObjsBasedOnDirection(E_ARObjectComponentsDirection direction)
    {
        List<GameObject> foundBones = new List<GameObject>();

        ARObjectPartScript[] allParts = GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript part in allParts)
        {
            if (part.GetBoneData() != null && part.GetBoneData()._BoneDirection == direction)
            {
                foundBones.Add(part.gameObject);
            }
        }
        return foundBones;
    }

    public GameObject GetARObjectSpecificPartBasedOnType(E_ARObjectParts part)
    {
        ARObjectPartScript[] allParts = GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript specificPart in allParts)
        {
            if (specificPart != null && specificPart.GetARObjectPartData() != null)
            {
                if (specificPart.GetARObjectPartData()._ARObjectPartType == part)
                {
                    return specificPart.gameObject;
                }
            }
        }

        return null;
    }

    public float GetMaxARObjectLenghtDistance()
    {
        return _MaxARObjectLenghtDistance;
    }

    public CustomSplineInstantiate GetCustomSplineInstantiateScript()
    {
        return _CustomSplineInstantiateScript;
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
            case E_ARObjectStates.SetLenghtState: /* O fa amb un lister al ON Value change*/  break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////// AROBJECT BEHAVIOURS //////////////////////////////////////////////////////////////////////////

    GameObject GetSliderFromSpecificSubPanel(string subPanelName)
    {
        if (_HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel(subPanelName).Count != 0)
        {
            foreach (GameObject slider in _HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel(subPanelName))
            {
                if (slider.GetComponent<UIBehaviourComponent>().GetUIComponentType() == E_UIComponents.Slider)
                {
                    return  slider;
                }
            }
        }
        return null;
    }

    Button GetButtonWithSpecificNameFromSpecificSubPanel(string subPanelName, string specificNameOnButton)
    {
        if (_HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel(subPanelName).Count != 0)
        {
            foreach (GameObject obj in _HUDManagerScript.GetAllOfUIComponentsFromSpecificSubPanel(subPanelName))
            {
                if (obj.GetComponent<UIBehaviourComponent>().GetUIComponentType() == E_UIComponents.Button && obj.name.Contains(specificNameOnButton))
                {
                    return obj.GetComponent<Button>();
                }
            }
        }
        return null;
    }

    GameObject GetFeedbackTextFromSpecificSlider(GameObject slider)
    {
        if (slider == null)
        {
            return null;
        }

        UIBehaviourComponent[] allBehaviours = slider.GetComponentsInChildren<UIBehaviourComponent>(true);

        foreach (var uiBehaviour in allBehaviours)
        {
            if (uiBehaviour.GetUIComponentType() == E_UIComponents.Text)
            {
                return uiBehaviour.gameObject;
            }
        }

        return null;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
    
    void RotateYAxisARObject()
    {
        if (GetSliderFromSpecificSubPanel("RotationSubPanel"))
        {
            float _YValueRot = GetSliderFromSpecificSubPanel("RotationSubPanel").GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult;
            this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x, _YValueRot, this.transform.localEulerAngles.z);
        }
        else
        {
            Debug.LogError("ARObjectScript -> Error alhora d'agafar el slider de rotació... (Alvaro)");
        }
    }

    public void MinusRotateARObject()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("RotationSubPanel");
        
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
        GameObject sliderObj = GetSliderFromSpecificSubPanel("RotationSubPanel");

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
        GameObject sliderObj = GetSliderFromSpecificSubPanel("RotationSubPanel");

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
        Button minusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("RotationSubPanel", "Minus"); // MINUS
        if (minusBtn != null)
        {
            minusBtn.onClick.RemoveListener(MinusRotateARObject);
            minusBtn.onClick.AddListener(MinusRotateARObject);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Minus per assignar l'esdeveniment.");
        }

        Button plusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("RotationSubPanel", "Plus"); //PLUS
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

    //////////////////////////////////////////////////////// SET AROBJECT LENGHT ////////////////////////////////////////////////////////////////////////

    public void InitSetLenghtState()
    {
        _lenghtSliderRef = GetSliderFromSpecificSubPanel("SetARObjectLenghtSupPanel");

        if (_lenghtSliderRef != null)
        {
            _lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMax = _MaxARObjectLenghtDistance;

            _lenghtSliderRef.GetComponent<Slider>().onValueChanged.RemoveListener(delegate { SetARObjectLenght(); });
            _lenghtSliderRef.GetComponent<Slider>().onValueChanged.AddListener(delegate { SetARObjectLenght(); });

            _feedbackLenghtText = GetFeedbackTextFromSpecificSlider(_lenghtSliderRef);

            AssignLenghtButtonsEvents();
        }
        else
        {
            Debug.LogWarning("InitSetLenghtState: El slider de la llargada no s'ha trobat (segurament el panell està ocult). Ens saltem la UI de moment.");
        }
      
        // GET RIGHT BONES
        _boneRightObjList = GetAllBoneObjsBasedOnDirection(E_ARObjectComponentsDirection.RIGHT);

        // GET RIGHT FOOT
        _footRightObj = GetARObjectSpecificPartBasedOnType(E_ARObjectParts.FootRight);
    }

    void SetARObjectLenght()
    {
        if (_lenghtSliderRef == null) return;
        if (_boneRightObjList.Count == 0) return;

        float currentSliderValue = (_lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult);

        foreach (GameObject boneObj in _boneRightObjList)
        {
            Vector3 bonePos = boneObj.transform.localPosition;
            boneObj.transform.localPosition = new Vector3(currentSliderValue * 100, bonePos.y, bonePos.z);
            _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght = currentSliderValue;
        }

        if(_footRightObj != null)
        {
            _footRightObj.transform.localPosition = new Vector3(currentSliderValue, _footRightObj.transform.localPosition.y, _footRightObj.transform.localPosition.z);
        }
       
        _feedbackLenghtText.GetComponent<TMP_Text>().text = currentSliderValue.ToString("F2") + "m";
    }

    public void MinusLenghtARObject()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetARObjectLenghtSupPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            // Arrodonim 
            float currentVal = sliderData.sliderResult;
            float newLenght = Mathf.Floor(currentVal / 0.05f) * 0.05f;

            if (Mathf.Approximately(newLenght, currentVal))
            {
                newLenght -= 0.05f;
            }

            sliderData.sliderResult = Mathf.Clamp(newLenght, sliderData.sliderMin, sliderData.sliderMax);

            SetARObjectLenght();

            UpdateLenghtSlider();
        }
    }

    public void PlusLenghtARObject()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetARObjectLenghtSupPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            // Arrodonim 
            float currentVal = sliderData.sliderResult;
            float newLenght = Mathf.Ceil(currentVal / 0.05f) * 0.05f;

            if (Mathf.Approximately(newLenght, currentVal))
            {
                newLenght += 0.05f;
            }

            sliderData.sliderResult = Mathf.Clamp(newLenght, sliderData.sliderMin, sliderData.sliderMax);

            SetARObjectLenght();

            UpdateLenghtSlider();
        }
    }

    void UpdateLenghtSlider()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetARObjectLenghtSupPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float normalizedStartingSliderValue = (sliderData.sliderResult - sliderData.sliderMin) /
                                                 (sliderData.sliderMax - sliderData.sliderMin);

            sliderObj.GetComponent<UnityEngine.UI.Slider>().value = normalizedStartingSliderValue;
        }
    }

    void AssignLenghtButtonsEvents()
    {
        Button minusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("SetARObjectLenghtSupPanel", "Minus");
        if (minusBtn != null)
        {
            minusBtn.onClick.RemoveListener(MinusLenghtARObject);
            minusBtn.onClick.AddListener(MinusLenghtARObject);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Minus per a la mida.");
        }

        Button plusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("SetARObjectLenghtSupPanel", "Plus");

        if (plusBtn != null)
        {
            plusBtn.onClick.RemoveListener(PlusLenghtARObject);
            plusBtn.onClick.AddListener(PlusLenghtARObject);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Plus per a la mida.");
        }
    }

    public void SetARObjectLenghtByNum(float newLenght)
    {
        InitSetLenghtState();

        if (_boneRightObjList.Count == 0) return;

        // 1. Guardem primer el valor real
        if (_lenghtSliderRef != null)
        {
            _lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult = newLenght;
        }

        // 2. Apliquem la posició 
        foreach (GameObject boneObj in _boneRightObjList)
        {
            Vector3 bonePos = boneObj.transform.localPosition;
            boneObj.transform.localPosition = new Vector3(newLenght * 100, bonePos.y, bonePos.z);
        }

        if (_footRightObj != null)
        {
            _footRightObj.transform.localPosition = new Vector3(newLenght, _footRightObj.transform.localPosition.y, _footRightObj.transform.localPosition.z);
        }

        _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght = newLenght;

        // 3. Modifiquem el slider
        if (_lenghtSliderRef != null)
        {
            var sliderData = _lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData();

            float normalizedValue = (newLenght - sliderData.sliderMin) / (sliderData.sliderMax - sliderData.sliderMin);

            Slider unitySlider = _lenghtSliderRef.GetComponent<UnityEngine.UI.Slider>();
            unitySlider.value = normalizedValue;
        }

        if (_feedbackLenghtText != null)
        {
            _feedbackLenghtText.GetComponent<TMP_Text>().text = newLenght.ToString("F2") + "m";
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////// SET LAMAS LENGHT ////////////////////////////////////////////////////////////////////////

    public void InitSetLamasLenghtState()
    {
        _lamasLenghtSliderRef = GetSliderFromSpecificSubPanel("SetLamasLenghtSubPanel");

        // 🛠️ INJECTEM EL MÀXIM AL SCRIPT DEL SPLINE Abans de configurar la UI
        if (_CustomSplineInstantiateScript != null)
        {
            _CustomSplineInstantiateScript.SetupMaxLamasDistance(_MaxLamasLenghtDistance);
        }

        if (_lamasLenghtSliderRef != null)
        {
            // El slider de la UI ara tindrà com a límit màxim de dades exactament els metres reals (ex: 10 o 15)
            _lamasLenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMax = _MaxLamasLenghtDistance;

            _lamasLenghtSliderRef.GetComponent<Slider>().onValueChanged.RemoveListener(delegate { SetLamasLenght(); });
            _lamasLenghtSliderRef.GetComponent<Slider>().onValueChanged.AddListener(delegate { SetLamasLenght(); });

            _lamasFeedbackLenghtText = GetFeedbackTextFromSpecificSlider(_lamasLenghtSliderRef);

            AssignLamasLenghtButtonsEvents();
        }
        else
        {
            Debug.LogWarning("InitSetLenghtState: El slider de la llargada no s'ha trobat.");
        }
    }

    void AssignLamasLenghtButtonsEvents()
    {
        Button minusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("SetLamasLenghtSubPanel", "Minus");

        if (minusBtn != null)
        {
            minusBtn.onClick.RemoveListener(MinusLamasLenght);
            minusBtn.onClick.AddListener(MinusLamasLenght);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Minus per a la mida (lamas).");
        }

        Button plusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("SetLamasLenghtSubPanel", "Plus");

        if (plusBtn != null)
        {
            plusBtn.onClick.RemoveListener(PlusLamasLenght);
            plusBtn.onClick.AddListener(PlusLamasLenght);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Plus per a la mida (lamas).");
        }
    }

    public void MinusLamasLenght()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetLamasLenghtSubPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            // Arrodonim 
            float currentVal = sliderData.sliderResult;
            float newLenght = Mathf.Floor(currentVal / 0.05f) * 0.05f;

            if (Mathf.Approximately(newLenght, currentVal))
            {
                newLenght -= 0.05f;
            }

            sliderData.sliderResult = Mathf.Clamp(newLenght, sliderData.sliderMin, sliderData.sliderMax);

            SetLamasLenght();

            UpdateLamasLenghtSlider();
        }
    }

    public void PlusLamasLenght()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetLamasLenghtSubPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            // Arrodonim 
            float currentVal = sliderData.sliderResult;
            float newLenght = Mathf.Ceil(currentVal / 0.05f) * 0.05f;

            if (Mathf.Approximately(newLenght, currentVal))
            {
                newLenght += 0.05f;
            }

            sliderData.sliderResult = Mathf.Clamp(newLenght, sliderData.sliderMin, sliderData.sliderMax);

            SetLamasLenght();

            UpdateLamasLenghtSlider();
        }
    }

    void UpdateLamasLenghtSlider()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetLamasLenghtSubPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float normalizedStartingSliderValue = (sliderData.sliderResult - sliderData.sliderMin) /
                                                 (sliderData.sliderMax - sliderData.sliderMin);

            sliderObj.GetComponent<UnityEngine.UI.Slider>().value = normalizedStartingSliderValue;
        }
    }

    void SetLamasLenght()
    {
        if (_lamasLenghtSliderRef == null) return;
        if (_CustomSplineInstantiateScript == null) return;

        // 1. Agafem les dades calculades en metres del teu component custom de UI
        var sliderData = _lamasLenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData();
        float currentSliderValueInMeters = sliderData.sliderResult;

        // 2. Traduïm els metres a un valor normalitzat entre 0 i 1 (que és el que demana la persiana)
        float normalizedValue = (currentSliderValueInMeters - sliderData.sliderMin) /
                               (sliderData.sliderMax - sliderData.sliderMin);

        normalizedValue = Mathf.Clamp01(normalizedValue);

        // 3. Passem el valor al script del Spline utilitzant la funció pública
        _CustomSplineInstantiateScript.SetSliderValueFromUI(normalizedValue);

        // 4. Actualitzem el text de la pantalla per a que l'usuari vegi els metres reals (ex: "1.45m")
        if (_lamasFeedbackLenghtText != null)
        {
            _lamasFeedbackLenghtText.GetComponent<TMPro.TMP_Text>().text = _CustomSplineInstantiateScript.GetCurrentPosition().ToString("F2") + "m";
        }

        _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght = _CustomSplineInstantiateScript.GetCurrentPosition();
    }

   public void SetLamasLenghtByNum(float newLenght)
   {
        InitSetLamasLenghtState();
        
        _CustomSplineInstantiateScript.SetLamasLenghtByNum(newLenght, _MaxLamasLenghtDistance, _lamasLenghtSliderRef, _lamasFeedbackLenghtText);
   }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}