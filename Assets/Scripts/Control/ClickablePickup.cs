using UnityEngine;
using Impingement.Inventory;

namespace Impingement.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private PickupInRange _pickupInRange;
        private Pickup _pickup;

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_pickupInRange.PlayerInRange)
                {
                    _pickup.PickupItem(callingController.GetInventoryController(), callingController.GetItemDropper());
                }
            }
            return true;
        }
    }
}