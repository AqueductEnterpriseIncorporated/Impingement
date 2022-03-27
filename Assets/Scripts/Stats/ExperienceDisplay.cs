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
            var fromValue = _baseStats.GetBaseStat(enumStats.ExperienceToLevelUp, _baseStats.GetLevel() - 1);
            var toValue = _baseStats.GetStat(enumStats.ExperienceToLevelUp);
            if (Mathf.Approximately(fromValue, toValue))
            {
                fromValue = 0;
            }
            var remappedValue = Remap(_experienceController.GetExperiencePoints(),
                fromValue,
                toValue, 0, 1);
            _expValueText.text = _experienceController.GetExperiencePoints() + "/" +
                                 _baseStats.GetStat(enumStats.ExperienceToLevelUp);
            _experienceImage.fillAmount = remappedValue;
        }

        public float Remap (float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}