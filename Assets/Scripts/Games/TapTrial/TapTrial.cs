using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.TapTrial
{
    public class TapTrial : Minigame
    {
        [Header("References")]
        public TapTrialPlayer player;

        public static TapTrial instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        public void Tap(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("tapTrial/ook",   beat),
                new MultiSound.Sound("tapTrial/tap",   beat + 1.0f),
            });

            GameObject beatAction = new GameObject();
            beatAction.transform.SetParent(this.transform);
            BeatAction.New(beatAction, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 0.0f, delegate { player.anim.Play("TapPrepare", 0, 0); }),
                new BeatAction.Action(beat + 1.0f, delegate { player.anim.Play("Tap", 0, 0); }),
            });
        }

        public void DoubleTap(float beat)
        {

        }

        public void TripleTap(float beat)
        {

        }

        public void JumpTap(float beat)
        {

        }

        public void FinalJumpTap(float beat)
        {

        }
    }
}