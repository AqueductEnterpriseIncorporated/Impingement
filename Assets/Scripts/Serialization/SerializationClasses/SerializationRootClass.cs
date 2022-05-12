using System.Collections.Generic;
using System.Numerics;
using Impingement.Combat;
using Impingement.Dungeon;
using Impingement.Inventory;
using Impingement.Stats;

namespace Impingement.Serialization.SerializationClasses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    #region Dungeon
    public class Enemies
    {
        public List<RoomEnemy> RoomEnemies { get; set; }
    }

    public class RoomEnemy
    {
        public string RoomName { get; set; }
        public List<int> RemovedEnemies { get; set; }
    }

    public class Pickups
    {
        public string PickupName { get; set; }
        public string PickupPosition { get; set; }
    }

    public class RoomModifier
    {
        public string ModifierName { get; set; }
        public string RandomlyGeneratedObjectSpawnsAmount { get; set; }
        public List<string> RandomlyGeneratedObjectPrefabNamesList { get; set; }
    }

    public class Board
    {
        public bool Visited { get; set; }
        public List<bool> Status { get; set; }
    }

    public class SerializableDungeonData
    {
        public List<Enemies> Enemies { get; set; }
        public List<Pickups> Pickups { get; set; }
        public List<RoomModifier> RoomModifiers { get; set; }
        public string DungeonSize { get; set; }
        public string AreaLevel { get; set; }
        public List<DungeonGenerator.Cell> Board { get; set; }
    }
    
    public class UpgradeItems
    {
        public string Name { get; set; }
        public bool Unlocked { get; set; }
        public bool Equipped { get; set; }
    }
    #endregion

    #region Hideout
    public class SerializableHideoutData
    {
        public List<UpgradeItems> UpgradeItems { get; set; }
    }
    #endregion

    #region Player
    public class Item
    {
        public string ItemId { get; set; }
        public int ItemIndex{ get; set; }
    }

    public class ItemCoordinates
    {
        public string PositionX{ get; set; }
        public string PositionY{ get; set; }
        public string PositionZ{ get; set; }
    }
    public class DroppedItem
    {
        public string ItemId { get; set; }
        public ItemCoordinates Position { get; set; }

    }
    
    public class PlayerInventory
    {
        public int InventorySize { get; set; }
        public List<Item> InventoryItems { get; set; }
    }
    
    public class PlayerDroppedItems
    {
        public List<DroppedItem> DroppedItems { get; set; }
    }

    public class SerializablePlayerData
    {
        public PlayerInventory Inventory { get; set; }
        public PlayerDroppedItems SerializableDroppedItems { get; set; }
        public int Experience { get; set; }
        public string Weapon { get; set; }
        public int Currency { get; set; }
    }
    #endregion
}