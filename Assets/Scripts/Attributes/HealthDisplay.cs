﻿using TMPro;
using UnityEngine;

namespace Impingement.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private HealthController _healthController;
        [SerializeField] private TMP_Text _healthValueText;

        private void Update()
        {
            _healthValueText.text = _healthController.GetHealthPoints() + "/" + _healthController.GetMaxHealthPoints();
        }
    }
}