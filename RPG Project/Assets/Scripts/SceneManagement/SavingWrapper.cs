using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        // config params
        [SerializeField] float fadeInTime = .2f;

        // cached references
        SavingSystem savingSystem;
        Fader fader;

        void Awake()
        {                     
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();            
            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Save(); 
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSaveFile();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void DeleteSaveFile()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}

