using System;
using Impingement.Control;
using UnityEngine;
using UnityEngine.Audio;

namespace Impingement.UI.Settings
{
    public class InGameSettingsManager : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private string[] _parameters;
        [SerializeField] private int _defaultTextureQuality = 0;
        [SerializeField] private int _defaultAAQuality = 1;
        [SerializeField] private bool _defaultFullscreen = true;
        
        private void Start()
        {
            ApplyVolumeSettings();
            ApplyGraphicSettings();
            ApplyInputSettings();
            //Application.targetFrameRate = 140;
        }

        private void ApplyInputSettings()
        {
            _inputManager.Setup();
        }

        private void ApplyGraphicSettings()
        {
            
            if (PlayerPrefs.HasKey("TextureQualityPreference"))
            {
                QualitySettings.masterTextureLimit =
                    PlayerPrefs.GetInt("TextureQualityPreference");
            }
            else
            {
                QualitySettings.masterTextureLimit = _defaultTextureQuality;
                PlayerPrefs.SetInt("TextureQualityPreference", 
                    _defaultTextureQuality);
            }

            if (PlayerPrefs.HasKey("AntiAliasingPreference"))
            {
                QualitySettings.antiAliasing = PlayerPrefs.GetInt("AntiAliasingPreference");
            }
            else
            {
                QualitySettings.antiAliasing = _defaultAAQuality;
                PlayerPrefs.SetInt("AntiAliasingPreference", 
                    _defaultAAQuality);
            }

            if (PlayerPrefs.HasKey("FullscreenPreference"))
            {
                Screen.fullScreen =
                    Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
            }
            else
            {
                Screen.fullScreen = _defaultFullscreen;
                PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(_defaultFullscreen));
            }
        }
        

        private void ApplyVolumeSettings()
        {
            foreach (var parameter in _parameters)
            {
                var value = PlayerPrefs.GetFloat(parameter);
                _mixer.SetFloat(parameter, value);
            }
        }
    }
}