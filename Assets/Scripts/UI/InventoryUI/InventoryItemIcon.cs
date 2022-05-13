using Impingement.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI.InventoryUI
{
    /// <summary>
    /// To be put on the icon representing an playerInventory Item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] private GameObject _textContainter;
        [SerializeField] private TMP_Text _itemNumber;
        
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
                if (number > 0)
                {
                    _textContainter.SetActive(true);
                    _itemNumber.text = number.ToString();
                }
                else
                {
                    _textContainter.SetActive(false);
                }
            }
        }
    }
}