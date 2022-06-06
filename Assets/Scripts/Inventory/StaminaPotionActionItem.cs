using Impingement.Attributes;
using Impingement.Control;
using UnityEngine;

namespace Impingement.Inventory
{
    [CreateAssetMenu(menuName = ("Inventory/Action Items/Stamina potion"))]
    public class StaminaPotionActionItem : ActionItem
    {
        [SerializeField] private int _pointsToHeal;
        
        public override void Use(PlayerController player, AudioSource audioSource)
        {
            base.Use(player, audioSource);
            player.GetStaminaController().AddStamina(_pointsToHeal);
        }
    }
}