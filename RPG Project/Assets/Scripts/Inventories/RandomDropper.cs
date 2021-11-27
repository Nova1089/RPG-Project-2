using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        // configs
        [Tooltip("How far the pickups can be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1;
        [SerializeField] DropLibrary dropLibrary;        

        // constants
        const int attempts = 30;

        // cached references
        BaseStats baseStats;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
        }

        public void RandomDrop()
        {
            var baseStats = GetComponent<BaseStats>();
            if (baseStats == null) return;
            
            var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.item, drop.number);
            }         
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < attempts; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPoint, out hit, .1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }
    }
}
