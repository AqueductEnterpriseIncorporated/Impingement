using Impingement.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI.InventoryUI
{
    /// <summary>
    /// To be put on the icon representing an playerInventory Item. Allows the slot to
    /// update the icon and Number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] private GameObject _textContainer;
        [SerializeField] private TMP_Text _itemNumber;
        
        public void SetItem(InventoryItem item)
        {
            SetItem(item, 0);
        }
        
        public void SetItem(InventoryItem item, int number)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();
            }

            if (_itemNumber)
            {
                if (number <= 1)
                {
                    _textContainer.SetActive(false);
                }
                else
                {
                    _textContainer.SetActive(true);
                    _itemNumber.text = number.ToString();
                }
            }
        }
    }
}