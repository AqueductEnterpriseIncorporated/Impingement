using Impingement.Combat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Movement
{
    public class MovementController : NetworkBehaviour
    {
        //public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        private NavMeshAgent _navMeshAgent;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void Move(Vector3 worldPosition)
        {
            if(!IsOwner) { return; }
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
            GetComponent<CombatController>().RemoveTarget();
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
        [ServerRpc]
        private void SubmitStopRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            _navMeshAgent.isStopped = true;
            SubmitStopRequestClientRpc();
        }

        [ServerRpc]
        private void SubmitPositionRequestServerRpc(Vector3 worldPosition, ServerRpcParams rpcParams = default)
        {
            _navMeshAgent.destination = worldPosition;
            _navMeshAgent.isStopped = false;
            SubmitPositionRequestClientRpc(worldPosition);
        }
        #endregion
    }

}