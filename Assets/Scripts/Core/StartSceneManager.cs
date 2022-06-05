using System;
using System.Collections;
using System.Linq;
using Impingement.Playfab;
using Impingement.Steam;
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
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private SteamAuth _steamAuth;
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
        private readonly string _noInternetException = "/Client/LoginWithCustomID: Cannot resolve destination host";
        private readonly string _noSteamException = "Steamworks is not initialized.";
        
        private void Start()
        {
            _playfabManager = FindObjectOfType<PlayfabManager>();

            _playfabManager.ValueSyncedAndConnected += OnValueSyncedAndConnected;
        }

        private void Update()
        {
            if (_steamAuth.isActiveAndEnabled && _playfabManager.IsAuthorized)
            {
                _statusText.text = "\nSteam авторизован\nPlyfab авторизован";
            }
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
                ResetEye();
                return;
            }

            var randomX = Random.Range(-7, 7);
            var randomY = Random.Range(2.5f, 2.5f);
            Vector3 randomVector = new Vector3(randomX, randomY, 0);
            _isDefaultPosition = false;
            _eyeGameObject.transform.position = Random.insideUnitSphere * 2.5f;
        }

        private void ResetEye()
        {
            _eyeGameObject.transform.position = _defaultPosition;
            _isDefaultPosition = true;
            var lensDistortion =  _processVolume.profile.GetSetting<LensDistortion>();
            lensDistortion.enabled = new BoolParameter(){value = true};
            lensDistortion.intensity.value = 0;
            lensDistortion.scale.value = 0.3f;
        }

        public void LoadHideout()
        {
            var newStatusText = "";
            if (!_playfabManager.IsAuthorized || !_steamAuth.IsAuthorized)
            {
                if (!_playfabManager.IsAuthorized)
                {
                    newStatusText += "Playfab не авторизован";
                }
                if (!_steamAuth.IsAuthorized)
                {
                    newStatusText += "Steam не авторизован";
                }

                _statusText.text = newStatusText;
                return;
            }
            StartCoroutine(IncreaseIntencity());
        }

        private IEnumerator IncreaseIntencity()
        {
            _eyeGameObject.transform.position = _defaultPosition;

            var lensDistortion =  _processVolume.profile.GetSetting<LensDistortion>();
            lensDistortion.enabled = new BoolParameter(){value = true};
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

            SceneManager.LoadSceneAsync("Hideout");
        }

        public void Login(string id)
        {
            _playfabManager.Login(id);
            _statusText.text = "";
            //_startButton.enabled = false;
            _isLoading = true;
        }

        private void OnValueSyncedAndConnected(bool value)
        {
            //SceneManager.LoadSceneAsync("Hideout");
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void Error(string errorReport)
        {
            _statusText.text = "Ошибка: " + errorReport;
            if (errorReport == _noInternetException)
            {
                var text = _statusText.text.Insert(0, "Отсутствует интернет\n\n");
                _statusText.text = text;
            }
            if (errorReport == _noSteamException)
            {
                var text = _statusText.text.Insert(0, "Steam не запущен\n\n");
                _statusText.text = text;
            }
            //_startButton.enabled = true;
            _isLoading = false;
            ResetEye();
        }
    }
}