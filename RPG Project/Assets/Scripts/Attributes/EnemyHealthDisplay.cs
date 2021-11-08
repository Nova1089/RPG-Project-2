using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        // cached references
        Text healthValueText;
        GameObject player;
        Fighter fighter;
        Health enemyHealth;

        void Awake()
        {
            healthValueText = GetComponent<Text>();
            player = GameObject.FindWithTag("Player");
            fighter = player.GetComponent<Fighter>();            
        }

        void Update()
        {
            enemyHealth = fighter.GetTarget();
            if (enemyHealth == null)
            {
                healthValueText.text = "N/A";
            }
            else
            {
                healthValueText.text = $"{enemyHealth.GetCurrentHP():0.0} / {enemyHealth.GetMaxHP()}";
            }
        }
    }
}

