using System;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// Provides storage for the player playerInventory. A configurable number of
    /// _slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        [Tooltip("Allowed size")]
        [SerializeField] private int _inventorySize = 16;
        [SerializeField] private int _defaultSize = 16;

        private InventorySlot[] _slots;

        public struct InventorySlot
        {
            public InventoryItem Item;
            public int Number;
        }

        /// <summary>
        /// Broadcasts when the items in the _slots are added/removed.
        /// </summary>
        public event Action InventoryUpdated;

        public int InventorySize
        {
            get => _inventorySize;
            set => _inventorySize = value;
        }

        public InventorySlot[] Slots
        {
            get => _slots;
            set => _slots = value;
        }

        public int DefaultSize
        {
            get => _defaultSize;
            set => _defaultSize = value;
        }

        public InventorySlot[] GetItems()
        {
            return Slots;
        }

        /// <summary>
        /// Could this Item fit anywhere in the playerInventory?
        /// </summary>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        /// <summary>
        /// How many _slots are in the playerInventory?
        /// </summary>
        public int GetSize()
        {
            return Slots.Length;
        }
        
        public int GetNumberInSlot(int index)
        {
            return Slots[index].Number;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The Item to add.</param>
        /// <returns>Whether or not the Item could be added.</returns>
        public bool AddToFirstEmptySlot(InventoryItem item, int number)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            Slots[i].Item = item;
            Slots[i].Number += number;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
            return true;
        }

        /// <summary>
        /// Is there an instance of the Item in the playerInventory?
        /// </summary>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                if (object.ReferenceEquals(Slots[i].Item, item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return the Item type in the given slot.
        /// </summary>
        public InventoryItem GetItemInSlot(int slot)
        {
            return Slots[slot].Item;
        }

        /// <summary>
        /// Remove the Item from the given slot.
        /// </summary>
        public void RemoveFromSlot(int slot, int number)
        {
            Slots[slot].Number -= number;
            if (Slots[slot].Number <= 0)
            {
                Slots[slot].Number = 0;
                Slots[slot].Item = null;
            }
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
        }

        /// <summary>
        /// Will add an Item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The Item type to add.</param>
        /// <returns>True if the Item was added anywhere in the playerInventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            if (Slots[slot].Item != null)
            {
                return AddToFirstEmptySlot(item, number); ;
            }

            var i = FindStack(item);
            if (i >= 0)
            {
                slot = i;
            }

            Slots[slot].Item = item;
            Slots[slot].Number += number;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
            return true;
        }

        public void SetItemToSlot(int slot, InventoryItem item, int number)
        {
            Slots[slot].Item = item;
            Slots[slot].Number = number;
            if (InventoryUpdated != null)
            {
                InventoryUpdated();
            }
        }
        
        private void Awake()
        {
            Slots = new InventorySlot[InventorySize];
            // _slots[0] = InventoryItem.GetFromID("cf3570fd-3587-4040-9cf2-4182a612b9be");
            // _slots[4] = InventoryItem.GetFromID("f71e50ee-f2f2-4e71-bc4c-41c4c09df600");
            // _slots[15] = InventoryItem.GetFromID("cf3570fd-3587-4040-9cf2-4182a612b9be");
        }

        /// <summary>
        /// Find a slot that can accomodate the given Item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            int i = FindStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }

            return i;
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all _slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].Item == null)
                {
                    return i;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Find an existing stack of this item type.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>-1 if no stack exists or if the item is not stackable</returns>
        private int FindStack(InventoryItem item)
        {
            if (!item.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < Slots.Length; i++)
            {
                if (object.ReferenceEquals(Slots[i].Item, item))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}