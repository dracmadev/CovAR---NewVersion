using Assets.SimpleLocalization.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class TutorialParentScript : MonoBehaviour
{
    [HideInInspector] public TutorialPhases phase;
    [HideInInspector] public string startingPanelName;
    [HideInInspector] public string startingSubpanelName;
    public bool isPhaseActive;
    public bool isPhaseCompleted;

    public int currentStepNumber;

    private List<Object> currentStepObjects = new List<Object>();

    [Header("General")]
    private HUDManagerScript _HUDManagerScript;
    private Transform tutorialParentTransform;

    [Header("RENOLIT")]
    [Space]
    public GameObject messageTextPrefab;
    public GameObject messageTextWithNextPrefab;
    public GameObject pointerArrowPrefab;

    [Header("POOL MAKER")]
    [Space]
    [SerializeField] public GameObject messageTextPrefab_PoolMaker;
    [SerializeField] public GameObject messageTextWithNextPrefab_PoolMaker;
    [SerializeField] public GameObject pointerArrowPrefab_PoolMaker;

    private void Start()
    {
        _HUDManagerScript = FindFirstObjectByType<HUDManagerScript>();
        
        if (GameObject.FindWithTag("TutorialUI"))
            tutorialParentTransform = GameObject.FindWithTag("TutorialUI").transform;

        TutorialManager.OnExitTutorial += CompletePhase;
    }

    private void Update()
    {
        if (TutorialManager.isTutorialActive)
        {
            if (TutorialManager.currentPhase == phase && !isPhaseActive)
            {
                Invoke(nameof(GetStartingPanels), 0.5f);

                isPhaseActive = true;
                isPhaseCompleted = false;
                currentStepNumber = 0;
                Debug.Log("Tutorial Active Phase: " + phase);

                StartCoroutine(nameof(DoSpecificPhase));
            }
            else if (TutorialManager.currentPhase != phase && isPhaseActive)
            {
                CompletePhase();
            }
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ShowMessage(string messageKey, bool nextButton = true)
    {
        GameObject text;
       
        text = Instantiate(nextButton ? messageTextWithNextPrefab_PoolMaker : messageTextPrefab_PoolMaker, tutorialParentTransform.GetChild(0)); //* revisar parent (canvas a part?)
        
       
        currentStepObjects.Add(text);

        LocalizedTMPText localizedText;
        if (text.GetComponent<LocalizedTMPText>() != null)
            localizedText = text.GetComponent<LocalizedTMPText>();
        else if (text.GetComponentInChildren<LocalizedTMPText>() != null)
            localizedText = text.GetComponentInChildren<LocalizedTMPText>();
        else return;

        localizedText.LocalizationKey = messageKey;
    }

    public void HighlightButton(Button button)
    {
        if (button.transform.GetComponent<Outline>() == null)
        {
            Outline outline = button.transform.AddComponent<Outline>();
            currentStepObjects.Add(outline);

            outline.effectDistance = new Vector2(25f, -25f);
            outline.effectColor = new Color32(33, 187, 209, 255);
        }
    }

    public void HighlightButton(Image image)
    {
        if (image.transform.GetComponent<Outline>() == null)
        {
            Outline outline = image.transform.AddComponent<Outline>();
            currentStepObjects.Add(outline);

            outline.effectDistance = new Vector2(25f, -25f);
            outline.effectColor = new Color32(33, 187, 209, 255);
        }
    }

    public void PointToButton(Button button, Vector2 direction, float xPos = 400f)
    {
        GameObject arrow;
        /*
        if (_AppRunning == E_AppSelector.PoolMaker)
        {
            arrow = Instantiate(pointerArrowPrefab_PoolMaker, tutorialParentTransform);
        }
        else
        {
            arrow = Instantiate(pointerArrowPrefab, tutorialParentTransform);
        }*/

        arrow = Instantiate(pointerArrowPrefab, tutorialParentTransform);
        currentStepObjects.Add(arrow);


        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

        arrow.transform.position = new Vector3(xPos, button.transform.position.y, button.transform.position.z);
            //button.transform.position + button.transform.right * xPos;
    }

    public void PointToButton(Image image, Vector2 direction, float xPos = 400f)
    {
        GameObject arrow = Instantiate(pointerArrowPrefab, tutorialParentTransform);
        currentStepObjects.Add(arrow);


        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

        arrow.transform.position = new Vector3(xPos, image.transform.position.y, image.transform.position.z);
    }

    public void RequireButtonToContinue(Button button)
    {
        currentStepObjects.Add(button.AddComponent<NextStepTutorial>());
    }

    public void DisableAllPanelButtons(Button exceptionButton = null)
    {
        Button[] buttons = _HUDManagerScript._PanelsGO.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (!button.gameObject.activeInHierarchy)
                continue;

            if (button != exceptionButton && button.interactable)
            {
                button.interactable = false;
                TutorialManager.buttonsToActivate.Add(button);
            }
        }

        if (exceptionButton != null)
        {
            exceptionButton.interactable = true;
        }
    }

    public void DisableAllPanelButtons(Button[] exceptionButton)
    {
        Button[] buttons = _HUDManagerScript._PanelsGO.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (!button.gameObject.activeInHierarchy)
                continue;

            foreach (Button _button in exceptionButton)
            {
                if (button != _button && button.interactable)
                {
                    button.interactable = false;
                    TutorialManager.buttonsToActivate.Add(button);
                }
            }
        }

        if (exceptionButton != null)
        {
            foreach (Button _button in exceptionButton)
                _button.interactable = true;
        }
    }

    public void DisableAllSubpanelButtons(Button exceptionButton = null)
    {
        Button[] buttons = _HUDManagerScript._SubPanelsGO.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (!button.gameObject.activeInHierarchy)
                continue;

            if (button != exceptionButton && button.interactable)
            {
                button.interactable = false;
                TutorialManager.buttonsToActivate.Add(button);
            }
        }

        if (exceptionButton != null)
        {
            exceptionButton.interactable = true;
        }
    }

    public void DisableAllSubpanelButtons(Button[] exceptionButton)
    {
        Button[] buttons = _HUDManagerScript._SubPanelsGO.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (!button.gameObject.activeInHierarchy)
                continue;

            foreach (Button _button in exceptionButton)
            {
                if (button != _button && button.interactable)
                {
                    button.interactable = false;
                    TutorialManager.buttonsToActivate.Add(button);
                }
            }
        }

        if (exceptionButton != null)
        {
            foreach (Button _button in exceptionButton)
                _button.interactable = true;
        }
    }

    private void DeleteCurrentStepTutorialElements()
    {
        foreach (var element in  currentStepObjects)
        {
            Destroy(element);
        }
        currentStepObjects.Clear();
    }

    //public void UnhighlightButton(Image image)
    //{
    //    if(image.transform.GetComponent<Outline>() != null)
    //        Destroy(image.transform.GetComponent<Outline>());
    //}

    public abstract IEnumerator DoSpecificPhase();

    public void NextStep()
    {
        DeleteCurrentStepTutorialElements();
        currentStepNumber++;
    }

    public void CompletePhase()
    {
        if (!this) return;

        isPhaseActive = false;
        isPhaseCompleted = true;

        StopCoroutine(nameof(DoSpecificPhase));
        DeleteCurrentStepTutorialElements();
    }

    private void GetStartingPanels()
    {
        startingPanelName = _HUDManagerScript.GetCurrentPanelOnScreenName();
        startingSubpanelName = _HUDManagerScript.GetCurrentSubPanelOnScreenName();

        foreach (TutorialParentScript tutorialScript in FindObjectsByType<TutorialParentScript>(FindObjectsSortMode.None))
        {
            if ((int)tutorialScript.phase == Mathf.Max(0, (int)TutorialManager.currentPhase - 1))
            {
                TutorialManager.lastPhaseScript = tutorialScript;
            }
        }
    }
}
