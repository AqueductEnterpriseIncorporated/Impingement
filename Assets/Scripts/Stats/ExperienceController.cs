using System;
using UnityEngine;

namespace Impingement.Stats
{
    public class ExperienceController : MonoBehaviour
    {
        public event Action OnExperienceGained;
        [SerializeField] private int _experiencePoints = 0;

        public void GainExperience(int experience)
        {
            _experiencePoints += experience;
            OnExperienceGained?.Invoke();
        }

        public int GetExperiencePoints()
        {
            return _experiencePoints;
        }
    }
}