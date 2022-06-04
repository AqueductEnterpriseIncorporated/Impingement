using System.Collections.Generic;
using UnityEngine;

namespace Impingement.UI
{
    public class ScrollController : MonoBehaviour
    {
        public bool CanScroll;
        [SerializeField] public List<Transform> HudObjects;
        
        private void Update()
        {
            CanScroll = true;
            foreach (var hudObject in HudObjects)
            {
                if (hudObject.gameObject.activeSelf)
                {
                    CanScroll = false;
                }
            }
        }
    }
}