using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using GameDevTV.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using GameDevTV.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
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
        Equipment equipment;


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
            equipment = GetComponent<Equipment>();
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);            
        }

        void OnEnable()
        {
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        void Start()
        {
            currentWeapon.ForceInit();                                 
        }

        private void UpdateWeapon()
        {
            EquipableItem item = equipment.GetItemInSlot(EquipLocation.Weapon);

            if (item)
            {
                WeaponConfig weapon = (WeaponConfig)item;
                EquipWeapon(weapon);
            }
            else
            {
                EquipWeapon(defaultWeapon);
            }

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

            if (!IsInWeaponRange(target.transform))
            {
                myMover.MoveTo(target.transform.position, runSpeedWhileAttacking);
            }
            else
            {
                myMover.Cancel();
                AttackBehavior();
            }            
        }

        bool IsInWeaponRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.GetRange();
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
            if (target == null) return false;
            if (target.IsDead()) return false;
            if (!myMover.CanMoveTo(target.transform.position) && !IsInWeaponRange(target.transform)) return false;
            return true;
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
