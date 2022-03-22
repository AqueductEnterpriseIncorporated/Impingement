using System;
using Impingement.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI
{
    public class CooldownDisplay : MonoBehaviour
    {
        [SerializeField] private CombatController _combatController;
        [SerializeField] private Image _image;

        private void Update()
        {
            _image.fillAmount = _combatController.TimeSinceLastAttack / _combatController.TimeBetweenAttacks;
            if (_image.fillAmount == 1)
            {
                _image.fillAmount = 0;
            }
        }
    }
}