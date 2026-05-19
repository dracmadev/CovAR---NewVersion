using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPhaseTutorial : TutorialParentScript
{
    private void Awake()
    {
        phase = TutorialPhases.Main;
    }

    public override IEnumerator DoSpecificPhase()
    {
        // Step 0: Intro
        ShowMessage("Tuto.Main1");

        var buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length - 1; i++)
        {
            HighlightButton(buttons[i]);
        }

        while (currentStepNumber < 1)
        {
            DisableAllPanelButtons();
            DisableAllSubpanelButtons();
            yield return null;
        }
        // Step 1: Restablecer modelo

        ShowMessage("Tuto.Main2");
        yield return null;
        HighlightButton(GetComponentsInChildren<Button>()[0]);

        yield return new WaitUntil(() => currentStepNumber == 2);
        // Step 2: Manipular modelo

        ShowMessage("Tuto.Main3");
        HighlightButton(GetComponentsInChildren<Button>()[1]);

        yield return new WaitUntil(() => currentStepNumber == 3);
        // Step 3: Complementos

        ShowMessage("Tuto.Main4");
        HighlightButton(GetComponentsInChildren<Button>()[2]);

        yield return new WaitUntil(() => currentStepNumber == 4);
        // Step 4: Opciones avanzadas

        ShowMessage("Tuto.Main5");
        HighlightButton(GetComponentsInChildren<Button>()[3]);

        yield return new WaitUntil(() => currentStepNumber == 5);
        // Step 5: Pulsa manipular

        ShowMessage("Tuto.Main6", false);
        HighlightButton(GetComponentsInChildren<Button>()[1]);
        RequireButtonToContinue(GetComponentsInChildren<Button>()[1]);
        PointToButton(GetComponentsInChildren<Button>()[1], Vector2.left);
        DisableAllPanelButtons(GetComponentsInChildren<Button>()[1]);

        yield return new WaitUntil(() => currentStepNumber > 5);
        TutorialManager.NextPhase();
    }
}
