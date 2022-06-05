using System;
using System.Collections.Generic;
using System.Linq;
using Impingement.UI;
using UnityEngine;

namespace Impingement.Control
{
    [CreateAssetMenu(menuName = ("Settings/InputManager"))]
    public class InputManager : ScriptableObject
    {
        public List<ButtonKeys> ButtonKeysList;
        public event Action<string, string> HotKeyChanged;
        Dictionary<KeyCode, Action> keyCodeDic = new Dictionary<KeyCode, System.Action>();

        const int alphaStart = 49;
        const int alphaEnd = 57;

        public void ChangeHotKey(string buttonName, KeyCode code)
        {
            ButtonKeysList.FirstOrDefault(i => i.ButtonName == buttonName)!
                .ActiveButtonKeyCode = code;
            HotKeyChanged?.Invoke(buttonName, GetButtonKeyCodeAsString(code));
        }

        public bool GetKeyDown(string buttonName)
        {
            if (!FindObjectOfType<BugPanelController>().Parent.activeSelf)
            {
                if (ButtonKeysList.All(i => i.ButtonName != buttonName))
                {
                    return false;
                }

                return Input.GetKeyDown(ButtonKeysList.FirstOrDefault(i => i.ButtonName == buttonName)!
                    .ActiveButtonKeyCode);
            }

            return false;
        }

        public bool GetKey(string buttonName)
        {
            if (ButtonKeysList.All(i => i.ButtonName != buttonName))
            {
                return false;
            }

            return Input.GetKey(ButtonKeysList.FirstOrDefault(i => i.ButtonName == buttonName)!.ActiveButtonKeyCode);
        }

        public bool GetKeyUp(string buttonName)
        {
            if (ButtonKeysList.All(i => i.ButtonName != buttonName))
            {
                return false;
            }

            return Input.GetKeyUp(ButtonKeysList.FirstOrDefault(i => i.ButtonName == buttonName)!.ActiveButtonKeyCode);
        }

        public void Setup()
        {
            FirstRun();

            foreach (var button in ButtonKeysList)
            {
                if (PlayerPrefs.HasKey(button.ButtonName))
                {
                    KeyCode buttonKeyCode =
                        ConvertStringToKeyCode(PlayerPrefs.GetString(button.ButtonName));
                    button.ActiveButtonKeyCode = buttonKeyCode;
                }
            }
        }

        private void FirstRun()
        {
            if (ButtonKeysList.Count == 0)
            {
                return;
            }

            foreach (var button in ButtonKeysList)
            {
                if (!PlayerPrefs.HasKey(button.ButtonName))
                {
                    PlayerPrefs.SetString(button.ButtonName, button.DefaultButtonKeyCode.ToString());
                }
            }
        }

        public string GetButtonKeyCodeAsString(KeyCode keyCode)
        {
            if ((KeyCode)48 == keyCode)
            {
                return 0.ToString();
            }
            int number = 1;
            for (int i = alphaStart; i <= alphaEnd; i++)
            {
                KeyCode tempKeyCode = (KeyCode)i;
                if (keyCode == tempKeyCode)
                {
                    return number.ToString();
                }
                number++;

            }

            return keyCode.ToString();
        }

        public string GetButtonKeyCodeAsString(string buttonName)
        {
            var buttonKeyCode = ButtonKeysList.FirstOrDefault(keys => keys.ButtonName == buttonName)!
                .ActiveButtonKeyCode;
            if ((KeyCode)48 == buttonKeyCode)
            {
                return 0.ToString();
            }
            
            int number = 1;
            for (int i = alphaStart; i <= alphaEnd; i++)
            {
                KeyCode tempKeyCode = (KeyCode)i;
                if (buttonKeyCode == tempKeyCode)
                {
                    return number.ToString();
                }
                number++;
            }

            return ButtonKeysList.FirstOrDefault(i => i.ButtonName == buttonName)
                ?.ActiveButtonKeyCode.ToString();
        }
        
        public static KeyCode ConvertStringToKeyCode(string buttonName)
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), buttonName);
        }
    }

    [Serializable]
    public class ButtonKeys
    {
        public string ButtonName;
        public KeyCode ActiveButtonKeyCode;
        public KeyCode DefaultButtonKeyCode;
    }
}