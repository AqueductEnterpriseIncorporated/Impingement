using Impingement.Control;
using UnityEngine;

namespace Impingement.UI.Settings
{
    public class InputSettingsController : MonoBehaviour
    {
        [SerializeField] private Transform _contentParent;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private InputButtonView _inputButtonView;

        private void OnEnable()
        {
            DestroyChilds();

            foreach (var button in _inputManager.ButtonKeysList)
            {
                KeyCode buttonKeyCode;
                
                if (PlayerPrefs.HasKey(button.ButtonName))
                {
                    buttonKeyCode = InputManager.ConvertStringToKeyCode(PlayerPrefs.GetString(button.ButtonName));
                }
                else
                {
                    buttonKeyCode = button.ActiveButtonKeyCode;
                }
                var itemList = Instantiate(_inputButtonView, _contentParent);
                itemList.Setup(button.ButtonName, buttonKeyCode);
            }
        }

        private void DestroyChilds()
        {
            for (int i = 0; i < _contentParent.childCount; i++)
            {
                Destroy(_contentParent.GetChild(i).gameObject);
            }
        }

        public void ResetAllKeybindings()
        {
            DestroyChilds();

            foreach (var button in _inputManager.ButtonKeysList)
            {
                PlayerPrefs.SetString(button.ButtonName, button.DefaultButtonKeyCode.ToString());
                _inputManager.ChangeHotKey(button.ButtonName, button.DefaultButtonKeyCode);
                var itemList = Instantiate(_inputButtonView, _contentParent);
                itemList.Setup(button.ButtonName, button.ActiveButtonKeyCode);
            }
        }
    }
}