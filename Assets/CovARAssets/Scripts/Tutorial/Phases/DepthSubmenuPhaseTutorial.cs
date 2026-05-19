using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DepthSubmenuPhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.DepthSubmenu;
    }

    public override IEnumerator DoSpecificPhase()
    {
        DisableAllSubpanelButtons();
        DisableAllPanelButtons();
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().onClick.Invoke();

        ShowMessage("Tuto.Depth");

        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons();
            DisableAllPanelButtons();
            yield return null;
        }
        TutorialManager.NextPhase();
    }
}
