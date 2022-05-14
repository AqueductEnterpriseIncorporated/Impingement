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
        // CONFIG DATA

        [SerializeField] private InventoryItemIcon _icon = null;
        [SerializeField] private enumEquipLocation _equipLocation = enumEquipLocation.Weapon;

        private Equipment _playerEquipment;
        
        //todo: fix
        private void Awake() 
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _playerEquipment = player.GetComponent<Equipment>();
            _playerEquipment.EquipmentUpdated += RedrawUI;
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
            _playerEquipment.AddItem(_equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return new InventoryItem(); //_playerEquipment.GetItemInSlot(_equipLocation);
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
            _playerEquipment.RemoveItem(_equipLocation);
        }
        
        void RedrawUI()
        {
           // _icon.SetItem(_playerEquipment.GetItemInSlot(_equipLocation));
        }
    }
}