using UnityEngine;
using UnityEngine.UI;

public class NextStepTutorial : MonoBehaviour
{
    Button nextButton;

    private void Start()
    {
        nextButton = GetComponent<Button>();
        nextButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        foreach (TutorialParentScript tutorialScript in FindObjectsByType<TutorialParentScript>(FindObjectsSortMode.None))
        {
            if (tutorialScript.phase == TutorialManager.currentPhase)
                tutorialScript.NextStep();
        }
    }
}
