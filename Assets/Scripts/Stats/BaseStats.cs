using System;
using GameDevTV.Utils;
using Impingement.enums;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private Transform _spawnPosition = null;
        [SerializeField] private bool _shouldUseModifiers = true;
        private LazyValue<int> _currentLevel;

        private void Awake()
        {
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        public enumCharacterClass GetClass()
        {
            return _characterClass;
        }

        private void OnEnable()
        {
            if (_experienceController != null)
            {
                _experienceController.OnExperienceGained += UpdateLevel;
            }
        }

        private void UpdateLevel(bool isRealExp)
        {
            var newLevel = CalculateLevel();
            if (newLevel > _currentLevel.value)
            {
                _currentLevel.value = newLevel;
                LevelUp(isRealExp);
            }
        }

        private void OnDisable()
        {
            if (_experienceController != null)
            {
                _experienceController.OnExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            
        }

        public void SetLevel(int value, bool isRealExp)
        {
            _currentLevel.value = value;
            LevelUp(isRealExp);
        }

        private void LevelUp(bool isRealExp)
        {
            if (_levelUpEffect != null && SceneManager.GetActiveScene().name == "Dungeon")
            {
                if (isRealExp)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        var localPrefab = PhotonNetwork.Instantiate("VFX/" + _levelUpEffect.name,
                            _spawnPosition.position,
                            Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(_levelUpEffect, _spawnPosition.position,
                            Quaternion.identity);
                    }
                }
                //localPrefab.transform.SetParent(); = transform.parent.transform;
            }
            OnLevelUp?.Invoke();
        }

        public float GetStat(enumStats stat)
        {
            return GetBaseStat(stat) + GetAdditiveModifiers(stat) * (1 + GetPercentageModifiers(stat)/100);
        }

        private float GetBaseStat(enumStats stat)
        {
            return _progression.GetStat(stat, _characterClass, GetLevel());
        }

        public float GetBaseStat(enumStats stat, int level)
        {
            return _progression.GetStat(stat, _characterClass, level);
        }

        public int GetLevel()
        {
            try
            {
                return _currentLevel.value;
            }
            catch
            {
                return _startLevel;
            }
        }

        private int CalculateLevel()
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
        
        private float GetAdditiveModifiers(enumStats stat)
        {
            if (!_shouldUseModifiers)
            {
                return 0;
            }
            
            float total = 0;
            foreach (var provider in GetComponents<IModifierProvider>())
            {
                foreach (var additiveModifier in provider.GetAdditiveModifiers(stat))
                {
                    total += additiveModifier;
                }
            }

            return total;
        }
        
        private float GetPercentageModifiers(enumStats stat)
        {
            if (!_shouldUseModifiers)
            {
                return 0;
            }
            
            float total = 0;
            foreach (var provider in GetComponents<IModifierProvider>())
            {
                foreach (var additiveModifier in provider.GetPercentageModifiers(stat))
                {
                    total += additiveModifier;
                }
            }

            return total;
        }
    }
}