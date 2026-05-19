using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FastActionsPhaseTutorial : TutorialParentScript
{
    public Button catalogButton;

    private void Awake()
    {
        phase = TutorialPhases.FastActions;
    }

    public override IEnumerator DoSpecificPhase()
    {
        ShowMessage("Tuto.FastActions1");
        HighlightButton(catalogButton);
        PointToButton(catalogButton, Vector2.right, 2400f);

        while (currentStepNumber < 1)
        {
            DisableAllPanelButtons();
            DisableAllSubpanelButtons();
            yield return null;
        }

        ShowMessage("Tuto.FastActions2");
       // if (_AppRunning == E_AppSelector.Renolit) HighlightButton(GetComponentsInChildren<Button>(true)[0]);
        PointToButton(GetComponentsInChildren<Button>(true)[0], Vector2.right, 2400f);

        yield return new WaitUntil(() => currentStepNumber == 2);

        ShowMessage("Tuto.FastActions3");
        //if (_AppRunning == E_AppSelector.Renolit) HighlightButton(GetComponentsInChildren<Button>(true)[1]);
        PointToButton(GetComponentsInChildren<Button>(true)[1], Vector2.right, 2400f);

        yield return new WaitUntil(() => currentStepNumber == 3);

        ShowMessage("Tuto.FastActions4");
        //if (_AppRunning == E_AppSelector.Renolit) HighlightButton(GetComponentsInChildren<Button>(true)[2]);
        PointToButton(GetComponentsInChildren<Button>(true)[2], Vector2.right, 2400f);

        yield return new WaitUntil(() => currentStepNumber >= 4);

        TutorialManager.NextPhase();
    }
}
