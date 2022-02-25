using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Editor 
{
    public class TempoFinder : MonoBehaviour
    {
        [SerializeField] private GameObject tempoFinder;
        private bool pressed;
        private float timePressed;
        [SerializeField] private GameObject BPMText;
        private void Awake() 
        {
            pressed = false;
            timePressed = 0f;
        }
        public void SwitchTempoDialog()
        {
            if(tempoFinder.activeSelf) {
                tempoFinder.SetActive(false);
                timePressed = 0;
                BPMText.GetComponent<BPMText>().ResetText();
            } else {
                tempoFinder.SetActive(true);
            }
        }
        public void TapBPM()
        {
            pressed = true;
        }
        private void Update()
        {
            timePressed += Time.deltaTime;
            if(pressed)
            {
                pressed = false;
                BPMText.GetComponent<BPMText>().ChangeText(timePressed);
                timePressed = 0;
            }
        }
    }
}