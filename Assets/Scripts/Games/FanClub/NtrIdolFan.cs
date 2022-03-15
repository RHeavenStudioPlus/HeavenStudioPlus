using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;
using NaughtyBezierCurves;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_FanClub
{
    public class NtrIdolFan : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject motionRoot;
        [SerializeField] private GameObject headRoot;
        public Animator animator;
        public Animator headAnimator;
        public ParticleSystem fanClapEffect;
        public GameObject shadow;

        [Header("Properties")]
        [NonSerialized] public bool player = false;
        [NonSerialized] public bool hitValid = false;
        public float jumpStartTime = -99f;
        bool stopBeat = false;
        bool stopCharge = false;
        bool hasJumped = false;

        float clappingStartTime = 0f;

        private void Update()
        {
            var cond = Conductor.instance;
            if (player)
            {
                if (PlayerInput.Pressed())
                {
                    ClapStart(false);
                }
                if (PlayerInput.Pressing())
                {
                    if (cond.songPositionInBeats > clappingStartTime + 1f && !stopCharge)
                    {
                        animator.Play("FanClapCharge", 0, 0);
                        stopCharge = true;
                    }
                }
                if (PlayerInput.PressedUp())
                {
                    if (stopCharge)
                    {
                        JumpStart(false);
                    }
                    else
                    {
                        animator.Play("FanFree", 0, 0);
                        stopBeat = false;
                    }
                }
            }
            
            float jumpPos = cond.GetPositionFromBeat(jumpStartTime, 1f);
            if (cond.songPositionInBeats >= jumpStartTime && cond.songPositionInBeats < jumpStartTime + 1f)
            {
                hasJumped = true;
                float yMul = jumpPos * 2f - 1f;
                float yWeight = -(yMul*yMul) + 1f;
                motionRoot.transform.localPosition = new Vector3(0, 3f * yWeight);
                shadow.transform.localScale = new Vector3((1f-yWeight*0.8f) * 1.4f, (1f-yWeight*0.8f) * 1.4f, 1f);
            }
            else
            {
                motionRoot.transform.localPosition = new Vector3(0, 0);
                shadow.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
                if (hasJumped && player)
                {
                    animator.Play("FanPrepare", 0, 0);
                    stopBeat = false;
                }
                hasJumped = false;
            }
        }

        public void ClapStart(bool hit, bool force = false, bool doCharge = false, bool fromAutoplay = false)
        {
            var cond = Conductor.instance;
            if (hit)
            {
                print("HIT");
                if (doCharge)
                    BeatAction.New(this.gameObject, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(cond.songPositionInBeats + 0.1f, delegate { 
                            if (PlayerInput.Pressing() || fromAutoplay)
                            {
                                animator.Play("FanClapCharge", 0, 0);
                                stopCharge = true;
                            }
                        }),
                    });
            }
            else
            {
                print("missed");
                FanClub.instance.AngerOnMiss();
            }
            if (fromAutoplay || !force)
            {
                stopBeat = true;
                jumpStartTime = -99f;
                animator.Play("FanClap", 0, 0);
                Jukebox.PlayOneShotGame("fanClub/play_clap");
                Jukebox.PlayOneShotGame("fanClub/crap_impact");
                clappingStartTime = cond.songPositionInBeats;
            }
            if (fromAutoplay && !doCharge)
            {
                BeatAction.New(this.gameObject, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(cond.songPositionInBeats + 0.1f, delegate { 
                        animator.Play("FanFree", 0, 0);
                        stopBeat = false;
                    }),
                });
                
            }
        }

        public void JumpStart(bool hit, bool force = false, bool fromAutoplay = false)
        {
            var cond = Conductor.instance;
            if (hit)
            {
                print("HIT");
            }
            else
            {
                print("missed");
                FanClub.instance.AngerOnMiss();
            }
            if (fromAutoplay || !force)
            {
                animator.Play("FanJump", 0, 0);
                Jukebox.PlayOneShotGame("fanClub/play_jump");
                jumpStartTime = cond.songPositionInBeats;
                stopCharge = false;
            }
        }

        public void Bop()
        {
            if (!stopBeat)
                animator.Play("FanBeat", 0, 0);
        }

        public void ClapParticle()
        {
            fanClapEffect.Play();
        }

        public void MakeAngry(bool flip = false)
        {
            headAnimator.Play("FanFaceAngry", 0, 0);
            if (flip)
            {
                headRoot.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }
}
