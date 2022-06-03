using Photon.Pun;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of _item and the Number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        private InventoryItem _item;
        private int _number;

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of _item this prefab represents.</param>
        public void Setup(InventoryItem item, int number)
        {
            _item = item;
            _number = number;
        }

        public InventoryItem GetItem()
        {
            return _item;
        }
        
        public int GetNumber()
        {
            return _number;
        }

        public void PickupItem(InventoryController inventoryController, ItemDropper itemDropper)
        {
            bool foundSlot = inventoryController.AddToFirstEmptySlot(_item, _number);
            if (foundSlot)
            {
                if (PhotonNetwork.InRoom)
                {
                    GetComponent<PhotonView>().RPC(nameof(RPCDestroyItem), RpcTarget.AllBufferedViaServer);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        [PunRPC]
        private void RPCDestroyItem()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                //Destroy(gameObject);
            }
        }

        public bool CanBePickedUp(InventoryController inventoryController)
        {
            return inventoryController.HasSpaceFor(_item);
        }
    }
}