using Impingement.Currency;
using TMPro;
using UnityEngine;

namespace Impingement.UI
{
    public class PlayerCurrencyDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] PlayerCurrencyController _playerCurrencyController;

        private void Update()
        {

            _tmpText.text = _playerCurrencyController.MyCurrency.ToString();
        }
    }
}