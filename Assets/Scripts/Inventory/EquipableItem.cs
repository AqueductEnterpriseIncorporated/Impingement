using Impingement.enums;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = ("Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] enumEquipLocation _allowedEquipLocation = enumEquipLocation.Weapon;
        
        public enumEquipLocation GetAllowedEquipLocation()
        {
            return _allowedEquipLocation;
        }
    }
}