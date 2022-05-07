using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbTapLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("tapTrial", "Tap Trial \n<color=#eb5454>[WIP don't use]</color>", "93ffb3", false, false, new List<GameAction>()
            {
                new GameAction("tap",                   delegate { TapTrial.instance.Tap(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("double tap",            delegate { TapTrial.instance.DoubleTap(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("triple tap",            delegate { TapTrial.instance.TripleTap(eventCaller.currentEntity.beat); }, 4.0f, false),
                new GameAction("jump tap",              delegate { TapTrial.instance.JumpTap(eventCaller.currentEntity.beat); }, 2.0f, false),
                new GameAction("final jump tap",        delegate { TapTrial.instance.FinalJumpTap(eventCaller.currentEntity.beat); }, 2.0f, false),
            });
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_TapTrial;

    public class TapTrial : Minigame
    {
        [Header("References")]
        public TapTrialPlayer player;
        public GameObject tap;

        public static TapTrial instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        // The following is all solely animations for placeholder. This isn't playable

        public void Tap(float beat)
        {
            Jukebox.PlayOneShotGame("tapTrial/ook");
            player.anim.Play("TapPrepare", 0, 0);

            CreateTap(beat);
        }

        public void DoubleTap(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("tapTrial/ookook",   beat),
                new MultiSound.Sound("tapTrial/ookook",   beat + 0.5f)
            });

            player.anim.Play("DoubleTapPrepare", 0, 0);

            GameObject beatAction = new GameObject();
            beatAction.transform.SetParent(this.transform);
            BeatAction.New(beatAction, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 0.0f, delegate { CreateTap(beat, 1); }),
                new BeatAction.Action(beat + 0.5f, delegate { CreateTap(beat + 0.5f, 1); }),
            });
        }

        public void TripleTap(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("tapTrial/ooki1",   beat),
                new MultiSound.Sound("tapTrial/ooki2",   beat + 0.5f)
            });

            player.anim.Play("PosePrepare", 0, 0);
            player.tripleOffset = 0;

            GameObject beatAction = new GameObject();
            beatAction.transform.SetParent(this.transform);
            BeatAction.New(beatAction, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 1.0f, delegate { CreateTap(beat + 1.0f, 2); }),
                new BeatAction.Action(beat + 1.5f, delegate { CreateTap(beat + 1.5f, 2); }),
                new BeatAction.Action(beat + 2.0f, delegate { CreateTap(beat + 2.0f, 2); }),
            });
        }

        public void JumpTap(float beat)
        {

        }

        public void FinalJumpTap(float beat)
        {

        }

        public void CreateTap(float beat, int type = 0)
        {
            GameObject _tap = Instantiate(tap);
            _tap.transform.parent = tap.transform.parent;
            _tap.SetActive(true);
            Tap t = _tap.GetComponent<Tap>();
            t.startBeat = beat;
            t.type = type;
        }
    }
}