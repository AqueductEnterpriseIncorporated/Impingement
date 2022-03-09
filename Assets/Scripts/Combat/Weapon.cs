using UnityEngine;
using UnityEngine.Events;

namespace Impingement.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onHit;
        public void OnHit()
        {
            _onHit.Invoke();
        }
    }
}