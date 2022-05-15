using System.Collections.Generic;
using UnityEngine;

namespace Impingement.Inventory
{
    [CreateAssetMenu(menuName = ("Inventory/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] private DropConfig[] _potentialDrops;
        [SerializeField] private float[] _dropChancePercentage;
        [SerializeField] private int[] _minDrops;
        [SerializeField] private int[] _maxDrops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem Item;
            public float[] RelativeChance;
            public int[] MinNumber;
            public int[] MaxNumber;
            public int GetRandomNumber(int level)
            {
                if (!Item.IsStackable())
                {
                    return 1;
                }
                int min = GetByLevel(MinNumber, level);
                int max = GetByLevel(MaxNumber, level);
                return UnityEngine.Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem Item;
            public int Number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        bool ShouldRandomDrop(int level)
        {
            return Random.Range(0, 100) < GetByLevel(_dropChancePercentage, level);
        }

        int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(_minDrops, level);
            int max = GetByLevel(_maxDrops, level);
            return Random.Range(min, max);
        }

        Dropped GetRandomDrop(int level)
        {
            var drop = SelectRandomItem(level);
            var result = new Dropped();
            result.Item = drop.Item;
            result.Number = drop.GetRandomNumber(level);
            return result;
        }

        DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0;
            foreach (var drop in _potentialDrops)
            {
                chanceTotal += GetByLevel(drop.RelativeChance, level);
                if (chanceTotal > randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }

        float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var drop in _potentialDrops)
            {
                total += GetByLevel(drop.RelativeChance, level);
            }
            return total;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }
            if (level > values.Length)
            {
                return values[values.Length - 1];
            }
            if (level <= 0)
            {
                return default;
            }
            return values[level - 1];
        }
    }
}