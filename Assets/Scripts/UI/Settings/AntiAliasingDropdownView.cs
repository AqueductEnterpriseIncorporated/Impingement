using TMPro;
using UnityEngine;

namespace Impingement.UI.Settings
{
    public class AntiAliasingDropdownView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _aaDropdown;
        [SerializeField] private TMP_Dropdown _qualityPresetDropdown;
        
        private void Start()
        {
            _aaDropdown.value = 
                    PlayerPrefs.GetInt("AntiAliasingPreference");
        }

        public void SetAntiAliasing()
        {
            int aaIndex = _aaDropdown.value;
            QualitySettings.antiAliasing = aaIndex;
            //_qualityPresetDropdown.value = 6;
            PlayerPrefs.SetInt("AntiAliasingPreference", 
                _aaDropdown.value);
        }
    }
}