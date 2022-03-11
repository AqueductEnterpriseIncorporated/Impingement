using System.Collections.Generic;
using Impingement.Dungeon;

namespace Impingement.Serialization.SerializationClasses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Enemies
    {
        public string EnemyName { get; set; }
        public string EnemyPosition { get; set; }
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
}