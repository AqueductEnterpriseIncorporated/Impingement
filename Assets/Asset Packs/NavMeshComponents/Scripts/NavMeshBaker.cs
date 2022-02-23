using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _navMeshSurface;

    private void Start()
    {
        _navMeshSurface.BuildNavMesh();
    }
}
