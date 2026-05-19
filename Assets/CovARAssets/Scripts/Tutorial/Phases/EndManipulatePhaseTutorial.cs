using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndManipulatePhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.EndManipulate;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.EndManipulate", false);
        HighlightButton(GetComponent<Button>());
        RequireButtonToContinue(GetComponent<Button>());
        PointToButton(GetComponent<Button>(), Vector2.left);

        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons();
            DisableAllPanelButtons(GetComponent<Button>());
            yield return null;
        }
        TutorialManager.NextPhase();
    }
}
