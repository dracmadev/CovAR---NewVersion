using UnityEngine;

[System.Serializable]
public class St_ARObjectFeedbackPartData
{
    [HideInInspector] public ARObjectPartScript partScript;

    [Header("Name that Bone parent part from A to H:")]
    public E_ARObjectFeedbackParts _ARObjectBoneParentType;
    [Header("Wich side is this bone related?")]
    public E_SidesInLetters _SideRelated;
    [Header("Bones Related?")]
    public GameObject[] _ARObjectBones;
    [Header("Which direction?")]
    public E_ARObjectReSizeDirections _ARObjectReSizeDirection;
    [Header("This side is movable?:")]
    public bool _bIsMovable = true;
    [Header("This side's shelf is extensible?:")]
    public bool _bIsShelfExtensible = true;
    [Header("If the side is concave, assign related side. Else, leave it null")]
    public ARObjectPartScript relatedSideBoneParent;

    [Header("Values: (Automatic)")]
    public float currentValue;
    public float minValue;
    public float maxValue;
    public float currentShelfWidth;
    [HideInInspector] public Vector3 startingPosition;
}