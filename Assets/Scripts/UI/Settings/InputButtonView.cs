using System;
using System.Linq;
using Impingement.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI.Settings
{
    public class InputButtonView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _buttonName;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Color _activeColor;
        [SerializeField] private InputManager _inputManager;
        private Color _defaultColor;
        private bool _isListening;

        private void Start()
        {
            _defaultColor = _buttonImage.color;
        }

        public void Setup(string buttonName, KeyCode buttonKeyCode)
        {
            _buttonName.text = buttonName;
            _buttonText.text = _inputManager.GetButtonKeyCodeAsString(buttonKeyCode);
        }

        private void Update()
        {
            if (_isListening)
            {
                _buttonImage.color = _activeColor;
                if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape))
                {
                    foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                    {
                        if(Input.GetKeyDown(keyCode))
                        {
                            PlayerPrefs.SetString(_buttonName.text, keyCode.ToString());
                            _buttonText.text = _inputManager.GetButtonKeyCodeAsString(keyCode);
                            _inputManager.ChangeHotKey(_buttonName.text, keyCode);
                            _isListening = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                _buttonImage.color = _defaultColor;
            }
        }

        public void ButtonClick()
        {
            _isListening = !_isListening;
        }
    }
}