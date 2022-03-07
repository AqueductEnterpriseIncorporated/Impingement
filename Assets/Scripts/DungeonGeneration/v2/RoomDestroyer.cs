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
                //PhotonNetwork.Destroy(transform.parent.gameObject);

                //PhotonNetwork.Destroy(other.gameObject);
            }
            if (other.CompareTag("ClosedRoom"))
            {
                PhotonNetwork.Destroy(other.transform.parent.gameObject);
            }
        }
    }
}