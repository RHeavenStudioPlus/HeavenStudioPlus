using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_FanClub
{
    public class NtrIdolAri : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] ParticleSystem idolClapEffect;
        [SerializeField] ParticleSystem idolWinkEffect;
        [SerializeField] ParticleSystem idolKissEffect;
        [SerializeField] ParticleSystem idolWinkArrEffect;

        [SerializeField] SpriteRenderer baseHead;
        [SerializeField] GameObject facePoser;

        [Header("References")]
        public Material coreMat;

        Animator animator;
        float previousEyeX, previousEyeY;
        float nextEyeX, nextEyeY;
        double eyeEaseStartBeat;
        float eyeEaseLength;
        EasingFunction.Ease eyeEase = EasingFunction.Ease.Instant;

        private void Start()
        {
            animator = GetComponent<Animator>();
            ToggleFacePoser(false);
        }

        private void Update()
        {
            var cond = Conductor.instance;
            if (cond is null)
                return;

            float prog = cond.GetPositionFromBeat(eyeEaseStartBeat, eyeEaseLength);
            if (prog > 1)
            {
                SetEyeTarget(nextEyeX, nextEyeY);
            }
            else if (prog >= 0)
            {
                float x = EasingFunction.GetEasingFunction(eyeEase)(previousEyeX, nextEyeX, prog);
                float y = EasingFunction.GetEasingFunction(eyeEase)(previousEyeY, nextEyeY, prog);
                animator.SetFloat("EyeX", Mathf.Clamp(x, -1, 1));
                animator.SetFloat("EyeY", Mathf.Clamp(y, -1, 1));
            }
        }

        public void ClapParticle()
        {
            idolClapEffect.Play();
        }

        public void WinkParticle()
        {
            idolWinkEffect.Play();
        }

        public void KissParticle()
        {
            idolKissEffect.Play();
        }

        public void WinkArrangeParticle()
        {
            idolWinkArrEffect.Play();
        }

        public void ToSpot(bool unspot = true)
        {
            if (unspot)
                coreMat.SetColor("_AddColor", new Color(0, 0, 0, 0));
            else
                coreMat.SetColor("_AddColor", new Color(0, 100 / 255f, 200 / 255f, 0));
        }

        public void ToggleFacePoser(bool toggle)
        {
            facePoser.SetActive(toggle);
            baseHead.enabled = !toggle;
        }

        public void SetMouthShape(int type)
        {
            animator.SetInteger("Mouth Type", type);
        }

        public void SetEyeShape(int leftType, int rightType)
        {
            float interval = 1f / (float)FanClub.EyeShape.Shine;
            animator.SetFloat("Left Eye Type", interval * leftType);
            animator.SetFloat("Right Eye Type", interval * rightType);
        }

        public void SetEyeTarget(float x, float y)
        {
            previousEyeX = x;
            previousEyeY = y;
            nextEyeX = x;
            nextEyeY = y;
            eyeEase = EasingFunction.Ease.Instant;
            eyeEaseStartBeat = double.MaxValue;
            eyeEaseLength = 0;
            animator.SetFloat("EyeX", Mathf.Clamp(x, -1, 1));
            animator.SetFloat("EyeY", Mathf.Clamp(y, -1, 1));
        }

        public void SetEyeTargetEase(double startBeat, float length, float nextX, float nextY, EasingFunction.Ease ease)
        {
            previousEyeX = nextEyeX;
            previousEyeY = nextEyeY;

            eyeEaseStartBeat = startBeat;
            eyeEaseLength = length;
            nextEyeX = nextX;
            nextEyeY = nextY;
            eyeEase = ease;
        }
    }
}