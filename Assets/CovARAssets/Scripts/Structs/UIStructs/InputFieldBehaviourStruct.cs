using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public class St_InputFieldBehaviourStruct
{
    [Space()]
    [Header("INPUT FIELD BEHAVIOUR:")]
    [Space()]
    [Header("IntInRange -> Minimum & Maximum values:")]
    public int _min = 0;
    public int _max = 0;
    [Header("MaxNumberLenght -> maximum lenght value:")]
    public int _maxLenght = 0;
    [Header("Remeber me behaviour:")]
    public bool bRemeberMe;
    public string playerPrefName;
    [Header("Eye for password:")]
    public GameObject _eyeGameObj;
    public Sprite _eyeOpen;
    public Sprite _eyeClosed;
    public Color _eyeClosedColor;
    public Color _eyeOpenColor;
    [Header("Multilanguage variables")]
    public string key;
}
