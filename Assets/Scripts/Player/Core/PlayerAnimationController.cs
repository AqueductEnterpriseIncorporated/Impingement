using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    private void Update()
    {
        Vector3 globalVelocity = _navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(globalVelocity);
        float speed = localVelocity.z;
        _animator.SetFloat("forwardSpeed", speed);
    }
}
