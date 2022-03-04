using UnityEngine;

namespace Impingement.NavMesh
{
    public class NavigationBaker : MonoBehaviour
    {
        [SerializeField] private GameObject[] _surfaces;

        public void Bake()
        {
            _surfaces = GameObject.FindGameObjectsWithTag("Floor");
            foreach (var surface in _surfaces)
            {
                surface.GetComponent<NavMeshSurface>().BuildNavMesh();
            }
        }
    }
}