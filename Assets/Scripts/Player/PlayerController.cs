using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private void Awake()
    {
        //if(!IsOwner) { return; }
        //if (Camera.main != null) Camera.main.GetComponentInParent<PlayerCameraController>().Target = gameObject;
    }
}
