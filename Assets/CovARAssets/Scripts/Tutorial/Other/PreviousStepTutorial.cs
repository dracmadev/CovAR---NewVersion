using UnityEngine;
using UnityEngine.UI;

public class PreviousStepTutorial : MonoBehaviour
{
    Button previousButton;

    private void Start()
    {
        previousButton = GetComponent<Button>();
        previousButton.interactable = false;
        Invoke(nameof(ActivateListener), 1f);
    }

    private void ActivateListener()
    {
        previousButton.interactable = true;
        previousButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        TutorialManager.PreviousPhase();
    }
}
