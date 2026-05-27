using UnityEngine;


public class ARObjectPartScript : MonoBehaviour
{
    [Header("AR OBJECT PART SCRIPT")]
    [Space(25)]
    [Header("ARObject general parts data:")]
    [SerializeField] private E_ARObjectGeneralParts _ARObjectGeneralPart;
    //SPECIFIC PART
    [Space()]
    [Header("ARObject specific parts data:")]
    [SerializeField] private St_ARObjectPart _ARObjectPartData;
    //BONES PART
    [Space()]
    [Header("ARObject Bone data:")]
    [SerializeField] private St_ARObjectBonesData _ARObjectBoneData;
    //Feedback PART
    [Space()]
    [Header("ARObject specific parts data:")]
    [SerializeField] private St_ARObjectFeedbackPartData _ARObjectBoneParentData;
    [Space()]
    [Header("For multiple bone parents decals")]
    [SerializeField] private St_ARObjectMultipleBoneParent _ARObjectMultipleBoneParentData;
    [Space()]
    [Header("Other important object reference: (Automatic)")]
    [SerializeField] private GameObject _CurrentARObject;

    ARObjectManager _ARObjectManager;
    bool bAlreadyAdded;
    void Start()
    {
        InitObjectPart();
    }


    void InitObjectPart()
    {
        _ARObjectPartData._ARObjectPartReference = this.gameObject;

        
        
    }

    /////////////////////////////////////////////////////////////////////// GETTERS /////////////////////////////////////////////////////////////////////////////////////
    public St_ARObjectPart GetARObjectPartData()
    {
        return _ARObjectPartData;
    }

    public E_ARObjectGeneralParts GetARGeneralObjectPart()
    {
        return _ARObjectGeneralPart;
    }

    public St_ARObjectBonesData GetBoneData()
    {
        return _ARObjectBoneData;
    }

    public St_ARObjectFeedbackPartData GetBoneParentData()
    {
        return _ARObjectBoneParentData;
    }

    public St_ARObjectMultipleBoneParent GetMultipleBoneParentData()
    {
        return _ARObjectMultipleBoneParentData;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /////////////////////////////////////////////////////////// ON ENABLE ON DESTROY //////////////////////////////////////////////////////////////////////////
    private void OnEnable()
    {
        if (GameObject.FindGameObjectWithTag("ARObjectManager"))
        {
            _ARObjectManager = GameObject.FindGameObjectWithTag("ARObjectManager").GetComponent<ARObjectManager>();
        }

      
    }

    private void OnDestroy()
    {
       
    }

    /*
    void AddAndDeleteARObjectPartsList(E_ARObjectParts part, bool add)
    {
        if(_ARObjectGeneralPart == E_ARObjectGeneralParts.PoolSpecificPart)
        {

            if(_ARObjectPartData._ARObjectPartType == part)
            {
                if(_ARObjectManager != null)
                {
                    if(add)
                    {
                        //Debug.Log("****AD******");
                        _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().AddObjectToARObjectPartsList(this.gameObject, false);
                        bAlreadyAdded = true;
                    }
                    else
                    {
                        //Debug.Log("****REMOVE******");
                        _ARObjectManager.GetCurrentARObject().GetComponent<ARObjectScript>().RemoveObjectFromARPartsList(this.gameObject);
                    }
                }
                else
                {
                   // Debug.Log("*******************NULL?*********************");
                }
            }
            else
            {
               // Debug.Log("[" + _ARObjectPartData._ARObjectPartType + "] -> ******** NOT A CORRECT PART ************************");
            }
        }

    }
    */
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
