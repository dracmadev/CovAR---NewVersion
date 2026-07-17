using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

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
    [SerializeField] private Color _WrongColorFeedback;
    [SerializeField] private Color _CurrentColor;
    [SerializeField] private Image _CurrentBlueprintModelImage;
    [Header("ANNOTATIONS:")]
    [SerializeField] private TMP_InputField _AnnotationsInputText;

   
    ARObjectManager _ARObjectManager;
    private WebRequestManager _WebRequestManager;

    /////////////////////////////////////////////////////////////////// DEFAULT FUNCTIONS //////////////////////////////////////////////////////////////////////////
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// INIT ///////////////////////////////////////////////////////////////////////////

    public void InitBasicInfo(ARObjectManager objManager)
    {
        _ARObjectManager = objManager;

        if (GameObject.FindGameObjectWithTag("WebRequestManager"))
        {
            _WebRequestManager = GameObject.FindGameObjectWithTag("WebRequestManager").GetComponent<WebRequestManager>();
        }

        InitInfo();
    }

    void InitInfo()
    {
        if (_ARObjectManager == null) return;

        //CURRENT MODEL (Name, image, blueprint image & color)
        _CurrentModelText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey); 
        _CurrentModelImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialSprite;
        _CurrentBlueprintModelImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentModelModel._BlueprintModelImage;
        _CurrentModelColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey);

        _WebRequestManager.SetModelBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey));

        //CURRENT LAMAS (Name, image)
        _CurrentLamasText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey);
        _CurrentLamasImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialSprite;
        _CurrentLamasColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey);

        _WebRequestManager.SetLamasBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey));

        //TOP CLADDING
        //Debug.Log("TOP CLADDING MATERIAL: " + _ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialType);
        if (_ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialType != E_MaterialType.None)
        {
            _CanvasGroupTopCladdingBlock.alpha = 1f;
            _CurrentTopCladdingColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialKey);
            _WebRequestManager.SetTopCladdingColor(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialKey));
        }
        else
        {
            _CanvasGroupTopCladdingBlock.alpha = 0f;
            _WebRequestManager.SetTopCladdingColor("None");
        }

        //SIDES CLADDING
        if (_ARObjectManager.GetCovARObjectData()._CurrentSidesCladdingMaterial._MaterialType != E_MaterialType.None)
        {
            _CanvasGroupSidesCladdingBlock.alpha = 1f;
            _CurrentSidesCladdingColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentSidesCladdingMaterial._MaterialKey);
            _WebRequestManager.SetSidesCladdingColor(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentSidesCladdingMaterial._MaterialKey));
        }
        else
        {
            _CanvasGroupSidesCladdingBlock.alpha = 0f;
            _WebRequestManager.SetSidesCladdingColor("None");
        }

        //DIMENSIONS
        _CurrentModelLenghtInputText.text = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString();
        _CurrentLamasLenghtInputText.text = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString();

        _WebRequestManager.SetDimensions(_ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString() + " x " + _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString());
      
        InitDimensionsInputTextBehaviour();

    }

    void InitDimensionsInputTextBehaviour()
    {
        _CurrentModelLenghtInputText.onValueChanged.RemoveListener(delegate { OnValueModelLenghtChangeBehaviour(); });
        _CurrentModelLenghtInputText.onValueChanged.AddListener(delegate { OnValueModelLenghtChangeBehaviour(); });

        _CurrentLamasLenghtInputText.onValueChanged.RemoveListener(delegate { OnValueLamasLenghtChangeBehaviour(); });
        _CurrentLamasLenghtInputText.onValueChanged.AddListener(delegate { OnValueLamasLenghtChangeBehaviour(); });
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// GETTER /////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////// SETTER /////////////////////////////////////////////////////////////////////////




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////////////////////////////////// bEHAVIOUR /////////////////////////////////////////////////////////////////////////

    public void OnValueModelLenghtChangeBehaviour()
    {
        OnValueChangedCheck(_CurrentModelLenghtInputText);
    }

    public void OnValueLamasLenghtChangeBehaviour()
    {
        OnValueChangedCheck(_CurrentLamasLenghtInputText);
    }

    void OnValueChangedCheck(TMP_InputField inputField)
    {
        // Si l'input està buit (perquè l'acabem d'esborrar a l'animació), 
        // tornem al color normal i sortim per evitar bucles infinits
        if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.image.color = _CurrentColor;
            return;
        }

        string inputText = inputField.text.Trim();
        inputText = inputText.Replace(',', '.');

        if (float.TryParse(inputText, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedValue))
        {
            // Si el número és vàlid, posem el color correcte
            inputField.image.color = _CurrentColor;
        }
        else
        {
            // Si està malament, posem el color d'error i cridem la vibració/escala
            inputField.image.color = _WrongColorFeedback;
            WrongValueAnim(inputField);
        }
    }

    void WrongValueAnim(TMP_InputField inputField)
    {
        if (inputField == null) return;

        // Aturem qualsevol animació d'escala prèvia en aquest transform i forcem la mida original
        inputField.transform.DOKill(true);
        inputField.transform.localScale = Vector3.one; // Força a tornar a escala 1

        Sequence wrongSequence = DOTween.Sequence();

        // 1. Es fa gran fins a 1.05 en un eix horitzontal/vertical en 0.1 segons
        wrongSequence.Append(inputField.transform.DOScale(1.05f, 0.1f).SetEase(Ease.OutQuad));

        // 2. Torna immediatament a la seva mida original (1.0) en 0.1 segons més
        wrongSequence.Append(inputField.transform.DOScale(1.0f, 0.1f).SetEase(Ease.InQuad));

        // 3. Quan s'acaba tot l'efecte de pols, esborrem el text i reactivem l'input
        wrongSequence.OnComplete(() =>
        {
            inputField.text = "";
            inputField.ActivateInputField();
        });
    }

    public void OnUpdateDimensions()
    {
        _WebRequestManager.SetDimensions(_CurrentModelLenghtInputText.text.ToString() + " x " + _CurrentLamasLenghtInputText.text.ToString());
    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
