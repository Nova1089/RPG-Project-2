using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        // config params        
        [SerializeField] float attackCooldown = 1f;        
        [SerializeField, Range(0,1)] float runSpeedWhileAttacking = .75f;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;

        // cached references
        Health target;
        Mover myMover;
        ActionScheduler myActionScheduler;
        Animator myAnimator;
        WeaponConfig currentWeaponConfig;
        BaseStats baseStats;
        LazyValue<Weapon> currentWeapon;


        // logic variables
        float timeSinceLastAttack = Mathf.Infinity;

        void Awake()
        {
            myMover = GetComponent<Mover>();
            myActionScheduler = GetComponent<ActionScheduler>();
            myAnimator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);            
        }

        void Start()
        {
            currentWeapon.ForceInit();                                 
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            if (weapon == null) return;
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, myAnimator);
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (!CanAttack(target)) return;
            if (currentWeaponConfig == null) return;

            bool isInWeaponRange = Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.GetRange();

            if (!isInWeaponRange)
            {
                myMover.MoveTo(target.transform.position, runSpeedWhileAttacking);
            }
            else
            {
                myMover.Cancel();
                AttackBehavior();
            }            
        }

        void AttackBehavior()
        {            
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= attackCooldown)
            {
                TriggerAttackAnimation();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttackAnimation()
        {
            myAnimator.ResetTrigger("stopAttacking");
            myAnimator.SetTrigger("attack"); // This animation will trigger the Hit() event.
        }

        // Animation Event
        private void Hit()
        {
            if (CanAttack(target))
            {
                float damage = baseStats.GetStat(Stat.Damage);               

                if (currentWeaponConfig.HasProjectile())
                {
                    currentWeaponConfig.LaunchProjectile(target, rightHandTransform, leftHandTransform, gameObject, damage);
                }
                else
                {
                    target.TakeDamage(gameObject, damage);
                }
                
                if (currentWeapon.value != null)
                {
                    currentWeapon.value.OnHit();
                }
            }
        }

        // Animation event of bow
        void Shoot()
        {
            Hit();
        }

        public bool CanAttack(Health target)
        {
            return target != null && !target.IsDead();
        }

        public void Attack(CombatTarget combatTarget)
        {
            myActionScheduler.StartAction(this);            
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttackAnimation();
            target = null;
            myMover.Cancel();
        }

        private void StopAttackAnimation()
        {
            myAnimator.ResetTrigger("attack");
            myAnimator.SetTrigger("stopAttacking");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            if (currentWeaponConfig == null) return null;
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (String)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public Health GetTarget()
        {
            return target;
        }
    }
}
