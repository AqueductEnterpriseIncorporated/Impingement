using UnityEngine;
using Impingement.Inventory;
using Impingement.UI.Tooltip;

namespace Impingement.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : TooltipSpawner, IRaycastable
    {
        [SerializeField] private PickupInRange _pickupInRange;
        private Pickup _pickup;

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            //UpdateTooltip();
            if (Input.GetMouseButtonDown(0))
            {
                if (_pickupInRange.PlayerInRange)
                {
                    _pickup.PickupItem(callingController.GetInventoryController(), callingController.GetItemDropper());
                }
            }
            return true;
        }

        public override bool CanCreateTooltip()
        {
            var Item = GetComponent<IItemHolder>().GetItem();
            if (!Item) return false;
        
            return true;
        }
        
        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            if (!itemTooltip) return;
        
            var item = _pickup.GetItem();
        
            itemTooltip.Setup(item);
        }
    }
}