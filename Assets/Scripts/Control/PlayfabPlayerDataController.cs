using System;
using System.Collections.Generic;
using Impingement.Combat;
using Impingement.Playfab;
using Impingement.Stats;
using PlayFab.ClientModels;
using UnityEngine;

namespace Impingement.Control
{
    public class PlayfabPlayerDataController : MonoBehaviour
    {
        [SerializeField] private List<WeaponConfig> _availableWeapon;
        private PlayfabManager _playfabManager;
        private ExperienceController _experienceController;
        private CombatController _combatController;

        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _playfabManager = FindObjectOfType<PlayfabManager>();
            _experienceController = GetComponent<ExperienceController>();
        }

        private void Start()
        {
            _playfabManager.LoadData(OnDataReceivedPlayerData);
        }

        public void SavePlayerData()
        {
            _playfabManager.UploadData(new Dictionary<string, string>()
            {
                {"Experience", GetComponent<ExperienceController>().GetExperiencePoints().ToString()},
                {"Weapon", GetComponent<CombatController>().GetCurrentWeapon().name},

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