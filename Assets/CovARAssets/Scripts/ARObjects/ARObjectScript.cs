using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
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
    [SerializeField] private float _DistanceBetweenAxis = 3.0f;
    [SerializeField] private GameObject _CustomFabricCoverAxisPrefab;
    [SerializeField] private List<GameObject> _FabricCoverAxisList;

    [Header("Cover Fabric Axis SecuritySystems: ")]
    [SerializeField] private bool bFabricCoverStartSecuritySystemsActive = false;
    [SerializeField] private float _DistanceBetweenStartSecuritySystems = 1.0f;
    [SerializeField] private float _DistanceBetweenStartSecuritySystemAndLimits = 0.2f;
    [SerializeField] private GameObject _CustomFabricCoverStartSSPrefab;
    [SerializeField] private List<GameObject> _FabricCoverStartSSList;
    [Space()]
    [SerializeField] private bool bFabricCoverEndSecuritySystemsActive = false;
    [SerializeField] private float _DistanceBetweenEndSecuritySystems = 1.0f;
    [SerializeField] private float _DistanceBetweenEndsSecuritySystemAndLimits = 0.2f;
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
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            // 1. Arrodonim cap amunt al múltiple de 5 més proper
            // Exemple: si estigués a -41f / 5f = -8.2 -> Ceil(-8.2) = -8 -> -8 * 5 = -40f
            float currentRotRounded = Mathf.Ceil(sliderData.sliderResult / 5f) * 5f;

            // 2. Sumem 5 graus per avançar cap a +180
            float newRot = currentRotRounded + 5f;

            // 3. Limitem dins el rang
            sliderData.sliderResult = Mathf.Clamp(newRot, sliderData.sliderMin, sliderData.sliderMax);

            // 4. Apliquem la rotació
            this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x, sliderData.sliderResult, this.transform.localEulerAngles.z);

            // 5. Sincronitzem el slider visual
            UpdateRotationSlider();
        }
    }

    public void PlusRotateARObject()
    {
        GameObject sliderObj = GetSliderFromSpecificSubPanel("RotationSubPanel");

        if (sliderObj != null)
        {
            var sliderData = sliderObj.GetComponent<UIBehaviourComponent>().GetSliderData();

            // 1. Arrodonim cap avall al múltiple de 5 més proper (perquè volem que baixi cap a -180)
            // Exemple: 132f / 5f = 26.4 -> Floor(26.4) = 26 -> 26 * 5 = 130f
            float currentRotRounded = Mathf.Floor(sliderData.sliderResult / 5f) * 5f;

            // 2. Restem 5 graus directament per avançar cap a -180
            float newRot = currentRotRounded - 5f;

            // 3. Limitem dins el rang permès (-180 a 180)
            sliderData.sliderResult = Mathf.Clamp(newRot, sliderData.sliderMin, sliderData.sliderMax);

            // 4. Apliquem la rotació
            this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x, sliderData.sliderResult, this.transform.localEulerAngles.z);

            // 5. Sincronitzem el slider visual
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
            sliderObj.GetComponent<UnityEngine.UI.Slider>().value = Mathf.Clamp01(normalizedSliderValue);
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

        float currentSliderValue = (_lenghtSliderRef.GetComponent<UIBehaviourComponent>().GetSliderData().sliderResult);

        foreach (GameObject boneObj in _boneRightObjList)
        {
            if(boneObj != null)
            {
                Vector3 bonePos = boneObj.transform.localPosition;
                boneObj.transform.localPosition = new Vector3(currentSliderValue * 100, bonePos.y, bonePos.z);
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

                // Calculem la nova posició X usant 'currentSliderValue'
                float targetX = startX + ((currentSliderValue - 1f) * 100f);
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
            boneObj.transform.localPosition = new Vector3(newLenght * 100, bonePos.y, bonePos.z);
        }

        if (_boneRightObjListThatDontFollowAbsoluteDistance.Count != 0)
        {
            foreach (GameObject boneObj in _boneRightObjListThatDontFollowAbsoluteDistance)
            {
                if (boneObj != null && _initialBoneXPositions.TryGetValue(boneObj, out float startX))
                {
                    // Calculem la posició absoluta: Posició Inicial + Increment del Slider (partint de 1f com a base)
                    float targetX = startX + ((newLenght - 1f) * 100f);

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


    ////////////////////////////////////////////////////////// FABRIC COVER AXIS ///////////////////////////////////////////////////////////////////////////

    public void CreateFabricCoverAxis()
    {
        if (!bFabricCoverAxisBehaviourActive)
        {
            ClearFabricCoverAxis();
            return;
        }

        // Netegem tant els eixos com els seus ossos abans de generar els nous
        ClearFabricCoverAxis();

        if (_ARObjectManager == null || _ARObjectManager.GetCovARObjectData() == null || _CustomFabricCoverAxisPrefab == null)
        {
            Debug.LogWarning("CreateFabricCoverAxis: Falten referències.");
            return;
        }

        float currentLenght = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght;
        // Obtenim el valor actual de l'amplada de l'objecte (en metres)
        float currentWidthValue = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;

        float nextAxisPositionMeters = _DistanceBetweenAxis;

        while (nextAxisPositionMeters < currentLenght)
        {
            float targetZ = nextAxisPositionMeters;

            // 1. Instanciar l'eix intermedi
            GameObject newAxis = Instantiate(_CustomFabricCoverAxisPrefab, this.transform);
            newAxis.transform.localPosition = new Vector3(0f, 0f, targetZ);
            newAxis.transform.localRotation = Quaternion.identity;

            _FabricCoverAxisList.Add(newAxis);

            // 2. Buscar els ossos RIGHT que hi ha DINS d'aquest prefab acabat de crear
            List<GameObject> axisRightBones = GetRightBonesFromSpecificAxis(newAxis);

            // 3. Afegir-los a la llista general de control i aplicar-los l'amplada immediatament
            foreach (GameObject boneObj in axisRightBones)
            {
                if (boneObj != null)
                {
                    _rightBonesFromAxisCoversList.Add(boneObj);

                    // Apliquem l'amplada actual multiplicada per 100 en l'eix X local del os
                    Vector3 bonePos = boneObj.transform.localPosition;
                    boneObj.transform.localPosition = new Vector3(currentWidthValue * 100f, bonePos.y, bonePos.z);
                }
            }

            nextAxisPositionMeters += _DistanceBetweenAxis;
        }
    }

    // Funció auxiliar per buscar ossos RIGHT només dins de l'eix passat per paràmetre
    private List<GameObject> GetRightBonesFromSpecificAxis(GameObject axisRoot)
    {
        List<GameObject> foundBones = new List<GameObject>();

        // Busquem els scripts en els fills de l'objecte arrel de la barra creada
        ARObjectPartScript[] allParts = axisRoot.GetComponentsInChildren<ARObjectPartScript>(true);

        foreach (ARObjectPartScript part in allParts)
        {
            // Si té dades d'os i la direcció és UP o RIGHT (en aquest cas filtrem per RIGHT basat en el teu enu)
            if (part.GetBoneData() != null && part.GetBoneData()._BoneDirection == E_ARObjectComponentsDirection.RIGHT)
            {
                foundBones.Add(part.gameObject);
            }
        }

        return foundBones;
    }

    public void ClearFabricCoverAxis()
    {
        // Destruir eixos
        if (_FabricCoverAxisList != null)
        {
            foreach (GameObject axis in _FabricCoverAxisList)
            {
                if (axis != null) Destroy(axis);
            }
            _FabricCoverAxisList.Clear();
        }

        // Netejar la llista d'ossos vells (ja s'han destruït en esborrar els eixos pares)
        _rightBonesFromAxisCoversList.Clear();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////////// FABRIC SECURITY SYSTEM START /////////////////////////////////////////////////////////////////////////////////////


    public void CreateFabricCoverStartSecuritySystems()
    {
        if (!bFabricCoverStartSecuritySystemsActive)
        {
            ClearFabricCoverStartSecuritySystems();
            return;
        }

        ClearFabricCoverStartSecuritySystems();

        if (_ARObjectManager == null || _ARObjectManager.GetCovARObjectData() == null || _CustomFabricCoverStartSSPrefab == null)
        {
            Debug.LogWarning("CreateFabricCoverStartSecuritySystems: Falten referències.");
            return;
        }

        float totalWidth = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;

        if (totalWidth <= 0 || _DistanceBetweenStartSecuritySystems <= 0) return;

        // --- LÒGICA AMB MARGES ALS LÍMITS ---
        // Calculem l'amplada útil real que ens queda per repartir els sistemes de seguretat
        float usefulWidth = totalWidth - (2f * _DistanceBetweenStartSecuritySystemAndLimits);

        // Control de seguretat: si la lona és tan estreta que els marges es trepitgen o no deixen espai,
        // col·loquem un sol ancoratge al centre exacte i sortim.
        if (usefulWidth <= 0)
        {
            GameObject centerAnchor = Instantiate(_CustomFabricCoverStartSSPrefab, this.transform);
            Vector3 prefabLocalPos = _CustomFabricCoverStartSSPrefab.transform.localPosition;
            centerAnchor.transform.localPosition = new Vector3(totalWidth / 2f, prefabLocalPos.y, -0.1f);
            centerAnchor.transform.localRotation = _CustomFabricCoverStartSSPrefab.transform.localRotation;
            _FabricCoverStartSSList.Add(centerAnchor);
            return;
        }

        // Calculem els espais necessaris basant-nos en l'amplada útil
        int numberOfSpaces = Mathf.CeilToInt(usefulWidth / _DistanceBetweenStartSecuritySystems);

        float calculatedDistance = usefulWidth / numberOfSpaces;
        float minimumDistanceAllowed = _DistanceBetweenStartSecuritySystems / 2f;

        if (calculatedDistance < minimumDistanceAllowed && numberOfSpaces > 1)
        {
            numberOfSpaces--;
        }

        int totalAnchors = numberOfSpaces + 1;
        float finalDistanceBetweenAnchors = usefulWidth / numberOfSpaces;

        // EL NOU CANVI: El punt d'inici en X ja no és 0, sinó el límit de marge que has definit
        float startX = _DistanceBetweenStartSecuritySystemAndLimits;

        // --- INSTANCIACIÓ ---
        for (int i = 0; i < totalAnchors; i++)
        {
            // Calculem la posició X local sumant la distància a partir del marge inicial
            float targetX = startX + (i * finalDistanceBetweenAnchors);

            GameObject newAnchor = Instantiate(_CustomFabricCoverStartSSPrefab, this.transform);

            Vector3 prefabLocalPos = _CustomFabricCoverStartSSPrefab.transform.localPosition;
            Quaternion prefabLocalRot = _CustomFabricCoverStartSSPrefab.transform.localRotation;

            newAnchor.transform.localPosition = new Vector3(targetX, prefabLocalPos.y, -0.1f);
            newAnchor.transform.localRotation = prefabLocalRot;

            _FabricCoverStartSSList.Add(newAnchor);
        }
    }

    public void ClearFabricCoverStartSecuritySystems()
    {
        // Destruir els sistemes de seguretat de l'inici
        if (_FabricCoverStartSSList != null)
        {
            foreach (GameObject anchor in _FabricCoverStartSSList)
            {
                if (anchor != null)
                {
                    Destroy(anchor);
                }
            }
            _FabricCoverStartSSList.Clear();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////// SECURITY SYSTEM END //////////////////////////////////////////////////////////////////////////////////////////

    public void CreateFabricCoverEndSecuritySystems()
    {
        if (!bFabricCoverEndSecuritySystemsActive)
        {
            ClearFabricCoverEndSecuritySystems();
            return;
        }

        ClearFabricCoverEndSecuritySystems();

        if (_ARObjectManager == null || _ARObjectManager.GetCovARObjectData() == null || _CustomFabricCoverEndSSPrefab == null)
        {
            Debug.LogWarning("CreateFabricCoverEndSecuritySystems: Falten referències.");
            return;
        }

        float totalWidth = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght;
        float currentLength = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght;

        if (totalWidth <= 0 || _DistanceBetweenEndSecuritySystems <= 0) return;

        // --- LÒGICA AMB MARGES ALS LÍMITS ---
        float usefulWidth = totalWidth - (2f * _DistanceBetweenEndsSecuritySystemAndLimits);

        // Control de seguretat si és massa estreta
        if (usefulWidth <= 0)
        {
            GameObject centerAnchor = Instantiate(_CustomFabricCoverEndSSPrefab, this.transform);
            Vector3 prefabLocalPos = _CustomFabricCoverEndSSPrefab.transform.localPosition;

            centerAnchor.transform.localPosition = new Vector3(totalWidth / 2f, prefabLocalPos.y, currentLength);

            // Apliquem la rotació original del prefab + 180 graus a la Y
            Quaternion targetRotation = _CustomFabricCoverEndSSPrefab.transform.localRotation * Quaternion.Euler(0f, 180f, 0f);
            centerAnchor.transform.localRotation = targetRotation;

            _FabricCoverEndSSList.Add(centerAnchor);
            return;
        }

        int numberOfSpaces = Mathf.CeilToInt(usefulWidth / _DistanceBetweenEndSecuritySystems);
        float calculatedDistance = usefulWidth / numberOfSpaces;
        float minimumDistanceAllowed = _DistanceBetweenEndSecuritySystems / 2f;

        if (calculatedDistance < minimumDistanceAllowed && numberOfSpaces > 1)
        {
            numberOfSpaces--;
        }

        int totalAnchors = numberOfSpaces + 1;
        float finalDistanceBetweenAnchors = usefulWidth / numberOfSpaces;

        float startX = _DistanceBetweenEndsSecuritySystemAndLimits;

        // --- INSTANCIACIÓ ---
        for (int i = 0; i < totalAnchors; i++)
        {
            float targetX = startX + (i * finalDistanceBetweenAnchors);

            GameObject newAnchor = Instantiate(_CustomFabricCoverEndSSPrefab, this.transform);

            Vector3 prefabLocalPos = _CustomFabricCoverEndSSPrefab.transform.localPosition;

            // Calculem la rotació combinant la original del prefab amb un gir de 180 a la Y
            Quaternion prefabLocalRot = _CustomFabricCoverEndSSPrefab.transform.localRotation;
            Quaternion rotatedY = prefabLocalRot * Quaternion.Euler(0f, 180f, 0f);

            newAnchor.transform.localPosition = new Vector3(targetX, prefabLocalPos.y, currentLength);
            newAnchor.transform.localRotation = rotatedY; // Apliquem la rotació amb el gir de 180

            _FabricCoverEndSSList.Add(newAnchor);
        }
    }

    public void ClearFabricCoverEndSecuritySystems()
    {
        if (_FabricCoverEndSSList != null)
        {
            foreach (GameObject anchor in _FabricCoverEndSSList)
            {
                if (anchor != null)
                {
                    Destroy(anchor);
                }
            }
            _FabricCoverEndSSList.Clear();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


}