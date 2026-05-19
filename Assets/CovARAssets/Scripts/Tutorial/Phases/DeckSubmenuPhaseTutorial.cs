using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeckSubmenuPhaseTutorial : TutorialParentScript
{
    [SerializeField] Button[] deckButtons;
    [SerializeField] Button backButton;

    private void Awake()
    {
        phase = TutorialPhases.DeckSubmenu;
    }

    public override IEnumerator DoSpecificPhase()
    {
        DisableAllSubpanelButtons();
        DisableAllPanelButtons();
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().onClick.Invoke();

        ShowMessage("Tuto.Deck1");

        while (currentStepNumber < 1)
        {
            DisableAllPanelButtons(deckButtons);
            yield return null;
        }

        ShowMessage("Tuto.Deck2");

        yield return new WaitUntil(() => currentStepNumber == 2);

        ShowMessage("Tuto.Deck3", false);
        HighlightButton(backButton);
        RequireButtonToContinue(backButton);
        PointToButton(backButton, Vector2.left);
        while (currentStepNumber < 3)
        {
            DisableAllPanelButtons(backButton);
            yield return null;
        }

        TutorialManager.NextPhase();
    }
}
