using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        // cached references
        GameObject player;
        Health health;
        Text healthValueText;

        void Awake()
        {
            player = GameObject.FindWithTag("Player");
            health = player.GetComponent<Health>();
            healthValueText = GetComponent<Text>();
        }

        void Update()
        {
            healthValueText.text = $"{health.GetCurrentHP():0.0} / {health.GetMaxHP()}";
        }
    }
}

