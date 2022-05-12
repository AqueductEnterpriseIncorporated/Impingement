using System;
using System.Collections.Generic;
using Impingement.Combat;
using Impingement.Control;
using Impingement.Currency;
using Impingement.Inventory;
using Impingement.Serialization.SerializationClasses;
using Impingement.SerializationAPI;
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
        private InventoryController _inventoryController;

        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _playfabManager = FindObjectOfType<PlayfabManager>();
            _experienceController = GetComponent<ExperienceController>();
            _playerCurrencyController = GetComponent<PlayerCurrencyController>();
            _playerController = GetComponent<PlayerController>();
            _inventoryController = GetComponent<InventoryController>();
        }

        private void Start()
        {
            _playfabManager.LoadData(OnDataReceivedPlayerData);
        }

        public void SetupPlayer(string id)
        {
            _playfabManager.LoadData(OnDataReceivedPlayerData, id);
        }

        public void SavePlayerData()
        {
            _playfabManager.UploadJson("PlayerData", GenerateJson());
        }

        private string GenerateJson()
        {
            PlayerInventory inventory = new PlayerInventory
            {
                InventorySize = _inventoryController.GetSize()
            };
            var items = _inventoryController.GetItems();
            inventory.InventoryItems = new List<Item>();
            for (var index = 0; index < items.Length; index++)
            {
                var item = items[index];
                if (item is null)
                {
                    continue;
                }

                inventory.InventoryItems.Add(new Item()
                {
                    ItemId = item.GetItemID(),
                    ItemIndex = index
                });
            }

            SerializablePlayerData playerData = new SerializablePlayerData
            {
                Currency = _playerCurrencyController.MyCurrency,
                Experience = _experienceController.GetExperiencePoints(),
                Weapon = _combatController.GetCurrentWeapon().name,
                Inventory = inventory
            };
            return StringSerializationAPI.Serialize(typeof(SerializablePlayerData),  playerData);
        }

        public void ResetPlayerData()
        {
            _playfabManager.UploadData(new Dictionary<string, string>()
            {
                {"Experience", "0"},
                {"Weapon", "Unarmed"},
                {"Inventory", ""},
            });
        }
        
        public SerializablePlayerData GetData(string data)
        {
            return (SerializablePlayerData) StringSerializationAPI.Deserialize(typeof(SerializablePlayerData), data);
        }
        
        private void OnDataReceivedPlayerData(GetUserDataResult getUserDataResult)
        {
            if (getUserDataResult != null && getUserDataResult.Data.ContainsKey("PlayerData"))
            {
                var json = getUserDataResult.Data["PlayerData"].Value;
                var playerData = GetData(json);
                _experienceController.GainExperience(Convert.ToInt32(playerData.Experience));
                _playerCurrencyController.MyCurrency = Convert.ToInt32(playerData.Currency);
                if (playerData.Inventory.InventoryItems.Count > 0)
                {
                    _inventoryController.Slots = new InventoryItem[playerData.Inventory.InventorySize];

                    foreach (var item in playerData.Inventory.InventoryItems)
                    {
                        var newItem = InventoryItem.GetFromID(item.ItemId);
                        _inventoryController.SetItemToSlot(item.ItemIndex, newItem);
                    }
                }
                else
                {
                    _inventoryController.Slots = new InventoryItem[_inventoryController.DefaultSize];
                }

                //todo: refactoring
                foreach (var weaponConfig in _availableWeapon)
                {
                    if (weaponConfig.name == playerData.Weapon)
                    {
                        _combatController.EquipWeapon(weaponConfig);
                        break;
                    }
                }
            }
        }
    }
}