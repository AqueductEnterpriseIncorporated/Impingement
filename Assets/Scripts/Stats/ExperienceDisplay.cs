using Impingement.enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] private ExperienceController _experienceController;
        [SerializeField] private BaseStats _baseStats;
        [SerializeField] private TMP_Text _expValueText;
        [SerializeField] private Image _experienceImage;

        private void Update()
        {
            _expValueText.text = _experienceController.GetExperiencePoints().ToString() + "/"  +  _baseStats.GetStat(enumStats.ExperienceToLevelUp);
            _experienceImage.fillAmount = _experienceController.GetExperiencePoints() / _baseStats.GetStat(enumStats.ExperienceToLevelUp);
        }
    }
}