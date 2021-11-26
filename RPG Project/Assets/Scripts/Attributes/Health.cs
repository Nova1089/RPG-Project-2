using GameDevTV.Utils;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {    
        // cached references
        Animator myAnimator;
        ActionScheduler myActionScheduler;
        BaseStats baseStats;

        // logic variables
        bool isDead = false;
        LazyValue<float> healthPoints;

        // events
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;

        void Awake()
        {
            myAnimator = GetComponent<Animator>();
            myActionScheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        void OnEnable()
        {
            baseStats.onLevelUp += SetHealthOnLevelUp;
        }

        void Start()
        {
            healthPoints.ForceInit();            
        }

        void Update()
        {
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }

        void OnDisable()
        {
            baseStats.onLevelUp -= SetHealthOnLevelUp;
        }

        private void Die()
        {
            if (isDead == false)
            {
                myAnimator.SetTrigger("die");
                isDead = true;
                myActionScheduler.CancelCurrentAction();                
            }            
        }

        public void TakeDamage(GameObject attacker, float damageAmount)
        {            
            takeDamage.Invoke(damageAmount);
            if (healthPoints.value - damageAmount > 0)
            {
                healthPoints.value -= damageAmount;
            }
            else
            {
                RewardExperience(attacker);
                healthPoints.value = 0;
                onDie.Invoke();
                Die();
            }
        }

        private void RewardExperience(GameObject attacker)
        {
            Experience attackerExperience = attacker.GetComponent<Experience>();
            if (attackerExperience != null)
            {
                float xpAmount = baseStats.GetStat(Stat.ExperienceReward);
                attackerExperience.GainExperience(xpAmount);
            }
        }

        public float GetPercentage()
        {
            return healthPoints.value / baseStats.GetStat(Stat.Health) * 100;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }

        void SetHealthOnLevelUp(float currentLevelMaxHP, float nextLevelMaxHP)
        {
            float currentPercentOfMaxHealth = healthPoints.value / currentLevelMaxHP;
            healthPoints.value = currentPercentOfMaxHealth * nextLevelMaxHP;
        }

        public float GetCurrentHP()
        {
            return healthPoints.value;
        }

        public float GetMaxHP()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public void Heal(float amount)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + amount, GetMaxHP());
        }
    } 
} 
