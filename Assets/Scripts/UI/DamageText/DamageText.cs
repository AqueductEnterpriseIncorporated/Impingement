using TMPro;
using UnityEngine;

namespace Impingement.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        public void SetText(string text)
        {
            _text.text = $"{text:0}";
        }

        public void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}