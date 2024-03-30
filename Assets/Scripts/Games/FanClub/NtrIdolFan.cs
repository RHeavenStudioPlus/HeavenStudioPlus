using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; //don't ask
using System;

using NaughtyBezierCurves;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

namespace HeavenStudio.Games.Scripts_FanClub
{
    public class NtrIdolFan : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject motionRoot;
        [SerializeField] private GameObject headRoot;
        [SerializeField] private SortingGroup sortingGroup;
        public Animator animator;
        public ParticleSystem fanClapEffect;
        public GameObject shadow;

        [Header("Properties")]
        [NonSerialized] public bool player = false;
        public double jumpStartTime = double.MinValue;
        bool stopBeat = false;
        bool stopCharge = false;
        bool hasJumped = false;

        double clappingStartTime = double.MinValue;

        public void AddHit(double beat, int type = 0)
        {
            if (player)
            {
                switch (type)
                {
                    case 0:
                        FanClub.instance.ScheduleInput(beat, 1f, FanClub.InputAction_BasicPress, ClapJust, ClapThrough, Out);
                        break;
                    case 1:
                        FanClub.instance.ScheduleInput(beat, 1f, FanClub.InputAction_FlickRelease, JumpJust, JumpThrough, JumpOut);
                        break;
                    case 2:
                        FanClub.instance.ScheduleInput(beat, 1f, FanClub.InputAction_BasicPress, ChargeClapJust, ClapThrough, Out);
                        break;
                    default:
                        FanClub.instance.ScheduleInput(beat, 1f, FanClub.InputAction_BasicPress, LongClapJust, ClapThrough, Out);
                        break;
                }
            }
        }

        public void ClapJust(PlayerActionEvent caller, float state)
        {
            bool auto = GameManager.instance.autoplay;
            ClapStart(state < 1f && state > -1f, false, auto ? 0.1f : 0f);
        }

        public void ChargeClapJust(PlayerActionEvent caller, float state)
        {
            bool auto = GameManager.instance.autoplay;
            ClapStart(state < 1f && state > -1f, true, auto ? 1f : 0f);
        }

        public void LongClapJust(PlayerActionEvent caller, float state)
        {
            bool auto = GameManager.instance.autoplay;
            ClapStart(state < 1f && state > -1f, false, auto ? 1f : 0f);
        }

        public void JumpJust(PlayerActionEvent caller, float state)
        {
            JumpStart(state < 1f && state > -1f);
        }

        public void ClapThrough(PlayerActionEvent caller) {
            FanClub.instance.AngerOnMiss();
        }

        public void JumpThrough(PlayerActionEvent caller) {
            FanClub.instance.AngerOnMiss();
        }

        public void Out(PlayerActionEvent caller) {}

        public void JumpOut(PlayerActionEvent caller) {
            var cond = Conductor.instance;
            if (stopCharge)
            {
                caller.CanHit(false);
            }
        }

