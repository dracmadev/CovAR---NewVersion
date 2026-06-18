using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLoginBehaviour : MonoBehaviour
{
    [Header("LOGIN")]
    [Space()]
    [SerializeField] private TMP_InputField _EmailName;
    [SerializeField] private TMP_InputField _PasswordName;
    [SerializeField] private Button _LoginButton;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LoginButtonBehaviour();
    }

    ////////////////////////////////////////////////////// BEHAVIOUR ///////////////////////////////////////////////////////////
    
    void LoginButtonBehaviour()
    {
        if (_EmailName.text != "" && _PasswordName.text != "")
        {
           _LoginButton.interactable = true;
        }
        else
        {
            _LoginButton.interactable = false;
        }

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
