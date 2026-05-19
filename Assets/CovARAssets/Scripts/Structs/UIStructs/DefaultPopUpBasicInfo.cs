using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class St_BasicPopUpData
{
    
    public Sprite popUpImage;
    public string popUpDescription;


    // Constructor per inicialitzar els camps
    public St_BasicPopUpData(Sprite puImage, string puDescription)
    {
        this.popUpImage = puImage;
        this.popUpDescription = puDescription;
    }
}

