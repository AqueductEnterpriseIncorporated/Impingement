using System;
using System.Collections;
using Impingement.Playfab;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Impingement.Core
{
    using UnityEngine;

    public class StartSceneManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private PostProcessVolume _processVolume;
        [SerializeField] private Button _startButton;
        [SerializeField] private PlayfabManager _playfabManager;
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private GameObject _eyeGameObject;
        [SerializeField] private Vector3 _defaultPosition = new Vector3(0, 0,0);
        [SerializeField] private Transform _camera;
        [SerializeField] private float _playCooldown = 5f;
        [SerializeField] private float _timer;
        private bool _isDefaultPosition;
        private bool _isLoading;
        
        private void Start()
        {
            _playfabManager = FindObjectOfType<PlayfabManager>();

            _playfabManager.ValueSyncedAndConnected += OnValueSyncedAndConnected;
        }

        private void Update()
        {
            if(_isLoading) { return; }
            _timer += Time.deltaTime;
            _eyeGameObject.transform.LookAt(_camera);

            if (_timer > _playCooldown)
            {
                StartCoroutine(PlayVideo());
                MoveEye();
            }
        }
        
        private IEnumerator PlayVideo()
        {
            _videoPlayer.targetCameraAlpha = 1;

            yield return new WaitForSeconds(0.75f);

            _videoPlayer.targetCameraAlpha = 0;
            _timer = 0;
        }
        
        private void MoveEye()
        {
            if (!_isDefaultPosition)
            {
                _eyeGameObject.transform.position = _defaultPosition;
                _isDefaultPosition = true;
                return;
            }

            var randomX = Random.Range(-7, 7);
            var randomY = Random.Range(2.5f, 2.5f);
            Vector3 randomVector = new Vector3(randomX, randomY, 0);
            _isDefaultPosition = false;
            _eyeGameObject.transform.position = Random.insideUnitSphere * 2.5f;
        }
        
        public void LoadHideout()
        {
            if (_inputField.text == null) { return; }
            StartCoroutine(IncreaseIntencity());
        }

        private IEnumerator IncreaseIntencity()
        {
            _eyeGameObject.transform.position = _defaultPosition;

            var lensDistortion =  _processVolume.profile.GetSetting<LensDistortion>();
            lensDistortion.enabled = new BoolParameter(){value = true};
            _playfabManager.Login(_inputField.text);
            _startButton.enabled = false;
            _isLoading = true;
            for (int i = 0; i < 101; i++)
            {
                lensDistortion.intensity.value = i;
                yield return new WaitForSeconds(0.01f);
            }
            for (float i = 0.3f; i < 1.5f; i += 0.1f)
            {
                lensDistortion.scale.value = i;
                yield return new WaitForSeconds(0.005f);
            }
        }
        
        private void OnValueSyncedAndConnected(bool value)
        {
            SceneManager.LoadSceneAsync("Hideout");
        }
    }
}