using UnityEngine;

namespace Impingement.Control
{
    public class FootstepController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _audioClips;
        
        /// <summary>
        /// Animation event
        /// </summary>
        private void Footstep()
        {
            _audioSource.PlayOneShot(_audioClips[Random.Range(0, _audioClips.Length)]);
        }
    }
}