        private void Update()
        {
            var cond = Conductor.instance;

            if (player)
            {
                if (PlayerInput.GetIsAction(FanClub.InputAction_BasicPress))
                {
                    if (!FanClub.instance.IsExpectingInputNow(FanClub.InputAction_BasicPress.inputLockCategory))
                    {
                        if (FanClub.instance.JudgementPaused)
                        {
                            ClapStart(true);
                        }
                        else
                        {
                            ClapStart(false);
                            FanClub.instance.ScoreMiss();
                        }
                    }
                }
                if (PlayerInput.GetIsAction(FanClub.InputAction_BasicPressing))
                {
                    if (clappingStartTime != double.MinValue && cond.songPositionInBeatsAsDouble > clappingStartTime + 2f && !stopCharge)
                    {
                        animator.speed = 1f;
                        animator.Play("FanClapCharge", -1, 0);
                        stopCharge = true;
                    }
                }
                if (PlayerInput.GetIsAction(FanClub.InputAction_FlickRelease))
                {
                    float nonTouchDelay = 2f;
                    if (PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch)
                    {
                        nonTouchDelay = 0f;
                        stopCharge = true;
                    }
                    if (clappingStartTime != double.MinValue && cond.songPositionInBeatsAsDouble > clappingStartTime + nonTouchDelay && stopCharge
                        && !FanClub.instance.IsExpectingInputNow(FanClub.InputAction_FlickRelease.inputLockCategory))
                    {
                        if (FanClub.instance.JudgementPaused)
                        {
                            JumpStart(true);
                        }
                        else
                        {
                            JumpStart(false);
                            FanClub.instance.ScoreMiss();
                        }
                    }
                    else
                    {
                        animator.speed = 1f;
                        animator.Play("FanFree", -1, 0);
                        stopBeat = false;
                        clappingStartTime = double.MinValue;
                    }
                }
                if (PlayerInput.GetIsAction(FanClub.InputAction_TouchRelease))
                {
                    animator.speed = 1f;
                    animator.Play("FanFree", -1, 0);
                    stopBeat = false;
                    stopCharge = false;
                    clappingStartTime = double.MinValue;
                }
            }
            
            float jumpPos = cond.GetPositionFromBeat(jumpStartTime, 1f);
            if (cond.songPositionInBeatsAsDouble >= jumpStartTime && cond.songPositionInBeatsAsDouble < jumpStartTime + 1f)
            {
                hasJumped = true;
                float yMul = jumpPos * 2f - 1f;
                float yWeight = -(yMul*yMul) + 1f;
                motionRoot.transform.localPosition = new Vector3(0, 3f * yWeight);
                shadow.transform.localScale = new Vector3((1f-yWeight*0.8f) * 1.4f, (1f-yWeight*0.8f) * 1.4f, 1f);
                animator.DoScaledAnimation("FanJump", jumpStartTime);
            }
            else
            {
                motionRoot.transform.localPosition = new Vector3(0, 0);
                shadow.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
                if (hasJumped)
                {
                    SoundByte.PlayOneShotGame("fanClub/landing_impact", pitch: UnityEngine.Random.Range(0.95f, 1f), volume: 1f/4);
                    if (player)
                    {
                        animator.Play("FanPrepare", -1, 0);
                        stopBeat = false;
                    }
                }
                animator.speed = 1f;
                hasJumped = false;
            }
        }

        public void ClapStart(bool hit, bool doCharge = false, float autoplayRelease = 0f)
        {
            if (!hit)
            {
                FanClub.instance.AngerOnMiss();
            }

            if (FanClub.instance.JudgementPaused)
            {
                FanClub.instance.JudgementInputPaused = true;
            }

            var cond = Conductor.instance;
            hasJumped = false;
            stopBeat = true;
            jumpStartTime = -99f;
            animator.speed = 1f;
            animator.Play("FanClap", -1, 0);
            SoundByte.PlayOneShotGame("fanClub/play_clap");
            SoundByte.PlayOneShotGame("fanClub/crap_impact");
            clappingStartTime = cond.songPositionInBeatsAsDouble;

            if (doCharge)
                BeatAction.New(this, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(cond.songPositionInBeatsAsDouble + 0.1f, delegate { 
                        if (PlayerInput.GetIsAction(FanClub.InputAction_BasicPressing) || autoplayRelease > 0f)
                        {
                            animator.Play("FanClapCharge", -1, 0);
                            stopCharge = true;
                        }
                    }),
                });

            if (autoplayRelease > 0f && !doCharge)
            {
                BeatAction.New(this, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(cond.songPositionInBeatsAsDouble + autoplayRelease, delegate { 
                        animator.Play("FanFree", -1, 0);
                        stopBeat = false;
                    }),
                });
                
            }
        }

        public void JumpStart(bool hit)
        {
            if (!hit)
            {
                FanClub.instance.AngerOnMiss();
            }

            var cond = Conductor.instance;
            animator.Play("FanJump", -1, 0);
            SoundByte.PlayOneShotGame("fanClub/play_jump");
            jumpStartTime = cond.songPositionInBeatsAsDouble;
            clappingStartTime = double.MinValue;
            stopCharge = false;
        }

        public bool IsJumping()
        {
            var cond = Conductor.instance;
            return (cond.songPositionInBeatsAsDouble >= jumpStartTime && cond.songPositionInBeatsAsDouble < jumpStartTime + 1f);
        }

        public void Bop()
        {
            if (!stopBeat)
            {
                animator.speed = 1f;
                animator.Play("FanBeat");
            }
        }

        public void ClapParticle()
        {
            fanClapEffect.Play();
        }

        public void MakeAngry(bool flip = false)
        {
            if (flip)
            {
                animator.Play("Head.FanFaceAngryFlip", -1, 0);
            }
            else
            {
                animator.Play("Head.FanFaceAngry", -1, 0);
            }
        }

        public void SetRow(int row, int origin = 0)
        {
            sortingGroup.sortingOrder = origin + row;
        }
    }
}
