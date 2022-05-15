using System.Collections.Generic;
using Impingement.enums;
using Impingement.Stats;
using UnityEngine;

namespace Impingement.Inventory
{
    [CreateAssetMenu (menuName = ("Inventory/Equipable Item with stats"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] private Modifier[] _additiveModifiers;
        [SerializeField] private Modifier[] _percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public enumStats Stat;
            public float Value;
        }

        public IEnumerable<float> GetAdditiveModifiers(enumStats stat)
        {
            foreach (var modifier in _additiveModifiers)
            {
                if (modifier.Stat == stat)
                {
                    yield return modifier.Value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(enumStats stat)
        {
            foreach (var modifier in _percentageModifiers)
            {
                if (modifier.Stat == stat)
                {
                    yield return modifier.Value;
                }
            }
        }
    }
}