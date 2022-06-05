using Impingement.Attributes;
using Impingement.Control;
using UnityEngine;

namespace Impingement.Inventory
{
    [CreateAssetMenu(menuName = ("Inventory/Action Items/Heal potion"))]
    public class HealPotionActionItem : ActionItem
    {
        [SerializeField] private int _pointsToHeal;
        
        public override void Use(PlayerController player, AudioSource audioSource)
        {
            base.Use(player, audioSource);
            player.GetHealthController().Heal(_pointsToHeal);
        }
    }
}