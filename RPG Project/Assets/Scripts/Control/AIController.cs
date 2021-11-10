using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        // config params
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 4f;
        [SerializeField] float aggroCooldown = 5f;
        [SerializeField] PatrolPath myPatrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        [SerializeField, Range(0, 1)] float patrolSpeedFraction = .2f;
        [SerializeField] float shoutDistance = 5f;

        // cached references
        Health player;
        Fighter myFighter;
        Health myHealth;
        Mover myMover;
        ActionScheduler myActionScheduler;        

        // logic variables
        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        int currentWaypointIndex = 0;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;        

        void Awake()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Health>();
            myFighter = GetComponent<Fighter>();
            myHealth = GetComponent<Health>();
            myMover = GetComponent<Mover>();
            myActionScheduler = GetComponent<ActionScheduler>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        void Start()
        {
            guardPosition.ForceInit();
        }

        void Update()
        {
            if (player == null) return;
            if (myHealth.IsDead()) return;

            if (IsWithinChaseRange())
            {
                AggravateAndShout();
            }

            if (IsAggravated() && myFighter.CanAttack(player))
            {                
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }
            UpdateTimers();
        }
        
        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }

        // Called from a Unity Event on enemy prefab health script
        public void AggravateAndShout()
        {
            Aggravate();
            AggravateNearbyEnemies();
        }

        Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        bool IsAggravated()
        {
            if (player == null) return false;

            if (timeSinceAggravated < aggroCooldown) return true; 

            return Vector3.Distance(this.transform.position, player.transform.position) <= chaseDistance;
        }

        bool IsWithinChaseRange()
        {
            return Vector3.Distance(this.transform.position, player.transform.position) <= chaseDistance;
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            myFighter.Attack(player.GetComponent<CombatTarget>());            
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;
                ai.Aggravate();
            }
        }  

        private void SuspicionBehavior()
        {
            myActionScheduler.CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardPosition.value;            

            if (myPatrolPath != null)
            {
                if (isAtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoints();                                        
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint >= waypointDwellTime)
            {
                myMover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }            
        }

        private bool isAtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoints()
        {
            currentWaypointIndex = myPatrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return myPatrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        // called by Unity editor when the object is selected
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
