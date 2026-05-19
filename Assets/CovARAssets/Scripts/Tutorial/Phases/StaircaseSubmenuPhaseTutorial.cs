using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaircaseSubmenuPhaseTutorial : TutorialParentScript
{
    [SerializeField] Button[] staircaseButtons;
    [SerializeField] Button backButton;

    private void Awake()
    {
        phase = TutorialPhases.StaircaseSubmenu;
    }

    public override IEnumerator DoSpecificPhase()
    {
        DisableAllSubpanelButtons();
        DisableAllPanelButtons();
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().onClick.Invoke();

        ShowMessage("Tuto.Staircase1");

        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons(staircaseButtons);
            yield return null;
        }

        ShowMessage("Tuto.Staircase2");

        while (currentStepNumber < 2)
        {
            DisableAllSubpanelButtons(staircaseButtons);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        backButton.onClick.Invoke();

        TutorialManager.NextPhase();
    }
}
