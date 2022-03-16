using UnityEngine;

namespace Impingement.UI
{
    public class MinimapController : MonoBehaviour
    {
        [SerializeField] private GameObject _player;

        private void LateUpdate()
        {
            var newPosition = _player.transform.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }
}