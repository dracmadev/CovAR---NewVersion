using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;


#if UNITY_EDITOR
using UnityEditor; 
#endif

public class HUDManagerScript : MonoBehaviour
{
    [Header("HUD MANAGER")]
    [Space(25)]
    [Header("Where's HUD?")]
    [SerializeField] private bool bOnARScene;
    [Header("Panels list:")]
    [SerializeField] private St_UIPanelStruct[] _UIPanelList;
    [Space()]
    [Header("Time between panels:")]
    [SerializeField] private float transitionTime;
    [Space()]
    [Header("Write the name of the first panel to show:")]
    [SerializeField] private string firstPanelToShow;
    private static string OverrideFirstPanel = null;
    [Header("Current panel on screen: (Exposed for debug)")]
    [SerializeField] private GameObject currentPanelOnScreenGameObject;
    [SerializeField] private string currentPanelOnScreenName;
    [SerializeField] public string lastPanelOnScreenName;
    [Space()]
    [Header("SubPanels list:")]
    [SerializeField] private St_UIPanelStruct[] _UISubPanelList;
    [HideInInspector] public bool showInformationMenu = true;
    private const string showInfoMenuKey = "ShowInformationMenu";
    [Header("Current Sub-panel on screen: (Exposed for debug)")]
    [SerializeField] private GameObject currentSubPanelOnScreen;
    [SerializeField] private string currentSubPanelOnScreenName;
    [SerializeField] private string lastSubPanelOnScreenName;
    [SerializeField] private string justInCasetSubPanelOnScreenName;
    [Header("MAX loading time waiting:")]
    [SerializeField] private int _MaxTimeOnLoadingScreen = 20;

    [Header("FastActions reference:")]
    [SerializeField] public GameObject _fastActionsPanelRef;
    [SerializeField] private bool bFastActionsIsOnScreen;
    [SerializeField] private bool bFastActionsWasOnScreen;


    [Header("HUD ELEMENTS (Not-Automatically)")]
    [SerializeField] public GameObject _PanelsGO;
    [SerializeField] public GameObject _SubPanelsGO;
    [SerializeField] private GameObject _PopUpsGO;
    [SerializeField] private GameObject _WaterMark;
    [SerializeField] private GameObject _AppLogoMark;
    Vector3 _originalPositionOfPoolMakerMark;

    bool bStopTraking;
    private bool isTransitioning = false;
    private bool isSubTraveling = false;
    float deltaTime;

    private ARObjectManager _ARObjectManager;

    void Start()
    {
        InitHUDManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.productName.Contains("DEV"))
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    ///////////////////////////////////////////////// INIT HUD /////////////////////////////////////////////////////////////////

