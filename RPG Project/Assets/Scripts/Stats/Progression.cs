using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookupTable();
            Dictionary<Stat, float[]> statDictionary = lookupTable[characterClass];
            if (statDictionary[stat].Length < level) return statDictionary[stat][statDictionary[stat].Length - 1];
            return statDictionary[stat][level - 1];
        }

        public int GetNumberOfLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookupTable();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }
        
        private void BuildLookupTable()
        {
            if (lookupTable != null) return;
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progClass in characterClasses)
            {
                lookupTable.Add(progClass.characterClass, BuildStatDictionary(progClass));
            }
        }

        private Dictionary<Stat, float[]> BuildStatDictionary(ProgressionCharacterClass progClass)
        {
            Dictionary<Stat, float[]> statDictionary = new Dictionary<Stat, float[]>();

            foreach (ProgressionStat progStat in progClass.stats)
            {
                statDictionary.Add(progStat.stat, progStat.levels);
            }
            return statDictionary;
        }    

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;            
        }
        
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}