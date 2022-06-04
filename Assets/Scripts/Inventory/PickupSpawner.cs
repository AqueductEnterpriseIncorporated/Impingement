using Photon.Pun;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory _item.
    /// </summary>
    public class PickupSpawner : MonoBehaviourPun
    {
        [SerializeField] InventoryItem _item;
        [SerializeField] int _number = 1;

        private void Awake()
        {
            // var lobbyManager = FindObjectOfType<LobbyManager>();
            // if (lobbyManager)
            // {
            //     lobbyManager.PlayerConntected += OnPlayerConnected;
            // }
            // else
            // {
                SpawnPickup();
            //}
        }

        private void OnPlayerConnected()
        {
            SpawnPickup();
        }

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        public bool IsCollected() 
        { 
            return GetPickup() == null;
        }

        private void SpawnPickup()
        {
            var spawnedPickup = _item.SpawnPickup(transform.position, _number);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }
    }
}