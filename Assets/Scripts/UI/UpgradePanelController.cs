using Impingement.Control;
using Impingement.Currency;
using Impingement.Playfab;
using UnityEngine;

namespace Impingement.UI
{
    public class UpgradePanelController : MonoBehaviour
    {
        public PlayerCurrencyController InteractingPlayer;

        [SerializeField] private PlayfabHideoutDataController _playfabHideoutDataController;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Transform _content;
        private PlayerController _playerController;

        public PlayfabHideoutDataController GetController()
        {
            return _playfabHideoutDataController;
        }

        public HideoutUpgradeItemView[] GetAllItems()
        {
            HideoutUpgradeItemView[] itemViews = new HideoutUpgradeItemView[_content.childCount];
            for (int i = 0; i < _content.childCount; i++)
            {
                itemViews[i] = _content.GetChild(i).gameObject.GetComponent<HideoutUpgradeItemView>();
            }

            return itemViews;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                _playerController = player;
                if(!player.GetPhotonView().IsMine){return;}
                InteractingPlayer = player.GetPlayerCurrencyController();
                _panel.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(!_playerController.GetPhotonView().IsMine){return;}

            _panel.SetActive(false);
        }
    }
}