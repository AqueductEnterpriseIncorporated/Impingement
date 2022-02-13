using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MovementController : NetworkBehaviour
{    
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Camera _playerCamera;
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Position.Value = transform.position;
    }

    private void Update()
    {
        _navMeshAgent.destination = Position.Value;

        if(Input.GetAxis("Fire1") > 0f)
        {
            Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask))
            {
                Move(hit.point);
            }
        }
    }

    private void Move(Vector3 worldPosition)
    {
        if(!IsOwner) { return; }
        if (NetworkManager.Singleton.IsServer)
        {
            Position.Value = worldPosition;
            _navMeshAgent.destination = Position.Value;
        }
        else
        {
            SubmitPositionRequestServerRpc(worldPosition);
        }
    }

    [ServerRpc]
    private void SubmitPositionRequestServerRpc(Vector3 worldPosition, ServerRpcParams rpcParams = default)
    {
        Position.Value = worldPosition;
    }
}
