using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Core
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [SerializeField] private float _zoomSpeed = 10f;
        [SerializeField] private float _zoomInMax = 30f;
        [SerializeField] private float _zoomOutMax = 20f;
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineInputProvider _inputProvider;
        private Camera _camera;

        private void Start()
        {
            if(!IsOwner) { return; }
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            _inputProvider = _virtualCamera.GetComponent<CinemachineInputProvider>();
            _camera = _virtualCamera.GetComponent<Camera>();
            _virtualCamera.m_Follow = gameObject.transform;
            _camera.fieldOfView = _virtualCamera.m_Lens.FieldOfView;
        }

        public Camera GetPlayerCamera()
        {
            return _camera;
        }

        private void Update()
        {
            float z = _inputProvider.GetAxisValue(2);
            if (z != 0)
            {
                ZoomScreen(z);
            }
        }

        private void ZoomScreen(float increment)
        {
            float fov = _virtualCamera.m_Lens.FieldOfView;
            float target = Mathf.Clamp(fov + increment, _zoomInMax, _zoomOutMax);
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(fov, target, _zoomSpeed * Time.deltaTime);
            _camera.fieldOfView = Mathf.Lerp(fov, target, _zoomSpeed * Time.deltaTime);
            print(_virtualCamera.m_Lens.FieldOfView);
        }
    }
}

