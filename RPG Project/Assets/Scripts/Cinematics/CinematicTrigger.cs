using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool hasTriggered = false;

        // called by Unity when something hits the object's trigger collider
        void OnTriggerEnter(Collider otherCollider)
        {         
            if (hasTriggered == false && otherCollider.tag == "Player")
            {
                transform.GetComponentInParent<PlayableDirector>().Play();
                hasTriggered = true;
            }            
        }
    }
}

