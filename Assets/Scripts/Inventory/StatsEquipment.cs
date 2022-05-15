using System.Collections.Generic;
using Impingement.enums;
using Impingement.Stats;

namespace Impingement.Inventory
{
    public class StatsEquipment : EquipmentController, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(enumStats stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if(item == null) { continue; }

                foreach (var modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(enumStats stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if(item == null) { continue; }

                foreach (var modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}