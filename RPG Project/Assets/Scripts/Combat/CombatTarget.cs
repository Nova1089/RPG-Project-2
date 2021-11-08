using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    
    public class CombatTarget : MonoBehaviour, IRaycastable
    {        
        // cached references
        Health myHealth;
        CapsuleCollider myCapsuleCollider;
        PlayerController playerController;

        void Awake()
        {
            myHealth = GetComponent<Health>();
            myCapsuleCollider = GetComponent<CapsuleCollider>();
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }

        void Update()
        {
            if (myHealth.IsDead() && myCapsuleCollider != null)
            {
                myCapsuleCollider.enabled = false;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (fighter == null) return false;

            if (fighter.CanAttack(myHealth))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    fighter.Attack(this);                    
                }
                return true;
            }
            return false;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}

