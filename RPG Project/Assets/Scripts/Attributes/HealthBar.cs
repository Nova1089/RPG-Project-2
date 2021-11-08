using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        // Config params
        [SerializeField] Image foreground;
        
        // cached references
        Health health;
        Canvas canvas;

        void Awake()
        {
            health = GetComponentInParent<Health>();
            canvas = GetComponentInChildren<Canvas>();
        }
        void Update()
        {
            if (foreground == null) return;
            if (health == null) return;
            HandleEnabling();
            HandleMovingHealthBar();
        }

        void HandleEnabling()
        {
            if (health.GetPercentage() < 100f && health.GetPercentage() > 0f)
            {
                canvas.enabled = true;
            }
            else
            {
                canvas.enabled = false;
            }
        }

        private void HandleMovingHealthBar()
        {
            foreground.transform.localScale = new Vector3(health.GetPercentage() / 100, 1, 1);
        }
    }
}