    void InitHUDManager()
    {
        if (!string.IsNullOrEmpty(OverrideFirstPanel))
        {
            firstPanelToShow = OverrideFirstPanel;
            OverrideFirstPanel = null; // El buidem perquè no es quedi guardat per sempre
        }

        FromNothingToFirstPanel();

      //  showInformationMenu = true /*PlayerPrefs.GetInt(showInfoMenuKey, 0) == 1*/;

        if(GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        if(_AppLogoMark != null)
        {
            _originalPositionOfPoolMakerMark = _AppLogoMark.transform.localPosition;
           // OnHideTransitionBetweenScenes();
        }
       
        TutorialManager.OnExitTutorial += TravelToMainMenuFromTutorial;
        TutorialManager.OnPreviousPhaseTutorial += TravelToPreviousTutorialPhaseMenu;

    }

    private void OnDisable()
    {
        TutorialManager.OnExitTutorial -= TravelToMainMenuFromTutorial;
        TutorialManager.OnPreviousPhaseTutorial -= TravelToPreviousTutorialPhaseMenu;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////// GETTER ////////////////////////////////////////////////////////

    public string GetCurrentPanelOnScreenName()
    {
        return currentPanelOnScreenName;
    }

    public string GetCurrentSubPanelOnScreenName()
    {
        return currentSubPanelOnScreenName;
    }

    public bool GetStopTracking()
    {
        return bStopTraking;
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////// SETTER /////////////////////////////////////////////////////////////
    
    public void SetStopTracking(bool newState)
    {
        bStopTraking = newState;
    }

    public void SetOverrideFirstPanel(string newPanel)
    {
        OverrideFirstPanel = newPanel;
    }

    public void SetLastPanelOnScreenName(string name)
    {
        lastPanelOnScreenName = name;
    }
    public void SetLastSubPanelOnScreenName(string name)
    {
        lastSubPanelOnScreenName = name;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////// PANEL BEHAVIOUR HUD ////////////////////////////////////////////////////////////
    void ShowUIPanel(string name)
    {
        ShowAllUIFromSpecificPanel(GetUIPanelByName(name));
    }

    void HideUIPanel(string name)
    {
        HideAllUIFromSpecificPanel(GetUIPanelByName(name));
    }

    public GameObject GetUIPanelByName(string name)
    {
        foreach (St_UIPanelStruct uiPanel in _UIPanelList)
        {
            if (uiPanel._PanelName == name)
            {
                return uiPanel._PanelRef;
            }
        }

        return null;
    }

    public GameObject GetUISubPanelByName(string name)
    {
        foreach (St_UIPanelStruct uiPanel in _UISubPanelList)
        {
            if (uiPanel._PanelName == name)
            {
                return uiPanel._PanelRef;
            }
        }

        return null;
    }


    void ShowAllUIFromSpecificPanel(GameObject panel)
    {
        if(panel != null)
        {
            TraverseAndShow(panel.transform);
        }
        else
        {
            Debug.LogError("[ShowAllUIFromSpecificPanel] -> The GameobjectPanel == null...");
        }
        
    }
    void TraverseAndShow(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // 2. Intentem obtenir el component d'animació
            var anim = child.GetComponent<UIAnimationComponent>();

            // 3. Verifiquem condicions per mostrar el panell
            if (anim != null &&
                anim.GetUIComponentType() != E_UIComponents.DefaultPopUp &&
                anim.GetAnimationType() != E_UIAnimationType.None)
            {
                // Addicionalment, només fem el Show si és un dels tags rellevants o si no té cap d'ells
                // (això depèn de si vols que la resta d'objectes "normals" també es mostrin)
                anim.ShowPannel();
            }

            // Continua recorrent la jerarquia
            TraverseAndShow(child);
        }
    }

    void HideAllUIFromSpecificPanel(GameObject panel)
    {
        TraverseAndHide(panel.transform);
    }

    public void HideCurrentPanelOnScreen()
    {
        HideAllUIFromSpecificPanel(currentPanelOnScreenGameObject);
        currentPanelOnScreenGameObject = null;
    }

    void TraverseAndHide(Transform parent)
    {
        foreach (Transform child in parent)
        {
            var anim = child.GetComponent<UIAnimationComponent>();

            if (anim != null && anim.GetUIComponentType() != E_UIComponents.DefaultPopUp && anim.GetAnimationType() != E_UIAnimationType.None)
            {
                anim.HidePannel();
            }

            // Continua recorrent tots els fills
            TraverseAndHide(child);
        }
    }


    IEnumerator ShowFirstUIPanel()
    {
        //GetUIPanelByName(firstPanelToShow).SetActive(true);
        yield return new WaitForSeconds(transitionTime);
        ShowUIPanel(firstPanelToShow);
        currentPanelOnScreenGameObject = GetUIPanelByName(firstPanelToShow);
        currentPanelOnScreenName = firstPanelToShow;

     
        TutorialManager.isTutorialActive = PlayerPrefs.GetInt("Tutorial") == 1;
        TutorialManager.SetPhase(0);
        
    }

    public void TravelToPanel(string panelToShow)
    {
        if (!this) return;
        StartCoroutine(TravelToPanelCoroutine(currentPanelOnScreenGameObject, panelToShow));
    }

    public void TravelToLastPanel()
    {
        if (lastSubPanelOnScreenName != "")
        {
            TravelToSubPanel(lastSubPanelOnScreenName);
        }

        if(lastPanelOnScreenName != "")
        {
            StartCoroutine(TravelToPanelCoroutine(currentPanelOnScreenGameObject, lastPanelOnScreenName));
        }
        else // per si decas que sempre torni a algun lloc.
        {
            TravelToPanel("MainPanel");
            //TravelToSubPanel("FastActionsSubPanel");
        }

        if(bFastActionsWasOnScreen)
        {
            //OnClickShowFastActionsPanel(true);
        }

       
    }

    public void TravelToLastSubPanel()
    {
        if (lastSubPanelOnScreenName != "")
        {
            TravelToSubPanel(lastSubPanelOnScreenName);
        }
        else // per si decas que sempre torni a algun lloc.
        {
            TravelToPanel("MainPanel");
            //OnClickShowFastActionsPanel(true);
        }


    }

    IEnumerator TravelToPanelCoroutine(GameObject panelToHide, string panelToShow)
    {
        if (isTransitioning) yield break;

        // 1. Bloquegem tota la UI físicament
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.interactable = false;

        isTransitioning = true;

        if (GetUIPanelByName(panelToShow) != null)
        {
            //HideAllOfSubMenus();

            if (panelToHide != null)
            {
                HideAllUIFromSpecificPanel(panelToHide);
                yield return new WaitForSeconds(transitionTime);
            }

            ShowUIPanel(panelToShow);
            currentPanelOnScreenGameObject = GetUIPanelByName(panelToShow);

            if (panelToShow != "ReFindModelPanel")
            {
                if (currentPanelOnScreenName != panelToShow)
                {
                    lastPanelOnScreenName = currentPanelOnScreenName;
                }
                currentPanelOnScreenName = panelToShow;
            }
        }
        else
        {
            Debug.LogError("HUDManager -> Error al fer travel between panels");
        }

        // 2. Alliberem tant la bool com la interactivitat de la UI
        isTransitioning = false;
        if (cg != null) cg.interactable = true;
    }

    public void TravelToMainMenuFromTutorial()
    {
        foreach (var button in TutorialManager.buttonsToActivate)
            button.interactable = true;
        TutorialManager.buttonsToActivate.Clear();

        _ARObjectManager.SetActivePlaneFinder(true);

        TravelToPanel("MainPanel");
       
        _ARObjectManager.SetCurrentARObjectState("DefaultState");
    }

    public void TravelToPreviousTutorialPhaseMenu()
    {
        if (TutorialManager.lastPhaseScript != null)
        {
            if (TutorialManager.lastPhaseScript.startingSubpanelName == "") Debug.Log("LAST PHASE SCRIPT STARTING SUBPANEL NAME = EMPTY, SHOWING FAST ACTIONS PANEL INSTEAD");
            //OnClickShowFastActionsPanel(true);
            else
                TravelToSubPanel(TutorialManager.lastPhaseScript.startingSubpanelName);
            TravelToPanel(TutorialManager.lastPhaseScript.startingPanelName);
        }
        else
        {
            Debug.Log("LAST PHASE SCRIPT = NULL");
        }
    }

    void FromNothingToFirstPanel()
    {
        StartCoroutine(ShowFirstUIPanel());
    }

    public List<GameObject> GetAllOfUIComponentsFromSpecificPanel(string MenuName)
    {
        List<GameObject> listOfUIComponents = new();

        Transform panelTransform = GetUIPanelByName(MenuName).transform;

        // Aconsegueix TOTS els components del tipus UIBehaviourComponent dins del panell (incloent-hi el mateix si cal)
        UIBehaviourComponent[] components = panelTransform.GetComponentsInChildren<UIBehaviourComponent>(true);

        foreach (var comp in components)
        {
            listOfUIComponents.Add(comp.gameObject);
        }

        return listOfUIComponents;
    }

    public void ForceARObjectStateAndLastSubPanel()
    {
        if (_ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().GetARObjectCurrentState() != E_ARObjectStates.DefaultState)
        {
            Debug.Log("ForcingState");
            SetLastSubPanelOnScreenName("");
            _ARObjectManager.SetCurrentARObjectState("DefaultState");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////// SUB PANEL BEHAVIOUR ///////////////////////////////////////////////////

    public void HideAllOfSubMenus()
    {
        if(_UISubPanelList.Length != 0)
        {
            foreach(St_UIPanelStruct child in _UISubPanelList)
            {
               
                HideAllUIFromSpecificPanel(child._PanelRef);
                
            }
            currentSubPanelOnScreen = null;

            lastSubPanelOnScreenName = currentSubPanelOnScreenName;
            
            currentSubPanelOnScreenName = "";
        } 
    }
    IEnumerator ShowSpecificSubMenuCorutine(string subMenuName)
    {
        HideAllOfSubMenus();
        yield return new WaitForSeconds(transitionTime/3);
        ShowAllUIFromSpecificPanel(GetUISubPanelByName(subMenuName));
    }

    public void ShowSpecificSubMenu(string subMenuName)
    {
        if (subMenuName == currentSubPanelOnScreenName) { return; }

        StartCoroutine(ShowSpecificSubMenuCorutine(subMenuName));
        currentSubPanelOnScreenName = subMenuName;
    }

    public List<GameObject> GetAllOfUIComponentsFromSpecificSubPanel(string subMenuName)
    {
        List<GameObject> listOfUIComponents = new();

        if (GetUISubPanelByName(subMenuName) == null)
            return listOfUIComponents;

        Transform panelTransform = GetUISubPanelByName(subMenuName).transform;

        // Aconsegueix TOTS els components del tipus UIBehaviourComponent dins del panell (incloent-hi el mateix si cal)
        UIBehaviourComponent[] components = panelTransform.GetComponentsInChildren<UIBehaviourComponent>(true);

        foreach (var comp in components)
        {
            listOfUIComponents.Add(comp.gameObject);
        }

        return listOfUIComponents;
    }

    public void TravelToSubPanel(string subPanelName)
    {
        if (subPanelName != currentSubPanelOnScreenName)
        {
            lastSubPanelOnScreenName = currentSubPanelOnScreenName;
            currentSubPanelOnScreenName = subPanelName;

            StartCoroutine(TravelToSubPanelCoroutine(currentSubPanelOnScreen, subPanelName));
        }
    }

    IEnumerator TravelToSubPanelCoroutine(GameObject subPanelToHide, string subPanelToShow)
    {
        // 1. Bloqueig preventiu per codi
        if (isSubTraveling) yield break;

        // Obtenim el Canvas Group per desactivar els clics
        CanvasGroup cg = GetComponent<CanvasGroup>();

        if (GetUISubPanelByName(subPanelToShow) != null)
        {
            isSubTraveling = true;

            // 2. Bloquegem la interactivitat física
            if (cg != null) cg.interactable = false;

            if (subPanelToHide != null)
            {
                HideAllUIFromSpecificPanel(subPanelToHide);
            }

            yield return new WaitForSeconds(transitionTime);

            ShowUISubPanel(subPanelToShow);
            currentSubPanelOnScreen = GetUISubPanelByName(subPanelToShow);
            currentSubPanelOnScreenName = subPanelToShow;
        }
        else
        {
            Debug.LogError("HUDManager -> Error al fer travel between SubPanels");
        }

        // 3. Alliberem el bloqueig i tornem a activar els botons
        isSubTraveling = false;
        if (cg != null) cg.interactable = true;
    }
    void ShowUISubPanel(string name)
    {
        ShowAllUIFromSpecificPanel(GetUISubPanelByName(name));
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////// DesSelect behaviour ////////////////////////////////////////////////////


    public void DeselectAllUIFromSpecificPanel(GameObject myself)
    {
        TraverseAndDeselect(currentPanelOnScreenGameObject.transform, myself);
    }

    void TraverseAndDeselect(Transform parent, GameObject myself)
    {
   
        foreach (Transform child in parent)
        {
            var anim = child.GetComponent<UIAnimationComponent>();
            if (anim && anim.GetUIComponentType() == E_UIComponents.Button && anim.gameObject != myself && anim.GetSelectedState())
            {
                child.GetComponent<UIBehaviourComponent>().SetButtonSelectedState(false);
                anim.DesSelectButton();
                //Debug.Log("[UnSelect] -> " + child.name);
            }

            // Recórrer recursivament *aquesta mateixa funció*
            TraverseAndDeselect(child, myself);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////// SET SHOW INFO MENU BOOL //////////////////////////////////////////////////////////

    public void SetShowInformationMenu(bool value)
    {
        showInformationMenu = value;
        PlayerPrefs.SetInt(showInfoMenuKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////// QUIT APP //////////////////////////////////////////////////////////////////

    public void QuitApp()
    {
        StartCoroutine(QuitAppCorutine());
    }

    IEnumerator QuitAppCorutine()
    {
        HideAllOfSubMenus();
        HideAllUIFromSpecificPanel(currentPanelOnScreenGameObject);
        if(bOnARScene)
        {
            yield return new WaitForSeconds(1.5f);
        }
       
        yield return new WaitForSeconds(transitionTime);
#if UNITY_EDITOR
        // Si som a l'Editor, atura el mode de reproducció
        if (bOnARScene)
        {
           
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(transitionTime);
        EditorApplication.isPlaying = false;
#else
            // Si és un build (Windows, Android, etc.), tanca l'aplicació
            Application.Quit();
#endif
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////// PLANE FINDER //////////////////////////////////////////////////////////////
    
    public void DetectGroundAndTravelToNextPanel()
    {
        if(lastPanelOnScreenName == "" || TutorialManager.isTutorialActive)
        {
            TravelToPanel("PoolShapePanel");
        }
        else
        {           
            StartCoroutine(ReturnToMainPanelFromReFindModel());
        }
        TutorialManager.NextPhase();
    }

    public void CloseReFindModelPanel()
    {
        HideAllUIFromSpecificPanel(GetUIPanelByName("ReFindModelPanel"));
    }

    IEnumerator ReturnToMainPanelFromReFindModel()
    {
        CloseReFindModelPanel();
        //SetStopTracking(false);
        yield return new WaitForSeconds(0.25f);
        _ARObjectManager.SetCurrentARObjectState("DefaultState");
        //TravelToSubPanel("FastActionsSubPanel");
        TravelToPanel("MainPanel");
      
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /////////////////////////////////////////////////////// SCENES /////////////////////////////////////////////////////////////////
    
    public void OnClickMoveToLoginSceneAndOpeningSpecificPanel(string specificPanel)
    {
        OverrideFirstPanel = specificPanel;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_Login_PO");
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////// SCENES /////////////////////////////////////////////////////////////////
    void OnGUI()
    {
        if (Application.productName.Contains("DEV"))
        {
            int fps = Mathf.CeilToInt(1.0f / deltaTime);

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 48;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(10, 10, 300, 50), fps + " FPS", style);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////// SCREENSHOOT THINGS //////////////////////////////////////////////////////////////
    
    void ShowPanelsGO(bool show)
    {
        CanvasGroup cgPanels = _PanelsGO.GetComponent<CanvasGroup>();

        if(_PanelsGO != null && cgPanels != null)
        {
            if(show)
            {
                cgPanels.alpha = 1;
            }
            else
            {
                cgPanels.alpha = 0;
            }
        }
    }

   void ShowSubPanelsGO(bool show)
    {
        CanvasGroup cgSubPanels = _SubPanelsGO.GetComponent<CanvasGroup>();

        if (_SubPanelsGO != null && cgSubPanels != null)
        {
            if (show)
            {
                cgSubPanels.alpha = 1;
            }
            else
            {
                cgSubPanels.alpha = 0;
            }
        }
    }

    void ShowPopUpGO(bool show)
    {
        CanvasGroup cgPopUps = _PopUpsGO.GetComponent<CanvasGroup>();

        if (_PopUpsGO != null && cgPopUps != null)
        {
            if (show)
            {
                cgPopUps.alpha = 1;
            }
            else
            {
                cgPopUps.alpha = 0;
            }
        }
    }

    void ShowWaterMarkGO(bool show)
    {
        if(_WaterMark != null)
        {
            CanvasGroup cgWaterMark = _WaterMark.GetComponent<CanvasGroup>();

            if  (cgWaterMark != null)
            {
                if (show)
                {
                    cgWaterMark.alpha = 1;
                }
                else
                {
                    cgWaterMark.alpha = 0;
                }
            }
        }
       
    }

    public void HideAllHUD()
    {
        ShowPanelsGO(false);
        ShowSubPanelsGO(false);
        ShowPopUpGO(false);
        ShowWaterMarkGO(false);
    }

    public void ShowAllHUD()
    {
        ShowPanelsGO(true);
        ShowSubPanelsGO(true);
        ShowPopUpGO(true);
        ShowWaterMarkGO(false);
    }

    public void HideHUDAndShowWaterMark()
    {
        ShowPanelsGO(false);
        ShowSubPanelsGO(false);
        ShowPopUpGO(false);
        ShowWaterMarkGO(true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////// LOADING SCREEN BEHAVIOUR ////////////////////////////////////////////////////////////
    
    public void InitLoadingCountdown()
    {
        StartCoroutine(LoadingCountDown());
    }

    public IEnumerator LoadingCountDown()
    {
        yield return new WaitForSeconds(_MaxTimeOnLoadingScreen);
        HideAllOfSubMenus();
       
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  
}
