using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class APTopInformationBlock : MonoBehaviour
{
    [Header("[ASTRALPOOL] TOP INFORMATION BLOCK")]
    [Space()]
    [Header("CUSTOMER INFO:")]
    [SerializeField] private TMP_Text _CurrentCustomerNameText;
    [SerializeField] private TMP_Text _CurrentCustomerEmailText;
    [Header("OTHER INFO:")]
    [SerializeField] private TMP_Text _CurrentOrderDate;
    [SerializeField] private TMP_Text _CurrentOrderLocation;
    [SerializeField] private Image _CurrentCompanyImage;

    [Header("Companys sprite images:")]
    [SerializeField] private Sprite _AstralPoolSprite;
    [SerializeField] private Sprite _BACPoolSystemSprite;


    private WebRequestManager _WebRequestManager;

    /////////////////////////////////////////////////////////////////// DEFAULT FUNCTIONS //////////////////////////////////////////////////////////////////////////
    void Start()
    {

    }

    void Update()
    {

    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////////////

    public void InitTopInformationBlock(St_OrderData _OrderData, string orderLocation)
    {
        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        _CurrentCustomerNameText.text = _OrderData._CustomerName;
        _CurrentCustomerEmailText.text = _OrderData._CustomerMail;
        _CurrentOrderDate.text = _OrderData._CurrentDate;
        _CurrentOrderLocation.text = orderLocation;

        _WebRequestManager.SetBasicOrderData(_OrderData);


        if(_CurrentCompanyImage != null)
        {
            switch(_WebRequestManager.GetCurrentCompany())
            {
                case E_CompanyType.Astralpool:
                    _CurrentCompanyImage.sprite = _AstralPoolSprite;
                    break;
                    case E_CompanyType.BACPoolSystems: 
                    _CurrentCompanyImage.sprite = _BACPoolSystemSprite;
                    break;
            }
        }

    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// SETTER /////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
