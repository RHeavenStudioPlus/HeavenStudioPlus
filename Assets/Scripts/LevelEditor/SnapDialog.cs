using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Editor 
{
    public class SnapDialog : MonoBehaviour
    {
        [SerializeField] private GameObject snapSetter;
        private void Awake() 
        {
        }

        public void SwitchSnapDialog()
        {
            if(snapSetter.activeSelf) {
                snapSetter.SetActive(false);
            } else {
                snapSetter.SetActive(true);
            }
        }

        private void Update()
        {
            
        }
    }
}