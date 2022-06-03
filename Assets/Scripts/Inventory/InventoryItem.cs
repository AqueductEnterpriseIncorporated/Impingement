using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// A ScriptableObject that represents any Item that can be put in an
    /// playerInventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or
    /// `EquipableItem`.
    /// </remarks>
    [CreateAssetMenu(menuName = ("Inventory/Item"))]
    public class InventoryItem : ScriptableObject, ISerializationCallbackReceiver    {
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField] private string _itemID = null;
        [Tooltip("Item name to be displayed in UI.")]
        [SerializeField] private string _displayName = null;
        [Tooltip("Item _description to be displayed in UI.")]
        [SerializeField][TextArea] private string _description = null;
        [Tooltip("The UI _icon to represent this Item in the playerInventory.")]
        [SerializeField] private Sprite _icon = null;
        [Tooltip("The prefab that should be spawned when this Item is dropped.")]
        [SerializeField] private Pickup _pickup = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same playerInventory slot.")]
        [SerializeField] private bool _stackable = false;

        public static Dictionary<string, InventoryItem> ItemLookupCache;
        
        /// <summary>
        /// Get the playerInventory Item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// InventoryController Item instance corresponding to the ID.
        /// </returns>
        public static InventoryItem GetFromID(string itemID)
        {
            if (ItemLookupCache == null)
            {
                ItemLookupCache = new Dictionary<string, InventoryItem>();
                var itemList = Resources.LoadAll<InventoryItem>("");
                foreach (var item in itemList)
                {
                    if (ItemLookupCache.ContainsKey(item._itemID))
                    {
                        Debug.LogError(string.Format("Its a duplicate Impingement.UI.InventorySystem ID for objects: {0} and {1}", ItemLookupCache[item._itemID], item));
                        continue;
                    }

                    ItemLookupCache[item._itemID] = item;
                }
            }

            if (itemID == null || !ItemLookupCache.ContainsKey(itemID)) return null;
            return ItemLookupCache[itemID];
        }

        /// <summary>
        /// Spawn the pickup gameobject into the world.
        /// </summary>
        /// <param name="position">Where to spawn the pickup.</param>
        /// <returns>Reference to the pickup object spawned.</returns>
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            Pickup pickup = null;
            if (PhotonNetwork.InRoom)
            {
                pickup = PhotonNetwork.Instantiate("ItemPickups/" + _pickup.name, position, Quaternion.identity).GetComponent<Pickup>();
            }
            else
            {
                pickup = Instantiate(_pickup);
                pickup.transform.position = position;
            }
            pickup.Setup(this, number);
            return pickup;
        }
        
        public Sprite GetIcon()
        {
            return _icon;
        }

        public string GetItemID()
        {
            return _itemID;
        }

        public bool IsStackable()
        {
            return _stackable;
        }
        
        public string GetDisplayName()
        {
            return _displayName;
        }

        public string GetDescription()
        {
            return _description;
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
    }
}
