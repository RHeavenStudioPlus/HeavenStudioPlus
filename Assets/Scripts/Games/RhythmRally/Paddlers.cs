using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.RhythmRally
{
    public class Paddlers : PlayerActionObject
    {
        private RhythmRally game;
        private Animator playerAnim;
        private Animator opponentAnim;
        private Conductor cond;

        void Awake()
        {
            game = RhythmRally.instance;
            playerAnim = game.playerAnim;
            opponentAnim = game.opponentAnim;
            cond = Conductor.instance;
        }

        void Update()
        {
            if (!game.served || game.missed || !game.started) return;

            float stateBeat = Conductor.instance.GetPositionFromMargin(game.targetBeat, 1f);
            StateCheck(stateBeat);

            if (PlayerInput.Pressed())
            {
                if (state.perfect)
                {
                    Ace();
                }
                else if (state.notPerfect())
                {
                    Miss();
                    Jukebox.PlayOneShot("miss");
                    playerAnim.Play("Swing", 0, 0);
                }
                else
                {
                    // Play "whoosh" sound here
                    playerAnim.Play("Swing", 0, 0);
                }
            }

            if (stateBeat > Minigame.EndTime())
            {
                Miss();
            }
        }

        void Ace()
        {
            game.served = false;

            var hitBeat = cond.songPositionInBeats;

            var bounceBeat = game.targetBeat + 1f;

            if (game.rallySpeed == RhythmRally.RallySpeed.Slow)
            {
                bounceBeat = game.targetBeat + 2f;
            }
            else if (game.rallySpeed == RhythmRally.RallySpeed.SuperFast)
            {
                bounceBeat = game.targetBeat + 0.5f;
            }

            playerAnim.Play("Swing", 0, 0);
            MultiSound.Play(new MultiSound.Sound[] { new MultiSound.Sound("rhythmRally/Return", hitBeat), new MultiSound.Sound("rhythmRally/ReturnBounce", bounceBeat) });
        }

        void Miss()
        {
            game.served = false;
            game.missed = true;

            var whistleBeat = game.targetBeat + 1f;

            if (game.rallySpeed == RhythmRally.RallySpeed.Slow)
            {
                whistleBeat = game.targetBeat + 2f;
            }
            else if (game.rallySpeed == RhythmRally.RallySpeed.SuperFast)
            {
                whistleBeat = game.targetBeat + 0.5f;
            }

            MultiSound.Play(new MultiSound.Sound[] { new MultiSound.Sound("rhythmRally/Whistle", whistleBeat) });
        }

        public override void OnAce()
        {
            Ace();
        }
    }
}
