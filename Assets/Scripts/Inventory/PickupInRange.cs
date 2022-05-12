using Impingement.Control;
using UnityEngine;

namespace Impingement.Inventory
{
    public class PickupInRange: MonoBehaviour
    {
        public bool PlayerInRange;
        private PlayerController _playerController;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var playerController))
            {
                PlayerInRange = true;
                _playerController = playerController;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var playerController))
            {
                if (_playerController == playerController)
                {
                    PlayerInRange = false;
                    _playerController = null;
                }
            }
        }
    }
}