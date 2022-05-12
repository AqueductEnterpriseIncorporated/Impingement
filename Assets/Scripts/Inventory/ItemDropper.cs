﻿using System.Collections.Generic;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour
    {
        private List<Pickup> _droppedItems = new List<Pickup>();

        public List<Pickup> DroppedItems
        {
            get => _droppedItems;
            set => _droppedItems = value;
        }

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation());
        }
        
        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }
        
        public void SpawnPickup(InventoryItem item, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation);
            DroppedItems.Add(pickup);
        }
    }
}