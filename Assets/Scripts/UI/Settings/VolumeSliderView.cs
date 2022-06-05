using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Impingement.UI.Settings
{
    public class VolumeSliderView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private string _parameter;

        private void Start()
        {
            var value = PlayerPrefs.GetFloat(_parameter);
            _slider.value = value;
        }

        public void OnChangedValue()
        {
            _mixer.SetFloat(_parameter, Mathf.Log10(_slider.value) * 20);
            PlayerPrefs.SetFloat(_parameter, _slider.value);
        }
    }
}