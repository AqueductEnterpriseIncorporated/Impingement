using UnityEngine;

namespace Impingement.UI
{
    public class ScrollController : MonoBehaviour
    {
        public bool CanScroll;
        [SerializeField] private Transform[] _hudObjects;
        private void Update()
        {
            CanScroll = true;
            foreach (var hudObject in _hudObjects)
            {
                if (hudObject.gameObject.activeSelf)
                {
                    CanScroll = false;
                }
            }
        }
    }
}