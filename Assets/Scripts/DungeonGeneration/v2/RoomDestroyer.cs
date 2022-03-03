using Photon.Pun;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    public class RoomDestroyer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("SpawnPoint"))
            {
                PhotonNetwork.Destroy(other.gameObject);
            }
        }
    }
}