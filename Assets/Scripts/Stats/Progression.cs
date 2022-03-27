using System.Collections.Generic;
using Impingement.enums;
using UnityEngine;

namespace Impingement.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Make new progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] _characterClasses = null;

        private Dictionary<enumCharacterClass, Dictionary<enumStats, float[]>> _lookupTable = null;
        
        public float GetStat(enumStats stat, enumCharacterClass characterClass, int level)
        {
            BuildLookup();
            float[] levels = _lookupTable[characterClass][stat];
            if (levels.Length < level) { return 0; }
            return levels[Mathf.Max(0, level - 1)];
        }

        public int GetLevels(enumStats stat, enumCharacterClass characterClass)
        {
            BuildLookup();
            float[] levels = _lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if(_lookupTable != null) { return; }

            _lookupTable = new Dictionary<enumCharacterClass, Dictionary<enumStats, float[]>>();
            
            foreach (var progressionCharacterClass in _characterClasses)
            {
                var startLookupTable = new Dictionary<enumStats, float[]>();
                
                foreach (var progressionStat in progressionCharacterClass.Stats)
                {
                    startLookupTable[progressionStat.stat] = progressionStat.Levels;
                }
                
                _lookupTable[progressionCharacterClass.CharacterClass] = startLookupTable;
            }
        }

        [System.Serializable]
        private class ProgressionCharacterClass
        {
            public enumCharacterClass CharacterClass;
            public ProgressionStat[] Stats;
        }
        
        [System.Serializable]
        private class ProgressionStat
        {
            public enumStats stat;
            public float[] Levels;
        }
    }
}