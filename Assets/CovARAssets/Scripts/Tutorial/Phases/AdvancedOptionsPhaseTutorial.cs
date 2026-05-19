using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedOptionsPhaseTutorial : TutorialParentScript
{
    [SerializeField] Button saveButton;
    [SerializeField] Button confirmSaveButton;
    [SerializeField] Button closeSavePopupButton;
    [SerializeField] Button blueprintButton;
    [SerializeField] Button[] blueprintButtons;
    [SerializeField] Button closeBlueprint;

    private void Awake()
    {
        phase = TutorialPhases.Blueprint;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.AdvancedOptions1", false);
        HighlightButton(GetComponent<Button>());
        RequireButtonToContinue(GetComponent<Button>());
        PointToButton(GetComponent<Button>(), Vector2.left);

        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons();
            DisableAllPanelButtons(GetComponent<Button>());
            yield return null;
        }

        ShowMessage("Tuto.AdvancedOptions2", false);
        HighlightButton(saveButton);
        RequireButtonToContinue(saveButton);
        PointToButton(saveButton, Vector2.left);

        while (currentStepNumber < 2)
        {
            DisableAllPanelButtons(saveButton);
            yield return null;
        }

        RequireButtonToContinue(confirmSaveButton);

        while (currentStepNumber < 3)
        {
            DisableAllPanelButtons(confirmSaveButton);
            yield return null;
        }

        closeSavePopupButton.onClick.Invoke();
        ShowMessage("Tuto.AdvancedOptions3", false);
        HighlightButton(blueprintButton);
        RequireButtonToContinue(blueprintButton);
        PointToButton(blueprintButton, Vector2.left);

        while (currentStepNumber < 4)
        {
            DisableAllPanelButtons(blueprintButton);
            yield return null;
        }

        RequireButtonToContinue(closeBlueprint);
        while (currentStepNumber < 5)
        {
            DisableAllPanelButtons(blueprintButtons);
            yield return null;
        }

        TutorialManager.NextPhase();
    }
}
