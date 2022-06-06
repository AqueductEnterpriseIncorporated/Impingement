using GameDevTV.Utils;
using Impingement.enums;
using Impingement.Stats;
using UnityEngine;

namespace Impingement.Attributes
{
    public class StaminaController : MonoBehaviour
    {
        [SerializeField] private float _regenerationRate;
        [SerializeField] private BaseStats  _baseStats;
        private LazyValue<float> _staminaPoints;

        private void Awake()
        {
            _staminaPoints = new LazyValue<float>(GetInitialStamina);
        }

        private void Start()
        {
            _staminaPoints.ForceInit();
            _baseStats.OnLevelUp += RegenerateFullStamina;
            InvokeRepeating(nameof(PassiveRegenerationStamina), 0f, 1f / _regenerationRate);
        }

        private float GetInitialStamina()
        {
            return _baseStats.GetStat(enumStats.Stamina);
        }

        private void OnDestroy()
        {
            _baseStats.OnLevelUp -= RegenerateFullStamina;
        }
        
        private void RegenerateFullStamina()
        {
            _staminaPoints.value = GetMaximumStaminaPoints();
        }

        private void PassiveRegenerationStamina()
        {
            if (_staminaPoints.value < GetMaximumStaminaPoints())
            {
                _staminaPoints.value = Mathf.Min(_staminaPoints.value + _baseStats.GetStat(enumStats.StaminaRegen),
                    GetMaximumStaminaPoints());
            }
        }

        public float GetCurrentStaminaPoints()
        {
            return _staminaPoints.value;
        }
        
        public float GetMaximumStaminaPoints()
        {
            return _baseStats.GetStat(enumStats.Stamina);
        }

        public void SpendStamina(float points)
        {
            _staminaPoints.value = Mathf.Max(_staminaPoints.value - points, 0);
        }
        
        public void AddStamina(float point)
        {
            _staminaPoints.value = Mathf.Min(_staminaPoints.value + point, GetMaximumStaminaPoints());
        }
    }
}