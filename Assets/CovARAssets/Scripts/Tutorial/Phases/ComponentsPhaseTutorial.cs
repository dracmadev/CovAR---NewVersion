using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ComponentsPhaseTutorial : TutorialParentScript
{
    [SerializeField] Button[] componentsButtons;
    [SerializeField] Button backButton;

    private void Awake()
    {
        phase = TutorialPhases.Components;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.Components1", false);
        HighlightButton(GetComponent<Button>());
        RequireButtonToContinue(GetComponent<Button>());
        PointToButton(GetComponent<Button>(), Vector2.left);

        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons();
            DisableAllPanelButtons(GetComponent<Button>());
            yield return null;
        }

        ShowMessage("Tuto.Components2");
        RequireButtonToContinue(backButton);
        RequireButtonToContinue(backButton);

        while (currentStepNumber < 2)
        {
            DisableAllPanelButtons(componentsButtons);
            yield return null;
        }

        ShowMessage("Tuto.Components3", false);
        HighlightButton(backButton);
        RequireButtonToContinue(backButton);
        PointToButton(backButton, Vector2.left);

        while (currentStepNumber < 3)
        {
            DisableAllPanelButtons(componentsButtons.Append(backButton).ToArray());
            yield return null;
        }
        TutorialManager.NextPhase();
    }
}
