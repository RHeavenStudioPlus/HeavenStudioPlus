using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;
using DG.Tweening;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_CropStomp
{
    public class Veggie : MonoBehaviour
    {
        static float pickedRotationSpeed = -1080f;

        public bool isMole;
        public Sprite[] veggieSprites;
        public Animator moleAnim;
        public SpriteRenderer veggieSprite;
        public Transform veggieTrans;
        public BezierCurve3D curve;
        private BezierCurve3D hitCurve;

        public double targetBeat;
        private double stompedBeat;
        private double pickedBeat;
        private float pickTime = 1f;
        private int veggieState = 0;
        private bool boinked; // Player got barely when trying to pick.
        private bool pickEligible = true;
        private int veggieType;

        private double landBeat;

        private Tween squashTween;

        private CropStomp game;

        public void Init()
        {
            game = CropStomp.instance;

            // if (Conductor.instance.isPlaying)
                game.ScheduleInput(targetBeat - 1, 1f, CropStomp.InputAction_BasicPress, StompJust, StompMiss, Out);

            if (!isMole)
            {
                veggieType = UnityEngine.Random.Range(0, veggieSprites.Length);
                veggieSprite.sprite = veggieSprites[veggieType];
            }
            else
            {
                pickTime = 1.5f;
            }
        }

        private bool gotStomped; // Safeguard in case nested Update() call breaks.
        private void Update()
        {
            if (!game.isMarching) return;

            switch (veggieState)
            {
                case -1: MissedUpdate(); return;
                // case 0:
                case 2: PickedUpdate(); return;
                case 1:
                    float airPosition = Conductor.instance.GetPositionFromBeat(stompedBeat, landBeat - stompedBeat);
                    veggieTrans.position = curve.GetPoint(Mathf.Clamp(airPosition, 0, 1));

                    if (PlayerInput.GetIsAction(CropStomp.InputAction_FlickRelease) && !game.IsExpectingInputNow(CropStomp.InputAction_FlickRelease))
                    {
                        pickEligible = false;
                    }
                    break;
                // default:
            }
        }

        private void StompJust(PlayerActionEvent caller, float state)
        {
            if (GameManager.instance.autoplay)
            {
                StompVeggie(true);
                return;
            }

            if (state >= 1f)
                veggieState = -1;
            else if (state > -1f)
                StompVeggie(false);
        }

        private void StompMiss(PlayerActionEvent caller)
        {
            veggieState = -1;
            caller.Disable();
        }

        private void Out(PlayerActionEvent caller) { }

        private void PickJust(PlayerActionEvent caller, float state)
        {
            game.bodyAnim.DoScaledAnimationAsync("Pick", 0.5f);
            game.isFlicking = true;
            if (!pickEligible) return;
            if (GameManager.instance.autoplay)
            {
                PickVeggie(true);
                return;
            }

            if (state <= -1f || state >= 1f)
            {
                veggieState = -1;
                boinked = true;

                curve.transform.localScale = Vector3.one; // Return curve to normal size in the case of mole curves.

                for (int i = 0; i < 2; i++)
                {
                    var key = curve.KeyPoints[i];
                    var keyPos = key.Position;
                    key.Position = new Vector3(keyPos.x, veggieTrans.position.y + (i * 2), keyPos.z);
                }

                // var key1 = curve.KeyPoints[0];
                // var key1Pos = key1.Position;
                // key1.Position = new Vector3(key1Pos.x, veggieTrans.position.y, key1Pos.z);

                // var key2 = curve.KeyPoints[1];
                // var key2Pos = key2.Position;
                // key2.Position = new Vector3(key2Pos.x, veggieTrans.position.y + 2f, key2Pos.z);

                pickedBeat = Conductor.instance.unswungSongPositionInBeatsAsDouble;

                SoundByte.PlayOneShot("miss");

                MissedUpdate();
            }
            else
            {
                PickVeggie(false);
            }
        }

        private void PickMiss(PlayerActionEvent caller) 
        {
            veggieState = -1;
            
            if (!isMole) SoundByte.PlayOneShotGame("cropStomp/veggieMiss");
            caller.Disable();
        }

        bool moleLaughing;
        private void MissedUpdate()
        {
            if (boinked)
            {
                float fallPosition = Conductor.instance.GetPositionFromBeat(pickedBeat, 1f);
                fallPosition = Mathf.Clamp(fallPosition, 0, 1);
                veggieTrans.position = curve.GetPoint(fallPosition);

                if (fallPosition < 1f)
                {
                    var rotSpeed = isMole ? pickedRotationSpeed : -pickedRotationSpeed;
                    veggieTrans.rotation = Quaternion.Euler(0, 0, veggieTrans.rotation.eulerAngles.z + (rotSpeed * Time.deltaTime));
                }
                else
                {
                    veggieTrans.rotation = Quaternion.Euler(0, 0, 180f);
                }
            }
            else
            {
                if (isMole && !moleLaughing)
                {
                    var distDiff = transform.position.x - game.farmerTrans.position.x;
                    if (distDiff > 1.5f)
                    {
                        moleAnim.DoScaledAnimationAsync("Chuckle", 0.5f);
                        moleLaughing = true;
                    }
                }
            }
        }

        private void PickedUpdate()
        {
            float pickPosition = Conductor.instance.GetPositionFromBeat(pickedBeat, pickTime);
            pickPosition = Mathf.Clamp(pickPosition, 0, 1);
            veggieTrans.position = hitCurve.GetPoint(pickPosition);

            var rotSpeed = isMole ? -pickedRotationSpeed : pickedRotationSpeed;
            veggieTrans.rotation = Quaternion.Euler(0, 0, veggieTrans.rotation.eulerAngles.z + (rotSpeed * Time.deltaTime));

            if (!isMole)
            {
                var veggieScale = Mathf.Min(1.5f - pickPosition, 1f);
                veggieTrans.localScale = Vector2.one * veggieScale;
                if (pickPosition >= 1f)
                {
                    game.CollectPlant(veggieType);
                }
            }
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

            ParticleSystem spawnedHit = Instantiate(game.hitParticle, game.hitParticle.transform.parent);

            spawnedHit.Play();

            veggieState = 1;
            game.ScheduleInput(targetBeat, isMole ? 0.5f : 1f, CropStomp.InputAction_FlickRelease, PickJust, PickMiss, Out);
            targetBeat = cond.GetUnSwungBeat(targetBeat + (isMole ? 0.5f : 1f));

            stompedBeat = cond.unswungSongPositionInBeatsAsDouble;

            landBeat = targetBeat + cond.SecsToBeats(Minigame.NgLateTime() - 1, cond.GetBpmAtBeat(targetBeat));

            if (autoTriggered)
            {
                game.Stomp();
                game.bodyAnim.DoScaledAnimationAsync("Stomp", 0.5f);
            }

            if (!isMole)
            {
                MultiSound.Play(
                    new MultiSound.Sound[] { new MultiSound.Sound("cropStomp/veggieOh", targetBeat - 0.5f) }
                );
            }
            else
            {
                moleAnim.DoScaledAnimationAsync("Idle", 0.5f);
            }

            var veggieScale = veggieTrans.localScale;
            veggieTrans.localScale = new Vector3(veggieScale.x * 0.5f, veggieScale.y, veggieScale.z);
            squashTween = veggieTrans.DOScaleX(veggieScale.x, cond.pitchedSecPerBeat * 0.5f);

            Update(); // Update flying veggie state immediately.
        }

        private void PickVeggie(bool autoTriggered)
        {
            veggieState = 2;

            if (autoTriggered)
            {
                game.bodyAnim.DoScaledAnimationAsync("Pick", 0.5f);
                game.isFlicking = true;
            }

            var key1 = game.pickCurve.KeyPoints[0];
            var keyPos = key1.Position;
            key1.Position = new Vector3(keyPos.x, veggieTrans.position.y, keyPos.z);

            pickedBeat = Conductor.instance.unswungSongPositionInBeatsAsDouble;

            if (!isMole)
            {
                BeatAction.New(this, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(pickedBeat + 0.5f, delegate { veggieSprite.sortingOrder = -1; }),
                    new BeatAction.Action(pickedBeat + pickTime, delegate { GameObject.Destroy(gameObject); })
                });

                SoundByte.PlayOneShotGame("cropStomp/veggieKay");

                hitCurve = game.pickCurve;
            }
            else
            {
                BeatAction.New(this, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(pickedBeat + pickTime, delegate { GameObject.Destroy(gameObject); })
                });

                SoundByte.PlayOneShotGame("cropStomp/GEUH");

                hitCurve = game.moleCurve;
            }

            if (squashTween != null)
                squashTween.Kill(true);

            PickedUpdate();
        }
    }
}
