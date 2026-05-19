using UnityEngine;
using UnityEngine.UI;

public class SkipTutorial : MonoBehaviour
{
    Button skipButton;

    private void Start()
    {
        skipButton = GetComponent<Button>();
        skipButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        TutorialManager.ExitTutorial();
    }
}
