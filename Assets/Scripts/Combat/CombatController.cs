using Impingement.Core;
using Impingement.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Combat
{
    public class CombatController : NetworkBehaviour, IAction
    {
        [SerializeField] private float _weaponRange = 2f;
        private MovementController _movementController;
        private Transform _targetTransform;

        private void Start()
        {
            _movementController = GetComponent<MovementController>();
        }

        private void Update()
        {
            if(_targetTransform == null) { return; }
            if (!GetIsInRange())
            {
                _movementController.Move(_targetTransform.position);
            }
            else
            {
                _movementController.Stop();
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _targetTransform.position) < _weaponRange;
        }

        public void SetTarget(CombatTarget target)
        {
            GetComponent<ActionScheduleController>().StartAction(this);
            _targetTransform = target.transform;
        }

        private void RemoveTarget()
        {
            _targetTransform = null;
        }

        public void Cancel()
        {
            RemoveTarget();
        }
    }
}