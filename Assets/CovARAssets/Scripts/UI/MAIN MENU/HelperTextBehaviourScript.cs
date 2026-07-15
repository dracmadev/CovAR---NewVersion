using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class HelperTextBehaviourScript : MonoBehaviour
{
    [Header("HELPER TEXT BEHAVIOUR SCRIPT")]
    [Space()]
    [Header("TMP_Text Ref: (Automatically)")]
    [SerializeField] private TMP_Text _TextHelper;
    [Header("Feedback colors")]
    [SerializeField] private Color _IncorrectMessageColor;
    [SerializeField] private Color _CorrectMessageColor;
    void Start()
    {
        InitHelperText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitHelperText()
    {
        if (this.gameObject.GetComponent<TMP_Text>())
        {
            _TextHelper = this.gameObject.GetComponent<TMP_Text>();
        }
    }

    public void ShowLoginIncorrectMessage(string key)
    {
        _TextHelper.DOKill();

        string localizedKey = $"Login.HelperText.{key}";
        string localizedText = LocalizationManager.Localize(localizedKey);

        _TextHelper.text = localizedText;

        _TextHelper.color = _IncorrectMessageColor;


        _TextHelper.DOFade(0, 3f).SetDelay(5f).SetEase(Ease.InOutQuad);
    }
    public void ShowLoginCorrectMessage(string key)
    {
        _TextHelper.DOKill();

        string localizedKey = $"Login.HelperText.{key}";
        string localizedText = LocalizationManager.Localize(localizedKey);

        _TextHelper.text = localizedText;

        _TextHelper.color = _CorrectMessageColor;


        _TextHelper.DOFade(0, 3f).SetDelay(5f).SetEase(Ease.InOutQuad);
    }
}
