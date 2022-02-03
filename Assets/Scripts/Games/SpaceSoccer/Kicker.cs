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
        public bool player;

        [Header("Components")]
        private Animator anim;
        public Ball ball;
        public Transform rightLeg;
        public Transform leftLeg;

        private void Start()
        {
            anim = GetComponent<Animator>();
            GameObject rightLeg = new GameObject();
            rightLeg.transform.SetParent(this.transform);
            rightLeg.transform.position = new Vector3(-0.67f, -1.48f);

            GameObject leftLeg = new GameObject("leftLeg");
            leftLeg.transform.SetParent(this.transform);
            leftLeg.transform.position = new Vector3(0f, -1.48f);

            this.rightLeg = rightLeg.transform;
            this.leftLeg = leftLeg.transform;
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
            aceTimes = 0;

            if (player)
                Jukebox.PlayOneShotGame("spaceSoccer/kick");

            if (highKick)
            {
                if (kickLeft)
                {
                    anim.Play("HighKickLeft_0", 0, 0);
                }
                else
                {
                    anim.Play("HighKickRight_0", 0, 0);
                }
            }
            else if (!highKick)
            {
                if (kickLeft)
                {
                    anim.Play("KickLeft", 0, 0);
                }
                else
                {
                    anim.Play("KickRight", 0, 0);
                }
            }

            if (ball == null) return;

            if (highKick == false)
            {
                if (ball != null && hit)
                    ball.Kick(player);
            }
            else
            {
                kickPrepare = true;
            }

            ResetState();
        }

        public void HighKick(bool hit)
        {
            kickTimes++;

            if (kickLeft)
            {
                anim.Play("HighKickLeft_0", 0, 0);
            }
            else
            {
                anim.Play("HighKickRight_0", 0, 0);
            }

            if (hit && ball)
            {
                ball.HighKick();

                if (player)
                Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe1_hit");
            }
            else
            {
                if (player)
                    Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe1");
            }

            ResetState();
        }

        public void Toe(bool hit)
        {
            if (kickLeft)
            {
                anim.Play("ToeLeft", 0, 0);
            }
            else
            {
                anim.Play("ToeRight", 0, 0);
            }

            if (player)
            {
                if (hit && ball)
                {
                    Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe3_hit");
                }
                else
                {
                    Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe3");
                }
            }

            if (hit && ball)
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
                    float normalizedBeat = Conductor.instance.GetPositionFromBeat(ball.dispensedBeat, 2f);
                    StateCheck(normalizedBeat, !player);
                    CheckIfFall(normalizedBeat);

                    if (player)
                    {
                        if (PlayerInput.Pressed())
                        {
                            if (state.perfect)
                            {
                                KickCheck(true);
                            }
                            else
                            {
                                KickCheck(false, true);
                            }
                        }
                    }
                }
                else if (ball.kicked.enabled)
                {
                    float normalizedBeat = Conductor.instance.GetPositionFromBeat(ball.kicked.startBeat, 1f);
                    StateCheck(normalizedBeat, !player);
                    CheckIfFall(normalizedBeat);

                    if (player)
                    {
                        if (PlayerInput.Pressed())
                        {
                            if (state.perfect)
                            {
                                KickCheck(true);
                            }
                            else
                            {
                                KickCheck(false, true);
                            }
                        }
                    }
                }
                else if (ball.highKicked.enabled)
                {
                    float normalizedBeat = Conductor.instance.GetPositionFromBeat(ball.highKicked.startBeat, 1.5f);
                    if (!kickPrepare)
                    {
                        float normalizedBeatPrepare = Conductor.instance.GetPositionFromBeat(ball.highKicked.startBeat, 1f);
                        StateCheck(normalizedBeatPrepare, !player);
                        CheckIfFall(normalizedBeat);

                        if (player)
                        {
                            if (PlayerInput.AltPressed())
                            {
                                Kick(false, true);
                            }
                        }
                    }
                    else
                    {
                        StateCheck(normalizedBeat, !player);
                        CheckIfFall(normalizedBeat);

                        if (player)
                        {
                            if (PlayerInput.AltPressedUp())
                            {
                                if (state.perfect)
                                {
                                    Toe(true);
                                }
                                else
                                {
                                    Toe(false);
                                }
                            }
                        }
                    }
                }
                else if (ball.toe.enabled)
                {
                    float normalizedBeat = Conductor.instance.GetPositionFromBeat(ball.toe.startBeat, 1.5f);
                    StateCheck(normalizedBeat, !player);
                    CheckIfFall(normalizedBeat);

                    if (player)
                    {
                        if (PlayerInput.Pressed())
                        {
                            if (state.perfect)
                            {
                                KickCheck(true);
                            }
                            else
                            {
                                KickCheck(false, true);
                            }
                        }
                    }
                }
            }
            else
            {
                if (player)
                {
                    if (PlayerInput.Pressed())
                    {
                        KickCheck(false, true);
                    }
                }
            }
        }

        private void KickCheck(bool hit, bool overrideState = false)
        {
            if (canHighKick)
            {
                HighKick(hit);
            }
            else if (canKick)
            {
                Kick(hit);
            }
            else if (!canKick && !canHighKick && overrideState)
            {
                Kick(hit);
            }
        }

        private void CheckIfFall(float normalizedBeat)
        {
            if (normalizedBeat > 1.05f && !GameManager.instance.autoplay)
            {
                Jukebox.PlayOneShotGame("spaceSoccer/missNeutral");
                ball = null;
                ResetState();
            }
        }
    }
}