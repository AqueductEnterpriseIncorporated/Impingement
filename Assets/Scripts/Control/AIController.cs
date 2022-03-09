using GameDevTV.Utils;
using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using Impingement.Resources;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Control
{
    public class AIController: MonoBehaviourPun
    {
        [Range(0,1)]
        [SerializeField] private float _patrolSpeedFraction = 0.2f;
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime =  5f;   
        [SerializeField] private float _waypointDwellTime =  3f;   
        [SerializeField] private float _waypointTolerance = 1;
        [SerializeField] private PatrolPath _patrolPath;
        GameObject _playerTarget;
        private PhotonView _photonView;
        private CombatController _combatController;
        private HealthController _healthController;
        private MovementController _movementController;
        private LazyValue<Vector3> _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex;

        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _healthController = GetComponent<HealthController>();
            _movementController = GetComponent<MovementController>();
            _photonView = GetComponent<PhotonView>();
            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (PhotonNetwork.IsConnected)
            {
                _photonView.RPC(nameof(ProcessAIControllerRPC), RpcTarget.AllViaServer);
            }

            else
            {
                ProcessAIControllerRPC();
            }
        }

        [PunRPC]
        private void ProcessAIControllerRPC()
        {
            //if(!PhotonNetwork.IsMasterClient) { return; }
            
            if (_healthController.IsDead()) { return; }

            UpdateTimers();

            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) { return; }
            
            foreach (var player in players)
            {
                if (player.GetComponent<HealthController>().IsDead())
                {
                    continue;
                }
                
                if (!(InAttackRange(player.gameObject) || !_combatController.CanAttack(player.gameObject)))
                {
                    continue;
                }
                
                _playerTarget = player;
            }
            
            if (_playerTarget != null && InAttackRange(_playerTarget) && _combatController.CanAttack(_playerTarget))
            {
                AttackBehaviour(_playerTarget);
                return;
            }

            //bug: баг анимации пока ждут _suspicionTime
            if (_timeSinceLastSawPlayer < _suspicionTime)
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
            Vector3 nextPosition = _guardPosition.value;
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
                _movementController.StartMoving(nextPosition, _patrolSpeedFraction);
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