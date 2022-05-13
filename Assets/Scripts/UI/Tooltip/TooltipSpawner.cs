using UnityEngine;
using UnityEngine.EventSystems;

namespace Impingement.UI.Tooltip
{
    /// <summary>
    /// Abstract base class that handles the spawning of a _tooltip prefab at the
    /// correct position on screen relative to a cursor.
    /// 
    /// Override the abstract functions to create a _tooltip spawner for your own
    /// data.
    /// </summary>
    public abstract class TooltipSpawner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("The prefab of the _tooltip to spawn.")]
        [SerializeField] private GameObject _tooltipPrefab = null;

        private GameObject _tooltip = null;

        /// <summary>
        /// Called when it is time to update the information on the _tooltip
        /// prefab.
        /// </summary>
        /// <param name="tooltip">
        /// The spawned _tooltip prefab for updating.
        /// </param>
        public abstract void UpdateTooltip(GameObject tooltip);
        
        /// <summary>
        /// Return true when the _tooltip spawner should be allowed to create a _tooltip.
        /// </summary>
        public abstract bool CanCreateTooltip();
        
        private void OnDestroy()
        {
            ClearTooltip();
        }

        private void OnDisable()
        {
            ClearTooltip();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            var parentCanvas = GetComponentInParent<Canvas>();

            if (_tooltip && !CanCreateTooltip())
            {
                ClearTooltip();
            }

            if (!_tooltip && CanCreateTooltip())
            {
                _tooltip = Instantiate(_tooltipPrefab, parentCanvas.transform);
            }

            if (_tooltip)
            {
                UpdateTooltip(_tooltip);
                PositionTooltip();
            }
        }

        private void PositionTooltip()
        {
            // Required to ensure corners are updated by positioning elements.
            Canvas.ForceUpdateCanvases();

            var tooltipCorners = new Vector3[4];
            _tooltip.GetComponent<RectTransform>().GetWorldCorners(tooltipCorners);
            var slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            bool below = transform.position.y > Screen.height / 2;
            bool right = transform.position.x < Screen.width / 2;

            int slotCorner = GetCornerIndex(below, right);
            int tooltipCorner = GetCornerIndex(!below, !right);

            _tooltip.transform.position = slotCorners[slotCorner] - tooltipCorners[tooltipCorner] + _tooltip.transform.position;
        }

        private int GetCornerIndex(bool below, bool right)
        {
            if (below && !right) return 0;
            else if (!below && !right) return 1;
            else if (!below && right) return 2;
            else return 3;

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            ClearTooltip();
        }

        private void ClearTooltip()
        {
            if (_tooltip)
            {
                Destroy(_tooltip.gameObject);
            }
        }
    }
}