using Photon.Pun;
using UnityEngine;

namespace Impingement.Dungeon
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
                    if(!GetComponent<PhotonView>().IsMine) { return; }
                    PhotonNetwork.Destroy(gameObject);
                    return;
                }
            }

            GetComponent<Collider>().enabled = true;
        }
    }

}