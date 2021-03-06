using System;
using GameDevTV.Utils;
using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using Impingement.Attributes;
using Impingement.Stats;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Control
{
    public class AIController: MonoBehaviourPun
    {
        #region fields

        [Range(0,1)]
        [SerializeField] private float _patrolSpeedFraction = 0.2f;
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime =  5f;
        [SerializeField] private float _aggroCooldownTime =  5f;
        [SerializeField] private float _timeSinceAggrevated =  Mathf.Infinity;
        [SerializeField] private float _waypointDwellTime =  3f;
        [SerializeField] private float _waypointTolerance = 1;
        [SerializeField] private PatrolPath _patrolPath;
        [SerializeField] PlayerController _playerTarget;
        [SerializeField] private ActionScheduleController _actionScheduleController;
        [SerializeField] private BaseStats _baseStats;
        [SerializeField] private bool _canAgro = true;
        //private PlayerController[] _players;
        private PlayerController _activePlayer;
        private PhotonView _photonView;
        private CombatController _combatController;
        private HealthController _healthController;
        private MovementController _movementController;
        private LazyValue<Vector3> _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _shoutDistance = 3f;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int _currentWaypointIndex;
        private bool _isPatrolPathNotNull;
        private float _startXRotation;

        #endregion

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        public BaseStats GetBaseStats()
        {
            return _baseStats;
        }
        
        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _healthController = GetComponent<HealthController>();
            _movementController = GetComponent<MovementController>();
            _photonView = GetComponent<PhotonView>();
            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            //_players = FindObjectsOfType<PlayerController>();
            _activePlayer = FindObjectOfType<PlayerController>();
            _isPatrolPathNotNull = _patrolPath != null;
            _guardPosition.ForceInit();
            _startXRotation = transform.rotation.x;
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        private void End()
        {
            //ignored
        }
        
        private void Update()
        {
            // if (PhotonNetwork.IsConnected)
            // {
            //     _photonView.RPC(nameof(ProcessAIControllerRPC), RpcTarget.AllViaServer);
            // }
            //
            // else
            // {
                ProcessAIControllerRPC();
            //}
        }

        //[PunRPC]
        private void ProcessAIControllerRPC()
        {
            //if(!PhotonNetwork.IsMasterClient) { return; }

            if (_healthController.IsDead())
            {
                return;
            }

            UpdateTimers();

            // if (_players.Length == 0)
            // {
            //     return;
            // }

            _activePlayer ??= FindObjectOfType<PlayerController>();
            //foreach (var player in _players)
            {
                if (_activePlayer.GetHealthController().IsDead())
                {
                    // if (_playerTarget == _activePlayer)
                    // {
                        _playerTarget = null;
                    //}

                    //continue;
                }

                if (!_canAgro)
                {
                    return;
                }

                if (IsAggrevated(_activePlayer.gameObject) || _combatController.CanAttack(_activePlayer.GetHealthController()))
                {

                    _playerTarget = _activePlayer;

                }
                else
                {
                    _playerTarget = null;
                }


                if (_playerTarget != null)
                {
                    AttackBehaviour(_playerTarget);
                }
                else if (_timeSinceLastSawPlayer < _suspicionTime)
                {
                    SuspicionBehaviour();
                }
                else
                {
                    //PatrolBehaviour();
                }
            }
        }

        private void SuspicionBehaviour()
        {
            _playerTarget = null;
            _actionScheduleController.CancelCurrentAction();
        }

        public void Aggrevate()
        {
            if (_canAgro)
            {
                _timeSinceAggrevated = 0;
            }
        }

        private void PatrolBehaviour()
        {
            // var nextPosition = _guardPosition.value;
            // if (_isPatrolPathNotNull)
            // {
            //     if (AtWaypoint())
            //     {
            //         _timeSinceArrivedAtWaypoint = 0;
            //         CycleWaypoint();
            //     }
            //
            //     nextPosition = GetCurrentWaypoint();
            //     
            //     if (_timeSinceArrivedAtWaypoint > _waypointDwellTime)
            //     {
            //         _movementController.StartMoving(nextPosition, _patrolSpeedFraction);
            //     }
            // }
        }

        private void AttackBehaviour(PlayerController player)
        {
            _timeSinceLastSawPlayer = 0;
            _combatController.SetTarget(player.GetHealthController());

            //AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            var hits = Physics.SphereCastAll(transform.position, _shoutDistance, Vector3.up, 0);
            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<AIController>(out var aiController))
                {
                    if (aiController == this) { continue; }
                    aiController.Aggrevate();
                }
            }
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
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

        private bool IsAggrevated(GameObject player)
        {
            var distance = Mathf.Abs((transform.position - player.transform.position).sqrMagnitude);//Vector3.Distance(transform.position, player.transform.position);
            return distance < Math.Pow(_chaseDistance, 2) || _timeSinceAggrevated < _aggroCooldownTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}