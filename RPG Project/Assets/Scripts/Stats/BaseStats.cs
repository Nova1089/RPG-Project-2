using GameDevTV.Utils;
using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField, Range(1, 99)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpFX;
        [SerializeField] bool shouldUseModifiers = false;

        // cached references
        Experience experience;

        // state variables
        LazyValue<int> currentLevel;

        // events
        public delegate void LevelUpDelegate(float currentLevelMaxHP, float nextLevelMaxHP);
        public event LevelUpDelegate onLevelUp;

        void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        void Start()
        {
            currentLevel.ForceInit();
        }

        void OnDisable()
        {
            // experience.onExperienceGained -= UpdateLevel;
        }

        void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                float currentLevelMaxHP = progression.GetStat(Stat.Health, characterClass, currentLevel.value);
                currentLevel.value = newLevel;
                float nextLevelMaxHP = progression.GetStat(Stat.Health, characterClass, currentLevel.value);
                onLevelUp(currentLevelMaxHP, nextLevelMaxHP);
                PlayLevelUpFX();
            }            
        }

        private void PlayLevelUpFX()
        {
            Instantiate(levelUpFX, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStatValue(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStatValue(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            foreach (IModifierProvider provider in providers)
            {
                IEnumerable<float> modifiers = provider.GetAdditiveModifiers(stat);
                foreach (float modifier in modifiers)
                {
                    total += modifier;
                }                
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0f;
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            foreach (IModifierProvider provider in providers)
            {
                IEnumerable<float> modifiers = provider.GetPercentageModifiers(stat);
                foreach (float modifier in modifiers)
                {
                    total += modifier;
                }
            }
            return total;
        }

        private int CalculateLevel()
        {
            if (experience == null) return startingLevel;
            float currentXP = experience.GetExperiencePoints();
            int penultimateLevel = progression.GetNumberOfLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    }
}
