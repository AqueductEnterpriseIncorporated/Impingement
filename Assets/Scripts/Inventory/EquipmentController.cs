using System;
using System.Collections.Generic;
using Impingement.enums;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class EquipmentController : MonoBehaviour
    {
        private Dictionary<enumEquipLocation, EquipableItem> _equippedItems = new Dictionary<enumEquipLocation, EquipableItem>();

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action EquipmentUpdated;

        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(enumEquipLocation equipLocation)
        {
            if (!_equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return _equippedItems[equipLocation];
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(enumEquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.GetAllowedEquipLocation() == slot);

            _equippedItems[slot] = item;

            if (EquipmentUpdated != null)
            {
                EquipmentUpdated();
            }
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(enumEquipLocation slot)
        {
            _equippedItems.Remove(slot);
            if (EquipmentUpdated != null)
            {
                EquipmentUpdated();
            }
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        public IEnumerable<enumEquipLocation> GetAllPopulatedSlots()
        {
            return _equippedItems.Keys;
        }
    }
}