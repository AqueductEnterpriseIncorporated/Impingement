using Impingement.Attributes;
using Impingement.Control;
using Impingement.enums;
using UnityEngine;

namespace Impingement.Core
{
    public class HighlightTarget : MonoBehaviour//, IRaycastable
    {
        [SerializeField] private Outline _outline;
        [SerializeField] private HealthController _healthController;
        private PlayerController _player;
        private bool _show;

        private void OnMouseEnter()
        {
            //_outline.enabled = true;
        }
        
        private void OnMouseExit()
        {
            // _outline.enabled = false;
            // if (_show)
            // {
            //     _player.GetTargetHealthDisplay().Show(_healthController, false);
            // }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (callingController.GetHealthController() == _healthController)
            {
                _show = false;
                callingController.GetTargetHealthDisplay().Show(_healthController, false);
                return false;
            }

            _show = true;
            _player = callingController;
            callingController.GetTargetHealthDisplay().Show(_healthController, true);
            return false;
        }

        public enumCursorType GetCursorType()
        {
            return enumCursorType.Movement;
        }
    }
}