using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RhythmHeavenMania.Editor 
{
    public class BPMText : MonoBehaviour
    {
        [SerializeField] private TMP_Text BPM;
        [SerializeField] private TMP_Text BPMRounded;
        public void ChangeText(float timePressed)
        {
            float thisBPM = 60 / timePressed; // BPM = 60/t
            BPM.text = $"{thisBPM}";
            BPMRounded.text = $"{Mathf.RoundToInt(thisBPM)}";
        }
        public void ResetText()
        {
            BPM.text = "---";
            BPMRounded.text = "---";
        }
    }
}
