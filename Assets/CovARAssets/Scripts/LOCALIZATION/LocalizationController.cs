using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class LocalizationController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _LanguageDropdown;
    private const string LangKey = "Language";

    private void Awake()
    {
        LocalizationManager.Read();

        if (PlayerPrefs.HasKey(LangKey))
        {
            string savedLang = PlayerPrefs.GetString(LangKey);
            LocalizationManager.Language = savedLang;
        }
        else
        {
            E_LanguageEnum systemLang = ConvertSystemLanguage(Application.systemLanguage);
            LocalizationManager.Language = systemLang.ToString();
            PlayerPrefs.SetString(LangKey, LocalizationManager.Language);
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        // 1. Configurem les opcions del Dropdown directament per codi 
        // per assegurar-nos que no depenem del que hi hagi escrit a l'inspector.
        ConfigurarDropdownVisual();

        // 2. Sincronitzem el valor actual del joc amb el Dropdown
        _LanguageDropdown.value = GetDropdownIndexFromLanguage(LocalizationManager.Language);
        _LanguageDropdown.RefreshShownValue();

        _LanguageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void ConfigurarDropdownVisual()
    {
        _LanguageDropdown.ClearOptions();

        // Creem la llista de textos exactament com vols que els vegi el jugador
        List<string> opcionsVisuals = new List<string>
        {
            "Español", // Posició 0 -> Correspòn a E_LanguageEnum.Spanish
            "Français" // Posició 1 -> Correspòn a E_LanguageEnum.French
            //"English"   // Posició 2 -> Correspòn a E_LanguageEnum.English
        };

        _LanguageDropdown.AddOptions(opcionsVisuals);
    }

    private void OnDropdownValueChanged(int index)
    {
        // L'índex de la llista (0, 1, 2) coincideix amb el valor de l'enum (Spanish, French, English)
        E_LanguageEnum selectedLang = (E_LanguageEnum)index;

        LocalizationManager.Language = selectedLang.ToString(); // "Spanish", "French" o "English"
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
            default: return E_LanguageEnum.Spanish;
        }
    }

    private int GetDropdownIndexFromLanguage(string langStr)
    {
        foreach (E_LanguageEnum lang in Enum.GetValues(typeof(E_LanguageEnum)))
        {
            if (lang.ToString() == langStr)
                return (int)lang;
        }

        return (int)E_LanguageEnum.Spanish;
    }
}