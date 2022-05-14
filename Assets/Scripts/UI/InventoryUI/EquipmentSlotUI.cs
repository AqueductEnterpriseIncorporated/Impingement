using Impingement.enums;
using Impingement.Inventory;
using Impingement.UI.Dragging;
using Impingement.UI.Tooltip;
using UnityEngine;

namespace Impingement.UI.InventoryUI
{
    /// <summary>
    /// An slot for the players equipment.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] private InventoryItemIcon _icon = null;
        [SerializeField] private enumEquipLocation _equipLocation = enumEquipLocation.Weapon;
        [SerializeField] private EquipmentController _playerEquipmentController;
        
        private void Awake() 
        {
            _playerEquipmentController.OnEquipmentUpdated += RedrawUI;
        }

        private void Start() 
        {
            RedrawUI();
        }
        
        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.GetAllowedEquipLocation() != _equipLocation) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            _playerEquipmentController.AddItem(_equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return _playerEquipmentController.GetItemInSlot(_equipLocation);
        }

        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void RemoveItems(int number)
        {
            _playerEquipmentController.RemoveItem(_equipLocation);
        }
        
        void RedrawUI()
        {
           _icon.SetItem(_playerEquipmentController.GetItemInSlot(_equipLocation));
        }
    }
}