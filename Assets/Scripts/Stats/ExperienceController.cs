using System;
using UnityEngine;

namespace Impingement.Stats
{
    public class ExperienceController : MonoBehaviour
    {
        public event Action<bool> OnExperienceGained;
        [SerializeField] private int _experiencePoints = 0;

        public void GainExperience(int experience, bool isRealExp)
        {
            _experiencePoints += experience;
            OnExperienceGained?.Invoke(isRealExp);
        }

        public int GetExperiencePoints()
        {
            return _experiencePoints;
        }
    }
}