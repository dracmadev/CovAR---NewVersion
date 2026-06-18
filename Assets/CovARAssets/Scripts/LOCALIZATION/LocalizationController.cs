using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using TMPro;
using System;

public class LocalizationController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _LanguageDropdown;
    private const string LangKey = "Language";
   

    private void Awake()
    {
        LocalizationManager.Read();

        // Si hi ha guardat, utilitzem el valor guardat
        if (PlayerPrefs.HasKey(LangKey))
        {
            string savedLang = PlayerPrefs.GetString(LangKey);
            LocalizationManager.Language = savedLang;
        }
        else
        {
           
            // Detectar idioma del sistema i assignar l'enum corresponent
            E_LanguageEnum systemLang = ConvertSystemLanguage(Application.systemLanguage);

            LocalizationManager.Language = systemLang.ToString();
            PlayerPrefs.SetString(LangKey, LocalizationManager.Language);
            PlayerPrefs.Save();
            
            

        }
    }

    private void Start()
    {
        // Sincronitzar dropdown
        _LanguageDropdown.value = GetDropdownIndexFromLanguage(LocalizationManager.Language);
        _LanguageDropdown.RefreshShownValue();

        _LanguageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {

            E_LanguageEnum selectedLang = (E_LanguageEnum)index;

            LocalizationManager.Language = selectedLang.ToString();
            PlayerPrefs.SetString(LangKey, LocalizationManager.Language);
            PlayerPrefs.Save();

    }

    private E_LanguageEnum ConvertSystemLanguage(SystemLanguage sysLang)
    {
        switch (sysLang)
        {
            case SystemLanguage.Spanish: return E_LanguageEnum.Spanish;
            case SystemLanguage.French: return E_LanguageEnum.French;
            case SystemLanguage.English: return E_LanguageEnum.English;
 
            default: return E_LanguageEnum.Spanish; // fallback
        }
    }

 


    private int GetDropdownIndexFromLanguage(string langStr)
    {
       
        foreach (E_LanguageEnum lang in Enum.GetValues(typeof(E_LanguageEnum)))
        {
            if (lang.ToString() == langStr)
                return (int)lang;
        }

        // Si falla, per defecte English
        return (int)E_LanguageEnum.Spanish;
       
    }
}