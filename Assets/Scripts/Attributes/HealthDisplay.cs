using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private HealthController _healthController;
        [SerializeField] private GameObject _healthTextParent;
        [SerializeField] private TMP_Text _healthValueText;
        [SerializeField] private Image _healthImage;

        private void Update()
        {
            if (PlayerPrefs.GetInt("ShowLifeText") == 0)
            {
                _healthTextParent.SetActive(false);
                return;
            }
            else
            {
                _healthTextParent.SetActive(true);
            }

            _healthValueText.text = String.Format("{0:0.}", _healthController.GetHealthPoints()) + "/" + String.Format(
                "{0:0.}", _healthController.GetMaxHealthPoints());
            _healthImage.fillAmount = _healthController.GetHealthPoints() / _healthController.GetMaxHealthPoints();
        }
    }
}