using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShelfSubmenuPhaseTutorial : TutorialParentScript
{
    [SerializeField] Button[] shelfButtons;
    [SerializeField] Button backButton;

    private void Awake()
    {
        phase = TutorialPhases.ShelfSubmenu;
    }

    public override IEnumerator DoSpecificPhase()
    {
        DisableAllSubpanelButtons();
        DisableAllPanelButtons();
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().onClick.Invoke();

        ShowMessage("Tuto.Shelf1");
        //PointToButton(GetComponentInChildren<TMP_Dropdown>().transform.GetComponent<Image>(), Vector2.right, -1150f);

        while (currentStepNumber < 1)
        {
            DisableAllPanelButtons(shelfButtons);
            yield return null;
        }

        /*
        if (_AppRunning == E_AppSelector.PoolMaker)
        {
            ShowMessage("Tuto.Shelf2");

            while (currentStepNumber < 2)
            {
                DisableAllPanelButtons(shelfButtons);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            backButton.onClick.Invoke();
        }*/

        TutorialManager.NextPhase();
    }
}
