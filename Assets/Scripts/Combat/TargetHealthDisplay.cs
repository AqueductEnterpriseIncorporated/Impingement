using Impingement.Resources;
using TMPro;
using UnityEngine;

namespace Impingement.Combat
{
    public class TargetHealthDisplay : MonoBehaviour
    {
        [SerializeField] private CombatController _combatController;
        [SerializeField] private TMP_Text _healthValueText;

        private void Update()
        {
            HealthController target = null;
            target = _combatController.GetTarget();
            if (target == null)
            {
                _healthValueText.text = "";
            }
            else
            {
                _healthValueText.text = target.GetHealthPoints().ToString();
                _healthValueText.text += "/" + target.GetMaxHealthPoints();
            }
        }
    }
}