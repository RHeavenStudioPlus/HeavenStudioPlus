using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using RhythmHeavenMania.Util;
using Starpelly;

namespace RhythmHeavenMania.Games.DJSchool
{
    public class Student : PlayerActionObject
    {
        public Animator anim;

        [Header("Properties")]
        public float holdBeat;
        public float swipeBeat;
        public bool isHolding;

        [Header("Components")]
        [SerializeField] private SpriteRenderer flash;
        [SerializeField] private GameObject flashFX;
        [SerializeField] private GameObject flashFXInverse;
        [SerializeField] private GameObject TurnTable;
        [SerializeField] private GameObject slamFX;

        private void Start()
        {
            anim = GetComponent<Animator>();
            TurnTable.GetComponent<Animator>().speed = 0;
        }

        private void Update()
        {
            if (!isHolding)
            {
                float normalizedBeatHold = Conductor.instance.GetPositionFromBeat(holdBeat, 2);

                StateCheck(normalizedBeatHold);

                if (PlayerInput.Pressed())
                {
                    if (state.perfect)
                    {
                        Hold(true);
                    }
                    else if (state.notPerfect())
                    {
                        Hold(false);
                    }
                }
            }
            else if (isHolding)
            {
                float normalizedBeatSwipe = Conductor.instance.GetPositionFromBeat(swipeBeat, 2);

                StateCheck(normalizedBeatSwipe);

                print(normalizedBeatSwipe); ;

                if (PlayerInput.PressedUp())
                {
                    if (state.perfect)
                    {
                        Swipe();
                    }
                    else if (state.notPerfect())
                    {
                        UnHold();
                    }
                }
            }
        }

        public void Hold(bool ace)
        {
            isHolding = true;

            Jukebox.PlayOneShotGame("djSchool/recordStop");
            anim.Play("Hold", 0, 0);

            if (ace)
            {
                FlashFX(true);
            }
        }

        public void UnHold()
        {
            anim.speed = -1;
            anim.Play("Hold", 0, 0);
        }

        public void Swipe()
        {
            isHolding = false;

            Jukebox.PlayOneShotGame("djSchool/recordSwipe");
            anim.Play("Swipe", 0, 0);

            FlashFX(false);

            TurnTable.GetComponent<Animator>().speed = 1;
            TurnTable.GetComponent<Animator>().Play("Student_Turntable_Swipe", 0, 0);

            Instantiate(slamFX).SetActive(true);
        }

        private void FlashFX(bool inverse)
        {
            GameObject prefab = flashFX;

            if (inverse)
                prefab = flashFXInverse;

            GameObject flashFX_ = Instantiate(prefab, this.transform.parent);
            flashFX_.SetActive(true);
            Destroy(flashFX_, 0.5f);

            flash.color = "D0FBFF".Hex2RGB();
            flash.color = new Color(flash.color.r, flash.color.g, flash.color.b, 0.85f);
            flash.DOColor(new Color(flash.color.r, flash.color.g, flash.color.b, 0), 0.15f);
        }

        public void TransitionBackToIdle()
        {
            // for letting go of "hold"
            if (anim.speed == 0)
            {
                anim.speed = 1;
                anim.Play("Idle", 0, 0);
            }
        }
    }
}