using Photon.Pun;
using UnityEngine;

namespace Impingement.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private GameObject _targetToDestroy;
        private void Update()
        {
            if(!GetComponent<ParticleSystem>().IsAlive())
            {
                if (PhotonNetwork.InRoom)
                {
                    if (_targetToDestroy != null)
                    {
                        _photonView.RPC(nameof(DestroyTargetRPC), RpcTarget.AllViaServer);

                    }
                    else
                    {
                        _photonView.RPC(nameof(DestroyRPC), RpcTarget.AllViaServer);
                    }
                }
                else
                {
                    DestroyRPC();
                    DestroyTargetRPC();
                }
            }
        }

        [PunRPC]
        private void DestroyRPC()
        {
            Destroy(gameObject);
        }
        
        [PunRPC]
        private void DestroyTargetRPC()
        {
            Destroy(_targetToDestroy);
        }
    }
}