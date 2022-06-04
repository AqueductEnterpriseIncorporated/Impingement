using System;
using Impingement.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI
{
    public class PlayerPanelItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _healthBar;
        private PlayerController _playerController;

        public void SetupPlayer(PlayerController playerController)
        {
            _playerController = playerController;
            _nameText.text = playerController.GetPhotonView().Controller.NickName;
        }

        private void Update()
        {
            if (gameObject.activeSelf)
            {
                UpdateHealthBar();

                print(_playerController.GetHealthController().CharacterName + _playerController.GetHealthController().GetHealthPoints());
            }
        }

        private void OnEnable()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (_playerController is null)
            {
                return;
            }

            var target = _playerController.GetHealthController();
            _healthBar.fillAmount = target.GetHealthPoints() / target.GetMaxHealthPoints();
        }
    }
}