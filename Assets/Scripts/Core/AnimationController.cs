using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Core
{
    public class AnimationController : NetworkBehaviour
    {
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;

        private void Start()
        {
            _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            Vector3 globalVelocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(globalVelocity);
            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }

        public void PlayAttackAnimation()
        {
            if (NetworkManager.IsServer)
            {
                _animator.SetTrigger("attack");
            }
            else
            {
                SubmitAnimationRequestServerRpc();
            }
        }
        
        
        #region Client
        [ClientRpc]
        private void SubmitAnimationRequestClientRpc(ServerRpcParams rpcParams = default)
        {
            _animator.SetTrigger("attack");
        }
        #endregion

        #region Server
        [ServerRpc]
        private void SubmitAnimationRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            _animator.SetTrigger("attack"); 
            //SubmitAnimationRequestClientRpc();
        }
        #endregion
    }
}
