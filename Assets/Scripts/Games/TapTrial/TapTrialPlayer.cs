using UnityEngine;

namespace RhythmHeavenMania.Games.TapTrial
{
    public class TapTrialPlayer : MonoBehaviour
    {
        [Header("References")]
        [System.NonSerialized] public Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }
    }
}