using System;
using Impingement.Combat;
using Impingement.Core;
using Impingement.Attributes;
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
        [SerializeField] private float _maxNavPathLength = 40f;
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

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath navMeshPath = new NavMeshPath();
            bool hasPath = UnityEngine.AI.NavMesh.CalculatePath(transform.position, destination,
                UnityEngine.AI.NavMesh.AllAreas, navMeshPath);
            if (!hasPath) { return false; }
            if (navMeshPath.status != NavMeshPathStatus.PathComplete) { return false; }
            if (GetPathLength(navMeshPath) > _maxNavPathLength) { return false; }

            return true;
        }
        
        private float GetPathLength(NavMeshPath navMeshPath)
        {
            float totalDistance = 0;

            if (navMeshPath.corners.Length < 2) { return totalDistance; }
            
            for (int i = 0; i < navMeshPath.corners.Length-1; i++)
            {
                totalDistance += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
            }

            return totalDistance;
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