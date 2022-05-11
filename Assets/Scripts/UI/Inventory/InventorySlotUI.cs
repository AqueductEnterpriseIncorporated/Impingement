﻿using UnityEngine;
using Impingement.UI.Dragging;

namespace Impingement.UI.Inventory
{
    public class InventorySlotUI : MonoBehaviour, IDragContainer<Sprite>
    {
        [SerializeField] InventoryItemIcon icon = null;
        
        public int MaxAcceptable(Sprite item)
        {
            if (GetItem() == null)
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(Sprite item, int number)
        {
            icon.SetItem(item);
        }

        public Sprite GetItem()
        {
            return icon.GetItem();
        }

        public int GetNumber()
        {
            return 1;
        }

        public void RemoveItems(int number)
        {
            icon.SetItem(null);
        }
    }
}