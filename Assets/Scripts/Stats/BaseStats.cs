using System;
using Impingement.enums;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Stats
{
    public class BaseStats : MonoBehaviour
    {
        public event Action OnLevelUp;
        [Range(1,99)]
        [SerializeField] private int _startLevel = 1;
        [SerializeField] private enumCharacterClass _characterClass;
        [SerializeField] private Progression _progression = null;
        [SerializeField] private ExperienceController _experienceController = null;
        [SerializeField] private GameObject _levelUpEffect = null;
        private int _currentLevel = 0;

        private void Start()
        {
            _currentLevel = CalculateLevel();
            if (_experienceController != null)
            {
                _experienceController.OnExperienceGained += UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            var newLevel = CalculateLevel();
            if (newLevel > _currentLevel)
            {
                _currentLevel = newLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            if (_levelUpEffect != null)
            {
                var localPrefab = PhotonNetwork.Instantiate(_levelUpEffect.name, transform.parent.transform.position,
                    Quaternion.identity);
                //localPrefab.transform.SetParent(); = transform.parent.transform;
            }
            OnLevelUp?.Invoke();
            print("LevelledUp");
        }

        public float GetStat(enumStats stat)
        {
            return _progression.GetStat(stat, _characterClass, GetLevel());
        }

        public int GetLevel()
        {
            if (_currentLevel < 1)
            {
                _currentLevel = CalculateLevel();
            }
            return _currentLevel;
        }

        public int CalculateLevel()
        {
            if (_experienceController == null) { return _startLevel;}
            
            var currentExp = _experienceController.GetExperiencePoints();
            var penultimateLevel = _progression.GetLevels(enumStats.ExperienceToLevelUp, _characterClass);
            for (int level = 1; level < penultimateLevel; level++)
            {
                var expToLevelUp = _progression.GetStat(enumStats.ExperienceToLevelUp, _characterClass, level);
                if (expToLevelUp > currentExp)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }
    }
}