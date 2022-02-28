using Cinemachine;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Core
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField] private float _zoomSpeed = 10f;
        [SerializeField] private float _zoomInMax = 30f;
        [SerializeField] private float _zoomOutMax = 20f;
        [SerializeField] CinemachineVirtualCamera _virtualCamera;
        [SerializeField] CinemachineInputProvider _inputProvider;
        [SerializeField] Camera _camera;
        private PhotonView _photonView;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            if (!_photonView.IsMine) { return; }
            _virtualCamera.m_Lens.FieldOfView = _zoomInMax;
            _virtualCamera.m_Follow = gameObject.transform;
            _camera.fieldOfView = _virtualCamera.m_Lens.FieldOfView;
        }

        public Camera GetPlayerCamera()
        {
            return _camera;
        }

        private void Update()
        {
            //if(!IsOwner) { return;}
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
        }
    }
}

