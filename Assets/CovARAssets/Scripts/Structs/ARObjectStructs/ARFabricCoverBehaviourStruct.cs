using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class St_ARObjectFabricCoverData
{
    [Header("FABRIC COVER PARTS DATA:")]
    public float _MinDistance; // ditancia minima
    public float _MaxDistance; // distancia maxima
    public bool bOffsetDistanceToLimits; //Afegim un offset al principi i al final?
    public float _DistanceToLimits;//True -> el offset en qüestió
    public float _DistanceToLimits02;//True -> el offset en qüestió
    public bool bSpawnObjectsByNumber; // creem els objectes segons un número determinat o segons la distancia entre ells
    public int _NumberOfObjectsToSpawn; //numero d'objectes a crear
    public bool bSpawnObjectsByDistance; // creem els objectes segons un número determinat o segons la distancia entre ells
    public float _DistanceBetweenObjects; // distancia entre objectes a crear
    public float _DistanceBetweenObjects02;// distancia entre objectes a crear

}