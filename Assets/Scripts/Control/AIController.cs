using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Control
{
    public class AIController: NetworkBehaviour
    {
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime =  5f;   
        [SerializeField] private float _waypointDwellTime =  3f;   
        [SerializeField] private float _waypointTolerance = 1;
        [SerializeField] private PatrolPath _patrolPath;
        private CombatController _combatController;
        private HealthController _healthController;
        private MovementController _movementController;
        private GameObject[] _players;
        private Vector3 _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex;

        private void Start()
        {
            _combatController = GetComponent<CombatController>();
            _healthController = GetComponent<HealthController>();
            _movementController = GetComponent<MovementController>();
            _guardPosition = transform.position;
        }

        private void Update()
        {
            if (_healthController.IsDead()) { return; }

            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) { return; }
            UpdateTimers();

            foreach (var player in players)
            {
                if(player.GetComponent<HealthController>().IsDead()) { continue; }
                
                if (InAttackRange(player) && _combatController.CanAttack(player))
                {
                    AttackBehaviour(player);
                    return;
                }
            }
            
            if(_timeSinceLastSawPlayer < _suspicionTime)
            {
                GetComponent<ActionScheduleController>().CancelCurrentAction();
            }
            else
            {
                PatrolBehaviour();
            }
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition;
            if (_patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > _waypointDwellTime)
            {
                _movementController.StartMoving(nextPosition);
            }
        }

        private void AttackBehaviour(GameObject player)
        {
            _timeSinceLastSawPlayer = 0;
            _combatController.SetTarget(player);
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distance = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distance < _waypointTolerance;
        }

        private bool InAttackRange(GameObject player)
        {
            var distance = Vector3.Distance(transform.position, player.transform.position);
            return distance < _chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}