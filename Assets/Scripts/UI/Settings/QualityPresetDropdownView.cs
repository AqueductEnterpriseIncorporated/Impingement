using TMPro;
using UnityEngine;

namespace Impingement.UI.Settings
{
    public class QualityPresetDropdownView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _qualityPresetDropdown;
        [SerializeField] private TMP_Dropdown _textureDropdown;
        [SerializeField] private TMP_Dropdown _aaDropdown;
        [SerializeField] private int _defaultQualityPreset = 3;
        private int _qualityIndex;

        private void Start()
        {
            if (PlayerPrefs.HasKey("QualitySettingPreference"))
            {
                _qualityIndex =
                    PlayerPrefs.GetInt("QualitySettingPreference");
                SetQuality();
                _qualityPresetDropdown.value = _qualityIndex;
            }
            else
            {
                PlayerPrefs.SetInt("QualitySettingPreference", 
                    _defaultQualityPreset);
                _qualityPresetDropdown.value = _defaultQualityPreset;
            }
        }

        public void SetQuality()
        { 
            _qualityIndex = _qualityPresetDropdown.value;

            if (_qualityIndex != 6)
                QualitySettings.SetQualityLevel(_qualityIndex);
            switch (_qualityIndex)
            {
                case 0: // quality level - very low
                    _textureDropdown.value = 3;
                    _aaDropdown.value = 0;
                    break;
                case 1: // quality level - low
                    _textureDropdown.value = 2;
                    _aaDropdown.value = 0;
                    break;
                case 2: // quality level - medium
                    _textureDropdown.value = 1;
                    _aaDropdown.value = 0;
                    break;
                case 3: // quality level - high
                    _textureDropdown.value = 0;
                    _aaDropdown.value = 0;
                    break;
                case 4: // quality level - very high
                    _textureDropdown.value = 0;
                    _aaDropdown.value = 1;
                    break;
                case 5: // quality level - ultra
                    _textureDropdown.value = 0;
                    _aaDropdown.value = 2;
                    break;
            }

            PlayerPrefs.SetInt("QualitySettingPreference",
                _qualityIndex);
        }
    }
}