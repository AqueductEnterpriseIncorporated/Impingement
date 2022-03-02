using Photon.Pun;
using UnityEngine;

namespace Impingement.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        private void Update()
        {
            if(!GetComponent<ParticleSystem>().IsAlive())
            {
                _photonView.RPC(nameof(DestroyRPC), RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        private void DestroyRPC()
        {
            Destroy(gameObject);
        }
    }
}