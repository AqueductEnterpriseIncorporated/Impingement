using UnityEngine;
using UnityEngine.UI;

namespace Impingement.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthBarImage;
        [SerializeField] private GameObject _healthBarParent;
        [SerializeField] private HealthController _healthController;

        private void Update()
        {
            if (!_healthController.IsDead())
            {
                _healthBarImage.fillAmount =
                    _healthController.GetHealthPoints() / _healthController.GetMaxHealthPoints();
            }
            else
            {
                _healthBarParent.SetActive(false);
            }
        }
    }
}