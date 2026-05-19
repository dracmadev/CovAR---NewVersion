using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlaneFinderPhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.PlaneFinder;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.PlaneFinder", false);
        yield return null;
    }
}
