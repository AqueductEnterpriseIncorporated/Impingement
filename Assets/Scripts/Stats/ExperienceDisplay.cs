using TMPro;
using UnityEngine;

namespace Impingement.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] private ExperienceController _experienceController;
        [SerializeField] private TMP_Text _expValueText;

        private void Update()
        {
            _expValueText.text = _experienceController.GetExperiencePoints().ToString();
        }
    }
}