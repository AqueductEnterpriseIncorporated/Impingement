using Impingement.Attributes;
using UnityEngine;

namespace Impingement.Inventory
{
    [CreateAssetMenu(menuName = ("Inventory/Action Items/Heal potion"))]
    public class HealPotionActionItem : ActionItem
    {
        [SerializeField] private int _pointsToHeal;
        public override void Use(GameObject user)
        {
            user.GetComponent<HealthController>().Heal(_pointsToHeal);
        }
    }
}