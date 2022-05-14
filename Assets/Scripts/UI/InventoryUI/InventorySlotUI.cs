using Impingement.Inventory;
using UnityEngine;
using Impingement.UI.Dragging;
using Impingement.UI.Tooltip;

namespace Impingement.UI.InventoryUI
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] private InventoryItemIcon _icon = null;
        
        private int _index;
        private InventoryController _inventoryController;
        
        public void Setup(InventoryController inventoryController, int index)
        {
            _inventoryController = inventoryController;
            _index = index;
            _icon.SetItem(_inventoryController.GetItemInSlot(_index), _inventoryController.GetNumberInSlot(index));
        }
        
        public int MaxAcceptable(InventoryItem item)
        {
            if (_inventoryController.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int number)
        {
            _inventoryController.AddItemToSlot(_index, item, number);
        }

        public InventoryItem GetItem()
        {
            return _inventoryController.GetItemInSlot(_index);
        }

        public int GetNumber()
        {
            return _inventoryController.GetNumberInSlot(_index);
        }

        public void RemoveItems(int number)
        {
            _inventoryController.RemoveFromSlot(_index, number);
        }
    }
}