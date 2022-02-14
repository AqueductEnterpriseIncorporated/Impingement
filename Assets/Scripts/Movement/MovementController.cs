using System;
using Impingement.Combat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Movement
{
    public class MovementController : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        private NavMeshAgent _navMeshAgent;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            _navMeshAgent.destination = Position.Value;
        }

        public void Move(Vector3 worldPosition)
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.IsHost)
            {
                Position.Value = worldPosition;
                _navMeshAgent.destination = Position.Value;
            }
            else
            {
                SubmitPositionRequestServerRpc(worldPosition);
            }
            _navMeshAgent.isStopped = false;
        }

        public void StartMoving(Vector3 worldPosition)
        {
            GetComponent<CombatController>().RemoveTarget();
            Move(worldPosition);
        }
        
        //todo: server - client bug
        public void Stop()
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.IsHost)
            {
                _navMeshAgent.isStopped = true;

            }
            else
            {
                SubmitStopRequestServerRpc();
            }
            _navMeshAgent.isStopped = true;
        }
        
        [ServerRpc]
        private void SubmitStopRequestServerRpc()
        {
            _navMeshAgent.isStopped = true;
        }


        [ServerRpc]
        private void SubmitPositionRequestServerRpc(Vector3 worldPosition, ServerRpcParams rpcParams = default)
        {
            Position.Value = worldPosition;
        }
    }

}