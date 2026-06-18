using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System.Globalization;
using Assets.SimpleLocalization.Scripts;




public class UIBehaviourComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI ANIMATION COMPONENT")]
    [Space(25)]
    [Header("Wich type of UI Component is?")]
    [SerializeField] private E_UIComponents UI_Component;
    [Header("Component reference: (Automatic)")]
    [SerializeField] private GameObject myPanel;
    [Header("Component reference: (Automatic)")]
    [SerializeField] private Button myButton;
    [Header("Component reference: (Automatic)")]
    [SerializeField] private Slider mySlider;
    [Header("Component reference: (Automatic)")]
    [SerializeField] private TMP_Text myText;
    [Header("Component reference: (Automatic)")]
    [SerializeField] private TMP_InputField myInputField;
    [Header("Component reference: (Automatic)")]
    [SerializeField] private TMP_Dropdown myDropdown;

    //Button behaviour script
    [Space()]
    [SerializeField] private St_ButtonBehaviourStruct St_ButtonBehaviourData;
    [Space()]
    [SerializeField] private St_ButtonTutorialPopUpData St_TutorialPopUpData;
    [Space()]
    [Header("Type of button:")]
    [SerializeField] private E_ButtonType buttonType;
    //InputField behaviour script
    [Space()]
    [Header("Type of InputText:")]
    [SerializeField] private E_InputFieldType inputFieldType;
    [Space()]
    [SerializeField] private St_InputFieldBehaviourStruct St_InputFieldBehaviourData;
    string validEmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    //Dropdown behaviour script
    [Space()]
    [SerializeField] private St_DropdownBehaviourStruct St_DropdownBehaviourData;
    //Text behaviour script 
    [Space()]
    [SerializeField] private St_TextBehaviourScript St_TextBehaviourData;
    //Slider behaviour script
    [Space()]
    [SerializeField] private St_SliderBehaviourStruct St_SliderBehaviourData;
    //Toggle behaviour script
    [Space()]
    [SerializeField] private St_ToggleBehaviourScript St_ToggleBehaviourScript;
    //Default popUp Behaviour
    [Space()]
    [SerializeField] private St_DefaultPopUpData St_DefaultPopUpBehaviourData;
    [Space()]
    [SerializeField] private bool bPremiumBehaviour;
    [SerializeField] private St_PremiumUIBehaviour _UIPremiumBehaviour;
    [Space()]
    [Header("Is This feature not Aviable yet?")]
    [SerializeField] private bool bFeatureNotAviable;

    [Header("Buttons you want to Hide:")]
    [SerializeField] private GameObject[] _ButtonsToHideArray;
    bool buttonsAreShown = true;
    //General local variables
    UIAnimationComponent _UIAnimationComponent;
    HUDManagerScript _HUDManagerScrit;
    ARObjectManager _ARObjectManagerScript;

    //Button local variables
    Sprite _currentButtonImage;
    String _currentButtonText;
    bool bIsSelected;
    bool bButtonIsHolding;
    float currentHoldTime;

    //PopUpLocal variables
    int currentContentOnScreen;

    void Start()
    {
        InitUIBehaviourComponent();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIBehaviourComponent();
        UpdateMobileState();
    }

    ///////////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////////////
    public void InitUIBehaviourComponent()
    {
        if(this.gameObject.GetComponent<UIAnimationComponent>())
        {
            _UIAnimationComponent = this.gameObject.GetComponent<UIAnimationComponent>();
        }

        switch (UI_Component)
        {
            case E_UIComponents.Button: InitButton();  break;
            case E_UIComponents.Dropdown: InitDropdown(); break;
            case E_UIComponents.Slider: InitSlider(); break;
            case E_UIComponents.Text: InitText(); break;
            case E_UIComponents.TextBox: InitTextBox(); break;
            case E_UIComponents.DefaultPopUp: InitDefaultPopUp(); break;
            case E_UIComponents.Toggle: InitToggle();  break;
        }

        _HUDManagerScrit = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();

        if(GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManagerScript = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }
            
    }

    void InitButton()
    {
        myButton = GetComponent<Button>();

        switch(buttonType)
        {
            case E_ButtonType.ImageButton:  GetImageButtonComponents(); break;
            case E_ButtonType.TextButton: break;
            case E_ButtonType.TextButtonWithoutBackground: break;
            case E_ButtonType.SpriteButton: break;
        }

        if(St_ButtonBehaviourData.bHaveTutorialPopUp)
        {
            if(GameObject.FindGameObjectWithTag("DefaultPopUp"))
            {
                St_TutorialPopUpData.defaultPopUpRef = GameObject.FindGameObjectWithTag("DefaultPopUp");
            }
            else
            {
                Debug.LogError("UIBehaviourComponent -> No s'ha trobat cap objecte amb el tag DefaultPopUp (Alvaro)");
            }           
        }
        if(bFeatureNotAviable)
        {
            InitNotAviableFeatureBehaviour();
        }
        else
        {
            InitPermiumBehaviour();
        }
    }
    void InitDropdown()
    {
        myDropdown = GetComponent<TMP_Dropdown>();

        if (!St_DropdownBehaviourData.bOptionsManually && !string.IsNullOrEmpty(St_DropdownBehaviourData.enumTypeName))
        {
            Type enumType = Type.GetType(St_DropdownBehaviourData.enumTypeName);
            
            if (enumType != null && enumType.IsEnum)
            {
                PopulateDropdownFromEnum(enumType);
            }
            else
            {
                Debug.LogError("ENUM-> El tipo especificado no es una enum o no se encuentra: " + St_DropdownBehaviourData.enumTypeName + "   (Alvaro)");
            }
        }

        myDropdown.value = St_DropdownBehaviourData.defaultValue;
    }
    void InitSlider()
    {
        mySlider = GetComponent<Slider>();
        //UpdateSliderValue(0);
        mySlider.onValueChanged.AddListener(UpdateSliderValue);
    }
    void InitText()
    {
        myText = GetComponent<TMP_Text>();
        RemeberMeBehaviour();

    }
    void InitTextBox()
    {
        myInputField = GetComponent<TMP_InputField>();

        myInputField.onEndEdit.AddListener(CheckContent);

        if (St_InputFieldBehaviourData.bRemeberMe)
        {
            if (PlayerPrefs.HasKey(St_InputFieldBehaviourData.playerPrefName))
            {
                myInputField.text = PlayerPrefs.GetString(St_InputFieldBehaviourData.playerPrefName);
            }
        }

        if(inputFieldType == E_InputFieldType.Password)
        {
            myInputField.contentType = TMP_InputField.ContentType.Password;
            myInputField.ForceLabelUpdate();

            if (St_InputFieldBehaviourData._eyeGameObj.GetComponent<Button>())
            {
                myButton = St_InputFieldBehaviourData._eyeGameObj.GetComponent<Button>();
                myButton.onClick.AddListener(OnClickChangeInputFieldContentType);
                myButton.image.sprite = St_InputFieldBehaviourData._eyeClosed;
                myButton.image.color = St_InputFieldBehaviourData._eyeClosedColor;
            }
        }
           
    }

    void InitDefaultPopUp()
    {
        myPanel = this.gameObject;

        foreach(Transform child0 in this.gameObject.transform)
        {
            foreach (Transform child1 in child0.gameObject.transform)
            {
                foreach (Transform child2 in child1.gameObject.transform)
                {
                
                }
            }
        }
    }

    void InitToggle()
    {
        myButton = this.gameObject.GetComponent<Button>();

        myButton.onClick.AddListener(ToggleBehaviour);

        if(St_ToggleBehaviourScript.bDependsOnPlayerPref)
        {
            if (!PlayerPrefs.HasKey(St_ToggleBehaviourScript._PlayerPref))
            {
                PlayerPrefs.SetInt(St_ToggleBehaviourScript._PlayerPref, 0);
                PlayerPrefs.Save();
                myButton.image.sprite = St_ToggleBehaviourScript._UnActiveToggle;
                myButton.image.color = St_ToggleBehaviourScript._OnActiveToggleColor;
            }
            else
            {
                int mobileMode = PlayerPrefs.GetInt(St_ToggleBehaviourScript._PlayerPref);

                if (mobileMode == 0)
                {
                    myButton.image.sprite = St_ToggleBehaviourScript._UnActiveToggle;
                    myButton.image.color = St_ToggleBehaviourScript._OnActiveToggleColor;
                    St_ToggleBehaviourScript.bCurrentToggleState = false;
                }
                else if (mobileMode == 1)
                {
                    myButton.image.sprite = St_ToggleBehaviourScript._OnActiveToggle;
                    myButton.image.color = St_ToggleBehaviourScript._UnActiveToggleColor;
                    St_ToggleBehaviourScript.bCurrentToggleState = true;
                }
            }
        }
        else
        {
            if(St_ToggleBehaviourScript.bCurrentToggleState)
            {
                myButton.image.sprite = St_ToggleBehaviourScript._OnActiveToggle;
                myButton.image.color = St_ToggleBehaviourScript._UnActiveToggleColor;
            }
            else
            {
                myButton.image.sprite = St_ToggleBehaviourScript._UnActiveToggle;
                myButton.image.color = St_ToggleBehaviourScript._OnActiveToggleColor;
            }
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// UPDATE /////////////////////////////////////////////////////////////////

    void UpdateUIBehaviourComponent()
    {
        switch (UI_Component)
        {
            case E_UIComponents.Button: HoldBehaviour(); break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// GETTERS /////////////////////////////////////////////////////////////////

    public St_ButtonBehaviourStruct GetButtonBehaviourData()
    {
        return St_ButtonBehaviourData;
    }

    public bool GetButtonIsSelectedState()
    {
        return bIsSelected;
    }

    public E_UIComponents GetUIComponentType()
    {
        return UI_Component;
    }

    public St_SliderBehaviourStruct GetSliderData()
    {
        return St_SliderBehaviourData;
    }

    public St_InputFieldBehaviourStruct GetInputFieldBehaviour()
    {
        return St_InputFieldBehaviourData;
    }

    public bool GetToggleCurrentState()
    {
        return St_ToggleBehaviourScript.bCurrentToggleState;
    }

    public Sprite GetSpriteFromImageButton()
    {
        return St_ButtonBehaviourData._buttonImage.sprite;
    }
    public GameObject GetTextFromImageButton()
    {
        return St_ButtonBehaviourData._buttonText.gameObject;
    }

    public Animator GetAnimatorFromPopUp()
    {
        return St_DefaultPopUpBehaviourData._PopUpAnimator;
    }

    public E_ButtonType GetButtonType()
    {
        return buttonType;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// SETTERS /////////////////////////////////////////////////////////////////
    void SetButtonShowTextOnImageButton(bool show)
    {
        if (St_ButtonBehaviourData._buttonText != null)
        {
            St_ButtonBehaviourData._buttonText.gameObject.SetActive(show);
        }
        else
        {
            Debug.LogError("IMAGE BUTTON -> La variable St_ButtonBehaviourData._buttonText = null!   (Alvaro)");
        }
    }

    public void SetButtonInMobileMode(bool active)
    {
        if (active)
        {
            St_ButtonBehaviourData.bButtonInMobileMode = active;
           // St_ButtonBehaviourData.bShowText = !active;
            //SetButtonShowTextOnImageButton(!active);
        }
        else
        {
            St_ButtonBehaviourData.bButtonInMobileMode = active;
            //St_ButtonBehaviourData.bShowText = !active;
            //SetButtonShowTextOnImageButton(!active);
        }
    }
    public void SetButtonSelectedState(bool isSelected)
    {
        bIsSelected = isSelected;
    }

    public void SetPopUpTitleVariable(string text)
    {
        string title = LocalizationManager.Localize(text);
        St_DefaultPopUpBehaviourData.popUpTitle = title;
    }

    public void SetPopUpContentVariable(St_BasicPopUpData[] content)
    {
        St_DefaultPopUpBehaviourData.popUpContentList = content;  
    }

    public void SetPopUpCurrentTitle()
    {

        if(St_DefaultPopUpBehaviourData._PopUpTitle != null)
        {
            St_DefaultPopUpBehaviourData._PopUpTitle.text = St_DefaultPopUpBehaviourData.popUpTitle;
        }
        else
        {
            Debug.LogError("UIBehaviourComponent -> Error en la carrega del InitDefaultPopUp()   (Alvaro)");
        }
    }
    public void SetPopUpCurrentContent(int currentIndex)
    {
        if (St_DefaultPopUpBehaviourData.popUpContentList[currentIndex] != null)
        {
            string descript = LocalizationManager.Localize(St_DefaultPopUpBehaviourData.popUpContentList[currentIndex].popUpDescription);
            St_DefaultPopUpBehaviourData._PopUpDescription.text = descript;
            St_DefaultPopUpBehaviourData._PopUpImage.sprite = St_DefaultPopUpBehaviourData.popUpContentList[currentIndex].popUpImage;
        }
        else
        {
            Debug.LogError("UIBehaviourComponent -> OUT OF RANGE...   (Alvaro)");
        }
    }

    public void SetPopUpCurrentAnimatorController(RuntimeAnimatorController animCont, UIBehaviourComponent defaultPopUpReference)
    {
        Animator animator = defaultPopUpReference.GetAnimatorFromPopUp();

        if (animator != null)
        {
            animator.runtimeAnimatorController = animCont;
        }
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// SLIDER BEHAVIOUR /////////////////////////////////////////////////////////////////

    private void UpdateSliderValue(float sliderValue)
    {
        if (float.IsNaN(sliderValue)) sliderValue = 0;
        float realValue = sliderValue * (St_SliderBehaviourData.sliderMax - St_SliderBehaviourData.sliderMin) + St_SliderBehaviourData.sliderMin;
        St_SliderBehaviourData.sliderResult = Mathf.Round(realValue * 100f) / 100f;
    }

    public void UpdateSliderValue()
    {
        float normalizedValue =
        (GetSliderData().sliderResult - GetSliderData().sliderMin)
        / (GetSliderData().sliderMax - GetSliderData().sliderMin);

        if (float.IsNaN(normalizedValue)) normalizedValue = 0;

        if (mySlider != null)
            mySlider.SetValueWithoutNotify(Mathf.Round(normalizedValue * 100f) / 100f);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// DROPDOWN BEHAVIOUR ///////////////////////////////////////////////////////////////
    void PopulateDropdownFromEnum(Type enumType)
    {
        myDropdown.ClearOptions();

        var enumValues = Enum.GetValues(enumType).Cast<Enum>();
        var options = enumValues.Select(e => e.ToString()).ToList();

        myDropdown.AddOptions(options);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// INPUT TEXT BEHAVIOUR ///////////////////////////////////////////////////////////////


    void RemeberMeBehaviour()
    {
        if(St_InputFieldBehaviourData.bRemeberMe)
        {
            if (PlayerPrefs.HasKey(St_InputFieldBehaviourData.playerPrefName))
            {
               myInputField.text = PlayerPrefs.GetString(St_InputFieldBehaviourData.playerPrefName);
               
            }
            else
            {
                Debug.LogError("UIBehaviourComponent -> InputField -> No existeix la etiqueta del Player pref... (Alvaro)");
            }
        }
    }


    public void CheckContent(string inputText)
    {
        switch (inputFieldType)
        {
            case E_InputFieldType.Default: break;
            case E_InputFieldType.IntInRange: CheckIfItsInTheRange(inputText); break;
            case E_InputFieldType.Email: CheckIfItsAnEmail(inputText); break;
            case E_InputFieldType.MaxNumberLenght: CheckIfItsUnderOfMaxLenghtValue(inputText); break;  
        } 
    }

    void CheckIfItsAnEmail(string txt)
    {
        Regex regex = new Regex(validEmailPattern);

        if (!regex.IsMatch(txt))
        {
            _UIAnimationComponent.AnimateInputField();
            myInputField.text = "";
        }

    }

    void CheckIfItsInTheRange(string txt)
    {
        if (float.TryParse(txt, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            if (result < St_InputFieldBehaviourData._min || result > St_InputFieldBehaviourData._max)
            {
                _UIAnimationComponent.AnimateInputField();
                myInputField.text = "";
            }
        }
        else
        {
            _UIAnimationComponent.AnimateInputField();
            myInputField.text = "";
        }
    }

    void CheckIfItsUnderOfMaxLenghtValue(string txt)
    {
        int result;
        string stringResult;

        if (int.TryParse(txt, out result))
        {
            stringResult = result.ToString();

            if (stringResult.Length != St_InputFieldBehaviourData._maxLenght)
            {
                _UIAnimationComponent.AnimateInputField();
                myInputField.text = "";
            }
        }
        else
        {
            _UIAnimationComponent.AnimateInputField();
            myInputField.text = "";
        }
    }

    void OnClickChangeInputFieldContentType()
    {
        if (myInputField.contentType == TMP_InputField.ContentType.Password)
        {
            myInputField.contentType = TMP_InputField.ContentType.Standard;
            myInputField.ForceLabelUpdate();
            myButton.image.sprite = St_InputFieldBehaviourData._eyeOpen;
            myButton.image.color = St_InputFieldBehaviourData._eyeOpenColor;
        }
        else if(myInputField.contentType == TMP_InputField.ContentType.Standard)
        {
            myInputField.contentType = TMP_InputField.ContentType.Password;
            myInputField.ForceLabelUpdate();
            myButton.image.sprite = St_InputFieldBehaviourData._eyeClosed;
            myButton.image.color = St_InputFieldBehaviourData._eyeClosedColor;
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// BUTTON BEHAVIOUR ///////////////////////////////////////////////////////////////


    void GetImageButtonComponents()
    {
        foreach(Transform child in this.gameObject.transform)
        {
            foreach (Transform grandChild in child.gameObject.transform)
            {
                if(grandChild.GetComponent<TMP_Text>())
                {
                    St_ButtonBehaviourData._buttonText = grandChild.GetComponent<TMP_Text>();
                    _currentButtonText = grandChild.GetComponent<TMP_Text>().text;

                    //Check if show or not Text under the image
                    SetButtonShowTextOnImageButton(St_ButtonBehaviourData.bShowText);
                }
                else if(grandChild.GetComponent<Image>())
                {
                    St_ButtonBehaviourData._buttonImage = grandChild.GetComponent<Image>();
                    _currentButtonImage = grandChild.GetComponent<Image>().sprite;
                }
            }
        }
    }

    public void ChangeImageOnClick()
    {
        if (St_ButtonBehaviourData.bChangeImage)
        {
            if (St_ButtonBehaviourData._buttonImage != null && St_ButtonBehaviourData._buttonImageChanged != null)
            {
                if (bIsSelected)
                {
                    St_ButtonBehaviourData._buttonImage.sprite = _currentButtonImage;
                    bIsSelected = false;
                }
                else
                {
                    St_ButtonBehaviourData._buttonImage.sprite = St_ButtonBehaviourData._buttonImageChanged;
                    bIsSelected = true;
                }
            }
            else
            {
                Debug.LogError("IMAGE BUTTON -> La variable St_ButtonBehaviourData._buttonImage = null o  St_ButtonBehaviourData._buttonImageChanged = null  (Alvaro)");
            }
        }
        else
        {
            if (bIsSelected)
            {
                bIsSelected = false;
               
            }
            else
            {
                bIsSelected = true;
            }
        }

        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        bButtonIsHolding = true;
        currentHoldTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bButtonIsHolding = false;
        currentHoldTime = 0f;
    }

    void HoldBehaviour()
    {
        if (PlayerPrefs.GetInt("HelpMode", 0) == 1)
        {
            if (St_ButtonBehaviourData.bHaveTutorialPopUp)
            {
                if (St_TutorialPopUpData.onClickDuration != 0)
                {
                    if (bButtonIsHolding)
                    {
                        currentHoldTime += Time.deltaTime;

                        if (currentHoldTime >= St_TutorialPopUpData.onClickDuration)
                        {
                            bButtonIsHolding = false;
                            OnHoldComplete();
                        }
                    }
                }
                else
                {
                    Debug.LogError("UIBehaviourComponent -> El temps de click es 0 en l'objecte " + this.gameObject.name + "   (Alvaro)");
                }
            }
        }
        
        

    }
    private void OnHoldComplete()
    {
        CreateDefaultPopUpBehaviour();
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// PopUp BEHAVIOUR ///////////////////////////////////////////////////////////////

    void CreateDefaultPopUpBehaviour()
    {
        //Pas 0 -> referenciem la referencia jaj, que sino escriure tot allo tota la estona, un conyazo.
        UIBehaviourComponent defaultPopUpReference = St_TutorialPopUpData.defaultPopUpRef.GetComponent<UIBehaviourComponent>();

        //Pas 1 -> omplo les variables del pop Up amb el titol i la llista del contingut
        defaultPopUpReference.SetPopUpTitleVariable(St_TutorialPopUpData.popUpTitle);
        defaultPopUpReference.SetPopUpContentVariable(St_TutorialPopUpData.popUpContentList);

        if(St_TutorialPopUpData.bUseAnimation)
        {
            defaultPopUpReference.SetPopUpCurrentAnimatorController(St_TutorialPopUpData._AnimCont, defaultPopUpReference);
        }
        

        if (defaultPopUpReference.St_DefaultPopUpBehaviourData.popUpContentList.Length != 0)
        {
            //Pas 2 -> Setegem el Titol (que no cambia)
            defaultPopUpReference.SetPopUpCurrentTitle();
            defaultPopUpReference.SetPopUpCurrentContent(0);
            currentContentOnScreen = 0;

            //Pas 3 -> Si nomes tenim una llista de contingut lenght = 1, no necesitem para nada les fletxes del popUp, per tant les treiem.
            if (defaultPopUpReference.St_DefaultPopUpBehaviourData.popUpContentList.Length == 1)
            {
                foreach(Button button in defaultPopUpReference.St_DefaultPopUpBehaviourData._PopUpButtonsList)
                {
                    button.gameObject.SetActive(false);
                }  
            }

            //Pas 4 -> Creem el pop up
            defaultPopUpReference.GetComponent<UIAnimationComponent>().ShowPannel();
        }
        else
        {
            Debug.LogError("UIBehaviourComponent -> Sense contingut no pot haver-hi PopUp    (Alvaro)");
        }  
    }


    public void ChangePopUpContent(bool bGoRight)
    {
        if (St_DefaultPopUpBehaviourData.popUpContentList.Length > 1)
        {
            if(bGoRight)
            {
                if((currentContentOnScreen + 1) > (St_DefaultPopUpBehaviourData.popUpContentList.Length -1))
                {
                    currentContentOnScreen = 0;
                }
                else
                {
                    currentContentOnScreen += 1;
                }

                SetPopUpCurrentContent(currentContentOnScreen);
            }
            else
            {
                if ((currentContentOnScreen - 1) < 0)
                {
                    currentContentOnScreen = St_DefaultPopUpBehaviourData.popUpContentList.Length-1;
                }
                else
                {
                    currentContentOnScreen -= 1;
                }

                SetPopUpCurrentContent(currentContentOnScreen);
            }
        }


    }




    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    //////////////////////////////////////////////////////// TOGGLE BEHAVIOUR /////////////////////////////////////////////////////////
    
    public void ToggleBehaviour()
    {
        if (St_ToggleBehaviourScript.bCurrentToggleState)
        {
            OnActiveToggle();
        }
        else
        {
            OnDesActiveToggle();
        }
    }

    public void OnActiveToggle()
    {
        if (PlayerPrefs.HasKey(St_ToggleBehaviourScript._PlayerPref))
        {
            PlayerPrefs.SetInt(St_ToggleBehaviourScript._PlayerPref, 0);
            PlayerPrefs.Save();
        }

        myButton.image.sprite = St_ToggleBehaviourScript._UnActiveToggle;
        myButton.image.color = St_ToggleBehaviourScript._OnActiveToggleColor;

        St_ToggleBehaviourScript.bCurrentToggleState = false;
    }

    public void OnDesActiveToggle()
    {
        if (PlayerPrefs.HasKey(St_ToggleBehaviourScript._PlayerPref))
        {
            PlayerPrefs.SetInt(St_ToggleBehaviourScript._PlayerPref, 1);
            PlayerPrefs.Save();
        }

        myButton.image.sprite = St_ToggleBehaviourScript._OnActiveToggle;
        myButton.image.color = St_ToggleBehaviourScript._UnActiveToggleColor;

        St_ToggleBehaviourScript.bCurrentToggleState = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ///////////////////////////////////////////////////////////// PREMIUM FUNCTIONS ////////////////////////////////////////////////////
    
    void InitPermiumBehaviour()
    {
        if (bPremiumBehaviour)
        {
            if (_UIPremiumBehaviour.bOnlyForPremiumUsers)
            {
                if (PlayerPrefs.GetString("PremiumState") == "0")
                {
                    myButton.interactable = false;
                    _UIPremiumBehaviour._GetPremiumButton.gameObject.SetActive(true);
                    
                    if(_UIPremiumBehaviour._GetPremiumButton != null)
                    {
                        _UIPremiumBehaviour._GetPremiumButton.onClick.AddListener(OnClickShowGetPremiumPopUp);
                    }

                    if (buttonType == E_ButtonType.ImageButton)
                    {
                        St_ButtonBehaviourData._buttonText.color = _UIPremiumBehaviour._TextDesactiveColor;
                        St_ButtonBehaviourData._buttonImage.color = _UIPremiumBehaviour._ImageDesactiveColor;
                    }
                    
                }
                else
                {
                    _UIPremiumBehaviour._GetPremiumButton.gameObject.SetActive(false);
                }
            }
            else
            {
                _UIPremiumBehaviour._GetPremiumButton.gameObject.SetActive(false);
            }
        }
        else
        {
           // _UIPremiumBehaviour._GetPremiumButton.gameObject.SetActive(false);
        }
    }

    void OnClickShowGetPremiumPopUp()
    {
        if(GameObject.FindGameObjectWithTag("GetPremiumPopUp"))
        {

            GameObject parentObject = GameObject.FindGameObjectWithTag("GetPremiumPopUp");
            parentObject.GetComponent<UIAnimationComponent>().ShowPannel();
        }
    }


    public void FindAndChangeImage(string parentTag)
    {
        GameObject parentObject = GameObject.FindWithTag(parentTag);

        if (parentObject != null)
        {
            Transform result = RecursiveSearch(parentObject.transform, "ImageToChange");

            if (result != null)
            {
                Image img = result.GetComponent<Image>();
                if (img != null)
                {
                    Debug.Log("S'ha trobat l'Image a: " + result.name);
                    // Aquí ja pots manipular l'imatge (ex: img.sprite = nouSprite;)
                }
            }
        }
    }

    // Funció recursiva per recórrer tota la jerarquia
    private Transform RecursiveSearch(Transform parent, string nameToFind)
    {
        foreach (Transform child in parent)
        {
            if (child.name == nameToFind)
            {
                return child;
            }

            // Cridem a la funció per als fills d'aquest fill
            Transform found = RecursiveSearch(child, nameToFind);
            if (found != null) return found;
        }
        return null;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////// NOT AVIABLE ////////////////////////////////////////////////////////////////

    void InitNotAviableFeatureBehaviour()
    {
        myButton.interactable = false;

        if (buttonType == E_ButtonType.ImageButton)
        {
            St_ButtonBehaviourData._buttonText.color = _UIPremiumBehaviour._TextDesactiveColor;
            St_ButtonBehaviourData._buttonImage.color = _UIPremiumBehaviour._ImageDesactiveColor;
        }

    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /////////////////////////////////////////////////// MOBILE MODE /////////////////////////////////////////////////////////////////////
    
    void UpdateMobileState()
    {
        if(PlayerPrefs.GetInt("MobileMode") == 1)
        {
            SetButtonInMobileMode(true);
        }
        else if(PlayerPrefs.GetInt("MobileMode") == 0)
        {
            SetButtonInMobileMode(false);

        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    


    //////////////////////////////////////////////// HIDE BUTTONS MODE //////////////////////////////////////////////////////////////////
    

     void OnClickShowButtonsOnHideModeArray()
     {
        if(_ButtonsToHideArray.Length != 0)
        {
            foreach (GameObject button in _ButtonsToHideArray)
            {
                if (button.GetComponent<UIAnimationComponent>())
                {
                    button.GetComponent<UIAnimationComponent>().ShowPannel();
                }
            }
        }
        
     }
     void OnClickHideButtonsOnHideModeArray()
     {
        if (_ButtonsToHideArray.Length != 0)
        {
            foreach (GameObject button in _ButtonsToHideArray)
            {
                if (button.GetComponent<UIAnimationComponent>())
                {
                    UIAnimationComponent _UiAnimComp = button.GetComponent<UIAnimationComponent>();

                    _UiAnimComp.HidePannel();

                    if (_UiAnimComp.GetButtonOnSelectData().DeselectWhenYouClick)
                    {
                        if (GetButtonBehaviourData().bButtonInMobileMode) //MOBILE
                        {
                            _UiAnimComp.DeselectAnimOnMobileMode();
                        }
                        else //NORMAL
                        {
                            _UiAnimComp.DeselectAnim();
                        }
                    }

                    _UiAnimComp.SetColorOfButtonDependingOnSelectState(false);
                }
            }

            _ARObjectManagerScript.SetCurrentARObjectState("DefaultState");
        }
     }

    public void OnClickFlipFlopHideButtonsBehaviour()
    {
        if (buttonsAreShown)
        {
            OnClickHideButtonsOnHideModeArray();
            buttonsAreShown = false;
        }
        else
        {
            OnClickShowButtonsOnHideModeArray();
            buttonsAreShown = true;
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





}
