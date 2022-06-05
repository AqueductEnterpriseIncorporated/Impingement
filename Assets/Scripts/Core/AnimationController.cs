using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Core
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] CharacterController _characterController;
        [SerializeField] string[] _mainAnimations;
        [SerializeField] AudioSource _audioSource;
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private PhotonView _photonView;
        private bool _isCharacterControllerNotNull;

        public bool IsPlaying()
        {
            foreach (var animationName in _mainAnimations)
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                    return true;
            }
            return false;
        }
        
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

        /// <summary>
        /// Attack animation event
        /// </summary>
        private void Stop()
        {
            _animator.SetBool("attackBool", false);

        }
        
        public void PlayAttackAnimation()
        {
            _animator.SetBool("attackBool", true);
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
