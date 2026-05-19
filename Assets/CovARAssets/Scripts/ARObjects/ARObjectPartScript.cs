using UnityEngine;


public class ARObjectPartScript : MonoBehaviour
{
    [Header("AR OBJECT PART SCRIPT")]
    [Space(25)]
    [Header("ARObject parts data:")]
    [SerializeField] private St_ARObjectPart _ARObjectPartData;
    [Header("Other important object reference: (Automatic)")]
    [SerializeField] private GameObject _CurrentARObject;

    void Start()
    {
        InitObjectPart();
    }



    void InitObjectPart()
    {
        _ARObjectPartData._ARObjectPartReference = this.gameObject;
    }

    public St_ARObjectPart GetARObjectPartData()
    {
        return _ARObjectPartData;
    }
}
