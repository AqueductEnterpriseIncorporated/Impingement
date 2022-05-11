﻿using Impingement.Inventory;
using UnityEngine;

namespace Impingement.UI.InventoryUI
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventorySlotUI _inventoryItemPrefab = null;
        [SerializeField] private InventoryController _playerInventory;
        [SerializeField] private Transform _inventoryTransform;
        
        private void Awake() 
        {
            _playerInventory.InventoryUpdated += Redraw;
        }

        private void Start()
        {
            Redraw();
        }
        
        private void Redraw()
        {
            foreach (Transform child in _inventoryTransform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < _playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(_inventoryItemPrefab, _inventoryTransform);
                itemUI.Setup(_playerInventory, i);
            }
        }
    }
}