using Impingement.Inventory;
using UnityEngine;
using TMPro;

namespace Impingement.UI.Tooltip
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private TextMeshProUGUI _bodyText = null;
        
        public void Setup(InventoryItem item)
        {
            _titleText.text = item.GetDisplayName();
            _bodyText.text = item.GetDescription();
        }
    }
}
