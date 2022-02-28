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

        // The following is all solely animations for placeholder. This isn't playable

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
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("tapTrial/ookook",   beat),
                new MultiSound.Sound("tapTrial/ookook",   beat + 0.5f),
                new MultiSound.Sound("tapTrial/tap",      beat + 1.0f),
                new MultiSound.Sound("tapTrial/tap",      beat + 1.5f),
            });

            GameObject beatAction = new GameObject();
            beatAction.transform.SetParent(this.transform);
            BeatAction.New(beatAction, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 0.0f, delegate { player.anim.Play("DoubleTapPrepare", 0, 0); }),
                new BeatAction.Action(beat + 1.0f, delegate { player.anim.Play("DoubleTap", 0, 0); }),
                new BeatAction.Action(beat + 1.5f, delegate { player.anim.Play("DoubleTap", 0, 0); }),
            });
        }

        public void TripleTap(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("tapTrial/ooki1",   beat),
                new MultiSound.Sound("tapTrial/ooki2",   beat + 0.5f),
                new MultiSound.Sound("tapTrial/tap",      beat + 2.0f),
                new MultiSound.Sound("tapTrial/tap",      beat + 2.5f),
                new MultiSound.Sound("tapTrial/tap",      beat + 3.0f),
            });

            GameObject beatAction = new GameObject();
            beatAction.transform.SetParent(this.transform);
            BeatAction.New(beatAction, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 0.0f, delegate { player.anim.Play("PosePrepare", 0, 0); }),
                new BeatAction.Action(beat + 2.0f, delegate { player.anim.Play("Tap", 0, 0); }),
                new BeatAction.Action(beat + 2.5f, delegate { player.anim.Play("DoubleTap", 0, 0); }),
                new BeatAction.Action(beat + 3.0f, delegate { player.anim.Play("Tap", 0, 0); }),
            });
        }

        public void JumpTap(float beat)
        {

        }

        public void FinalJumpTap(float beat)
        {

        }
    }
}