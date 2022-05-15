using Impingement.Inventory;
using Impingement.UI.Dragging;
using Impingement.UI.Tooltip;
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

        private ActionStore _store;

        //todo: fix
        private void Awake()
        {
            _store = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
            _store.OnStoreUpdated += UpdateIcon;
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
