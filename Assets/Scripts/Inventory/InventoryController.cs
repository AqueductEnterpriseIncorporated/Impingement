using System;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// _slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        [Tooltip("Allowed size")]
        [SerializeField] private int _inventorySize = 16;

        private InventoryItem[] _slots;
        
        /// <summary>
        /// Broadcasts when the items in the _slots are added/removed.
        /// </summary>
        public event Action InventoryUpdated;

        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        /// <summary>
        /// How many _slots are in the inventory?
        /// </summary>
        public int GetSize()
        {
            return _slots.Length;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(InventoryItem item)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            _slots[i] = item;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
            return true;
        }

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (object.ReferenceEquals(_slots[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public InventoryItem GetItemInSlot(int slot)
        {
            return _slots[slot];
        }

        /// <summary>
        /// Remove the item from the given slot.
        /// </summary>
        public void RemoveFromSlot(int slot)
        {
            _slots[slot] = null;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item)
        {
            if (_slots[slot] != null)
            {
                return AddToFirstEmptySlot(item); ;
            }

            _slots[slot] = item;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
            return true;
        }
        
        private void Awake()
        {
            _slots = new InventoryItem[_inventorySize];
            _slots[0] = InventoryItem.GetFromID("cf3570fd-3587-4040-9cf2-4182a612b9be");
            _slots[15] = InventoryItem.GetFromID("cf3570fd-3587-4040-9cf2-4182a612b9be");
            _slots[4] = InventoryItem.GetFromID("f71e50ee-f2f2-4e71-bc4c-41c4c09df600");
        }

        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            return FindEmptySlot();
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all _slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        // object ISaveable.CaptureState()
        // {
        //     var slotStrings = new string[_inventorySize];
        //     for (int i = 0; i < _inventorySize; i++)
        //     {
        //         if (_slots[i] != null)
        //         {
        //             slotStrings[i] = _slots[i].GetItemID();
        //         }
        //     }
        //     return slotStrings;
        // }
        //
        // void ISaveable.RestoreState(object state)
        // {
        //     var slotStrings = (string[])state;
        //     for (int i = 0; i < _inventorySize; i++)
        //     {
        //         _slots[i] = InventoryItem.GetFromID(slotStrings[i]);
        //     }
        //     if (InventoryUpdated != null)
        //     {
        //         InventoryUpdated();
        //     }
        // }
    }
}