using Impingement.Combat;
using Impingement.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Movement
{
    public class MovementController : NetworkBehaviour, IAction
    {
        //public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
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

        public void Move(Vector3 worldPosition)
        {
            if (NetworkManager.IsServer)
            {
                _navMeshAgent.destination = worldPosition;
                _navMeshAgent.isStopped = false;   
                SubmitPositionRequestClientRpc(worldPosition);
            }
            else
            {
                SubmitPositionRequestServerRpc(worldPosition);
            }
        }

        public void StartMoving(Vector3 worldPosition)
        {
            GetComponent<ActionScheduleController>().StartAction(this);
            Move(worldPosition);
        }
        
        public void Stop()
        {
            if (NetworkManager.IsServer)
            {
                _navMeshAgent.isStopped = true;
                SubmitStopRequestClientRpc();
            }
            else
            {
                SubmitStopRequestServerRpc();
            }
        }
        
        public void Cancel()
        {
            Stop();
        }

        #region Client
        [ClientRpc]
        private void SubmitStopRequestClientRpc(ServerRpcParams rpcParams = default)
        {
            _navMeshAgent.isStopped = true;
        }
        
        [ClientRpc]
        private void SubmitPositionRequestClientRpc(Vector3 worldPosition, ServerRpcParams rpcParams = default)
        {
            _navMeshAgent.destination = worldPosition;
            _navMeshAgent.isStopped = false;            
        }
        #endregion

        #region Server
        [ServerRpc(RequireOwnership = false)]
        private void SubmitStopRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            _navMeshAgent.isStopped = true;
            SubmitStopRequestClientRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SubmitPositionRequestServerRpc(Vector3 worldPosition, ServerRpcParams rpcParams = default)
        {
            _navMeshAgent.destination = worldPosition;
            _navMeshAgent.isStopped = false;
            SubmitPositionRequestClientRpc(worldPosition);
        }
        #endregion
    }

}