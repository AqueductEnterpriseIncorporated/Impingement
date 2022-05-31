using Impingement.Inventory;
using Impingement.UI.Dragging;
using UnityEngine;

namespace Impingement.UI.InventoryUI
{
    /// <summary>
    /// Handles spawning pickups when Item dropped into the world.
    /// 
    /// Must be placed on the root canvas where items can be dragged. Will be
    /// called if dropped over empty space. 
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        public void AddItems(InventoryItem item, int number)
        {
            var player = GetComponentInParent<InventoryUI>().CurrentPlayerController;
            player.GetComponent<ItemDropper>().DropItem(item, number);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return int.MaxValue;
        }
    }
}