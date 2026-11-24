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
    [SerializeField] private CanvasGroup _ModelBlockCG;
    [Header("LAMAS INFO:")]
    [SerializeField] private TMP_Text _CurrentLamasText;
    [SerializeField] private Image _CurrentLamasImage;
    [SerializeField] private CanvasGroup _LamasModelBlockCG;
    [Header("COLORS:")]
    [SerializeField] private TMP_Text _CurrentModelColorText;
    [SerializeField] private TMP_Text _CurrentLamasColorText;
    [SerializeField] private TMP_Text _CurrentTopCladdingColorText;
    [SerializeField] private TMP_Text _CurrentSidesCladdingColorText;
    [SerializeField] private CanvasGroup _ModelColorBlockCG;
    [SerializeField] private CanvasGroup _LamasModelColorBlockCG;
    [SerializeField] private CanvasGroup _CanvasGroupTopCladdingBlock;
    [SerializeField] private CanvasGroup _CanvasGroupSidesCladdingBlock;
    [Header("DIMENSIONS:")]
    [SerializeField] private TMP_InputField _CurrentModelLenghtInputText;
    [SerializeField] private TMP_InputField _CurrentLamasLenghtInputText;
    [SerializeField] private Color _WrongColorFeedback;
    [SerializeField] private Color _CurrentColor;
    [SerializeField] private Image _CurrentBlueprintModelImage;
    [SerializeField] private Image _TopModelViewImage;
    [SerializeField] private Sprite _GroundRollerTopViewImage;
    [SerializeField] private Sprite _FabricCoverTopViewImage;
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

        if(_WebRequestManager == null) return;

        if (_WebRequestManager.GetCurrentCompany() == E_CompanyType.Astralpool) // -> ASTRALPOOL
        {
            //MODEL DATA
            SetModelBlockData(true);
            SetModelColorBlockData(true);
            _WebRequestManager.SetModelBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey));

            //LAMAS DATA
            SetLamasBlockData(true);
            SetLamasColorBlockData(true);
            _WebRequestManager.SetLamasBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey));
            
            //BLUEPRINT IMAGE
            SetBlueprintTechnicalImage(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._BlueprintModelImage);
            SetBlueprintTopModelImage(_GroundRollerTopViewImage);

            //TOP CLADDING
            if (_ARObjectManager.GetCovARObjectData()._CurrentModelProduct._ProductType == E_ProductType.BencheAndCladdings && _ARObjectManager.GetCovARObjectData()._CurrentCompany._CompanyType == E_CompanyType.Astralpool)
            {
                SetTopCladdingData(true);
            }
            else
            {
                SetTopCladdingData(false);
            }

            //SIDES CLADDING
            if (_ARObjectManager.GetCovARObjectData()._CurrentModelProduct._ProductType == E_ProductType.BencheAndCladdings && _ARObjectManager.GetCovARObjectData()._CurrentCompany._CompanyType == E_CompanyType.Astralpool)
            {
                SetSidesCladdingData(true);
            }
            else
            {
                SetSidesCladdingData(false);
            }

            //DIMENSIONS
            SetDimensionsData();
            _WebRequestManager.SetDimensions(_ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString() + " x " + _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString());

            //INIT
            InitDimensionsInputTextBehaviour();

        }
        else // -> BAC POOL SYSTEMS
        {
             switch(_ARObjectManager.GetCovARObjectData()._CurrentModelProduct._ProductType)
             {
                case E_ProductType.DeckMountedSlattedCover:
                    
                    //MODEL DATA
                    SetModelBlockData(true);
                    SetModelColorBlockData(false);
                    _WebRequestManager.SetModelBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey));

                    //LAMAS DATA
                    SetLamasBlockData(true);
                    SetLamasColorBlockData(true);
                    _WebRequestManager.SetLamasBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey));

                    //BLUEPRINT IMAGE
                    SetBlueprintTechnicalImage(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._BlueprintModelImage);
                    SetBlueprintTopModelImage(_GroundRollerTopViewImage);

                    SetTopCladdingData(false);
                    SetSidesCladdingData(false);

                    //DIMENSIONS
                    SetDimensionsData();
                    _WebRequestManager.SetDimensions(_ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString() + " x " + _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString());

                    //INIT
                    InitDimensionsInputTextBehaviour();


                    break;
                case E_ProductType.DeckMountedIsolaCover:

                    //MODEL DATA
                    SetModelBlockData(true);
                    SetModelColorBlockData(false);
                    _WebRequestManager.SetModelBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey));

                    //LAMAS DATA
                    SetLamasBlockData(true);
                    SetLamasColorBlockData(true);
                    _WebRequestManager.SetLamasBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey));

                    //BLUEPRINT IMAGE
                    SetBlueprintTechnicalImage(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._BlueprintModelImage);
                    SetBlueprintTopModelImage(_GroundRollerTopViewImage);

                    SetTopCladdingData(false);
                    SetSidesCladdingData(false);

                    //DIMENSIONS
                    SetDimensionsData();
                    _WebRequestManager.SetDimensions(_ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString() + " x " + _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString());

                    //INIT
                    InitDimensionsInputTextBehaviour();

                    break;
                case E_ProductType.BencheAndCladdings:

                    //MODEL DATA
                    SetModelBlockData(true);

                    switch(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelType)
                    {
                       case E_ModelType.BAC_CladdingDavos: SetModelColorBlockData(false); break;
                       case E_ModelType.BAC_CladdingLido: SetModelColorBlockData(true); break;
                       case E_ModelType.BAC_CladdingQuadro: SetModelColorBlockData(true); break;
                    }
                    
                    _WebRequestManager.SetModelBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey));

                    //LAMAS DATA
                    SetLamasBlockData(true);
                    SetLamasColorBlockData(true);
                    _WebRequestManager.SetLamasBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey));

                    //BLUEPRINT IMAGE
                    SetBlueprintTechnicalImage(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._BlueprintModelImage);
                    SetBlueprintTopModelImage(_GroundRollerTopViewImage);

                    SetTopCladdingData(false);
                    SetSidesCladdingData(false);

                    //DIMENSIONS
                    SetDimensionsData();
                    _WebRequestManager.SetDimensions(_ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString() + " x " + _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString());

                    //INIT
                    InitDimensionsInputTextBehaviour();

                    break;
                case E_ProductType.FabricCover:

                    //MODEL DATA
                    SetModelBlockData(true);
                    SetModelColorBlockData(true);
                    _WebRequestManager.SetModelBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey));

                    //LAMAS DATA
                    SetLamasBlockData(false);
                    SetLamasColorBlockData(false);
                    _WebRequestManager.SetLamasBasicInfo(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey), LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey));

                    //BLUEPRINT IMAGE
                    SetBlueprintTechnicalImage(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._BlueprintModelImage);
                    SetBlueprintTopModelImage(_FabricCoverTopViewImage);

                    SetTopCladdingData(false);
                    SetSidesCladdingData(false);

                    //DIMENSIONS
                    SetDimensionsData();
                    _WebRequestManager.SetDimensions(_ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString() + " x " + _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString());

                    //INIT
                    InitDimensionsInputTextBehaviour();

                    break;
            }
        }

        
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

    void SetModelBlockData(bool state)
    {
        if (state)
        {
           _ModelBlockCG.alpha = 1f;
            _ModelBlockCG.gameObject.SetActive(true);
        }
        else
        {
            _ModelBlockCG.alpha = 0f;
            _ModelBlockCG.gameObject.SetActive(false);
        }
        _CurrentModelText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelModel._ModelKey);
        _CurrentModelImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialSprite;

    }

    void SetLamasBlockData(bool state)
    {
        if (state)
        {
            _LamasModelBlockCG.alpha = 1f;
            _LamasModelBlockCG.gameObject.SetActive(true);
        }
        else
        {
            _LamasModelBlockCG.alpha = 0f;
            _LamasModelBlockCG.gameObject.SetActive(false);
        }
        _CurrentLamasText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasModel._ModelKey);
        _CurrentLamasImage.sprite = _ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialSprite;
    }

    void SetModelColorBlockData(bool state)
    {
        if (state)
        {
            _ModelColorBlockCG.alpha = 1f;
            _ModelColorBlockCG.gameObject.SetActive(true);
        }
        else
        {
            _ModelColorBlockCG.alpha = 0f;
            _ModelColorBlockCG.gameObject.SetActive(false);
        }


        _CurrentModelColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentModelMaterial._MaterialKey);
    }

    void SetLamasColorBlockData(bool state)
    {
        if (state)
        {
            _LamasModelColorBlockCG.alpha = 1f;
            _LamasModelBlockCG.gameObject.SetActive(true);
        }
        else
        {
            _LamasModelColorBlockCG.alpha = 0f;
            _LamasModelBlockCG.gameObject.SetActive(false);
        }
        _CurrentLamasColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentLamasMaterial._MaterialKey);
    }

    void SetBlueprintTechnicalImage(Sprite image)
    {
        _CurrentBlueprintModelImage.sprite = image;
    }

    void SetBlueprintTopModelImage(Sprite image)
    {
        if(_TopModelViewImage != null)
        {
            _TopModelViewImage.sprite = image;
        }
    }

    void SetTopCladdingData(bool state)
    {
        if (state)
        {
            _CanvasGroupTopCladdingBlock.alpha = 1f;
            _CanvasGroupTopCladdingBlock.gameObject.SetActive(true);
            _CurrentTopCladdingColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialKey);
            _WebRequestManager.SetTopCladdingColor(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentTopCladdingMaterial._MaterialKey));
        }
        else
        {
            _CanvasGroupTopCladdingBlock.alpha = 0f;
            _WebRequestManager.SetTopCladdingColor("None");
            _CanvasGroupSidesCladdingBlock.gameObject.SetActive(false);
        }
    }

    void SetSidesCladdingData(bool state)
    {
        if (state)
        {
            _CanvasGroupSidesCladdingBlock.alpha = 1f;
            _CurrentSidesCladdingColorText.text = LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentSidesCladdingMaterial._MaterialKey);
            _WebRequestManager.SetSidesCladdingColor(LocalizationManager.Localize(_ARObjectManager.GetCovARObjectData()._CurrentSidesCladdingMaterial._MaterialKey));
            _CanvasGroupSidesCladdingBlock.gameObject.SetActive(true);
        }
        else
        {
            _CanvasGroupSidesCladdingBlock.alpha = 0f;
            _WebRequestManager.SetSidesCladdingColor("None");
            _CanvasGroupSidesCladdingBlock.gameObject.SetActive(false);
        }
    }

    void SetDimensionsData()
    {
        _CurrentModelLenghtInputText.text = _ARObjectManager.GetCovARObjectData()._CurrentARObjectLenght.ToString();
        _CurrentLamasLenghtInputText.text = _ARObjectManager.GetCovARObjectData()._CurrentLamasLenght.ToString();
    }


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
