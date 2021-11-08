using RPG.Control;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        // cached references
        PlayableDirector myPlayableDirector;
        GameObject player;
        PlayerController playerController;

        void Awake()
        {
            myPlayableDirector = GetComponent<PlayableDirector>();
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
        }

        void OnEnable()
        {
            myPlayableDirector.played += DisableControl;
            myPlayableDirector.stopped += EnableControl;
        }

        void OnDisable()
        {
            myPlayableDirector.played -= DisableControl;
            myPlayableDirector.stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector playableDirector)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            playerController.enabled = false;
        }

        void EnableControl(PlayableDirector playableDirector)
        {
            playerController.enabled = true;
        }
    }
}

