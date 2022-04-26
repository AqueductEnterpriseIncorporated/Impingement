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
            _healthBar.fillAmount = _playerController.GetStaminaController().GetCurrentStaminaPoints() /
                                    _playerController.GetStaminaController().GetMaximumStaminaPoints();
        }
    }
}