using System;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI.Settings
{
    public class FullscreenToggleView : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;

        private void Start()
        {
            var prefsValue = PlayerPrefs.GetInt("FullscreenPreference");
            _toggle.isOn = prefsValue == 1;
        }
        
        public void Toggle()
        {
            Screen.fullScreen = _toggle.isOn ;
            PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(_toggle.isOn));
        }
    }
}