using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintPopUpBehaviour : MonoBehaviour
{
    [Header("BLUEPRINT POP-UP BEHAVIOUR")]
    [Space()]
    [SerializeField] private TMP_InputField _CustomerNameInputText;
    [SerializeField] private TMP_InputField _CustomerMailInputText;
    [SerializeField] private Button _AcceptButton;

    string customerName;
    string customerMail;
    string  lastPanelBeforeBlueprintPopUp;
    HUDManagerScript _HUDManagerScript;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("HUD"))
        {
            _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        AcceptButtonBehaviour();
    }

    ///////////////////////////////////////////////////////////////////////// GETTER ////////////////////////////////////////////////////////////////////////////
    
    public string GetCustomerName()
    {
        return customerName;
    }

    public string GetCustomerMail()
    {
        return customerMail;
    }

    public string GetLastPanelBeforeBlueprintPopUp()
    {
        return lastPanelBeforeBlueprintPopUp;
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////////////////

    void AcceptButtonBehaviour()
     {
        if(_CustomerNameInputText.text == "" || _CustomerMailInputText.text == "")
        {
            _AcceptButton.interactable = false;
        }
        else
        {
            _AcceptButton.interactable = true;
            customerName = _CustomerNameInputText.text;
            customerMail = _CustomerMailInputText.text;
            lastPanelBeforeBlueprintPopUp = _HUDManagerScript.GetLastPanelOnScreenName();
        }
     }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
