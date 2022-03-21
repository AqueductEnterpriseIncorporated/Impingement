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
            if (other.TryGetComponent<PlayerCurrencyController>(out var player))
            {
                InteractingPlayer = player;
                _panel.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            _panel.SetActive(false);
        }
    }
}