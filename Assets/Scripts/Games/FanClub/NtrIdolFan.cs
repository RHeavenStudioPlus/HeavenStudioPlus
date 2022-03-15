using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_FanClub
{
    public class NtrIdolFan : MonoBehaviour
    {
        [Header("References")]
        public Animator animator;
        public Animator headAnimator;
        public ParticleSystem fanClapEffect;

        bool player = false;
        bool stopBeat = false;

        public void Bop()
        {
            if (animator.IsAnimationNotPlaying() && !stopBeat)
                animator.Play("FanBeat", 0, 0);
        }

        public void ClapParticle()
        {
            fanClapEffect.Play();
        }
    }
}