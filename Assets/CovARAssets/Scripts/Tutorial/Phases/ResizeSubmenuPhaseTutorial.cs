using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResizeSubmenuPhaseTutorial : TutorialParentScript
{
    public TMP_Dropdown sidesDropdown;

    private void Awake()
    {
        phase = TutorialPhases.ResizeSubmenu;
    }

    public override IEnumerator DoSpecificPhase()
    {
        DisableAllSubpanelButtons();
        DisableAllPanelButtons();
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().onClick.Invoke();

        ShowMessage("Tuto.Resize1");
        while (currentStepNumber < 1)
        {
            DisableAllSubpanelButtons();
            DisableAllPanelButtons();
            yield return null;
        }

        ShowMessage("Tuto.Resize2");
        PointToButton(sidesDropdown.transform.GetComponent<Image>(), Vector2.right, 2150f);

        while (currentStepNumber < 2)
        {
            yield return null;
        }
        TutorialManager.NextPhase();
    }
}
