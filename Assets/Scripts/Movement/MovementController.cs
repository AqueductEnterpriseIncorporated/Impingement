using System;
using Impingement.Combat;
using Impingement.Core;
using Impingement.Resources;
using Impingement.Saving;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Movement
{
    public class MovementController : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float _maximumSpeed = 6f; 
        [Range(0,1)]
        [SerializeField] private float _speedModifier = 1f; 
        private NavMeshAgent _navMeshAgent;
        private HealthController _healthController;

        private void Awake()
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
        
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3) state;
            _navMeshAgent.enabled = false;
            transform.position = position.ToVector();
            _navMeshAgent.enabled = true;
        }
    }
}