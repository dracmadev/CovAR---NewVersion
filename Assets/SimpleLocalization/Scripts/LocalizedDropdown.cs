using TMPro;
using UnityEngine;

namespace Assets.SimpleLocalization.Scripts
{
    /// <summary>
    /// Localize dropdown component.
    /// </summary>
    [RequireComponent(typeof(TMP_Dropdown))] // Això ja estava bé!
    public class LocalizedDropdown : MonoBehaviour
    {
        public string[] LocalizationKeys;

        public void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            var dropdown = GetComponent<TMP_Dropdown>();

            if (dropdown == null) return;

            int count = Mathf.Min(LocalizationKeys.Length, dropdown.options.Count);

            for (var i = 0; i < count; i++)
            {
                dropdown.options[i].text = LocalizationManager.Localize(LocalizationKeys[i]);
            }

            if (dropdown.value < dropdown.options.Count)
            {
                dropdown.captionText.text = dropdown.options[dropdown.value].text;
            }

            dropdown.RefreshShownValue();
        }
    }
}