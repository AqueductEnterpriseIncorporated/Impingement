using System;
using System.Collections.Generic;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// Provides the storage for an action bar. The bar has a finite Number of
    /// slots that can be filled and actions in the slots can be "used".
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class ActionStore : MonoBehaviour
    {
        private Dictionary<int, DockedItemSlot> _dockedItems = new Dictionary<int, DockedItemSlot>();

        public class DockedItemSlot 
        {
            public ActionItem Item;
            public int Number;
        }
        
        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action OnStoreUpdated;

        public Dictionary<int, DockedItemSlot> DockedItems => _dockedItems;

        public void StoreUpdated()
        {
            OnStoreUpdated?.Invoke();
        }
        
        /// <summary>
        /// Get the action at the given index.
        /// </summary>
        public ActionItem GetAction(int index)
        {
            if (DockedItems.ContainsKey(index))
            {
                return DockedItems[index].Item;
            }
            return null;
        }

        /// <summary>
        /// Get the Number of items left at the given index.
        /// </summary>
        /// <returns>
        /// Will return 0 if no Item is in the index or the Item has
        /// been fully consumed.
        /// </returns>
        public int GetNumber(int index)
        {
            if (DockedItems.ContainsKey(index))
            {
                return DockedItems[index].Number;
            }
            return 0;
        }

        /// <summary>
        /// Add an Item to the given index.
        /// </summary>
        /// <param name="item">What Item should be added.</param>
        /// <param name="index">Where should the Item be added.</param>
        /// <param name="number">How many items to add.</param>
        public void AddAction(InventoryItem item, int index, int number)
        {
            if (DockedItems.ContainsKey(index))
            {  
                if (object.ReferenceEquals(item, DockedItems[index].Item))
                {
                    DockedItems[index].Number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.Item = item as ActionItem;
                slot.Number = number;
                DockedItems[index] = slot;
            }
            if (OnStoreUpdated != null)
            {
                OnStoreUpdated();
            }
        }

        /// <summary>
        /// Use the Item at the given slot. If the Item is consumable one
        /// instance will be destroyed until the Item is removed completely.
        /// </summary>
        /// <param name="user">The character that wants to use this action.</param>
        /// <returns>False if the action could not be executed.</returns>
        public bool Use(int index, GameObject user)
        {
            if (DockedItems.ContainsKey(index))
            {
                DockedItems[index].Item.Use(user);
                if (DockedItems[index].Item.isConsumable())
                {
                    RemoveItems(index, 1);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a given Number of items from the given slot.
        /// </summary>
        public void RemoveItems(int index, int number)
        {
            if (DockedItems.ContainsKey(index))
            {
                DockedItems[index].Number -= number;
                if (DockedItems[index].Number <= 0)
                {
                    DockedItems.Remove(index);
                }
                if (OnStoreUpdated != null)
                {
                    OnStoreUpdated();
                }
            }
            
        }

        /// <summary>
        /// What is the maximum Number of items allowed in this slot.
        /// 
        /// This takes into account whether the slot already contains an Item
        /// and whether it is the same type. Will only accept multiple if the
        /// Item is consumable.
        /// </summary>
        /// <returns>Will return int.MaxValue when there is not effective bound.</returns>
        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;

            if (DockedItems.ContainsKey(index) && !object.ReferenceEquals(item, DockedItems[index].Item))
            {
                return 0;
            }
            if (actionItem.isConsumable())
            {
                return int.MaxValue;
            }
            if (DockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }
    }
}