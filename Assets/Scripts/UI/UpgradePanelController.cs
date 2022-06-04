using System;
using Impingement.Inventory;
using Impingement.Playfab;
using UnityEngine;

namespace Impingement.UI
{
    public class UpgradePanelController : MonoBehaviour
    {
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
            _panel.SetActive(false);
            PlayerInventoryController = null;
        }

        private void OnEnable()
        {
            FindObjectOfType<ScrollController>().HudObjects.Add(_panel.transform);
        }

        private void OnDestroy()
        {
            // var controller = FindObjectOfType<ScrollController>();
            // if (controller != null && controller.HudObjects.Contains(_panel.transform))
            // {
            //     controller.HudObjects.Remove(_panel.transform);
            // }
        }
    }
}