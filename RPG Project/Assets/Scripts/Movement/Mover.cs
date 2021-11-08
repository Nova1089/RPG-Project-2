using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        // config params
        [SerializeField] float maxSpeed = 6f;
        
        // cached references
        [SerializeField] Transform target;
        NavMeshAgent myNavMeshAgent;
        Animator myAnimator;
        ActionScheduler myActionScheduler;
        Health myHealth;

        void Awake()
        {
            myNavMeshAgent = GetComponent<NavMeshAgent>();
            myAnimator = GetComponent<Animator>();
            myActionScheduler = GetComponent<ActionScheduler>();
            myHealth = GetComponent<Health>();            
        }

        void Update()
        {
            myNavMeshAgent.enabled = !myHealth.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            myActionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {            
            myNavMeshAgent.destination = destination;
            myNavMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            myNavMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            myNavMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = myNavMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            myAnimator.SetFloat("forwardSpeed", speed);
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            myNavMeshAgent.enabled = false;
            transform.position = position.ToVector();
            myNavMeshAgent.enabled = true;
            myActionScheduler.CancelCurrentAction();
        }
    }
}
