using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Core
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] CharacterController _characterController;
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private PhotonView _photonView;
        private bool _isCharacterControllerNotNull;

        private void Start()
        {
            _isCharacterControllerNotNull = _characterController != null;
            _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _photonView = GetComponent<PhotonView>();
        }

        private void Update()
        {
            float speed;

            if (_isCharacterControllerNotNull)
            {
                speed = _characterController.velocity.magnitude;
                _animator.SetFloat("forwardSpeed", speed);
                return;
            }
            
            Vector3 globalVelocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(globalVelocity);
            
            speed = localVelocity.z;
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
