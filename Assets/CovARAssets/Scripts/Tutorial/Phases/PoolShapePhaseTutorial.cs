using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PoolShapePhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.PoolShape;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.PoolShape", false);

        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons();
            yield return null;
        }
    }
}
