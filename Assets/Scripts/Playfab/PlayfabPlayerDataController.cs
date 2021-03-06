using System;
using System.Collections.Generic;
using Impingement.Combat;
using Impingement.Control;
using Impingement.Inventory;
using Impingement.Serialization.SerializationClasses;
using Impingement.SerializationAPI;
using Impingement.Stats;
using Photon.Pun;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.Playfab
{
    public class PlayfabPlayerDataController : MonoBehaviour
    {
        [SerializeField] private List<WeaponConfig> _availableWeapon;
        [SerializeField] private PhotonView _photonView;
        private PlayerController _playerController;
        private PlayfabManager _playfabManager;
        private ExperienceController _experienceController;
        private CombatController _combatController;
        private InventoryController _inventoryController;
        private EquipmentController _equipmentController;
        private ItemDropper _itemDropper;
        private ActionStore _actionStore;

        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _playfabManager = FindObjectOfType<PlayfabManager>();
            _experienceController = GetComponent<ExperienceController>();
            _playerController = GetComponent<PlayerController>();
            _inventoryController = GetComponent<InventoryController>();
            _equipmentController = GetComponent<EquipmentController>();
            _itemDropper = GetComponent<ItemDropper>();
            _actionStore = GetComponent<ActionStore>();
        }

        private void Start()
        {
            // var lobbyManager = FindObjectOfType<LobbyManager>();
            // if (lobbyManager)
            // {
            //     lobbyManager.PlayerConntected += OnPlayerConnected;
            // }
            // else
            // {
            if (SceneManager.GetActiveScene().name != "Arena")
            {
                OnPlayerConnected();
            }
            else
            {
                var loadPanel = GameObject.Find("LoadPanel");
                if (loadPanel)
                {
                    Destroy(loadPanel);
                }
            }
            //_playfabManager.LoadData(OnDataReceivedPlayerData);
            //}

            // var loadPanel = GameObject.Find("LoadPanel");
            // if (loadPanel)
            // {
            //     loadPanel.GetComponentInChildren<TMP_Text>().text = "Загрузка данных игрока...";
            // }
        }

        private void OnPlayerConnected()
        {
            Debug.Log("Player connected event in PlayfabPlayerDataController Class");
            // if (PhotonNetwork.InRoom)
            // {
            //     _photonView.RPC(nameof(RPCSetupPlayer), RpcTarget.OthersBuffered);
            // }
            //if (PhotonNetwork.IsMasterClient || _photonView.IsMine)
            {
                _playfabManager.LoadData(OnDataReceivedPlayerData);
            }
        }

        public void GetOtherPLayerData(string id)
        {
            _playfabManager.LoadData(OnDataReceivedPlayerData, id);
        }

        [PunRPC]
        private void RPCSetupPlayer()
        {
            Debug.Log("RPCSetup method");
        }
        
        public void SavePlayerData()
        {
            if(SceneManager.GetActiveScene().name == "Arena") { return; }
            _itemDropper.RemoveDestroyedDrops();
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
                    if(droppedItem is null) { continue; }
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
            
            var equippedItems = new List<EquippedItem>();
            if (_equipmentController.EquippedItems.Count > 0)
            {
                foreach (var equippedItem in _equipmentController.EquippedItems)
                {
                    EquippedItem item = new EquippedItem()
                    {
                        ItemId = equippedItem.Value.GetItemID()
                    };
                    equippedItems.Add(item);
                }
            }
            PlayerEquippedItems playerEquippedItems = new PlayerEquippedItems()
            {
                EquippedItems = equippedItems
            };
            
            var actionItems = new List<Item>();
            if (_actionStore.DockedItems.Count > 0)
            {
                foreach (var actionItem in _actionStore.DockedItems)
                {
                    Item item = new Item()
                    {
                        ItemId = actionItem.Value.Item.GetItemID(),
                        Number = actionItem.Value.Number,
                        ItemIndex = actionItem.Key
                    };
                    actionItems.Add(item);
                }
            }
            PlayerActionItems playerActionItems = new PlayerActionItems()
            {
                ActionItems = actionItems
            };
            
            SerializablePlayerData playerData = new SerializablePlayerData
            {
                Experience = _experienceController.GetExperiencePoints(),
                //Weapon = _combatController.GetCurrentWeapon().name,
                Inventory = inventory,
                SerializableDroppedItems = playerDroppedItems,
                SerializablePlayerEquippedItems = playerEquippedItems,
                SerializablePlayerActionItems = playerActionItems
            };
            return StringSerializationAPI.Serialize(typeof(SerializablePlayerData),  playerData);
        }

        public void ResetPlayerData()
        {
            _playfabManager.UploadData(new Dictionary<string, string>()
            {
                {"PlayerData", ""},
            });
        }
        
        public SerializablePlayerData GetData(string data)
        {
            return (SerializablePlayerData) StringSerializationAPI.Deserialize(typeof(SerializablePlayerData), data);
        }
        
        private void OnDataReceivedPlayerData(GetUserDataResult getUserDataResult)
        {
            Debug.Log("Playfab data received");
            if (getUserDataResult != null && getUserDataResult.Data.ContainsKey("PlayerData") && getUserDataResult.Data["PlayerData"].Value != null)
            {
                var json = getUserDataResult.Data["PlayerData"].Value;
                var playerData = GetData(json);
                _experienceController.GainExperience(Convert.ToInt32(playerData.Experience), false);
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
                
                if (playerData.SerializablePlayerEquippedItems != null && playerData.SerializablePlayerEquippedItems.EquippedItems.Count > 0)
                {
                    foreach (var equippedItem in playerData.SerializablePlayerEquippedItems.EquippedItems)
                    {
                        EquipableItem item = (EquipableItem)EquipableItem.GetFromID(equippedItem.ItemId);
                        _equipmentController.EquippedItems[item.GetAllowedEquipLocation()] = item;
                    }
                    _equipmentController.EquipmentUpdated();
                }
                
                if (playerData.SerializablePlayerActionItems != null && playerData.SerializablePlayerActionItems.ActionItems.Count > 0)
                {
                    foreach (var actionItem in playerData.SerializablePlayerActionItems.ActionItems)
                    {
                        var item = ActionItem.GetFromID(actionItem.ItemId);
                        _actionStore.AddAction(item, actionItem.ItemIndex, actionItem.Number);
                    }
                    _actionStore.StoreUpdated();
                }
            }
            
            var loadPanel = GameObject.Find("LoadPanel");
            if (loadPanel)
            {
                Destroy(loadPanel);
            }
            
            _playerController.GetHealthController().Heal(_playerController.GetHealthController().GetHealthPoints());
        }
    }
}