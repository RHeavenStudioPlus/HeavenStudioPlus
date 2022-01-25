using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.SpaceSoccer
{
    public class Kicker : PlayerActionObject
    {
        [Header("Properties")]
        public bool canKick;
        public bool canHighKick;
        private bool kickPrepare = false;
        public bool kickLeft;
        public float dispenserBeat;
        public int kickTimes = 0;

        [Header("Components")]
        private Animator anim;
        public Ball ball;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public override void OnAce()
        {
            if (ball.highKicked.enabled)
            {
                if (!kickPrepare)
                {
                    Kick(false, true);
                }
                else
                {
                    Toe(true);
                }
            }
            else
            {
                if (canHighKick)
                {
                    HighKick(true);
                }
                else
                {
                    Kick(true);
                }
            }
        }

        public void Kick(bool hit, bool highKick = false)
        {
            kickTimes++;

            if (kickLeft)
            {
                anim.Play("KickLeft", 0, 0);
            }
            else
            {
                anim.Play("KickRight", 0, 0);
            }
            if (highKick == false)
            {
                ball.Kick();
            }
            else
            {
                kickPrepare = true;
            }
            Jukebox.PlayOneShotGame("spaceSoccer/kick");
            ResetState();
        }

        public void HighKick(bool hit)
        {
            if (hit)
            {
                Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe1_hit");
            }
            else
            {
                Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe1");
            }

            ball.HighKick();
            ResetState();
        }

        public void Toe(bool hit)
        {
            if (hit)
            {
                Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe3_hit");
            }
            else
            {
                Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe3");
            }
            ball.Toe();
            kickPrepare = false;
            ResetState();
        }

        private void Update()
        {
            if (kickTimes % 2 == 0)
            {
                kickLeft = false;
            }
            else
            {
                kickLeft = true;
            }

            List<Beatmap.Entity> keepUps = GameManager.instance.Beatmap.entities.FindAll(c => c.datamodel == "spaceSoccer/keep-up");
            List<Beatmap.Entity> highKicks = GameManager.instance.Beatmap.entities.FindAll(c => c.datamodel == "spaceSoccer/high kick-toe!");
            for (int i = 0; i < keepUps.Count; i++)
            {
                if ((keepUps[i].beat - 0.15f) <= Conductor.instance.songPositionInBeats && (keepUps[i].beat + keepUps[i].length) - 0.15f > Conductor.instance.songPositionInBeats)
                {
                    canKick = true;
                    canHighKick = false;
                    break;
                }
                else
                {
                    canKick = false;
                }
            }
            for (int i = 0; i < highKicks.Count; i++)
            {
                if ((highKicks[i].beat - 0.15f) <= Conductor.instance.songPositionInBeats && highKicks[i].beat + 1f > Conductor.instance.songPositionInBeats)
                {
                    canHighKick = true;
                    canKick = false;
                    break;
                }
                else
                {
                    canHighKick = false;
                }
            }

            if (ball)
            {
                if (ball.dispensing)
                {
                    float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(ball.dispensedBeat, 2f);
                    StateCheck(normalizedBeat);
                    CheckIfFall(normalizedBeat);

                    if (PlayerInput.Pressed())
                    {
                        if (state.perfect)
                        {
                            KickCheck();
                        }
                    }
                }
                else if (ball.kicked.enabled)
                {
                    float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(ball.kicked.startBeat, 1f);
                    StateCheck(normalizedBeat);
                    CheckIfFall(normalizedBeat);

                    if (PlayerInput.Pressed())
                    {
                        if (state.perfect)
                        {
                            KickCheck();
                        }
                    }
                }
                else if (ball.highKicked.enabled)
                {
                    float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(ball.highKicked.startBeat, 1.5f);
                    if (!kickPrepare)
                    {
                        float normalizedBeatPrepare = Conductor.instance.GetLoopPositionFromBeat(ball.highKicked.startBeat, 1f);
                        StateCheck(normalizedBeatPrepare);
                        CheckIfFall(normalizedBeat);

                        if (PlayerInput.Pressed())
                        {
                            Kick(false, true);
                        }
                    }
                    else
                    {
                        StateCheck(normalizedBeat);
                        if (PlayerInput.PressedUp())
                        {
                            if (state.perfect)
                            {
                                Toe(true);
                            }
                        }
                    }
                }
                else if (ball.toe.enabled)
                {
                    float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(ball.toe.startBeat, 1.5f);
                    StateCheck(normalizedBeat);
                    CheckIfFall(normalizedBeat);

                    if (PlayerInput.Pressed())
                    {
                        if (state.perfect)
                        {
                            KickCheck();
                        }
                    }
                }
            }

            if (PlayerInput.Pressed())
            {
                // Kick(false);
            }
        }

        private void KickCheck()
        {
            if (canHighKick)
            {
                HighKick(true);
            }
            else if (canKick)
            {
                Kick(true);
            }
        }

        private void CheckIfFall(float normalizedBeat)
        {
            if (normalizedBeat > 1.45f)
            {
                ball = null;
                ResetState();
            }
        }
    }
}