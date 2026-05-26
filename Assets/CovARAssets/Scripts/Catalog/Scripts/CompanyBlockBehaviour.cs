using UnityEngine;
using UnityEngine.UI;

public class CompanyBlockBehaviour : MonoBehaviour
{
    [Header("COMPANY BLOCK BEHAVIOUR")]
    [Space()]
    [Header("UI COMPONENTS:")]
    [SerializeField] private Image _CompanyImage;

    ARObjectManager _ARObjectManager;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///////////////////////////////////////////////////// INIT //////////////////////////////////////////////////////////////////////
    
    public void InitCompanyBlock()
    {
        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        SetImageCompany();
    }




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ///////////////////////////////////////////////////////// SETTER ////////////////////////////////////////////////////////////////
    
    void SetImageCompany()
    {
        if (_CompanyImage != null)
        {
            _CompanyImage.sprite = _ARObjectManager.GetCurrentCompanyData()._CompanyLogo;
        }
        else
        {
            Debug.LogWarning("No s'ha pogut establir la imatge de la companyia perquè la referència és null.");
        }
    }



    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
