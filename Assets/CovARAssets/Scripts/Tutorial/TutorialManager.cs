using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialPhases
{
    Welcome,
    PlaneFinder,
    PoolShape,
    FastActions,
    Main,
    Move,
    Rotate,
    ResizeSubmenu,
    DepthSubmenu,
    StaircaseSubmenu,
    DeckSubmenu,
    ShelfSubmenu,
    EndManipulate,
    Components,
    Blueprint,
    Final,
    NONE,
}

public static class TutorialManager
{
    public static bool isTutorialActive;
    public static TutorialPhases currentPhase = TutorialPhases.Welcome;
    public static TutorialParentScript lastPhaseScript = null;

    public static System.Action OnExitTutorial;
    public static List<Button> buttonsToActivate = new List<Button>();
    public static System.Action OnPreviousPhaseTutorial;
    //public static string previousPhaseStartingPanel = "";
    //public static string previousPhaseStartingSubpanel = "";

    public static void ActivateTutorial()
    {
        isTutorialActive = true;
        PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.Save();
    }

    public static void ExitTutorial()
    {
        isTutorialActive = false;
        PlayerPrefs.SetInt("Tutorial", 0);
        PlayerPrefs.Save();
        currentPhase = TutorialPhases.Welcome;
        OnExitTutorial?.Invoke();
    }

    public static void SetPhase(int phase)
    {
        currentPhase = (TutorialPhases)phase;
    }
    
    public static void SetPhase(TutorialPhases phase)
    {
        currentPhase = phase;
    }

    public static void NextPhase()
    {
        if (!isTutorialActive)
            return;

        currentPhase++;

        if (currentPhase == TutorialPhases.NONE)
            ExitTutorial();
    }

    public static void PreviousPhase()
    {
        foreach (var button in buttonsToActivate)
            button.interactable = true;
        buttonsToActivate.Clear();

        int newPhase = Mathf.Max(0, (int)currentPhase - 1);
        SetPhase(newPhase);

        OnPreviousPhaseTutorial?.Invoke();
    }
}
