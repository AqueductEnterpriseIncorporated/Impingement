using Impingement.Control;
using Impingement.Inventory;
using UnityEngine;

namespace Impingement.UI.InventoryUI
{
    /// <summary>
    /// To be placed on the root of the playerInventory UI. Handles spawning all the
    /// playerInventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public PlayerController CurrentPlayerController;
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