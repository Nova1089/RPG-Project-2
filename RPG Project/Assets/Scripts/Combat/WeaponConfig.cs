using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using System;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem
    {
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float damage = 20f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float range = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if (weaponPrefab != null)
            {
                Transform handTransform = GetCorrectHand(rightHand, leftHand);

                weapon = Instantiate(weaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetCorrectHand(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;

            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }

            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Health target, Transform rightHand, Transform leftHand, GameObject attacker, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetCorrectHand(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, attacker, calculatedDamage);
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }    

        public float GetRange()
        {
            return range;
        }
    }
}