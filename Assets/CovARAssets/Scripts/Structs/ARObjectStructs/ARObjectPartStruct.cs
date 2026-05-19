using UnityEngine; // Només necessitem Unity per al GameObject i el Serializable

[System.Serializable]
public class St_ARObjectPart
{
    [Header("Type of ARObject part:")]
    public E_ARObjectParts _ARObjectPartType; // Si l'error es queda aquí, revisa que l'Enum es digui EXACTAMENT així.

    [Header("ARObject part reference: (Automatic)")]
    public GameObject _ARObjectPartReference;
}