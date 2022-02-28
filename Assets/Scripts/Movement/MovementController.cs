using Impingement.Combat;
using Impingement.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Movement
{
    public class MovementController : MonoBehaviour, IAction
    {
        [SerializeField] private float _maximumSpeed = 6f; 
        [Range(0,1)]
        [SerializeField] private float _speedModifier = 1f; 
        private NavMeshAgent _navMeshAgent;
        private HealthController _healthController;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _healthController = GetComponent<HealthController>();
        }

        private void Update()
        {
            _navMeshAgent.enabled = !_healthController.IsDead();
        }

        public void Move(Vector3 worldPosition, float speedFraction)
        {
            _navMeshAgent.destination = worldPosition;
            _navMeshAgent.speed = _maximumSpeed * Mathf.Clamp01(speedFraction * _speedModifier);
            _navMeshAgent.isStopped = false;
        }

        public void StartMoving(Vector3 worldPosition, float speedFraction)
        {
            GetComponent<ActionScheduleController>().StartAction(this);
            Move(worldPosition, speedFraction);
        }

        public void Stop()
        {
            _navMeshAgent.isStopped = true;
        }

        public void Cancel()
        {
            Stop();
        }
    }
}