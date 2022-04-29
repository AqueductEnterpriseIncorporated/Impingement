using System;
using System.Collections.Generic;
using Impingement.Combat;
using Impingement.Control;
using Impingement.Currency;
using Impingement.Stats;
using PlayFab.ClientModels;
using UnityEngine;

namespace Impingement.Playfab
{
    public class PlayfabPlayerDataController : MonoBehaviour
    {
        [SerializeField] private List<WeaponConfig> _availableWeapon;
        private PlayerController _playerController;
        private PlayfabManager _playfabManager;
        private ExperienceController _experienceController;
        private PlayerCurrencyController _playerCurrencyController;
        private CombatController _combatController;

        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _playfabManager = FindObjectOfType<PlayfabManager>();
            _experienceController = GetComponent<ExperienceController>();
            _playerCurrencyController = GetComponent<PlayerCurrencyController>();
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            _playfabManager.LoadData(OnDataReceivedPlayerData);
        }

        public void SavePlayerData()
        {
            _playfabManager.UploadData(new Dictionary<string, string>()
            {
                {"Experience", _experienceController.GetExperiencePoints().ToString()},
                {"Weapon", _combatController.GetCurrentWeapon().name},
                {"Currency", _playerCurrencyController.MyCurrency.ToString()},
            });
        }

        public void ResetPlayerData()
        {
            _playfabManager.UploadData(new Dictionary<string, string>()
            {
                {"Experience", "0"},
                {"Weapon", "Unarmed"},
            });
        }

        private void OnDataReceivedPlayerData(GetUserDataResult getUserDataResult)
        {
            if (getUserDataResult == null) { return; }

            if (getUserDataResult.Data.ContainsKey("Experience"))
            {
                _experienceController.GainExperience(Convert.ToInt32(getUserDataResult.Data["Experience"].Value));
            }
            if (getUserDataResult.Data.ContainsKey("Currency"))
            {
                _playerCurrencyController.MyCurrency = Convert.ToInt32(getUserDataResult.Data["Currency"].Value);
            }
            if (getUserDataResult.Data.ContainsKey("Weapon"))
            {
                foreach (var weaponConfig in _availableWeapon)
                {
                    if (weaponConfig.name == getUserDataResult.Data["Weapon"].Value)
                    {
                        _combatController.EquipWeapon(weaponConfig);
                        break;
                    }
                }
            }
        }
    }
}