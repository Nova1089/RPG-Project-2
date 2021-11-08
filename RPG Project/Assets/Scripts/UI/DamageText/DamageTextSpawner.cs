using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        // config params
        [SerializeField] DamageText damageTextPrefab;

        public void Spawn(float damageAmount)
        {
            if (damageTextPrefab == null) return;
            DamageText prefabInstance = Instantiate(damageTextPrefab, this.transform);
            Text text = prefabInstance.GetComponentInChildren<Text>();
            text.text = $"{damageAmount}";
        }
    }
}
