using UnityEngine;
using UnityEngine.UI;
public class InGameSettingsBehaviour : MonoBehaviour
{
    [Header("IN GAME SETTINGS")]
    [Header("Movile mode")]
    [SerializeField] private Button _MovileModeButton;
    [SerializeField] private Sprite _MovileModeActive;
    [SerializeField] private Sprite _MovileModeUnactive;


    void Start()
    {
        InitInGameSettings();
    }

    
    void Update()
    {
        OnUpdateMovileMode();
    }

    /////////////////////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////////////////

    public void InitInGameSettings()
    {
        if(_MovileModeButton != null)
        {
            _MovileModeButton.onClick.AddListener(OnClickSetMobileMode);
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


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
