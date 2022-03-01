using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.CropStomp
{
    public class Veggie : PlayerActionObject
    {
        public bool isMole;
        public Sprite[] veggieSprites;
        public Sprite[] moleSprites;
        public SpriteRenderer veggieSprite;
        public Transform veggieTrans;
        public BezierCurve3D curve;

        public float targetBeat;
        private float stompedBeat;
        private int veggieState = 0;
        private bool boinked; // Player got barely when trying to pick.

        private float landBeat;

        private CropStomp game;

        private void Start()
        {
            game = CropStomp.instance;

            if (!isMole)
            {
                veggieSprite.sprite = veggieSprites[UnityEngine.Random.Range(0, veggieSprites.Length)];
            }
        }

        private bool gotStomped; // Safeguard in case nested Update() call breaks.
        private void Update()
        {
            if (!game.isMarching)
                return;

            // Veggie missed. Handle missed state.
            if (veggieState == -1)
            {
                MissedUpdate();
                return;
            }

            // Veggie picked. Handle picked state.
            if (veggieState == 2)
            {
                PickedUpdate();
                return;
            }

            var cond = Conductor.instance;

            float normalizedBeat = cond.GetPositionFromMargin(targetBeat, 1f);
            StateCheck(normalizedBeat);
            
            // In ground.
            if (veggieState == 0)
            {
                if (normalizedBeat > Minigame.LateTime())
                {
                    veggieState = -1;
                    return;
                }

                if (PlayerInput.Pressed())
                {
                    if (state.perfect)
                    {
                        StompVeggie(false);
                    }
                    else if (state.notPerfect())
                    {
                        veggieState = -1;
                    }
                }
            }
            // In air.
            else if (veggieState == 1)
            {
                float airPosition = cond.GetPositionFromBeat(stompedBeat, landBeat - stompedBeat);
                veggieTrans.position = curve.GetPoint(Mathf.Clamp(airPosition, 0, 1));

                if (normalizedBeat > Minigame.EndTime())
                {
                    veggieState = -1;
                    
                    // Stuff that happens upon veggie landing goes here.

                    return;
                }

                if (PlayerInput.PressedUp())
                {
                    if (state.perfect)
                    {
                        PickVeggie(false);
                    }
                    else if (state.notPerfect())
                    {
                        veggieState = -1;
                        boinked = true;

                        // Stuff that happens upon boink goes here.

                        MissedUpdate();
                    }

                    game.bodyAnim.Play("Pick", 0, 0);
                    game.isFlicking = true;
                }
            }
        }

        private void MissedUpdate()
        {
            if (boinked)
            {

            }
            else
            {

            }
        }

        private void PickedUpdate()
        {

        }

        private void StompVeggie(bool autoTriggered)
        {
            // Juuuuuust in case.
            if (gotStomped)
            {
                Debug.Log("Recursion moment?");
                return;
            }
            gotStomped = true;

            var cond = Conductor.instance;

            veggieState = 1;
            targetBeat = targetBeat + (isMole ? 0.5f : 1f);

            stompedBeat = cond.songPositionInBeats;

            landBeat = cond.GetBeatFromPositionAndMargin(Minigame.EndTime(), targetBeat, 1f);

            if (autoTriggered)
            {
                game.Stomp();
                game.bodyAnim.Play("Stomp", 0, 0);
            }

            ResetState();

            Update(); // Update flying veggie state immediately.
        }

        private void PickVeggie(bool autoTriggered)
        {
            veggieState = 2;

            if (autoTriggered)
            {
                game.bodyAnim.Play("Pick", 0, 0);
                game.isFlicking = true;
            }

            // Stuff that happens upon veggie picking goes here.

            PickedUpdate();
        }

        public override void OnAce()
        {
            if (veggieState == 0)
                StompVeggie(true);
            else
                PickVeggie(true);
        }
    }
}
