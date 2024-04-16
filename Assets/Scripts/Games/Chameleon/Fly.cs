using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_Chameleon
{
    public enum FlyType
    {
        Close,
        Far,
    }

    public class Fly : MonoBehaviour
    {
        public double startBeat, lengthBeat;
        private double currentBeat;
        [System.NonSerialized] public FlyType flyType;
        [SerializeField] private Animator flyAnim, wingAnim;
        public bool isFall { get; private set; }

        private Sound loopSound;
        
        float randomAngle = 0;
        Vector2 moveCurrentPos, moveNextPos, moveEndPos;
        bool moveFast;

        private Chameleon game;

        public void Init()
        {
            game = Chameleon.instance;

            string typePrefix = flyType switch
            {
                FlyType.Far => "Far",
                FlyType.Close => "Close",
                _ => throw new System.NotImplementedException()
            };

            moveCurrentPos = flyType switch
            {
                FlyType.Far => new Vector2(-4.5f, 5.4f),
                FlyType.Close => new Vector2(-6, 5.4f),
                _ => throw new System.NotImplementedException()
            };
            moveEndPos = flyType switch
            {
                FlyType.Far => new Vector2(5.15f, 1.6f),
                FlyType.Close => new Vector2(1.5f, -0.25f),
                _ => throw new System.NotImplementedException()
            };
            randomAngle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            moveNextPos = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) + moveEndPos;

            currentBeat = startBeat;

            loopSound = SoundByte.PlayOneShotGame("chameleon/fly" + typePrefix + "Loop", -1, 1, 1, true);
            MultiSound.Play(
                new MultiSound.Sound[] {
                    new MultiSound.Sound("chameleon/fly" + typePrefix + "1", startBeat + lengthBeat),
                    new MultiSound.Sound("chameleon/fly" + typePrefix + "2", startBeat + lengthBeat + 1),
                    new MultiSound.Sound("chameleon/fly" + typePrefix + "3", startBeat + lengthBeat + 2),
                }
            );
            
            flyAnim.enabled = false;
            BeatAction.New(game, new List<BeatAction.Action>()
            {
                // new BeatAction.Action(startBeat, delegate {
                //     var currentBeat = Conductor.instance.songPositionInBeatsAsDouble;
                //     flyAnim.DoScaledAnimationAsync("move" + typePrefix, 0.5f, (float)((currentBeat - startBeat)/8));
                // }),
                new BeatAction.Action(startBeat + lengthBeat, delegate {
                    loopSoundRelease();
                }),
                new BeatAction.Action(startBeat + lengthBeat + 3, delegate {
                    if (!flyAnim.enabled)
                    {
                        flyAnim.enabled = true;
                        flyAnim.DoScaledAnimationAsync("moveEnd" + typePrefix, 0.5f);
                    }
                }),
            });

            var InputAction = flyType switch
            {
                FlyType.Far => Chameleon.InputAction_Far,
                FlyType.Close => Chameleon.InputAction_Close,
                _ => throw new System.NotImplementedException()
            };
            game.ScheduleInput(startBeat + lengthBeat, 3, InputAction, JustCatch, MissCatch, Empty);

            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + lengthBeat + 6, delegate {
                    Destroy();
                }),
            });
        }

        private void Update()
        {
            var cond = Conductor.instance;
            if (cond.isPlaying && !cond.isPaused)
            {
                if (!flyAnim.enabled)
                {
                    float startBeatPosition = Conductor.instance.GetPositionFromBeat(startBeat, 2, ignoreSwing: true);
                    
                    if (startBeatPosition <= 1)
                    {
                        startBeatPosition = (startBeatPosition < 0.5) ? 0 : startBeatPosition * 2 - 1;
                        transform.position = Vector2.Lerp(moveCurrentPos, moveNextPos, startBeatPosition);
                    }
                    else
                    {
                        float currentBeatPosition = Conductor.instance.GetPositionFromBeat(currentBeat, moveFast ? 0.17: 0.5, ignoreSwing: true);;
                        if (currentBeatPosition > 1)
                        {
                            if (startBeatPosition > 1.5) moveFast = true;
                            
                            currentBeat = cond.songPositionInBeatsAsDouble;
                            moveCurrentPos = moveNextPos;
                            randomAngle = randomAngle + UnityEngine.Random.Range(0.7f, 1.3f * Mathf.PI);
                            moveNextPos = (moveFast ? 0.5f : 1) * new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) + moveEndPos;
                        }
                        else
                        {
                            float newPosX = EasingFunction.EaseInOutSine(moveCurrentPos.x, moveNextPos.x, currentBeatPosition);
                            float newPosY = EasingFunction.EaseInOutSine(moveCurrentPos.y, moveNextPos.y, currentBeatPosition);
                            transform.position = new Vector2(newPosX, newPosY);
                        }
                    }
                }
            }
        }

        void JustCatch(PlayerActionEvent caller, float state)
        {
            string typePrefix = flyType switch
            {
                FlyType.Far => "Far",
                FlyType.Close => "Close",
                _ => throw new System.NotImplementedException()
            };
            flyAnim.enabled = true;
            game.chameleonAnim.DoScaledAnimationAsync("tongue" + typePrefix, 0.5f);
            if (state <= -1f || state >= 1f)
            {
                isFall = true;
                flyAnim.DoScaledAnimationAsync("fall" + typePrefix, 0.5f);
                return;
            }
            game.currentFly = null;
            SoundByte.PlayOneShotGame("chameleon/eatCatch");
            SoundByte.PlayOneShotGame("chameleon/eatGulp", startBeat + lengthBeat + 3.25);
            wingAnim.Play("idle", 0, 0);
            flyAnim.DoScaledAnimationAsync("catch" + typePrefix, 0.5f);
            BeatAction.New(game, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + lengthBeat + 3.25, delegate {
                    game.chameleonAnim.DoScaledAnimationAsync("gurp", 0.5f);
                }),
            });
        }

        void MissCatch(PlayerActionEvent caller)
        {
            string typePrefix = flyType switch
            {
                FlyType.Far => "Far",
                FlyType.Close => "Close",
                _ => throw new System.NotImplementedException()
            };
            flyAnim.enabled = true;
            flyAnim.DoScaledAnimationAsync("gone" + typePrefix, 0.5f);
        }

        void Empty(PlayerActionEvent caller) { }

        private void loopSoundRelease()
        {
            if (loopSound != null)
            {
                loopSound.KillLoop(0);
                loopSound = null;
            }
        }

        private void Destroy()
        {
            if (game.currentFly == this) game.currentFly = null;
            Destroy(gameObject);
        }
    }
}