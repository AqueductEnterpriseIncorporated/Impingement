using System;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] GameObject Target;   
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private int _targetOffsetX;
    [SerializeField] private int _targetOffsetY;
    [SerializeField] private int _targetOffsetZ;
    [SerializeField] private int _cameraRotationX;
    [SerializeField] private int _cameraRotationY;
    [SerializeField] private int _cameraRotationZ;
    [SerializeField] private float _zoomSpeed = 3f;
    [SerializeField] private float _zoomInMax = 40f;
    [SerializeField] private float _zoomOutMax = 40f;
    [SerializeField] private CinemachineInputProvider _inputProvider;
    [SerializeField] private Camera _camera;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _virtualCamera.gameObject.SetActive(true);
            enabled = true;
        }
        base.OnNetworkSpawn();
    }

    private void Awake()
    {
        _camera.fieldOfView = _virtualCamera.m_Lens.FieldOfView;
    }

    private void Update()
    { 
        float z = _inputProvider.GetAxisValue(2);
        if (z != 0)
        {
            ZoomScreen(z);
        }
        _virtualCamera.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY ,_cameraRotationZ);
    }

    private void ZoomScreen(float increment)
    {
        float fov = _virtualCamera.m_Lens.FieldOfView;
        float target = Mathf.Clamp(fov + increment, _zoomInMax, _zoomOutMax);
        _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(fov, target, _zoomSpeed * Time.deltaTime);
        _camera.fieldOfView = Mathf.Lerp(fov, target, _zoomSpeed * Time.deltaTime);
    }
}

