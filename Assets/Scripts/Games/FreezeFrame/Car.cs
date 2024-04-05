using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HeavenStudio.Common;
using HeavenStudio.Games.Scripts_WizardsWaltz;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_FreezeFrame
{
    public class Car : MonoBehaviour
    {
        [SerializeField] Animator _Animator;
        [SerializeField] ParticleSystem _ParticleSystem;

        public double Beat;
        public string AnimName;
        public float Length;

        public void Setup(double beat, string animName, float length)
        {
            Beat = beat;
            AnimName = animName;
            Length = length;

            _ParticleSystem.PlayScaledAsync(0.5f, true);
            _Animator.DoScaledAnimationFromBeatAsync("Idle", 0.5f, 0f, 0);
        }
        void Update()
        {
            if (_Animator == null)
                return;

            float normalizedBeatFromStart = Conductor.instance.GetPositionFromBeat(0, 1);
            _Animator.DoNormalizedAnimation("Idle", normalizedBeatFromStart * 4 % 1, animLayer: 0);

            if (AnimName is null or "")
                return;

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(Beat, Length);
            _Animator.DoNormalizedAnimation(AnimName, normalizedBeat, animLayer: 1);

            if (Conductor.instance.songPositionAsDouble > Beat + Length + 3)
                Destroy(this);
        }
    }
}