using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;
using HeavenStudio.Util;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class CreditsLegalSettings : MonoBehaviour
    {
        private int SecretCounter = 0;
        private static bool SecretActive = false;
        [SerializeField] private GameObject secretObject;

        private void Start()
        {
            SecretCounter = 0;
            secretObject.SetActive(false);
        }

        public void OnClickCountUp()
        {
            SecretCounter++;
            Debug.Log("SecretCounter: " + SecretCounter);
            if (SecretCounter == 10)
            {
                secretObject.SetActive(true);
            }
        }

        public void OnClickSecret()
        {
            if (SecretActive) return;

            SecretActive = true;
            Jukebox.PlayOneShot("applause");
            Debug.Log("Activating Studio Dance...");
        }
    }
}