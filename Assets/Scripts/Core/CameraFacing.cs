using System;
using UnityEngine;

namespace Impingement.Core
{
    public class CameraFacing : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera)
            {
                transform.forward = _camera.transform.forward;
            }
            else
            {
                _camera = Camera.main;
            }
        }
    }
}