using Impingement.Control;
using Impingement.Inventory;
using Impingement.UI.Dragging;
using Impingement.UI.Tooltip;
using TMPro;
using UnityEngine;

namespace Impingement.UI.InventoryUI
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] private InventoryItemIcon _icon = null;
        [SerializeField] private int _index = 0;
        [SerializeField] private TMP_Text _hotKey;
        [SerializeField] private InputManager _inputManager;

        private ActionStore _store;
        
        private void Awake()
        {
            _store = GetComponentInParent<InventoryUI>().CurrentPlayerController.GetComponent<ActionStore>();
            _store.OnStoreUpdated += UpdateIcon;
            _hotKey.text = _inputManager.GetButtonKeyCodeAsString(string.Concat("Активная", _index + 1));
            _inputManager.HotKeyChanged += InputManagerOnHotKeyChanged;
        }

        private void InputManagerOnHotKeyChanged(string buttonName, string buttonCode)
        {
            if (buttonName == string.Concat("Активная", _index + 1))
            {
                _hotKey.text = buttonCode;
            }
        }
        
        public void AddItems(InventoryItem item, int number)
        {
            _store.AddAction(item, _index, number);
        }

        public InventoryItem GetItem()
        {
            return _store.GetAction(_index);
        }

        public int GetNumber()
        {
            return _store.GetNumber(_index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return _store.MaxAcceptable(item, _index);
        }

        public void RemoveItems(int number)
        {
            _store.RemoveItems(_index, number);
        }
        
        void UpdateIcon()
        {
            _icon.SetItem(GetItem(), GetNumber());
        }
    }
}
