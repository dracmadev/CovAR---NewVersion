using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MovePhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.Move;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.Move1", false);
        HighlightButton(GetComponent<Button>());
        RequireButtonToContinue(GetComponent<Button>());
        PointToButton(GetComponent<Button>(), Vector2.left);
        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons();
            DisableAllPanelButtons(GetComponent<Button>());
            yield return null;
        }

        ShowMessage("Tuto.Move2");

        while (currentStepNumber < 2)
        {
            DisableAllPanelButtons();
            yield return null;
        }
        TutorialManager.NextPhase();
    }
}
