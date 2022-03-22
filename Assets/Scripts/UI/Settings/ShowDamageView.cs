using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI.Settings
{
    public class ShowDamageView : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;

        private void Start()
        {
            var prefsValue = PlayerPrefs.GetInt("ShowDamageText");
            var value = prefsValue == 1;
            _toggle.isOn = value;
        }

        public void Toggle()
        {
            var value = _toggle.isOn ? 1 : 0;
            PlayerPrefs.SetInt("ShowDamageText", value);
        }
    }
}