using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

namespace HeavenStudio.Games.Scripts_SpaceSoccer
{
    public class Kicker : MonoBehaviour
    {
        SpaceSoccer game;

        [Header("Properties")]
        public bool canKick = true; //why was this false by default???
        public bool canHighKick;
        //private bool kickPrepare = false;    Unused value - Marc
        public bool kickLeft;
        bool kickLeftWhiff;
        public double dispenserBeat; //unused
        public int kickTimes = 0;
        public bool player;
        private string animName = "Enter";
        private float animLength;
        private double animStartBeat;
        private EasingFunction.Ease ease;
        bool stopBall;

        [Header("Components")]
        private Animator anim;
        public Ball ball;
        [SerializeField] private Animator enterExitAnim;

        PlayerActionEvent nextHit;
        PlayerActionEvent nextAutoKick;

        private void Awake()
        {
            game = SpaceSoccer.instance;
            anim = GetComponent<Animator>();
        }

        public void SetAnimParams(double beat, float length, string anim, int easeToPut)
        {
            animStartBeat = beat;
            animLength = length;
            animName = anim;
            ease = (EasingFunction.Ease)easeToPut;
            EasingFunction.Function func = EasingFunction.GetEasingFunction(ease);
            float newAnimPos = func(0, 1, 0);
            enterExitAnim.DoNormalizedAnimation(animName, newAnimPos);
        }

