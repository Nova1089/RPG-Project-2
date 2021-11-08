using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    // cached references
    Text textComponent;
    GameObject player;
    BaseStats playerBaseStats;

    void Awake()
    {
        textComponent = GetComponent<Text>();
        player = GameObject.FindWithTag("Player");
        playerBaseStats = player.GetComponent<BaseStats>();
    }

    void Update()
    {
        textComponent.text = $"{playerBaseStats.GetLevel()}";
    }
}
