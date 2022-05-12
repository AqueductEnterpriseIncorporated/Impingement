﻿using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of _item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        private InventoryItem _item;

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of _item this prefab represents.</param>
        public void Setup(InventoryItem item)
        {
            _item = item;
        }

        public InventoryItem GetItem()
        {
            return _item;
        }

        public void PickupItem(InventoryController inventoryController, ItemDropper itemDropper)
        {
            bool foundSlot = inventoryController.AddToFirstEmptySlot(_item);
            if (foundSlot)
            {
                for (int i = 0; i < itemDropper.DroppedItems.Count; i++)
                {
                    if (itemDropper.DroppedItems[i]._item.GetItemID() == _item.GetItemID())
                    {
                        itemDropper.DroppedItems.RemoveAt(i);
                    }
                }
                Destroy(gameObject);
            }
        }

        //todo: use
        public bool CanBePickedUp(InventoryController inventoryController)
        {
            return inventoryController.HasSpaceFor(_item);
        }
    }
}