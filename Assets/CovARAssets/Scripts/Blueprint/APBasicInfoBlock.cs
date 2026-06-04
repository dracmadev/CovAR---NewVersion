using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class APBasicInfoBlock : MonoBehaviour
{
    [Header("[ASTRALPOOL] BASIC INFO BLOCK")]
    [Space()]
    [Header("MODEL INFO:")]
    [SerializeField] private TMP_Text _CurrentModelText;
    [SerializeField] private Image _CurrentModelImage;
    [Header("LAMAS INFO:")]
    [SerializeField] private TMP_Text _CurrentLamasText;
    [SerializeField] private Image _CurrentLamasImage;
    [Header("COLORS:")]
    [SerializeField] private TMP_Text _CurrentModelColorText;
    [SerializeField] private TMP_Text _CurrentLamasColorText;
    [SerializeField] private TMP_Text _CurrentTopCladdingColorText;
    [SerializeField] private TMP_Text _CurrentSidesCladdingColorText;
    [SerializeField] private CanvasGroup _CanvasGroupTopCladdingBlock;
    [SerializeField] private CanvasGroup _CanvasGroupSidesCladdingBlock;
    [Header("DIMENSIONS:")]
    [SerializeField] private TMP_InputField _CurrentModelLenghtInputText;
    [SerializeField] private TMP_InputField _CurrentLamasLenghtInputText;
    [SerializeField] private Image _CurrentBlueprintModelImage;
    [Header("ANNOTATIONS:")]
    [SerializeField] private TMP_InputField _AnnotationsInputText;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
