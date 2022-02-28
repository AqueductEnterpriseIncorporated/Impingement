using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Core
{
    public class AnimationController : MonoBehaviour
    {
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private PhotonView _photonView;

        private void Start()
        {
            _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _photonView = GetComponent<PhotonView>();
        }

        private void Update()
        {
            Vector3 globalVelocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(globalVelocity);
            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }

        //[PunRPC]
        public void PlayTriggerAnimation(string triggerName)
        {
            _animator.SetTrigger(triggerName);
        }
        
        //[PunRPC]
        public void ResetTriggerAnimation(string triggerName)
        {
            _animator.ResetTrigger(triggerName);
        }
    }
}
