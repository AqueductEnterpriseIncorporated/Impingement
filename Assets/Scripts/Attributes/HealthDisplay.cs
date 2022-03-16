using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private HealthController _healthController;
        [SerializeField] private TMP_Text _healthValueText;
        [SerializeField] private Image _healthImage;

        private void Update()
        {
            _healthValueText.text = _healthController.GetHealthPoints() + "/" + _healthController.GetMaxHealthPoints();
            _healthImage.fillAmount = _healthController.GetHealthPoints() / _healthController.GetMaxHealthPoints();
        }
    }
}