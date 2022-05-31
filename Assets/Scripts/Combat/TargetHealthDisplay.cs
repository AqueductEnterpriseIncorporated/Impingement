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

        public void Show(HealthController target, bool value)
        {
            if (!value)
            {
                _imageParent.SetActive(false);
            }
            else
            {
                _imageParent.SetActive(true);
                _healthValueText.text = target.CharacterName;
                _healthImage.fillAmount = target.GetMaxHealthPoints() / target.GetHealthPoints();
            }
        }
    }
}