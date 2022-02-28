using UnityEngine;

using RhythmHeavenMania.Util;

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

        private void Update()
        {
            if (PlayerInput.Pressed())
            {
                Tap(false);
            }
        }

        public void Tap(bool hit)
        {
            Jukebox.PlayOneShotGame("tapTrial/tonk");
            anim.Play("Tap", 0, 0);
        }
    }
}