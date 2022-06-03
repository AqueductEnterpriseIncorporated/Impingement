using Impingement.Control;
using Impingement.Inventory;
using Impingement.Playfab;
using Photon.Pun;
using UnityEngine;

namespace Impingement.UI
{
    public class UpgradePanelController : MonoBehaviour
    {
        //public PlayerCurrencyController InteractingPlayer;

        [SerializeField] private PlayfabHideoutDataController _playfabHideoutDataController;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Transform _content;
        public int CurrencyItemIndex { get; set; }
        public InventoryController PlayerInventoryController { get; set; }

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
            if (other.TryGetComponent<InventoryController>(out var player))
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonView photonView = other.GetComponent<PhotonView>();
                    if (photonView == null || !photonView.IsMine)
                    {
                        return;
                    }

                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }
                }

                PlayerInventoryController = player;

                for (var i = 0; i < player.Slots.Length; i++)
                {
                    var slot = player.Slots[i];
                    if (slot.Item is CurrencyItem)
                    {
                        CurrencyItemIndex = i;
                        break;
                    }
                }

                _panel.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonView photonView = other.GetComponent<PhotonView>();
                if (photonView == null || !photonView.IsMine)
                {
                    return;
                }

                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }
            }

            _panel.SetActive(false);
            PlayerInventoryController = null;
        }
    }
}