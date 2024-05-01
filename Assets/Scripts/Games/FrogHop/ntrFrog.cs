using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_FrogHop
{
    public class ntrFrog : MonoBehaviour
    {
        //definitions
        #region Definitions

        [SerializeField] public Animator FrogAnim;
        [SerializeField] public List<SpriteRenderer> SpriteParts = new();
        [SerializeField] public Transform MissFace;
        [SerializeField] public SpriteRenderer Head;

        [NonSerialized] public int animSide = -1;
        [NonSerialized] public float scaleConstant = 1;
        [NonSerialized] public string loopAnim = null;
        [NonSerialized] public bool isBumped = false;

        #endregion

        //global methods
        #region Global Methods

        public void Start()
        {
            scaleConstant = FrogAnim.transform.localScale.x;
        }

        #endregion

        //frog methods
        #region Frog Methods

        public void Bop()
        {
            FrogAnim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
            isBumped = false;
        }

        public void Update()
        {
            if (loopAnim != null) FrogAnim.DoScaledAnimationAsync("Talk" + loopAnim, 0.5f, 0.5f, 1);
        }

        public void Talk(string type, double animEnd)
        {
            SpriteRenderer isMissing = null;
            if (MissFace != null)
            {
                isMissing = MissFace.GetComponent<SpriteRenderer>();
                if (isMissing.enabled) return;
            }

            loopAnim = type;
            BeatAction.New(this, new List<BeatAction.Action>()
            { new BeatAction.Action(animEnd, delegate { FrogAnim.DoScaledAnimationAsync("Talk" + type, 0.5f, 0.5f, 1); loopAnim = null; }) });
        }

        public void Hop(int side = 0, bool isLong = false)
        {
            SwapSide(side);
            
            FrogAnim.transform.localScale = new Vector3(animSide * scaleConstant, scaleConstant, 1);
            FrogAnim.DoScaledAnimationAsync(isLong ? "LongHop" : "Hop", 0.5f, animLayer: 0);

            isBumped = false;
        }

        public void Charge(int side = 0)
        {
            SwapSide(side);
            
            FrogAnim.transform.localScale = new Vector3(animSide * scaleConstant, scaleConstant, 1);
            FrogAnim.DoScaledAnimationAsync("Charge", 0.5f, animLayer: 0);

            isBumped = false;
        }

        public void Spin()
        {
            FrogAnim.DoScaledAnimationAsync("Spin", 0.5f, animLayer: 0);

            isBumped = false;
        }

        public void Glare()
        {
            FrogAnim.DoScaledAnimationAsync("Glare", 0.5f, 0.5f, 1);
        }

        public void Sweat()
        {
            FrogAnim.DoScaledAnimationAsync("Sweat", 0.5f, 0.5f, animLayer: 2);
        }

        public void Bump()
        {
            if (!isBumped)
            {
                isBumped = true;
                FrogAnim.transform.localScale = new Vector3(scaleConstant, scaleConstant, 1);

                FrogAnim.DoScaledAnimationAsync("Ouch", 0.5f, 0.5f, 1);
                FrogAnim.DoScaledAnimationAsync("Bump", 0.5f, animLayer: 0);

                SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_MISS");
                SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_MISS_BOING");
            }
        }

        public void SwapSide(int side)
        {
            if (side != 0) animSide = side;
            else animSide *= -1;
            if (MissFace != null) MissFace.localScale = new Vector3(animSide, 1, 1);
            Head.flipX = animSide > 0;
        }

        public void Darken(bool reverse = false)
        {
            if (!reverse) foreach (var a in SpriteParts) { a.color = new Color(0.5f, 0.5f, 0.5f, 1); }
            else foreach (var a in SpriteParts) { a.color = new Color(1, 1, 1, 1); }
        }

        #endregion
    }
}