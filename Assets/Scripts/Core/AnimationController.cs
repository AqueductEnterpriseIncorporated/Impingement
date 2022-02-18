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

        //bug: multiple triggers (client-server)
        public void PlayTriggerAnimation(string triggerName)
        {
            if (NetworkManager.IsServer)
            {
                _animator.SetTrigger(triggerName);
                //SubmitAnimationRequestClientRpc(triggerName);
            }
            else
            {
                SubmitAnimationRequestServerRpc(triggerName);
            }
        }
        
        public void ResetTriggerAnimation(string triggerName)
        {
            if (NetworkManager.IsServer)
            {
                _animator.ResetTrigger(triggerName);
            }
            else
            {
                SubmitResetAnimationRequestServerRpc(triggerName);
            }
        }
        
        #region Server
        [ServerRpc]
        private void SubmitAnimationRequestServerRpc(string triggerName, ServerRpcParams rpcParams = default)
        {
            //_animator.SetTrigger(triggerName); 
            SubmitAnimationRequestClientRpc(triggerName);
        }

        [ServerRpc]
        private void SubmitResetAnimationRequestServerRpc(string triggerName, ServerRpcParams rpcParams = default)
        {
            _animator.ResetTrigger(triggerName); 
        }
        #endregion

        #region Client
        [ClientRpc]
        private void SubmitAnimationRequestClientRpc(string triggerName, ServerRpcParams rpcParams = default)
        {
            _animator.SetTrigger(triggerName); 
        }
        #endregion
    }
}
