using UnityEngine;

public class APBlueprintManager : MonoBehaviour
{
    [Header("ASTRALPOOL BLUEPRINT MANAGER")]
    [Space()]
    [Header("BLOCKS REFERENCES:")]
    [SerializeField] private APTopInformationBlock _APTopInformationBlock;
    [SerializeField] private APBasicInfoBlock _APBasicInfoBlock;
    [SerializeField] private APRelatedProductsBlock _APRelatedProductsBlock;
    [Header("Other references:")]
    [SerializeField] private ARObjectManager _ARObjectManager;
    [SerializeField] private HUDManagerScript _HUDManagerScript;
    void Start()
    {
        InitAPCatalog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////////////

    void InitAPCatalog()
    {
        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

        if (GameObject.FindGameObjectWithTag("HUD"))
        {
            _HUDManagerScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManagerScript>();
        }




    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////// BEHAVIOUR /////////////////////////////////////////////////////////////////

    public void OnCickReturnToLastSubPanelFromBlueprint()
    {
        if(_HUDManagerScript != null)
        {
            _HUDManagerScript.TravelToLastPanel();
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
