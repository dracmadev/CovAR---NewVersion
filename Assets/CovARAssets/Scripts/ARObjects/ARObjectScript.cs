using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ARObjectScript : MonoBehaviour
{
    [Header("AR OBJECT SCRIPT")]
    [Space(25)]
    [Header("Type of ARObject:")]
    [SerializeField] private E_PoolType _PoolType;
    [SerializeField] private E_CoverType _CoverType;
    [SerializeField] private float _MaxARObjectLenghtDistance = 10f;
    [SerializeField] private float _MaxLamasLenghtDistance = 10f;
    [SerializeField] private float _MaxARObjectLenghtDistanceWithPool = 2.3f;
    [SerializeField] private float _MaxLamasLenghtDistanceWithPool = 4.1f;

    [Header("Current ARObject state: (Exposed for debug)")]
    [SerializeField] private E_ARObjectStates _ARObjectState;

    [Header("ARObject related products list:")]
    [SerializeField] private St_APRelatedProduct[] _ARObjectRelatedProductsList;
    
    [Header("ARObject parts: (Automatic)")]
    [SerializeField] private List<ARObjectPartScript> _ARObjectSpecificPartsList = new();
    [Header("ARObject Bones: (Automatic)")]
    [SerializeField] private List<ARObjectPartScript> _ARObjectBonesList = new();

    [Header("ARObjectManager reference: ")]
    [SerializeField] private ARObjectManager _ARObjectManager;
    [SerializeField] private CustomSplineInstantiate _CustomSplineInstantiateScript;

    [Header("Cover Fabric Axis Behaviour: ")]
    [SerializeField] private bool bFabricCoverAxisBehaviourActive = false;
    [SerializeField] private St_ARObjectFabricCoverData _FabricCoverAxisDataArray;
    [SerializeField] private GameObject _CustomFabricCoverAxisPrefab;
    [SerializeField] private List<GameObject> _FabricCoverAxisList;

    [Header("Cover Fabric Axis SecuritySystems: ")]
    [SerializeField] private bool bFabricCoverStartSecuritySystemsActive = false;
    [SerializeField] private St_ARObjectFabricCoverData[] _FabricCoverStartSecuritySystemsDataArray;
    [SerializeField] private GameObject _CustomFabricCoverStartSSPrefab;
    [SerializeField] private List<GameObject> _FabricCoverStartSSList;
    [Space()]
    [SerializeField] private bool bFabricCoverEndSecuritySystemsActive = false;
    [SerializeField] private St_ARObjectFabricCoverData[] _FabricCoverEndSecuritySystemsDataArray;
    [SerializeField] private GameObject _CustomFabricCoverEndSSPrefab;
    [SerializeField] private List<GameObject> _FabricCoverEndSSList;

    //LOCAL VARIABLES
    //MOVMENT
    bool isDragging;
    //HUD
    HUDManagerScript _HUDManagerScript;

    //OBJ LENGHT
    private  GameObject _lenghtSliderRef;
    private List<GameObject> _boneRightObjList = new List<GameObject>();
    private List<GameObject> _boneRightObjListThatDontFollowAbsoluteDistance = new List<GameObject>();
    private Dictionary<GameObject, float> _initialBoneXPositions = new Dictionary<GameObject, float>();
    private GameObject _footRightObj;
    private GameObject _feedbackLenghtText;

    //LAMAS LENGHT
    private GameObject _lamasLenghtSliderRef;
    private GameObject _lamasFeedbackLenghtText;

    //Fabric Cover Lenght 
    private GameObject _fabricCoverSliderRef;
    private GameObject _fabricCoverFeedbackText;
    private List<GameObject> _boneUpObjList = new List<GameObject>();
    private List<GameObject> _boneParentsList = new List<GameObject>();
    private List<GameObject> _rightBonesFromAxisCoversList = new List<GameObject>();




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

        //Test
        InitBonesBySteps();
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

    public List<GameObject> GetAllBoneObjsBasedOnDirection(E_ARObjectComponentsDirection direction, bool bDontRespectAbsolutDistance = false)
    {
        List<GameObject> foundBones = new List<GameObject>();

        ARObjectPartScript[] allParts = GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript part in allParts)
        {
            if (part.GetBoneData() != null && part.GetBoneData()._BoneDirection == direction && part.GetBoneData().bDontRespectAbsolutDistance == bDontRespectAbsolutDistance)
            {
                foundBones.Add(part.gameObject);
            }
        }
        return foundBones;
    }

    public List<GameObject> GetAllBoneObjsBasedOnSecondDirection(E_ARObjectComponentsDirection direction)
    {
        List<GameObject> foundBones = new List<GameObject>();

        ARObjectPartScript[] allParts = GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript part in allParts)
        {
            if (part.GetBoneData() != null && part.GetBoneData()._BoneSecondDirection == direction)
            {
                foundBones.Add(part.gameObject);
            }
        }
        return foundBones;
    }

    public List<GameObject> GetAllBoneParentsBasedOnMovmentType(E_ARObjectReSizeDirections direction)
    {
        List<GameObject> foundBones = new List<GameObject>();

        ARObjectPartScript[] allParts = GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript part in allParts)
        {
            if (part.GetBoneParentData() != null && part.GetBoneParentData()._ARObjectReSizeDirection == direction)
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

    public List<GameObject> GetARObjectSpecificPartListBasedOnType(E_ARObjectParts part)
    {
        List<GameObject> foundParts = new List<GameObject>();

        ARObjectPartScript[] allParts = GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript specificPart in allParts)
        {
            if (specificPart != null && specificPart.GetARObjectPartData() != null)
            {
                if (specificPart.GetARObjectPartData()._ARObjectPartType == part)
                {
                    foundParts.Add(specificPart.gameObject);
                }
            }
        }

        return foundParts;
    }

    public float GetMaxARObjectLenghtDistance()
    {
        return _MaxARObjectLenghtDistance;
    }

    public CustomSplineInstantiate GetCustomSplineInstantiateScript()
    {
        return _CustomSplineInstantiateScript;
    }

    public St_APRelatedProduct[] GetARObjectRelatedProductsList()
    {
        return _ARObjectRelatedProductsList;
    }
    public float GetARObjectMAXLenghtWithPool()
    {
        return _MaxARObjectLenghtDistanceWithPool;
    }
    public float GetARObjectMAXLamasLenghtWithPool()
    {
        return _MaxLamasLenghtDistanceWithPool;
    }
    
    public E_CoverType GetCoverType()
    {
        return _CoverType;
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
        GameObject sliderObj = GetSliderFromSpecificSubPanel("RotationSubPanel");

        if (sliderObj != null)
        {
            float sliderValue = sliderObj.GetComponent<UnityEngine.UI.Slider>().value; // Valor entre 0 i 1

            // Fórmula: a 0.5 dona 0 | a 0 dona 180 | a 1 dona -180
            float _YValueRot = (0.5f - sliderValue) * 360f;
           
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
            var slider = sliderObj.GetComponent<UnityEngine.UI.Slider>();
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float currentRotation = (0.5f - slider.value) * 360f;

            sliderData.sliderResult = Mathf.Clamp(
                currentRotation + 1f, -180f, 180f);

            this.transform.localRotation = Quaternion.Euler(
                this.transform.localEulerAngles.x,
                sliderData.sliderResult,
                this.transform.localEulerAngles.z);

            UpdateRotationSlider();
        }
    }

    public void PlusRotateARObject()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("RotationSubPanel");

        if (sliderObj != null)
        {
            var slider = sliderObj.GetComponent<UnityEngine.UI.Slider>();
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float currentRotation = (0.5f - slider.value) * 360f;

            sliderData.sliderResult = Mathf.Clamp(
                currentRotation - 1f, -180f, 180f);

            this.transform.localRotation = Quaternion.Euler(
                this.transform.localEulerAngles.x,
                sliderData.sliderResult,
                this.transform.localEulerAngles.z);

            UpdateRotationSlider();
        }
    }


    void UpdateRotationSlider()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("RotationSubPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            // Convertim els graus (de -180 a 180) a un valor normalitzat de Slider (de 0 a 1)
            float normalizedSliderValue = 0.5f - (sliderData.sliderResult / 360f);

            // Assegurem que no surti del rang 0 a 1 per seguretat
            sliderObj.GetComponent<UnityEngine.UI.Slider>().SetValueWithoutNotify(Mathf.Clamp01(normalizedSliderValue));
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

    public void InitBonesThatDontFollowAbsoluteDistance()
    {
        _boneRightObjListThatDontFollowAbsoluteDistance = GetAllBoneObjsBasedOnDirection(E_ARObjectComponentsDirection.RIGHT, true);

        if (_boneRightObjListThatDontFollowAbsoluteDistance.Count != 0)
        {
            // _initialBoneXPositions.Clear();
            foreach (GameObject boneObj in _boneRightObjListThatDontFollowAbsoluteDistance)
            {
                if (boneObj != null && !_initialBoneXPositions.ContainsKey(boneObj))
                {
                    _initialBoneXPositions.Add(boneObj, boneObj.transform.localPosition.x);
                }
            }
        }
    }

    public void InitSetLenghtState()
    {
        _lenghtSliderRef = GetSliderFromSpecificSubPanel("SetARObjectLenghtSupPanel");

        if (_lenghtSliderRef != null)
        {
            if(_ARObjectManager.GetPoolActiveState())
            {
                _lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMax = _MaxARObjectLenghtDistanceWithPool;
            }
            else
            {
                _lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMax = _MaxARObjectLenghtDistance;
            }
                

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

        //GET RIGHT BONES THAT DONT FOLLOW ABSOLUTE DISTANCE

        // GET RIGHT FOOT
        _footRightObj = GetARObjectSpecificPartBasedOnType(E_ARObjectParts.FootRight);
        //Debug.Log("I Getted the _footRightObj -> " + _footRightObj.gameObject.name);


        if (bFabricCoverEndSecuritySystemsActive)
        {
            ClearFabricCoverEndSecuritySystems();
        }

        if(bFabricCoverStartSecuritySystemsActive)
        {
            ClearFabricCoverStartSecuritySystems();
        }
    }

    void SetARObjectLenght()
    {
        if (_lenghtSliderRef == null) return;
        if (_boneRightObjList.Count == 0) return;

        UpdateCladdingAveragePosition( _lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult);

        float currentSliderValue = (_lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult);

        foreach (GameObject boneObj in _boneRightObjList)
        {
            if(boneObj != null)
            {
                Vector3 bonePos = boneObj.transform.localPosition;
                
                if (boneObj.GetComponent<ARObjectPartScript>().GetBoneData().bDivideBy100)
                {
                    boneObj.transform.localPosition = new Vector3(currentSliderValue, bonePos.y, bonePos.z);
                }
                else
                {
                    boneObj.transform.localPosition = new Vector3(currentSliderValue * 100, bonePos.y, bonePos.z);
                }

                   
                _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght = currentSliderValue;
            }
           
        }

        if(_footRightObj != null)
        {
            _footRightObj.transform.localPosition = new Vector3(currentSliderValue, _footRightObj.transform.localPosition.y, _footRightObj.transform.localPosition.z);
            //Debug.Log("Setting the object " + _footRightObj.gameObject.name + " -> " + _footRightObj.transform.localPosition);
        }
        else
        {
            Debug.Log("Foot right is null?...");
        }

        //Moving bones that dont follow absolute distance
        if (_boneRightObjListThatDontFollowAbsoluteDistance.Count != 0)
        {
            foreach (GameObject boneObj in _boneRightObjListThatDontFollowAbsoluteDistance)
            {
                if (boneObj == null) continue;

                // Si per algun motiu no s'havia guardat la posició inicial, la guardem ara mateix com a seguretat
                if (!_initialBoneXPositions.TryGetValue(boneObj, out float startX))
                {
                    startX = boneObj.transform.localPosition.x;
                    _initialBoneXPositions[boneObj] = startX;
                }

                bool divideBy100 = boneObj.GetComponent<ARObjectPartScript>().GetBoneData().bDivideBy100;

                float sliderIncrement = currentSliderValue - 1f;

                float targetX;

                if (divideBy100)
                {
                    // Exemple: X inicial 1.2 + increment 0.2 = 1.4
                    targetX = startX + sliderIncrement;
                }
                else
                {
                    // Per als models que treballen en centímetres
                    targetX = startX + sliderIncrement * 100f;
                }

                Vector3 currentPos = boneObj.transform.localPosition;

                boneObj.transform.localPosition = new Vector3(targetX, currentPos.y, currentPos.z);
            }
        }


        if (bFabricCoverAxisBehaviourActive)
        {

            if (_FabricCoverAxisList != null && _FabricCoverAxisList.Count != 0 && _rightBonesFromAxisCoversList != null)
            {
                foreach (GameObject axisBone in _rightBonesFromAxisCoversList)
                {
                    if (axisBone != null)
                    {
                        Vector3 bonePos = axisBone.transform.localPosition;
                        axisBone.transform.localPosition = new Vector3(currentSliderValue * 100f, bonePos.y, bonePos.z);
                    }
                }
            }
        }


        ApplyBonesSizeBySteps(currentSliderValue);

        _feedbackLenghtText.GetComponent<TMP_Text>().text = currentSliderValue.ToString("F2") + "m";

        if (bFabricCoverEndSecuritySystemsActive)
        {
           CreateFabricCoverEndSecuritySystems();
        }

        if (bFabricCoverStartSecuritySystemsActive)
        {
           CreateFabricCoverStartSecuritySystems();
        }

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

    public void UpdateLenghtSlider()
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

        UpdateCladdingAveragePosition(newLenght);

        if (_boneRightObjList.Count == 0) return;

        if (_lenghtSliderRef != null)
        {
            var sliderData = _lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData();
            sliderData.sliderMin = 1f;

            if(_ARObjectManager.GetPoolActiveState())
            {
                sliderData.sliderMax = _MaxARObjectLenghtDistanceWithPool;
            }
            else
            {
                sliderData.sliderMax = _MaxARObjectLenghtDistance;
            }

            sliderData.sliderResult = newLenght;

            Slider unitySlider = _lenghtSliderRef.GetComponent<UnityEngine.UI.Slider>();
            
            if (unitySlider != null)
            {
                float normalizedValue = 0f;
                if (sliderData.sliderMax - sliderData.sliderMin > 0f)
                {
                    normalizedValue = (newLenght - sliderData.sliderMin) / (sliderData.sliderMax - sliderData.sliderMin);
                }

                // Bloquegem temporalment el TEU mètode anònim original per a que no es trepitgi en instanciar
                unitySlider.onValueChanged.RemoveListener(delegate { SetARObjectLenght(); });

                unitySlider.value = Mathf.Clamp01(normalizedValue);

                // El tornem a activar netament per a quan l'usuari munti el dit
                unitySlider.onValueChanged.AddListener(delegate { SetARObjectLenght(); });
            }
        }

        // Apliquem posició
        foreach (GameObject boneObj in _boneRightObjList)
        {
            Vector3 bonePos = boneObj.transform.localPosition;

            if (boneObj.GetComponent<ARObjectPartScript>().GetBoneData().bDivideBy100)
            {
                boneObj.transform.localPosition = new Vector3(newLenght, bonePos.y, bonePos.z);
            }
            else
            {
                boneObj.transform.localPosition = new Vector3(newLenght * 100, bonePos.y, bonePos.z);
            }

                
        }

        if (_boneRightObjListThatDontFollowAbsoluteDistance.Count != 0)
        {
            foreach (GameObject boneObj in _boneRightObjListThatDontFollowAbsoluteDistance)
            {
                if (boneObj != null && _initialBoneXPositions.TryGetValue(boneObj, out float startX))
                {
                    // Calculem la posició absoluta: Posició Inicial + Increment del Slider (partint de 1f com a base)

                    float targetX = 0f;

                    if (boneObj.GetComponent<ARObjectPartScript>().GetBoneData().bDivideBy100)
                    {
                        targetX = startX + ((newLenght - 1f));
                    }
                    else
                    {
                        targetX = startX + ((newLenght - 1f) * 100f);
                    }

                   

                    Vector3 currentPos = boneObj.transform.localPosition;
                    boneObj.transform.localPosition = new Vector3(targetX, currentPos.y, currentPos.z);
                }
            }
        }

        if (_footRightObj != null)
        {
            _footRightObj.transform.localPosition = new Vector3(newLenght, _footRightObj.transform.localPosition.y, _footRightObj.transform.localPosition.z);
        }

        // Guardem dada al Manager 
        if (_ARObjectManager != null && _ARObjectManager.GetCovARObjectData() != null)
        {
            _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght = newLenght;
        }

        if (_feedbackLenghtText != null)
        {
            _feedbackLenghtText.GetComponent<TMP_Text>().text = newLenght.ToString("F2") + "m";
        }

        ApplyBonesSizeBySteps(newLenght);

    }


    private void InitBonesBySteps()
    {
        if (_ARObjectManager == null ||
            _ARObjectManager.GetCovARObjectData() == null)
        {
            Debug.LogWarning(
                "InitBonesBySteps: Falta el manager o les dades.",
                this);
            return;
        }

        float currentLength = _ARObjectManager
            .GetCovARObjectData()
            ._CurrentARObjectLenght;

        ApplyBonesSizeBySteps(currentLength);
    }

    private float GetUpperSizeStep(float requestedLength,IEnumerable<float> steps)
    {
        float lastStep = requestedLength;

        foreach (float step in steps)
        {
            lastStep = step;

            if (requestedLength <= step ||
                Mathf.Approximately(requestedLength, step))
            {
                return step;
            }
        }

        // Si excepcionalment se supera el màxim, conserva l'últim step.
        return lastStep;
    }


    private void ApplyBonesSizeBySteps(float requestedLength)
    {
        var allParts = GetComponentsInChildren<ARObjectPartScript>(true);

        //Debug.Log($"Applying bones size by steps for requested length: {requestedLength}");

        foreach (var part in allParts)
        {
            var boneData = part.GetBoneData();

            if (boneData == null || !boneData.bSizeBySteps)
                continue;

            var steps = boneData._StepsArray;

            if (steps == null || !steps.Any())
                continue;

            float adjustedLength = requestedLength + boneData._NegativeOffsetToSlider;

            float selectedStep = GetUpperSizeStep(adjustedLength, steps);

            float targetX = boneData.bDivideBy100 ? selectedStep : selectedStep * 100f;

            Vector3 position = part.transform.localPosition;

            part.transform.localPosition =  new Vector3(targetX, position.y, position.z);
        }
    }

    private void UpdateCladdingAveragePosition(float sliderValue)
    {
        var claddingObjects = GetARObjectSpecificPartListBasedOnType(E_ARObjectParts.CladdingMovingByAveragePosition);

        foreach (GameObject claddingObj in claddingObjects)
        {
            if (claddingObj == null) continue;

            var part = claddingObj.GetComponent<ARObjectPartScript>();
            if (part == null) continue;

            var partData = part.GetARObjectPartData();
            if (partData == null) continue;

            var steps = partData._StepsArray;

            if (steps == null || !steps.Any())
                continue;

            float offset = partData._NegativeOffsetToSlider;

            // Seleccionem la coberta que pot allotjar
            // la mida del slider més els marges.
            float selectedStep = GetUpperSizeStep(sliderValue + offset, steps);

            // Amplada disponible entre els dos marges.
            float usableWidth = selectedStep - offset;

            // Centrem la peça segons l'espai que falta per omplir.
            float targetX = (sliderValue - usableWidth) * 0.5f;

            Vector3 position = claddingObj.transform.localPosition;

            claddingObj.transform.localPosition = new Vector3(targetX, position.y, position.z);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////// SET LAMAS LENGHT ////////////////////////////////////////////////////////////////////////

    public void InitSetLamasLenghtState()
    {
        _lamasLenghtSliderRef = GetSliderFromSpecificSubPanel("SetLamasLenghtSubPanel");

        if (_CustomSplineInstantiateScript != null)
        {
            if(_ARObjectManager.GetPoolActiveState())
            {
                _CustomSplineInstantiateScript.SetupMaxLamasDistance(_MaxLamasLenghtDistanceWithPool);
            }
            else
            {
                _CustomSplineInstantiateScript.SetupMaxLamasDistance(_MaxLamasLenghtDistance);
            }
        }

        if (_lamasLenghtSliderRef != null)
        {
            if(_ARObjectManager.GetPoolActiveState())
            {
                _lamasLenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMax = _MaxLamasLenghtDistanceWithPool;
            }
            else
            {
                _lamasLenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMax = _MaxLamasLenghtDistance;
            }
                
            _lamasLenghtSliderRef.GetComponent<Slider>().onValueChanged.RemoveListener(delegate { SetLamasLenght(); });
            _lamasLenghtSliderRef.GetComponent<Slider>().onValueChanged.AddListener(delegate { SetLamasLenght(); });

            _lamasFeedbackLenghtText = GetFeedbackTextFromSpecificSlider(_lamasLenghtSliderRef);

            AssignLamasLenghtButtonsEvents();
        }
        else
        {
            Debug.LogWarning("InitSetLamasLenghtState: El slider de la llargada no s'ha trobat.");
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

    public void UpdateLamasLenghtSlider()
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

        // 1. Agafem dades calculades en metres 
        var sliderData = _lamasLenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData();
        float currentSliderValueInMeters = sliderData.sliderResult;

        // 2. Traduïm els metres
        float normalizedValue = (currentSliderValueInMeters - sliderData.sliderMin) /
                               (sliderData.sliderMax - sliderData.sliderMin);

        normalizedValue = Mathf.Clamp01(normalizedValue);

        // 3. Passem valor al script del Spline
        _CustomSplineInstantiateScript.SetSliderValueFromUI(normalizedValue);

        if (_lamasFeedbackLenghtText != null)
        {
            _lamasFeedbackLenghtText.GetComponent<TMPro.TMP_Text>().text = _CustomSplineInstantiateScript.GetCurrentPosition().ToString("F2") + "m";
        }

        _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght = _CustomSplineInstantiateScript.GetCurrentPosition();
    }

   public void SetLamasLenghtByNum(float newLenght)
   {
        InitSetLamasLenghtState();
        
        if(_CustomSplineInstantiateScript != null)
        {
            if(_ARObjectManager.GetPoolActiveState())
            {
                _CustomSplineInstantiateScript.SetLamasLenghtByNum(newLenght, _MaxLamasLenghtDistanceWithPool, _lamasLenghtSliderRef, _lamasFeedbackLenghtText);
            }
            else
            {
                _CustomSplineInstantiateScript.SetLamasLenghtByNum(newLenght, _MaxLamasLenghtDistance, _lamasLenghtSliderRef, _lamasFeedbackLenghtText);
            }
                
        }
        
   }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////// SET FABRIC COVER LENGHT ////////////////////////////////////////////////////////////////////////

    public void InitSetFabricCoverLenghtState()
    {
        _fabricCoverSliderRef = GetSliderFromSpecificSubPanel("SetFabricCoverLenghtSubPanel");

        if (_fabricCoverSliderRef != null)
        {
            _fabricCoverSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMin = 0f;
            _fabricCoverSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderMax = _MaxLamasLenghtDistance;

            _fabricCoverSliderRef.GetComponent<Slider>().onValueChanged.RemoveListener(delegate { SetFabricCoverLenght(); });
            _fabricCoverSliderRef.GetComponent<Slider>().onValueChanged.AddListener(delegate { SetFabricCoverLenght(); });

            _fabricCoverFeedbackText = GetFeedbackTextFromSpecificSlider(_fabricCoverSliderRef);

            AssignFabricCoverLenghtButtonsEvents();
        }
        else
        {
            Debug.LogWarning("InitSetFabricCoverLenghtState: El slider de la llargada de la coberta no s'ha trobat. Ens saltem la UI de moment.");
        }

        // GET UP BONES (Segona direcció)
        _boneUpObjList = GetAllBoneObjsBasedOnSecondDirection(E_ARObjectComponentsDirection.UP);
        _boneParentsList = GetAllBoneParentsBasedOnMovmentType(E_ARObjectReSizeDirections.UpOnZAxis);

        // CLEAR AXIS
        ClearFabricCoverAxis();

        if (bFabricCoverEndSecuritySystemsActive)
        {
            ClearFabricCoverEndSecuritySystems();
        }

        if (bFabricCoverStartSecuritySystemsActive)
        {
            ClearFabricCoverStartSecuritySystems();
        }
    }

    void SetFabricCoverLenght()
    {
        if (_fabricCoverSliderRef == null) return;

        float currentSliderValue = (_fabricCoverSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult);

        // 1. Calcular la Z de l'objecte (va de 50 a 1000, etc.)
        float objectTargetZ = currentSliderValue * 100f;

        // 2. Aplicar l'equació exacta per a la Z del BoneParent
        float parentTargetZ = (2f * objectTargetZ) - 100f;

        // Moure els ossos de la llista principal (Objecte)
        if (_boneUpObjList != null)
        {
            foreach (GameObject boneObj in _boneUpObjList)
            {
                if (boneObj != null)
                {
                    Vector3 currentPos = boneObj.transform.localPosition;
                    boneObj.transform.localPosition = new Vector3(currentPos.x, currentPos.y, objectTargetZ);
                }
            }
        }

        // Moure els ossos de la llista de Parents aplicant l'equació
        if (_boneParentsList != null)
        {
            foreach (GameObject parentObj in _boneParentsList)
            {
                var partScript = parentObj.GetComponent<ARObjectPartScript>();
                if (partScript != null && partScript.GetBoneParentData() != null && partScript.GetBoneParentData()._ARObjectBones != null)
                {
                    foreach (GameObject innerBone in partScript.GetBoneParentData()._ARObjectBones)
                    {
                        if (innerBone != null)
                        {
                            Vector3 currentPos = innerBone.transform.localPosition;
                            innerBone.transform.localPosition = new Vector3(currentPos.x, currentPos.y, parentTargetZ);
                        }
                    }
                }
            }
        }

        _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght = currentSliderValue;

        CreateFabricCoverAxis();
        CreateFabricCoverEndSecuritySystems();
        CreateFabricCoverStartSecuritySystems();

        if (_fabricCoverFeedbackText != null)
        {
            _fabricCoverFeedbackText.GetComponent<TMP_Text>().text = currentSliderValue.ToString("F2") + "m";
        }
    }

    public void MinusFabricCoverLenght()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetFabricCoverLenghtSubPanel");

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

            SetFabricCoverLenght();

            UpdateFabricCoverLenghtSlider();
        }
    }

    public void PlusFabricCoverLenght()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetFabricCoverLenghtSubPanel");

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

            SetFabricCoverLenght();

            UpdateFabricCoverLenghtSlider();
        }
    }

    void UpdateFabricCoverLenghtSlider()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("SetFabricCoverLenghtSubPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            float normalizedStartingSliderValue = (sliderData.sliderResult - sliderData.sliderMin) /
                                                 (sliderData.sliderMax - sliderData.sliderMin);

            sliderObj.GetComponent<UnityEngine.UI.Slider>().value = normalizedStartingSliderValue;
        }

        CreateFabricCoverAxis();
        CreateFabricCoverEndSecuritySystems();
        CreateFabricCoverStartSecuritySystems();
    }

    void AssignFabricCoverLenghtButtonsEvents()
    {
        Button minusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("SetFabricCoverLenghtSubPanel", "Minus");
        if (minusBtn != null)
        {
            minusBtn.onClick.RemoveListener(MinusFabricCoverLenght);
            minusBtn.onClick.AddListener(MinusFabricCoverLenght);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Minus per a la coberta de tela.");
        }

        Button plusBtn = GetButtonWithSpecificNameFromSpecificSubPanel("SetFabricCoverLenghtSubPanel", "Plus");

        if (plusBtn != null)
        {
            plusBtn.onClick.RemoveListener(PlusFabricCoverLenght);
            plusBtn.onClick.AddListener(PlusFabricCoverLenght);
        }
        else
        {
            Debug.LogWarning("No s'ha trobat el botó Plus per a la coberta de tela.");
        }
    }

    public void SetFabricCoverLenghtByNum(float newLenght)
    {
        InitSetFabricCoverLenghtState();

        if (_boneUpObjList.Count == 0) return;

        if (_fabricCoverSliderRef != null)
        {
            var sliderData = _fabricCoverSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData();
            sliderData.sliderMin = 0f;
            sliderData.sliderMax = _MaxLamasLenghtDistance;

            if (newLenght > _MaxLamasLenghtDistance)
            {
                newLenght = _MaxLamasLenghtDistance;
            }

            sliderData.sliderResult = newLenght;

            Slider unitySlider = _fabricCoverSliderRef.GetComponent<UnityEngine.UI.Slider>();
            if (unitySlider != null)
            {
                float normalizedValue = 0f;
                if (sliderData.sliderMax - sliderData.sliderMin > 0f)
                {
                    normalizedValue = (newLenght - sliderData.sliderMin) / (sliderData.sliderMax - sliderData.sliderMin);
                }

                unitySlider.onValueChanged.RemoveListener(delegate { SetFabricCoverLenght(); });
                unitySlider.value = Mathf.Clamp01(normalizedValue);
                unitySlider.onValueChanged.AddListener(delegate { SetFabricCoverLenght(); });
            }
        }

        // 1. Calcular els valors objectiu segons els metres introduïts
        float objectTargetZ = newLenght * 100f;          // Va de 50 a 1000, etc.
        float parentTargetZ = (2f * objectTargetZ) - 100f; // Apliquem l'equació lineal exacta (Ex: a 50 dona 0, a 100 dona 100)

        // 2. Apliquem posició als ossos de la llista principal (Objecte)
        foreach (GameObject boneObj in _boneUpObjList)
        {
            if (boneObj != null)
            {
                Vector3 bonePos = boneObj.transform.localPosition;
                boneObj.transform.localPosition = new Vector3(bonePos.x, bonePos.y, objectTargetZ);
            }
        }

        // 3. Apliquem posició als ossos interns del BoneParent amb l'equació lineal
        if (_boneParentsList != null)
        {
            foreach (GameObject parentObj in _boneParentsList)
            {
                var partScript = parentObj.GetComponent<ARObjectPartScript>();

                if (partScript != null && partScript.GetBoneParentData() != null && partScript.GetBoneParentData()._ARObjectBones != null)
                {
                    foreach (GameObject innerBone in partScript.GetBoneParentData()._ARObjectBones)
                    {
                        if (innerBone != null)
                        {
                            Vector3 bonePos = innerBone.transform.localPosition;
                            innerBone.transform.localPosition = new Vector3(bonePos.x, bonePos.y, parentTargetZ);
                        }
                    }
                }
            }
        }

        // Guardem dada al Manager 
        if (_ARObjectManager != null && _ARObjectManager.GetCovARObjectData() != null)
        {
            _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght = newLenght;
        }

        if (_fabricCoverFeedbackText != null)
        {
            _fabricCoverFeedbackText.GetComponent<TMP_Text>().text = newLenght.ToString("F2") + "m";
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  
    //////////////////////////////////////////////// FABRIC COVER AXIS ////////////////////////////////////////////////////////

    public void CreateFabricCoverAxis()
    {
        ClearFabricCoverAxis();

        if (!bFabricCoverAxisBehaviourActive) return;

        if (_ARObjectManager == null ||  _ARObjectManager.GetCovARObjectData() == null ||_CustomFabricCoverAxisPrefab == null ||_FabricCoverAxisDataArray == null)
        {
            Debug.LogWarning("CreateFabricCoverAxis: Falten referències.");
            return;
        }

        float currentLength = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght;

        float currentWidth = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;

        St_ARObjectFabricCoverData data = _FabricCoverAxisDataArray;

        if (!IsValidFabricAxisValue(currentLength) || !IsValidFabricAxisValue(currentWidth) ||  currentLength <= 0f ||  currentWidth <= 0f)
        {
            return;
        }

        // Aquesta regla s'aplica segons la llargada de la lona.
        if (currentLength < data._MinDistance ||  currentLength > data._MaxDistance)
        {
            return;
        }

        List<float> positions = CalculateFabricCoverAxisPositions( currentLength,data);

        if (_FabricCoverAxisList == null)
        {
            _FabricCoverAxisList = new List<GameObject>();
        }

        if (_rightBonesFromAxisCoversList == null)
        {
            _rightBonesFromAxisCoversList = new List<GameObject>();
        }

        foreach (float targetZ in positions)
        {
            // Mateixa instanciació i transformació que al teu codi original.
            GameObject newAxis = Instantiate(_CustomFabricCoverAxisPrefab, this.transform);

            newAxis.transform.localPosition = new Vector3(0f, 0f, targetZ);
            newAxis.transform.localRotation = Quaternion.identity;

            _FabricCoverAxisList.Add(newAxis);

            List<GameObject> axisRightBones = GetRightBonesFromSpecificAxis(newAxis);

            foreach (GameObject boneObj in axisRightBones)
            {
                if (boneObj == null) continue;

                _rightBonesFromAxisCoversList.Add(boneObj);

                Vector3 bonePos = boneObj.transform.localPosition;

                boneObj.transform.localPosition = new Vector3( currentWidth * 100f, bonePos.y, bonePos.z);
            }
        }
    }

    private List<float> CalculateFabricCoverAxisPositions(float length, St_ARObjectFabricCoverData data)
    {
        List<float> positions = new List<float>();

        const float tolerance = 0.00001f;

        if (data.bSpawnObjectsByNumber == data.bSpawnObjectsByDistance)
        {
            Debug.LogWarning( "FabricCoverAxis: Activa només un mode de generació.");
            return positions;
        }

        float minOffset = data.bOffsetDistanceToLimits? data._DistanceToLimits : 0f;

        if (!IsValidFabricAxisValue(minOffset) || minOffset < 0f)
        {
            Debug.LogWarning("FabricCoverAxis: Offset no vàlid.");
            return positions;
        }

        float maxOffset = minOffset;

        // El segon offset només intervé si és superior al primer.
        if (data.bOffsetDistanceToLimits)
        {
            if (!IsValidFabricAxisValue(data._DistanceToLimits02))
            {
                Debug.LogWarning("FabricCoverAxis: Segon offset no vàlid.");
                return positions;
            }

            if (data._DistanceToLimits02 > minOffset)
            {
                maxOffset = data._DistanceToLimits02;
            }
        }

        float availableLength = length - (2f * minOffset);

        // No hi caben les barres dels extrems.
        if (availableLength < -tolerance)
        {
            return positions;
        }

        // Els dos extrems coincideixen: una sola barra centrada.
        if (availableLength <= tolerance)
        {
            positions.Add(length / 2f);
            return positions;
        }

        // MODE PER NOMBRE
        if (data.bSpawnObjectsByNumber)
        {
            int count = data._NumberOfObjectsToSpawn;

            if (count <= 0)
            {
                Debug.LogWarning( "FabricCoverAxis: El nombre de barres ha de ser positiu.");
                return positions;
            }

            // En aquest mode mantenim el marge base i repartim uniformement.
            AddUniformFabricAxisPositions( positions, length, minOffset, count);

            return positions;
        }

        // MODE PER DISTÀNCIA
        float minSpacing = data._DistanceBetweenObjects;

        if (!IsValidFabricAxisValue(minSpacing) || minSpacing <= 0f)
        {
            Debug.LogWarning( "FabricCoverAxis: La separació ha de ser positiva.");
            return positions;
        }

        if (!IsValidFabricAxisValue(data._DistanceBetweenObjects02))
        {
            Debug.LogWarning("FabricCoverAxis: Segona separació no vàlida.");
            return positions;
        }

        // Si el segon valor és 0 o no supera el primer,
        // treballem amb una única separació de referència.
        float maxSpacing = data._DistanceBetweenObjects02 > minSpacing ? data._DistanceBetweenObjects02: minSpacing;

        // Nombre màxim d'espais que hi caben respectant el mínim.
        int maxSpaces = Mathf.FloorToInt( (availableLength + tolerance) / minSpacing);

        // Cas base: dues barres. Encara no hi cap una tercera.
        if (maxSpaces < 2)
        {
            AddUniformFabricAxisPositions(positions, length, minOffset, 2);
            return positions;
        }

        // Busquem una distribució uniforme que respecti tots dos intervals.
        // Comencem per la que permet més barres.
        for (int spaces = maxSpaces; spaces >= 2; spaces--)
        {
            // length = 2 * offset + spaces * spacing
            float lowestOffset = Mathf.Max(minOffset, (length - spaces * maxSpacing) / 2f);

            float highestOffset = Mathf.Min( maxOffset, (length - spaces * minSpacing) / 2f);

            if (lowestOffset > highestOffset + tolerance)
            {
                continue;
            }

            // Escollim el marge vàlid més proper al marge mínim.
            float offset = Mathf.Clamp(lowestOffset, minOffset, maxOffset);

            AddUniformFabricAxisPositions( positions,length, offset, spaces + 1);

            return positions;
        }

        // EXCEPCIÓ:
        // No hi ha cap combinació que respecti tots els intervals.
        // Augmentem el marge tant com sigui possible sense perdre barres.
        int fallbackSpaces = maxSpaces;

        float fallbackOffset = Mathf.Clamp( (length - fallbackSpaces * minSpacing) / 2f, minOffset, maxOffset );

        float usefulLength = length - 2f * fallbackOffset;

        // Els espais interiors respecten el rang.
        // Només els dos espais exteriors poden superar el màxim.
        float innerSpacing = Mathf.Clamp(usefulLength / fallbackSpaces, minSpacing, maxSpacing);

        int innerSpaces = fallbackSpaces - 2;

        float outerSpacing = (usefulLength - innerSpaces * innerSpacing) / 2f;

        positions.Add(fallbackOffset);

        // Barres interiors, simètriques respecte del centre.
        for (int i = 0; i < fallbackSpaces - 1; i++)
        {
            positions.Add( fallbackOffset + outerSpacing + i * innerSpacing);
        }

        positions.Add(length - fallbackOffset);

        return positions;
    }

    private void AddUniformFabricAxisPositions( List<float> positions,float length, float offset,int count)
    {
        if (count == 1)
        {
            positions.Add(length / 2f);
            return;
        }

        float spacing = (length - 2f * offset) / (count - 1);

        for (int i = 0; i < count; i++)
        {
            positions.Add(offset + i * spacing);
        }
    }

    private bool IsValidFabricAxisValue(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private List<GameObject> GetRightBonesFromSpecificAxis(GameObject axisRoot)
    {
        List<GameObject> foundBones = new List<GameObject>();

        ARObjectPartScript[] allParts = axisRoot.GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript part in allParts)
        {
            if (part.GetBoneData() != null &&part.GetBoneData()._BoneDirection ==E_ARObjectComponentsDirection.RIGHT)
            {
                foundBones.Add(part.gameObject);
            }
        }

        return foundBones;
    }

    public void ClearFabricCoverAxis()
    {
        if (_FabricCoverAxisList != null)
        {
            foreach (GameObject axis in _FabricCoverAxisList)
            {
                if (axis != null)
                {
                    Destroy(axis);
                }
            }

            _FabricCoverAxisList.Clear();
        }

        if (_rightBonesFromAxisCoversList != null)
        {
            _rightBonesFromAxisCoversList.Clear();
        }
    }


    //////////////////////////////////////////////// FABRIC SECURITY SYSTEM START ////////////////////////////////////////////////

    public void CreateFabricCoverStartSecuritySystems()
    {
        ClearFabricCoverStartSecuritySystems();

        if (!bFabricCoverStartSecuritySystemsActive) return;

        if (_ARObjectManager == null || _ARObjectManager.GetCovARObjectData() == null || _CustomFabricCoverStartSSPrefab == null)
        {
            Debug.LogWarning("CreateFabricCoverStartSecuritySystems: Falten referències.");
            return;
        }

        float totalWidth = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;

        if (_FabricCoverStartSSList == null)
        {
            _FabricCoverStartSSList = new List<GameObject>();
        }

        SpawnFabricCoverSecuritySystems(totalWidth,-0.1f, _FabricCoverStartSecuritySystemsDataArray,_CustomFabricCoverStartSSPrefab, _FabricCoverStartSSList, false);
    }

    public void ClearFabricCoverStartSecuritySystems()
    {
        if (_FabricCoverStartSSList == null) return;

        foreach (GameObject anchor in _FabricCoverStartSSList)
        {
            if (anchor != null)
            {
                Destroy(anchor);
            }
        }

        _FabricCoverStartSSList.Clear();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////// SECURITY SYSTEM END ////////////////////////////////////////////////////////

    public void CreateFabricCoverEndSecuritySystems()
    {
        ClearFabricCoverEndSecuritySystems();

        if (!bFabricCoverEndSecuritySystemsActive) return;

        if (_ARObjectManager == null || _ARObjectManager.GetCovARObjectData() == null ||_CustomFabricCoverEndSSPrefab == null)
        {
            Debug.LogWarning("CreateFabricCoverEndSecuritySystems: Falten referències.");
            return;
        }

        float totalWidth = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;
        float currentLength = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght;

        if (_FabricCoverEndSSList == null)
        {
            _FabricCoverEndSSList = new List<GameObject>();
        }

        SpawnFabricCoverSecuritySystems(totalWidth, currentLength,_FabricCoverEndSecuritySystemsDataArray, _CustomFabricCoverEndSSPrefab,_FabricCoverEndSSList, true);
    }

    public void ClearFabricCoverEndSecuritySystems()
    {
        if (_FabricCoverEndSSList == null) return;

        foreach (GameObject anchor in _FabricCoverEndSSList)
        {
            if (anchor != null)
            {
                Destroy(anchor);
            }
        }

        _FabricCoverEndSSList.Clear();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////// FUNCIONS AUXILIARS (SS) ////////////////////////////////////////////////////////

    private St_ARObjectFabricCoverData GetFabricCoverSecuritySystemsData( float totalWidth,St_ARObjectFabricCoverData[] dataArray)
    {
        if (dataArray == null) return null;

        St_ARObjectFabricCoverData selectedData = null;

        foreach (St_ARObjectFabricCoverData data in dataArray)
        {
            if (data == null) continue;

            if (totalWidth < data._MinDistance || totalWidth > data._MaxDistance)
            {
                continue;
            }

            // Si dos rangs comparteixen un límit, escollim el de màxim menor:
            // 2.69 -> 2 tensors; 4.99 -> 3 tensors.
            if (selectedData == null ||
                data._MaxDistance < selectedData._MaxDistance)
            {
                selectedData = data;
            }
        }

        return selectedData;
    }

    private void SpawnFabricCoverSecuritySystems(float totalWidth,float targetZ, St_ARObjectFabricCoverData[] dataArray,GameObject prefab,List<GameObject> spawnedObjects, bool rotateEnd)
    {
        if (float.IsNaN(totalWidth) || float.IsInfinity(totalWidth) ||totalWidth <= 0f)
        {
            return;
        }

        St_ARObjectFabricCoverData data = GetFabricCoverSecuritySystemsData(totalWidth, dataArray);

        if (data == null)
        {
            Debug.LogWarning( $"FabricCoverSecuritySystems: No hi ha cap regla per a {totalWidth:F2} m.");
            return;
        }

        // Cal activar exactament un dels dos modes.
        if (data.bSpawnObjectsByNumber == data.bSpawnObjectsByDistance)
        {
            Debug.LogWarning("FabricCoverSecuritySystems: Activa només un mode: per nombre o per distància.");
            return;
        }

        float offset = data.bOffsetDistanceToLimits ? data._DistanceToLimits : 0f;

        if (float.IsNaN(offset) || float.IsInfinity(offset) || offset < 0f)
        {
            Debug.LogWarning("FabricCoverSecuritySystems: Offset no vàlid.");
            return;
        }

        float usefulWidth = totalWidth - (2f * offset);

        // Evitem crear menys tensors dels configurats o superposar-los.
        if (usefulWidth <= 0f)
        {
            Debug.LogWarning("FabricCoverSecuritySystems: Els offsets no deixen amplada útil.");
            return;
        }

        int totalAnchors;

        if (data.bSpawnObjectsByNumber)
        {
            totalAnchors = data._NumberOfObjectsToSpawn;

            if (totalAnchors <= 0)
            {
                Debug.LogWarning( "FabricCoverSecuritySystems: El nombre d'objectes ha de ser positiu.");
                return;
            }
        }
        else
        {
            float requestedDistance = data._DistanceBetweenObjects;

            if (float.IsNaN(requestedDistance) || float.IsInfinity(requestedDistance) || requestedDistance <= 0f)
            {
                Debug.LogWarning("FabricCoverSecuritySystems: La distància ha de ser positiva.");
                return;
            }

            // Mode per distància: repartiment uniforme entre els dos límits.
            // La separació resultant no supera la distància configurada.
            int numberOfSpaces = Mathf.CeilToInt(usefulWidth / requestedDistance);

            totalAnchors = numberOfSpaces + 1;
        }

        float spacing = totalAnchors > 1 ? usefulWidth / (totalAnchors - 1) : 0f;

        Vector3 prefabLocalPos = prefab.transform.localPosition;
        Quaternion targetRotation = prefab.transform.localRotation;

        if (rotateEnd)
        {
            targetRotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        for (int i = 0; i < totalAnchors; i++)
        {
            // Si només es configura un tensor, el col·loquem al centre.
            float targetX = totalAnchors == 1 ? totalWidth / 2f : offset + (i * spacing);

            GameObject newAnchor = Instantiate(prefab, this.transform);

            newAnchor.transform.localPosition = new Vector3(targetX, prefabLocalPos.y, targetZ);

            newAnchor.transform.localRotation = targetRotation;

            spawnedObjects.Add(newAnchor);
        }
    }


    /////////////////////////////////////////////////////////////////////// DESACTIVE MASK //////////////////////////////////////////////////////////////////////////

    public void SetARObjectMaskState(bool active)
    {
        List<GameObject> maskList = GetARObjectSpecificPartListBasedOnType(E_ARObjectParts.Mask);

        if(maskList.Count != 0)
        {
            foreach (GameObject mask in maskList)
            {
                mask.SetActive(active);
                
            }
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}