        public void DispenseBall(double beat)
        {
            if (player)
            {
                nextHit = game.ScheduleInput(beat, ball.GetAnimLength(Ball.State.Dispensing), SpaceSoccer.InputAction_BasicPress, KickJust, Miss, Out);
            }
            else
            {
                double beatToKick = beat + ball.GetAnimLength(Ball.State.Dispensing);
                if (beatToKick < Conductor.instance.songPositionInBeatsAsDouble) beatToKick = ball.nextAnimBeat;
                if (ball.state == Ball.State.HighKicked)
                {
                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beatToKick - 0.5f, delegate { Kick(true, true); }),
                        new BeatAction.Action(beatToKick, delegate { Toe(true); }),
                        new BeatAction.Action(beatToKick + ball.GetAnimLength(Ball.State.Toe), delegate { KickCheck(true, false, beatToKick + ball.GetAnimLength(Ball.State.Toe)); }),
                    });
                }
                else
                {
                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beatToKick, delegate { KickCheck(true, false, beatToKick); }),
                    });
                }
            }
        }

        public void Kick(bool hit, bool highKick = false)
        {
            if (stopBall) return;

            if (player)
            {
                SoundByte.PlayOneShotGame("spaceSoccer/kick");
            }

            if (hit)
            {
                if (highKick)
                {
                    if (kickLeft)
                    {
                        anim.DoScaledAnimationAsync("HighKickLeft_0", 0.5f);
                    }
                    else
                    {
                        anim.DoScaledAnimationAsync("HighKickRight_0", 0.5f);
                    }
                }
                else
                {
                    if (kickLeft)
                    {
                        anim.DoScaledAnimationAsync("KickLeft", 0.5f);
                    }
                    else
                    {
                        anim.DoScaledAnimationAsync("KickRight", 0.5f);
                    }
                }
            }
            else
            {
                if (highKick)
                {
                    if (kickLeftWhiff)
                    {
                        anim.DoScaledAnimationAsync("HighKickLeft_0", 0.5f);
                    }
                    else
                    {
                        anim.DoScaledAnimationAsync("HighKickRight_0", 0.5f);
                    }
                }
                else
                {
                    if (kickLeftWhiff)
                    {
                        anim.DoScaledAnimationAsync("KickLeft", 0.5f);
                    }
                    else
                    {
                        anim.DoScaledAnimationAsync("KickRight", 0.5f);
                    }
                }
                kickLeftWhiff = !kickLeftWhiff;
            }


            if (ball == null) return;

            if (highKick == false)
            {
                kickTimes++;
                if (ball != null && hit)
                    ball.Kick(player);
            }
            else
            {
                //kickPrepare = true;    Unused value - Marc
            }
        }

        public void HighKick(bool hit)
        {
            if (stopBall) return;
            kickTimes++;
            if (hit)
            {
                if (kickLeft)
                {
                    anim.DoScaledAnimationAsync("HighKickLeft_0", 0.5f);
                }
                else
                {
                    anim.DoScaledAnimationAsync("HighKickRight_0", 0.5f);
                }
            }
            else
            {
                if (kickLeftWhiff)
                {
                    anim.DoScaledAnimationAsync("HighKickLeft_0", 0.5f);
                }
                else
                {
                    anim.DoScaledAnimationAsync("HighKickRight_0", 0.5f);
                }
                kickLeftWhiff = !kickLeftWhiff;
            }

            if (player) SoundByte.PlayOneShotGame("spaceSoccer/highkicktoe1");
            if (hit && ball)
            {
                ball.HighKick();

                if (player) SoundByte.PlayOneShotGame("spaceSoccer/highkicktoe1_hit");
            }

        }

        public void Toe(bool hit, bool flick = false)
        {
            if (stopBall) return;
            if (kickLeft)
            {
                anim.DoScaledAnimationAsync("ToeLeft", 0.5f);
            }
            else
            {
                anim.DoScaledAnimationAsync("ToeRight", 0.5f);
            }

            if (player)
            {
                SoundByte.PlayOneShotGame("spaceSoccer/highkicktoe3");
                if (hit && ball)
                {
                    SoundByte.PlayOneShotGame("spaceSoccer/highkicktoe3_hit");
                }
            }

            if (hit && ball)
                ball.Toe();

            if (!flick)
            {
                kickTimes++;
                //kickPrepare = false;    Unused value - Marc
            }
        }

        private void Update()
        {
            float normalizedBeat = Conductor.instance.GetPositionFromBeat(animStartBeat, animLength);
            if (normalizedBeat >= 0 && normalizedBeat <= 1)
            {
                EasingFunction.Function func = EasingFunction.GetEasingFunction(ease);
                float newAnimPos = func(0, 1, normalizedBeat);
                enterExitAnim.DoNormalizedAnimation(animName, newAnimPos);
            }
            if (kickTimes % 2 == 0)
            {
                kickLeft = false;
            }
            else
            {
                kickLeft = true;
            }

            var highKicks = GameManager.instance.Beatmap.Entities.FindAll(c => c.datamodel == "spaceSoccer/high kick-toe!");
            for (int i = 0; i < highKicks.Count; i++)
            {
                if ((highKicks[i].beat - 0.15f) <= Conductor.instance.songPositionInBeatsAsDouble && highKicks[i].beat + 1f > Conductor.instance.songPositionInBeatsAsDouble)
                {
                    canHighKick = true;
                    canKick = false;

                    if (ball)
                    {
                        ball.highKickSwing = 0.5f;
                    }
                    break;
                }
                else
                {
                    canKick = true;
                    canHighKick = false;
                }
            }

            if (player)
            {
                if (stopBall) return;
                if (PlayerInput.GetIsAction(SpaceSoccer.InputAction_BasicPress)
                    && !game.IsExpectingInputNow(SpaceSoccer.InputAction_BasicPress))
                {
                    if (ball == null)
                        KickCheck(false, true);
                    else
                        Kick(false, ball.canKick);

                }
                if (PlayerInput.GetIsAction(SpaceSoccer.InputAction_FlickRelease))
                {
                    if (PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch)
                    {
                        if (ball == null || !(game.IsExpectingInputNow(SpaceSoccer.InputAction_FlickRelease) || ball.canKick))
                        {
                            Toe(false);
                        }
                    }
                    if (ball != null)
                    {
                        if (ball.waitKickRelease)
                        {
                            ball.waitKickRelease = false;
                        }
                        else if (ball.canKick && !game.IsExpectingInputNow(SpaceSoccer.InputAction_FlickRelease))
                        {
                            ball.canKick = false;
                            Kick(false);
                        }
                    }
                }
            }
        }

        private void KickCheck(bool hit,  bool overrideState = false, double beat = 0f)
        {
            if (stopBall) return;
            if (canHighKick)
            {
                HighKick(hit);
                if (!player)
                {
                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + ball.GetAnimLength(Ball.State.Kicked), delegate { Kick(true, true); }),
                        new BeatAction.Action(beat + ball.GetAnimLength(Ball.State.Toe), delegate { Toe(true); }),
                        new BeatAction.Action(beat + ball.GetAnimLength(Ball.State.Toe) + 1.5f, delegate { KickCheck(true, false, beat + ball.GetAnimLength(Ball.State.Toe) + 1.5f); }),
                    });
                }
            }
            else if (canKick)
            {
                Kick(hit);
                if (!player)
                {
                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + ball.GetAnimLength(Ball.State.Kicked), delegate { KickCheck(true, false, beat + ball.GetAnimLength(Ball.State.Kicked)); }),
                    });
                }
            }
            else if (!canKick && !canHighKick && overrideState)
            {
                Kick(hit);
                if (!player)
                {
                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + ball.GetAnimLength(Ball.State.Kicked), delegate { KickCheck(true, false, beat + ball.GetAnimLength(Ball.State.Kicked)); }),
                    });
                }
            }


        }

        public void StopBall(bool stop)
        {
            stopBall = stop;
            if (ball != null && stop) Destroy(ball.gameObject);
        }

        void MissBall(double targetBeat)
        {
            if (stopBall) return;

            var cond = Conductor.instance;
            ball = null;
            // queue the miss sound
            MultiSound.Play(new MultiSound.Sound[] { new MultiSound.Sound("spaceSoccer/missNeutral", targetBeat + (float)cond.SecsToBeats(Minigame.NgLateTime()-1, 
                cond.GetBpmAtBeat(targetBeat)), SoundByte.GetPitchFromCents(UnityEngine.Random.Range(-75, 75), false)) });
        }

        private void KickJust(PlayerActionEvent caller, float state)
        {
            if (stopBall) return;
            if (ball == null || state >= 1f || state <= -1f) {  //todo: proper near miss feedback
                KickCheck(false, true);
                MissBall(caller.startBeat + caller.timer);
                return;
            }
            KickCheck(true);
            if (canHighKick)
            {
                // queue high kick inputs
                nextHit = game.ScheduleInput(caller.startBeat + caller.timer, ball.GetAnimLength(Ball.State.Toe), SpaceSoccer.InputAction_FlickRelease, ToeJust, Miss, Out);
                nextAutoKick = game.ScheduleAutoplayInput(caller.startBeat + caller.timer, ball.GetAnimLength(Ball.State.Kicked), SpaceSoccer.InputAction_BasicPress, ToePrepareJust, Out, Out);
                ball.canKick = true;
                ball.waitKickRelease = true;
            }
            else
            {
                // queue normal kick input
                nextHit = game.ScheduleInput(caller.startBeat + caller.timer, ball.GetAnimLength(Ball.State.Kicked), SpaceSoccer.InputAction_BasicPress, KickJust, Miss, Out);
            }
            game.hitBeats.Add(caller.startBeat + caller.timer);
        }

        private void Miss(PlayerActionEvent caller) 
        {
            if (stopBall) return;
            if (ball != null)
                MissBall(caller.startBeat + caller.timer);
            
            // if this were any other keep the beat game you'd cue the next input here
        }

        private void ToeJust(PlayerActionEvent caller, float state)
        {
            if (stopBall) return;

            if (ball == null || (!ball.canKick) || state >= 1f || state <= -1f) {  //todo: proper near miss feedback
                Toe(false);
                MissBall(caller.startBeat + caller.timer);
                return;
            }
            Toe(true);
            nextHit = game.ScheduleInput(caller.startBeat, 3f, SpaceSoccer.InputAction_BasicPress, KickJust, Miss, Out);
            ball.canKick = false;
            game.hitBeats.Add(caller.startBeat + caller.timer);
        }

        private void ToePrepareJust(PlayerActionEvent caller, float state)
        {
            if (stopBall) return;

            //autoplay only
            Kick(true, true);
        }

        private void Out(PlayerActionEvent caller) {}

        void OnDestroy()
        {
            if (nextHit != null)
                nextHit.Disable();
            if (nextAutoKick != null)
                nextAutoKick.Disable();
            if (ball != null) Destroy(ball.gameObject);
        }
    }
}