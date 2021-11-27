using GameDevTV.Inventories;
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
        [SerializeField] InventoryItem[] dropLibrary;
        [SerializeField] int numberOfDrops = 2;

        // constants
        const int attempts = 30;

        public void RandomDrop()
        {
            for (int i = 0; i < numberOfDrops; i++)
            {
                InventoryItem item = dropLibrary[Random.Range(0, dropLibrary.Length)];
                DropItem(item, 1);
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
