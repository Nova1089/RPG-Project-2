using RPG.Control;
using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        // config params
        [SerializeField] float timeToFadeOut = 1f;
        [SerializeField] float timeToFadeIn = 2f;
        [SerializeField] float fadeWaitTime = .5f;
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;

        // cached references
        Fader fader;
        SavingWrapper savingWrapper;

        void Awake()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        void OnTriggerEnter(Collider otherCollider)
        {
            if (otherCollider.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        IEnumerator Transition()
        {            
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            fader = FindObjectOfType<Fader>();

            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

           
            yield return fader.FadeOut(timeToFadeOut);            
            savingWrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            savingWrapper.Load();
            
            Portal otherPortal = GetOtherPortal();
            SetPlayerPositionAndRotation(otherPortal);
            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(timeToFadeIn);


            newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();
            foreach (Portal portal in portals)
            {
                if (portal.gameObject == this.gameObject) continue;
                if (portal.destination != this.destination) continue;
                return portal;
            }
            return null;
        }

        private void SetPlayerPositionAndRotation(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }
    }
}

