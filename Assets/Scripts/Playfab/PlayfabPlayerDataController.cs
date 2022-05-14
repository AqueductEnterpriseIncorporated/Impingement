using System;
using System.Collections.Generic;
using System.Linq;
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
        private ItemDropper _itemDropper;

        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _playfabManager = FindObjectOfType<PlayfabManager>();
            _experienceController = GetComponent<ExperienceController>();
            _playerCurrencyController = GetComponent<PlayerCurrencyController>();
            _playerController = GetComponent<PlayerController>();
            _inventoryController = GetComponent<InventoryController>();
            _itemDropper = GetComponent<ItemDropper>();
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
                var inventorySlot = items[index];
                if (inventorySlot.Item is null)
                {
                    continue;
                }

                inventory.InventoryItems.Add(new Item()
                {
                    ItemId = inventorySlot.Item.GetItemID(),
                    ItemIndex = index,
                    Number = inventorySlot.Number
                });
            }
            
            var droppedItems = new List<DroppedItem>();
            if (_itemDropper.DroppedItems.Count > 0)
            {
                foreach (var droppedItem in _itemDropper.DroppedItems)
                {
                    var itemCoordinates = new ItemCoordinates()
                    {
                        PositionX = droppedItem.transform.position.x.ToString(),
                        PositionY = droppedItem.transform.position.y.ToString(),
                        PositionZ = droppedItem.transform.position.z.ToString(),

                    };
                    DroppedItem item = new DroppedItem()
                    {
                        Position = itemCoordinates,
                        ItemId = droppedItem.GetItem().GetItemID(),
                        Number = droppedItem.GetNumber()
                    };
                    droppedItems.Add(item);
                }
            }
            PlayerDroppedItems playerDroppedItems = new PlayerDroppedItems()
            {
                DroppedItems = droppedItems
            };
            

            SerializablePlayerData playerData = new SerializablePlayerData
            {
                Currency = _playerCurrencyController.MyCurrency,
                Experience = _experienceController.GetExperiencePoints(),
                Weapon = _combatController.GetCurrentWeapon().name,
                Inventory = inventory,
                SerializableDroppedItems = playerDroppedItems
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
                if (playerData.Inventory != null && playerData.Inventory.InventoryItems.Count > 0)
                {
                    _inventoryController.Slots = new InventoryController.InventorySlot[playerData.Inventory.InventorySize];

                    foreach (var item in playerData.Inventory.InventoryItems)
                    {
                        var newItem = InventoryItem.GetFromID(item.ItemId);
                        _inventoryController.SetItemToSlot(item.ItemIndex, newItem, item.Number);
                    }
                }
                else
                {
                    _inventoryController.Slots = new InventoryController.InventorySlot[_inventoryController.DefaultSize];
                }

                var droppedItemsList = playerData.SerializableDroppedItems;
                
                if (playerData.SerializableDroppedItems != null && playerData.SerializableDroppedItems.DroppedItems.Count > 0)
                {
                    foreach (var item in droppedItemsList.DroppedItems)
                    {
                        var pickupItem = InventoryItem.GetFromID(item.ItemId);
                        Vector3 position = new Vector3(Convert.ToSingle(item.Position.PositionX),
                            Convert.ToSingle(item.Position.PositionY), Convert.ToSingle(item.Position.PositionZ));
                        _itemDropper.SpawnPickup(pickupItem, position, item.Number);
                    }
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