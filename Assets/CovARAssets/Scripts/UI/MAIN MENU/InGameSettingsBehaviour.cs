using UnityEngine;
using UnityEngine.UI;
public class InGameSettingsBehaviour : MonoBehaviour
{
    [Header("IN GAME SETTINGS")]
    [Header("Movile mode")]
    [SerializeField] private Button _MovileModeButton;
    [Header("Help mode")]
    [SerializeField] private Button _HelpModeButton;
    [Header("Sprites:")]
    [SerializeField] private Sprite _MovileModeActive;
    [SerializeField] private Sprite _MovileModeUnactive;


    void Start()
    {
        InitInGameSettings();
    }

    
    void Update()
    {
        OnUpdateMovileMode();
        OnUpdateHelpMode();
    }

    /////////////////////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////////////////

    public void InitInGameSettings()
    {
        if(_MovileModeButton != null)
        {
            _MovileModeButton.onClick.AddListener(OnClickSetMobileMode);
        }

        if(_HelpModeButton != null)
        {
            _HelpModeButton.onClick.AddListener(OnClickSetHelpMode);
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////////////

    void OnUpdateMovileMode()
    {
        if (PlayerPrefs.GetInt("MobileMode") == 1)
        {
            _MovileModeButton.image.sprite = _MovileModeActive;
        }
        else
        {
            _MovileModeButton.image.sprite = _MovileModeUnactive;
        }
    }

    void OnClickSetMobileMode()
    {
        if (PlayerPrefs.GetInt("MobileMode") == 1)
        {
            PlayerPrefs.SetInt("MobileMode", 0);
        }
        else
        {
            PlayerPrefs.SetInt("MobileMode", 1);
        }
    }

    void OnUpdateHelpMode()
    {
        if (PlayerPrefs.GetInt("HelpMode") == 1)
        {
            _HelpModeButton.image.sprite = _MovileModeActive;
        }
        else
        {
            _HelpModeButton.image.sprite = _MovileModeUnactive;
        }
    }

    void OnClickSetHelpMode()
    {
        if (PlayerPrefs.GetInt("HelpMode") == 1)
        {
            PlayerPrefs.SetInt("HelpMode", 0);
        }
        else
        {
            PlayerPrefs.SetInt("HelpMode", 1);
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
