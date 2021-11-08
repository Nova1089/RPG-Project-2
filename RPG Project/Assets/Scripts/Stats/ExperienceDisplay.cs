using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        // cached references
        Text textComponent;
        GameObject player;
        Experience experience;

        void Awake()
        {
            textComponent = GetComponent<Text>();
            player = GameObject.FindWithTag("Player");
            experience = player.GetComponent<Experience>();
        }

        void Update()
        {
            float experiencePoints = experience.GetExperiencePoints();
            textComponent.text = $"{experiencePoints}";
        }
    }
}
