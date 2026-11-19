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

    private Mesh _bakedColliderMesh;

    void Start()
    {
        InitObjectPart();
        RefreshSkinnedMeshCollider();
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
        if (_bakedColliderMesh != null)
            Destroy(_bakedColliderMesh);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////// UPDATE COLLIDERS //////////////////////////////////////////////////////////////////////////

    public void RefreshSkinnedMeshCollider()
    {
        if (_ARObjectGeneralPart != E_ARObjectGeneralParts.PoolSpecificPart)
            return;

        if (!TryGetComponent<SkinnedMeshRenderer>(out var skinnedRenderer) ||
            skinnedRenderer.sharedMesh == null ||
            !TryGetComponent<MeshCollider>(out var meshCollider))
        {
            return;
        }

        if (_bakedColliderMesh == null)
            _bakedColliderMesh = new Mesh { name = $"{gameObject.name} Drag Mesh" };

        meshCollider.sharedMesh = null;
        skinnedRenderer.BakeMesh(_bakedColliderMesh, useScale: true);
        meshCollider.sharedMesh = _bakedColliderMesh;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
