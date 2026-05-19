using Assets.SimpleLocalization.Scripts;
using System.Collections;
using UnityEngine;

public class WelcomePhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.Welcome;
    }

    public override IEnumerator DoSpecificPhase()
    {
        FindFirstObjectByType<ARObjectManager>().SetActivePlaneFinder(false);

        string textKey;
      
        textKey = "Tuto.Welcome.Renolit";
        ShowMessage(LocalizationManager.Localize(textKey));

        yield return new WaitUntil(() => currentStepNumber >= 1);

        FindFirstObjectByType<ARObjectManager>().SetActivePlaneFinder(true);
        TutorialManager.NextPhase();
    }
}
