using Photon.Pun;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    public class WallCollision : MonoBehaviour
    {
        private void Start()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, .01f);

            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Wall")
                {
                    PhotonNetwork.Destroy(gameObject);
                    return;
                }
            }

            GetComponent<Collider>().enabled = true;
        }
    }

}