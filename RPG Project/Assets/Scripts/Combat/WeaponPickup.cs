using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        // config params
        [SerializeField] WeaponConfig weapon;
        [SerializeField] float respawnTime = 3f;
        [SerializeField] float healthToRestore = 0f;

        void OnTriggerEnter(Collider otherCollider)
        {
            if (otherCollider.tag != "Player") return;
            Pickup(otherCollider.gameObject);
        }

        private void Pickup(GameObject subject)
        {
            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }

            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            Mover playerMover = callingController.GetComponent<Mover>();

            if (!playerMover.CanMoveTo(transform.position)) return false;
            
            if (Input.GetMouseButtonDown(0))
            {
                playerMover.StartMoveAction(transform.position, 1f);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}

