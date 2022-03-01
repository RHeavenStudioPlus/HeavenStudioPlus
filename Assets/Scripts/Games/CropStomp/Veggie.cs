using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;
using DG.Tweening;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.CropStomp
{
    public class Veggie : PlayerActionObject
    {
        static float pickedRotationSpeed = -1080f;

        public bool isMole;
        public Sprite[] veggieSprites;
        public Sprite[] moleSprites;
        public SpriteRenderer veggieSprite;
        public Transform veggieTrans;
        public BezierCurve3D curve;

        public float targetBeat;
        private float stompedBeat;
        private float pickedBeat;
        private int veggieState = 0;
        private bool boinked; // Player got barely when trying to pick.

        private float landBeat;

        private Tween squashTween;

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
                    
                    Jukebox.PlayOneShotGame("cropStomp/veggieMiss");

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

                        Jukebox.PlayOneShot("miss");

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
            float pickPosition = Conductor.instance.GetPositionFromBeat(pickedBeat, 1f);
            pickPosition = Mathf.Clamp(pickPosition, 0, 1);
            veggieTrans.position = game.pickCurve.GetPoint(pickPosition);

            veggieTrans.rotation = Quaternion.Euler(0, 0, veggieTrans.rotation.eulerAngles.z + (pickedRotationSpeed * Time.deltaTime));

            var veggieScale = Mathf.Min(1.5f - pickPosition, 1f);
            veggieTrans.localScale = Vector2.one * veggieScale;
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

            if (!isMole)
            {
                BeatAction.New(gameObject, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(targetBeat - 0.5f, delegate { Jukebox.PlayOneShotGame("cropStomp/veggieOh"); })
                });
            }

            var veggieScale = veggieTrans.localScale;
            veggieTrans.localScale = new Vector3(veggieScale.x * 0.5f, veggieScale.y, veggieScale.z);
            squashTween = veggieTrans.DOScaleX(veggieScale.x, cond.secPerBeat * 0.5f / cond.musicSource.pitch);

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
            
            var key1 = game.pickCurve.KeyPoints[0];
            var keyPos = key1.Position;
            key1.Position = new Vector3(keyPos.x, veggieTrans.position.y, keyPos.z);

            pickedBeat = Conductor.instance.songPositionInBeats;

            if (!isMole)
            {
                BeatAction.New(gameObject, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(pickedBeat + 0.5f, delegate { veggieSprite.sortingOrder = -1; }),
                    new BeatAction.Action(pickedBeat + 1f, delegate { GameObject.Destroy(gameObject); })
                });

                Jukebox.PlayOneShotGame("cropStomp/veggieKay");
            }

            if (squashTween != null)
                squashTween.Kill(true);

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
