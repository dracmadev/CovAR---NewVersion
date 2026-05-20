using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UIAnimationComponent : MonoBehaviour
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
    [SerializeField] private TMP_InputField myInputField;
    [Header("Component reference: (Automatic)")]
    [SerializeField] private TMP_Text myText;


    [Header("From wich side will appear?")]
    [SerializeField] private E_UIAnimationType animationType;

    //UI Component Struct
    [SerializeField] private St_UIComponentAnimation UI_ComponentStruct;
    //UI Button On Click
    [SerializeField] private E_ButtonOnClickAnimType UI_ButtonOnClickAnimType;
    [SerializeField] private St_ButtonOnClickAnimation UI_ButtonOnClickDefaultStruct;
    [SerializeField] private St_ButtonGoSidesAnimation UI_ButtonOnClickGoSidesStruct; 
    [SerializeField] private St_ButtonSwipeAnimation UI_ButtonOnClickSwipeStruct;
    [SerializeField] private St_ButtonOnSelectData UI_ButtonOnSelectedData;
   // UI Input Field
   [SerializeField] private St_InputFieldAnimationStruct UI_InputFieldStruct;

    //Variables locals
    private RectTransform UI_ComponentRect;
    private Vector2 UI_ComponentHiddenPosition;
    private Vector2 UI_ComponentHiddenPositionOnMobileMode;
    private Vector2 UI_ComponentVisiblePosition;
    private Vector2 UI_ComponentVisiblePositionOnMobileMode;
    private Vector2 UI_ComponentVisiblePositionOnSelectedState;
    private Vector2 UI_ComponentVisiblePositionOnSelectedStateOnMobileMode;
    private Vector3 UI_ComponentOriginalScale;
    private Vector3 UI_ComponentBaseScale;
    private Quaternion UI_ComponentOriginalRotation;
    bool bIsOnScreen;
    UIBehaviourComponent _UIBehaviourComponent;
    HUDManagerScript _HUDManagerScrit;

    // Update is called once per frame
    void Start()
    {
        InitUIComponent();
    }

    void Update()
    {
        
    }

    /////////////////////////////////////////////////////////////////// INIT /////////////////////////////////////////////////////////////////
    public void InitUIComponent()
    {
        if(this.gameObject.GetComponent<UIBehaviourComponent>())
        {
            _UIBehaviourComponent = this.gameObject.GetComponent<UIBehaviourComponent>();
        }
       
        switch (UI_Component)
        {
            case E_UIComponents.Panel: InitPanelComponent(); break;
            case E_UIComponents.Button: InitButtonComponent(); break;
            case E_UIComponents.Slider: InitSliderComponent(); break;
            case E_UIComponents.TextBox: InitTextBoxComponent(); break;
            case E_UIComponents.Text: InitTextComponent(); break;
        }

        _HUDManagerScrit = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();

        InitTypeAnimation();
    }

    void InitPanelComponent()
    {
        myPanel = this.transform.gameObject;
        UI_ComponentBaseScale = transform.localScale;
        UI_ComponentOriginalRotation = this.transform.rotation;
    }
    void InitButtonComponent()
    {
        //Debug.Log("INIT BUTTON");
        myButton = GetComponent<Button>();
        UI_ComponentBaseScale = Vector3.one;
        //UI_ComponentBaseScale = transform.localScale;
        UI_ComponentOriginalRotation = this.transform.rotation;
        _UIBehaviourComponent.SetButtonSelectedState(UI_ButtonOnSelectedData.bStartSelected);

      
        if (UI_ButtonOnSelectedData.bActiveSelectedState)
        {
            //La idea es que esta aixi perque tots els botons comencin desactivats (ja sigui per posicio o per color)
            //Encara aixi si forcem que un comenci activat, la funció de desselect no la fara (internament) i podrem forçar que un comenci activat
            //(Alvaro)
            DesSelectButton();
        }

        //myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClickBehaviour);
    }
    void InitSliderComponent()
    {
        mySlider = GetComponent<Slider>();
    }
    void InitTextBoxComponent()
    {
        myInputField = GetComponent<TMP_InputField>();
    }

    void InitTextComponent()
    {
        myText = GetComponent<TMP_Text>();
    }

    void InitTypeAnimation()
    {
        UI_ComponentRect = this.gameObject.GetComponent<RectTransform>();

        UI_ComponentVisiblePosition = UI_ComponentRect.anchoredPosition;

        switch (animationType)
        {
            case E_UIAnimationType.Right: // DRETA
                UI_ComponentHiddenPosition = UI_ComponentVisiblePosition + new Vector2(UI_ComponentStruct.hiddenPositionOnXAxis, 0);

                UI_ComponentVisiblePositionOnMobileMode = UI_ComponentVisiblePosition - new Vector2(UI_ComponentStruct.offsetPositionOnXAxisForMobileMode, 0);
                UI_ComponentHiddenPositionOnMobileMode = UI_ComponentVisiblePositionOnMobileMode + new Vector2((UI_ComponentStruct.hiddenPositionOnXAxis + UI_ComponentStruct.offsetPositionOnXAxisForMobileMode), 0);

                UI_ComponentVisiblePositionOnSelectedState = UI_ComponentVisiblePosition - new Vector2(UI_ButtonOnSelectedData.offsetPositionOnXAxisForSelectedMode, 0);
                UI_ComponentVisiblePositionOnSelectedStateOnMobileMode = UI_ComponentVisiblePositionOnMobileMode - new Vector2(UI_ButtonOnSelectedData.offsetPositionOnXAxisForSelectedMode, 0);
                break;

            case E_UIAnimationType.Left: // ESQUERRA
                UI_ComponentHiddenPosition = UI_ComponentVisiblePosition - new Vector2(UI_ComponentStruct.hiddenPositionOnXAxis, 0);

                UI_ComponentVisiblePositionOnMobileMode = UI_ComponentVisiblePosition + new Vector2(UI_ComponentStruct.offsetPositionOnXAxisForMobileMode, 0);
                UI_ComponentHiddenPositionOnMobileMode = UI_ComponentVisiblePositionOnMobileMode - new Vector2((UI_ComponentStruct.hiddenPositionOnXAxis - UI_ComponentStruct.offsetPositionOnXAxisForMobileMode), 0);

                UI_ComponentVisiblePositionOnSelectedState = UI_ComponentVisiblePosition + new Vector2(UI_ButtonOnSelectedData.offsetPositionOnXAxisForSelectedMode, 0);
                UI_ComponentVisiblePositionOnSelectedStateOnMobileMode = UI_ComponentVisiblePositionOnMobileMode + new Vector2(UI_ButtonOnSelectedData.offsetPositionOnXAxisForSelectedMode, 0);    
                break;

            case E_UIAnimationType.Up: // AMUNT
                 UI_ComponentHiddenPosition = UI_ComponentVisiblePosition + new Vector2(0, UI_ComponentStruct.hiddenPositionOnYAxis);
               
                break;

            case E_UIAnimationType.Down: // AVALL
                UI_ComponentHiddenPosition = UI_ComponentVisiblePosition - new Vector2(0, UI_ComponentStruct.hiddenPositionOnYAxis);

                UI_ComponentVisiblePositionOnMobileMode = UI_ComponentVisiblePosition + new Vector2(0, UI_ComponentStruct.offsetPositionOnYAxisForMobileMode);
                UI_ComponentHiddenPositionOnMobileMode = UI_ComponentVisiblePositionOnMobileMode * new Vector2(0, (UI_ComponentStruct.hiddenPositionOnXAxis + UI_ComponentStruct.offsetPositionOnYAxisForMobileMode));

                break;

            case E_UIAnimationType.Grow: // GROW
                UI_ComponentOriginalScale = this.transform.localScale;
                UI_ComponentBaseScale = this.transform.localScale;
                transform.localScale = Vector3.zero;
                break;
            case E_UIAnimationType.Appear: // APPEAR
                if (TryGetComponent(out CanvasGroup cg))
                {
                    UI_ComponentOriginalScale = transform.localScale;
                    UI_ComponentBaseScale = transform.localScale;
                    transform.localScale = UI_ComponentOriginalScale * 0.98f; 
                    cg.alpha = 0;
                    this.gameObject.SetActive(false);
                }
                else Debug.LogError("THIS UI COMPONENT [ " + gameObject.name + " ] DON'T HAVE A CANVAS GROUP!!");
                break;

        }

        if (animationType != E_UIAnimationType.None && animationType != E_UIAnimationType.Grow && animationType != E_UIAnimationType.Appear)
        {
            if (!UI_ComponentStruct.bDontStartHide)
            {
                UI_ComponentRect.anchoredPosition = UI_ComponentHiddenPosition;
            }
        }
        
    }
   

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// SETTERS ////////////////////////////////////////////////////////////////////////////
    public void SetOnScreenState(bool state)
    {
        bIsOnScreen = state;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// GETTERS ////////////////////////////////////////////////////////////////////////////

    public E_UIComponents GetUIComponentType()
    {
        return UI_Component;
    }

    public bool GetSelectedState()
    {
        return UI_ButtonOnSelectedData.bActiveSelectedState;
    }

    public E_UIAnimationType GetAnimationType()
    {
        return animationType;
    }

    public Sprite GetOnSelectedSprite()
    {
        return UI_ButtonOnSelectedData._OnSelectedSprite;
    }

    public Sprite GetUnSelectedSprite()
    {
        return UI_ButtonOnSelectedData._UnSelectedSprite;
    }

    public St_ButtonOnSelectData GetButtonOnSelectData()
    {
        return UI_ButtonOnSelectedData;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// ANIMATIONS PANEL /////////////////////////////////////////////////////////////////////////////////////
    void AppearGrowing() //AppearChangingScale
    {
        transform.DOScale(Vector3.one, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.appearEase);
    }

    void AppearWithAplhaAnim()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();

        gameObject.SetActive(true); //Ens assegurem que està actiu per animar

        transform.localScale = UI_ComponentOriginalScale * 0.98f;
        cg.alpha = 0;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(UI_ComponentOriginalScale, UI_ComponentStruct.animationDuration)
            .SetEase(UI_ComponentStruct.appearEase));
        seq.Join(cg.DOFade(1f, UI_ComponentStruct.animationDuration)
            .SetEase(UI_ComponentStruct.appearEase));
    }

    void DisappearWithAplhaAnim()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(UI_ComponentOriginalScale * 0.98f, UI_ComponentStruct.animationDuration)
            .SetEase(UI_ComponentStruct.disapearEase));
        seq.Join(cg.DOFade(0f, UI_ComponentStruct.animationDuration)
            .SetEase(UI_ComponentStruct.disapearEase));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false); // Set Active false
        });
    }




    void DisappearGrowing() //DisappearChangingScale
    {
        transform.DOScale(Vector3.zero, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.disapearEase)
            .OnComplete(() => gameObject.transform.localScale = Vector3.zero);
    }

    void AppearFromSides()
    {
        //UI_ComponentRect.DOAnchorPos(UI_ComponentVisiblePosition, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.appearEase);
        StartCoroutine(AppearFromSidesCoroutine());
    }

    IEnumerator AppearFromSidesCoroutine()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentVisiblePosition, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.appearEase);
        yield return new WaitForSeconds(UI_ButtonOnClickGoSidesStruct.animationDuration + 0.5f);

        if(UI_Component == E_UIComponents.Button && UI_ButtonOnSelectedData.bStartSelected && UI_ButtonOnSelectedData.bActiveSelectedState)
        {
            SelectedAnim();
        }
    }

    void DisappearFromSides()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentHiddenPosition, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.disapearEase);
    }

    void AppearFromSidesOnMobileMode()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentVisiblePositionOnMobileMode, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.appearEase);
    }

    void DisappearFromSidesOnMobileMode()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentHiddenPositionOnMobileMode, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.disapearEase);
    }
    public void ShowPannel()
    {
        if (!bIsOnScreen)
        {
            switch (animationType)
            {
                case E_UIAnimationType.Up:
                    AppearFromSides();
                    break;
                case E_UIAnimationType.Down:
                    if (_UIBehaviourComponent != null)
                    {
                        if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode)
                        {
                            AppearFromSidesOnMobileMode();
                        }
                        else
                        {
                            AppearFromSides();
                        }
                    }
                    break;
                case E_UIAnimationType.Right:
                    if (_UIBehaviourComponent != null)
                    {
                        if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode)
                        {
                            AppearFromSidesOnMobileMode();
                        }
                        else
                        {
                            AppearFromSides();
                        }
                    }
                    break;
                case E_UIAnimationType.Left:
                    if (_UIBehaviourComponent != null)
                    {
                        if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode)
                        {
                            AppearFromSidesOnMobileMode();
                        }
                        else
                        {
                            AppearFromSides();
                        }
                    }
                    break;
                case E_UIAnimationType.Grow: AppearGrowing(); break;
                case E_UIAnimationType.Appear: AppearWithAplhaAnim();break;
            }
            bIsOnScreen = true;
        }

    }
    public void ShowPannelWithFlipFlop()
    {
        if (!bIsOnScreen)
        {
            ShowPannel();
            bIsOnScreen = true;
        }
        else
        {
            HidePannel();
            bIsOnScreen = false;
        }

    }
    public void HidePannel()
    {
        switch (animationType)
        {
            case E_UIAnimationType.Up: DisappearFromSides(); break;
            case E_UIAnimationType.Down: DisappearFromSides(); break;
            case E_UIAnimationType.Right:
                if (_UIBehaviourComponent != null)
                {
                    if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode)
                    {
                        DisappearFromSidesOnMobileMode();
                    }
                    else
                    {
                        DisappearFromSides();
                    }
                }
                break;
            case E_UIAnimationType.Left:
                if (_UIBehaviourComponent != null)
                {
                    if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode)
                    {
                        DisappearFromSidesOnMobileMode();
                    }
                    else
                    {
                        DisappearFromSides();
                    }
                }
                break; 
            case E_UIAnimationType.Grow: DisappearGrowing(); break;
            case E_UIAnimationType.Appear: DisappearWithAplhaAnim(); break;
        }

        bIsOnScreen = false;

        if (_UIBehaviourComponent != null && _UIBehaviourComponent.GetButtonIsSelectedState())
        {
            _UIBehaviourComponent.ChangeImageOnClick();
        }
    }

    void SelectedAnim()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentVisiblePositionOnSelectedState, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.appearEase);
    }

    public void DeselectAnim()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentVisiblePosition, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.disapearEase);
    }
    void SelectedAnimOnMobileMode()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentVisiblePositionOnSelectedStateOnMobileMode, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.appearEase);
    }

    public void DeselectAnimOnMobileMode()
    {
        UI_ComponentRect.DOAnchorPos(UI_ComponentVisiblePositionOnMobileMode, UI_ComponentStruct.animationDuration).SetEase(UI_ComponentStruct.disapearEase);
    }

    public void ToggleInfoMenu()
    {
        _HUDManagerScrit.showInformationMenu = !_HUDManagerScrit.showInformationMenu;
        _HUDManagerScrit.SetShowInformationMenu(_HUDManagerScrit.showInformationMenu);

        if (_HUDManagerScrit.showInformationMenu)
        {
            ShowPannel();
            bIsOnScreen = true;
        }
        else
        {
            HidePannel();
            bIsOnScreen = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// ANIMATIONS INPUT FIELD /////////////////////////////////////////////////////////////////////////

    public void AnimateInputField()
    {
        this.gameObject.GetComponent<Image>().DOColor(UI_InputFieldStruct.wrongInputFieldColor, 0.1f);

        myInputField.transform.DOShakePosition(UI_InputFieldStruct.animDuration, UI_InputFieldStruct.shakeStrenght, 20, 90, false, true)
            .OnKill(() =>
            {
                this.gameObject.GetComponent<Image>().DOColor(UI_InputFieldStruct.originalInputFieldColor, 0.1f);
            });
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// ANIMATIONS ON CLICK /////////////////////////////////////////////////////////////////////////
    
    void OnClickBehaviour()
    {
        switch (UI_ButtonOnClickAnimType)
        {
            case E_ButtonOnClickAnimType.None: //Debug.Log("OnClick is in NONE..."); break;
            case E_ButtonOnClickAnimType.DefaultOnClick: OnClickDefaultAnim(); break;
            case E_ButtonOnClickAnimType.GoRight: OnClickGoRightAnim(); break;
            case E_ButtonOnClickAnimType.GoLeft: OnClickGoLeftAnim(); break;
            case E_ButtonOnClickAnimType.Swipe: OnClickSwipeBehaviour(); break;
        }

        if (_UIBehaviourComponent != null)
        {
            _UIBehaviourComponent.ChangeImageOnClick();
        }

        if(UI_ButtonOnSelectedData.bActiveSelectedState)
        {
            UpdateSelectedState();
        }


        if (_UIBehaviourComponent != null && _UIBehaviourComponent.GetButtonType() == E_ButtonType.HideUIButton)
        {
            _UIBehaviourComponent.OnClickFlipFlopHideButtonsBehaviour();
        }
    }
    
    void OnClickDefaultAnim()
    {
       // transform.DOKill(true);

        transform.DOScale(UI_ComponentBaseScale * UI_ButtonOnClickDefaultStruct.scaleUpFactor, UI_ButtonOnClickDefaultStruct.animationDuration).SetEase(UI_ButtonOnClickDefaultStruct.easeType).OnComplete(() =>
        {
            transform.DOScale(UI_ComponentBaseScale, UI_ButtonOnClickDefaultStruct.animationDuration).SetEase(UI_ButtonOnClickDefaultStruct.easeType);
        });
    }
    void OnClickGoRightAnim()
    {
        UI_ComponentRect.DOAnchorPosX(UI_ComponentVisiblePosition.x + UI_ButtonOnClickGoSidesStruct.moveAmount, (UI_ButtonOnClickGoSidesStruct.animationDuration / 2))
          .SetEase(Ease.OutQuad)
          .OnComplete(() =>
          {
              UI_ComponentRect.DOAnchorPosX(UI_ComponentVisiblePosition.x, (UI_ButtonOnClickGoSidesStruct.animationDuration/ 2))
                  .SetEase(Ease.InQuad);
          });
    }
    void OnClickGoLeftAnim()
    {
        UI_ComponentRect.DOAnchorPosX(UI_ComponentVisiblePosition.x - UI_ButtonOnClickGoSidesStruct.moveAmount, (UI_ButtonOnClickGoSidesStruct.animationDuration / 2))
          .SetEase(Ease.OutQuad)
          .OnComplete(() =>
          {
              UI_ComponentRect.DOAnchorPosX(UI_ComponentVisiblePosition.x, (UI_ButtonOnClickGoSidesStruct.animationDuration / 2))
                  .SetEase(Ease.InQuad);
          });
    }

    void OnClickSwipeAnim()
    {
        Vector3 flippedScale = transform.localScale;
        flippedScale.x *= -1;
        transform.DOScale(flippedScale, UI_ButtonOnClickSwipeStruct.animationDuration).SetEase(Ease.InOutSine);
    }
    void ReturnOnClickSwipeAnim()
    {
        transform.DOScale(UI_ComponentBaseScale, UI_ButtonOnClickSwipeStruct.animationDuration).SetEase(Ease.InOutSine);
    }

    void OnClickSwipeBehaviour()
    {
       
        if(UI_ButtonOnClickSwipeStruct.bDontDoFlipFlop)
        {
            OnClickSwipeAnim();
        }
        else
        {
            if(_UIBehaviourComponent.GetButtonIsSelectedState())
            {
                ReturnOnClickSwipeAnim();
            }
            else
            {
                OnClickSwipeAnim();
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// SELECTD / DESELECT BEHAVIOUR ////////////////////////////////////////////////////////
    
   

    public void UpdateSelectedState()
    {
        switch(UI_ButtonOnSelectedData._SelectButtonMode)
        {
            case E_SelectedButtonMode.SetPositionByOffset:

                if (_UIBehaviourComponent != null)
                {
                   // Debug.Log("[" + this.gameObject.name + "] -> bIsSelected = " + _UIBehaviourComponent.GetButtonIsSelectedState());
                    if (_UIBehaviourComponent.GetButtonIsSelectedState())
                    {
                        if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode) //MOBILE
                        {
                            SelectedAnimOnMobileMode();
                        }
                        else //NORMAL
                        {
                            SelectedAnim();
                        }
                        
                        _HUDManagerScrit.DeselectAllUIFromSpecificPanel(this.gameObject);
                    }
                    else
                    {
                        if(UI_ButtonOnSelectedData.DeselectWhenYouClick)
                        {
                            if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode) //MOBILE
                            {
                                DeselectAnimOnMobileMode();
                            }
                            else //NORMAL
                            {
                                DeselectAnim();
                            }
                        } 
                    }
                }

                break;

            case E_SelectedButtonMode.ChangeColor:

                if (_UIBehaviourComponent != null)
                {
                    
                    if (_UIBehaviourComponent.GetButtonIsSelectedState())
                    {
                        myButton.image.color = UI_ButtonOnSelectedData._OnSelectedColor;

                        if(_HUDManagerScrit != null)
                        {
                            _HUDManagerScrit.DeselectAllUIFromSpecificPanel(this.gameObject);
                        }
                    }
                }

            break;

            case E_SelectedButtonMode.SetSprite:

                if (_UIBehaviourComponent != null)
                {
                    if (_UIBehaviourComponent.GetButtonIsSelectedState())
                    {
                        myButton.image.sprite = UI_ButtonOnSelectedData._OnSelectedSprite;
                    }
                    else
                    {
                        myButton.image.sprite = UI_ButtonOnSelectedData._UnSelectedSprite;
                    }

                }

                break;

            case E_SelectedButtonMode.SetPositionByOffsetANDChangeColor:
               
                if (_UIBehaviourComponent != null)
                {
                    if (_UIBehaviourComponent.GetButtonIsSelectedState())
                    {
                        myButton.image.color = UI_ButtonOnSelectedData._OnSelectedColor;

                        if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode) //MOBILE
                        {
                            SelectedAnimOnMobileMode();
                        }
                        else //NORMAL
                        {
                            SelectedAnim();
                        }

                        _HUDManagerScrit.DeselectAllUIFromSpecificPanel(this.gameObject);
                    }
                    else
                    {
                       
                        if (UI_ButtonOnSelectedData.DeselectWhenYouClick)
                        {
                            myButton.image.color = UI_ButtonOnSelectedData._UnSelectedColor;
                            if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode) //MOBILE
                            {
                                DeselectAnimOnMobileMode();
                            }
                            else //NORMAL
                            {
                                DeselectAnim();
                            }

                        }
                    }
                }
                break;
        }
        
    }

    public void DesSelectButton()
    {
        if (!_UIBehaviourComponent.GetButtonIsSelectedState())
        {
            switch (UI_ButtonOnSelectedData._SelectButtonMode)
            {
                case E_SelectedButtonMode.SetPositionByOffset:

                    if (_UIBehaviourComponent != null)
                    {
                        if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode) //MOBILE
                        {
                            DeselectAnimOnMobileMode();
                        }
                        else //NORMAL
                        {
                            DeselectAnim();
                        }
                    }

                    break;

                case E_SelectedButtonMode.ChangeColor:

                    if (_UIBehaviourComponent != null)
                    {
                        myButton.image.color = UI_ButtonOnSelectedData._UnSelectedColor;
                    }
                    break;
                case E_SelectedButtonMode.SetPositionByOffsetANDChangeColor:
                    if (_UIBehaviourComponent != null)
                    {
                        if (_UIBehaviourComponent.GetButtonBehaviourData().bButtonInMobileMode) //MOBILE
                        {
                            DeselectAnimOnMobileMode();
                        }
                        else //NORMAL
                        {
                            DeselectAnim();
                        }

                        myButton.image.color = UI_ButtonOnSelectedData._UnSelectedColor;
                    }
                    break;
            }
        }
    }



    public void SetColorOfButtonDependingOnSelectState(bool selected)
    {
        if(selected)
        {
            myButton.image.color = UI_ButtonOnSelectedData._OnSelectedColor;
        }
        else
        {
            myButton.image.color = UI_ButtonOnSelectedData._UnSelectedColor;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


}
