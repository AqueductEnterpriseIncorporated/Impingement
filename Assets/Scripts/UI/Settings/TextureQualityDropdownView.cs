using TMPro;
using UnityEngine;

namespace Impingement.UI.Settings
{
    public class TextureQualityDropdownView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _textureDropdown;
        [SerializeField] private TMP_Dropdown _qualityPresetDropdown;
        
        private void Start()
        {
            _textureDropdown.value =
                PlayerPrefs.GetInt("TextureQualityPreference");
        }

        public void SetTextureQuality()
        {
            int textureIndex = _textureDropdown.value;
            QualitySettings.masterTextureLimit = textureIndex;
            //_qualityPresetDropdown.value = 6;
            PlayerPrefs.SetInt("TextureQualityPreference", 
                _textureDropdown.value);
        }
    }
}