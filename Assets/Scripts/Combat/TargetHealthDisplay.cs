using Impingement.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.Combat
{
    public class TargetHealthDisplay : MonoBehaviour
    {
        [SerializeField] private CombatController _combatController;
        [SerializeField] private TMP_Text _healthValueText;
        [SerializeField] private Image _healthImage;
        [SerializeField] private GameObject _imageParent;

        private void Update()
        {
            HealthController target = null;
            target = _combatController.GetTarget();
            if (target == null)
            {
                _imageParent.SetActive(false);
            }
            else
            {
                _imageParent.SetActive(true);
                _healthValueText.text = target.gameObject.name;
                //_healthValueText.text = target.GetHealthPoints().ToString();
                //_healthValueText.text += "/" + target.GetMaxHealthPoints();
                _healthImage.fillAmount = target.GetMaxHealthPoints() / target.GetHealthPoints();
            }
        }
    }
}