using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.CropStomp
{
    public class CropStomp : Minigame
    {
        const float stepDistance = 2.115f;

        float scrollRate => stepDistance / (Conductor.instance.secPerBeat * 2f / Conductor.instance.musicSource.pitch);
        float grassWidth;

        private float newBeat = -1f; // So that marching can happen on beat 0.
        private float marchStartBeat = -1f;
        private float marchOffset;
        private int currentMarchBeat;
        private int stepCount;

        public bool isMarching => marchStartBeat != -1f;

        public Animator legsAnim;
        public Transform farmerTrans;
        public SpriteRenderer grass;
        public Transform grassTrans;
        public Transform scrollingHolder;

        public static CropStomp instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            // Finding grass sprite width for grass scrolling.
            var grassSprite = grass.sprite;
            var borderLeft = grassSprite.rect.xMin + grassSprite.border.x;
            var borderRight = grassSprite.rect.xMax - grassSprite.border.z;
            var borderWidthPixels = borderRight - borderLeft;
            grassWidth = borderWidthPixels / grassSprite.pixelsPerUnit;
        }

        private void Update()
        {
            if (!isMarching)
                return;

            var cond = Conductor.instance;

            if (cond.ReportBeat(ref newBeat, marchOffset, true))
            {
                currentMarchBeat += 1;

                // Step.
                if (currentMarchBeat % 2 != 0)
                {
                    stepCount += 1;
                    var stepAnim = (stepCount % 2 != 0 ? "StepFront" : "StepBack");
                    
                    legsAnim.Play(stepAnim, 0, 0);
                }
                // Lift.
                else
                {
                    var liftAnim = (stepCount % 2 != 0 ? "LiftBack" : "LiftFront");
                    legsAnim.Play(liftAnim, 0, 0);

                    var farmerPos = farmerTrans.localPosition;
                    farmerTrans.localPosition = new Vector3(farmerPos.x - stepDistance, farmerPos.y, farmerPos.z);
                }
            }

            // Object scroll.
            var scrollPos = scrollingHolder.localPosition;
            var newScrollX = scrollPos.x + (scrollRate * Time.deltaTime);
            scrollingHolder.localPosition = new Vector3(newScrollX, scrollPos.y, scrollPos.z);

            // Grass scroll.
            var grassPos = grassTrans.localPosition;

            var newGrassX = grassPos.x + (scrollRate * Time.deltaTime);
            newGrassX = (newGrassX % (grassWidth * 4.5f));

            grassTrans.localPosition = new Vector3(newGrassX, grassPos.y, grassPos.z);
        }

        public void StartMarching(float beat)
        {
            marchStartBeat = beat;
            marchOffset = (marchStartBeat % 1) * Conductor.instance.secPerBeat / Conductor.instance.musicSource.pitch;
            currentMarchBeat = 0;
            stepCount = 0;
        }
    }
}
