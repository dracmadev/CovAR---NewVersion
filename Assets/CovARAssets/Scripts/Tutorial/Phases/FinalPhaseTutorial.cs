using System.Collections;
using UnityEngine;

public class FinalPhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.Final;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.Final");

        yield return new WaitUntil(() => currentStepNumber == 1);

        TutorialManager.NextPhase();
    }
}
