using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_Fillbots
{
    public enum BotSize
    {
        Small,
        Medium, 
        Large
    }

    public enum BotState
    {
        Idle,
        Holding,
        Ace,
        Just,
        Ng,
        Dance,
    }

    public enum EndAnim
    {
        Both,
        Ace,
        Just,
    }

    public class NtrFillbot : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private BotSize size;
        public double holdLength = 4f;
        [SerializeField] private float limbFallHeight = 15f;
        [System.NonSerialized] public double conveyerRestartLength = 0.5;

        [System.NonSerialized] public double startBeat = -1;
        [System.NonSerialized] public double conveyerStartBeat = -1;
        private double conveyerLength = 1;

        private Vector2 startPos;

        private float lerpDistance;
        private float lerpIdleDistance;
        [SerializeField] private float flyDistance;
        [SerializeField] private float stackDistanceRate;

        [Header("Body Parts")]
        [SerializeField] private Animator fullBody;
        private Material fullBodyMaterial;
        [SerializeField] private Animator legs;
        private Transform legsTrans;
        [SerializeField] private Animator body;
        private Transform bodyTrans;
        [SerializeField] private Animator head;
        private Transform headTrans;
        [SerializeField] private SpriteRenderer fuelFill;

        [SerializeField] private Animator fillAnim;

        private float legsPosY;
        private float bodyPosY;
        private float headPosY;

        private bool legsHaveFallen;
        private bool bodyHasFallen;
        private bool headHasFallen;

        private Fillbots game;

        private GameEvent beepEvent;

        private PlayerActionEvent releaseEvent;

        private Sound fillSound;

        private BotState _botState = BotState.Idle;
        public BotState botState { get { return _botState; }}
        private bool isExplode = false;
        [System.NonSerialized] public bool isStack;
        private double stackBeat, stackLength;

        [System.NonSerialized] public EndAnim endAnim;
        [System.NonSerialized] public bool altOK;

        private float normalizedFill;

        private void OnDestroy()
        {
            fillSoundRelease();
        }

        private void Awake()
        {
            game = Fillbots.instance;
            legsTrans = legs.GetComponent<Transform>();
            bodyTrans = body.GetComponent<Transform>();
            headTrans = head.GetComponent<Transform>();

            legsPosY = legsTrans.position.y;
            bodyPosY = bodyTrans.position.y;
            headPosY = headTrans.position.y;

            legsTrans.position = new Vector3(legsTrans.position.x, legsTrans.position.y + limbFallHeight);
            bodyTrans.position = new Vector3(bodyTrans.position.x, bodyTrans.position.y + limbFallHeight);
            headTrans.position = new Vector3(headTrans.position.x, headTrans.position.y + limbFallHeight);

            startPos = transform.position;

            lerpDistance = 0 - startPos.x;
            lerpIdleDistance = lerpDistance;
        }

        public void MoveConveyer(float normalized, float lerpDistance, float flyDistance = 0)
        {
            if (_botState is BotState.Holding)
            {
                StopConveyer();
                return;
            }

            transform.position = new Vector3(Mathf.LerpUnclamped(startPos.x, startPos.x + lerpDistance, normalized),
                                             Mathf.LerpUnclamped(startPos.y, startPos.y + flyDistance, normalized));
            if (normalized >= 8)
            {
                game.currentBots.Remove(this);
                Destroy(this.gameObject);
            }
        }

        public void StopConveyer()
        {
            startPos = transform.position;
            lerpIdleDistance = 0 - startPos.x;
        }

        public void StackToLeft(double beat, double length)
        {
            if (conveyerLength <= stackDistanceRate) return;
            isStack = true;
            stackBeat = beat - length;
            stackLength = length;
            conveyerStartBeat += stackDistanceRate;
            conveyerLength -= stackDistanceRate;
        }

        public void Init()
        {
            conveyerStartBeat = startBeat + 3;

            BeatAction.New(game, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat, delegate { legs.Play("Impact", 0, 0); legsHaveFallen = true; legsTrans.position = new Vector3(legsTrans.position.x, legsPosY); }),
                new BeatAction.Action(startBeat + 1, delegate { body.Play("Impact", 0, 0); bodyHasFallen = true; bodyTrans.position = new Vector3(bodyTrans.position.x, bodyPosY);}),
                new BeatAction.Action(startBeat + 2, delegate { head.Play("Impact", 0, 0); headHasFallen = true; headTrans.position = new Vector3(headTrans.position.x, headPosY);}),
                new BeatAction.Action(startBeat + 3, delegate
                {
                    fullBody.gameObject.SetActive(true);
                    legs.gameObject.SetActive(false);
                    head.gameObject.SetActive(false);
                    body.gameObject.SetActive(false);
                })
            });

            string sizePrefix = size switch
            {
                BotSize.Small => "small",
                BotSize.Medium => "medium",
                BotSize.Large => "big",
                _ => throw new System.NotImplementedException()
            };

            List<MultiSound.Sound> sounds = new();
            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, 1);
            for (int i = (int)Mathf.Ceil(Mathf.Max(normalizedBeat-0.1f, 0)); i <= 2; i++)
            {
                sounds.Add(new("fillbots/" + sizePrefix + "Fall", startBeat + i));
            }
            if (sounds.Count > 0) MultiSound.Play(sounds.ToArray(), true, true);

            game.ScheduleInput(startBeat, 4, Fillbots.InputAction_BasicPress, JustHold, Empty, Empty);

            game.currentBots.Add(this);
        }

        public void InitColor(Color fuelColor, Color lampColorOff, Color lampColorOn)
        {
            fullBodyMaterial = fullBody.GetComponent<SpriteRenderer>().material;
            fullBodyMaterial.SetColor("_ColorBravo", fuelColor);
            fullBodyMaterial.SetColor("_ColorAlpha", lampColorOff);

            Material botMaterial;
            
            botMaterial = head.GetComponent<SpriteRenderer>().material;
            botMaterial.SetColor("_ColorAlpha", lampColorOff);

            fuelFill.color = fuelColor;

            var full = fullBody.GetComponent<FullBody>();
            full.lampColorOff = lampColorOff;
            full.lampColorOn = lampColorOn;
        }

        private void Update()
        {
            var cond = Conductor.instance;

            if (cond.isPlaying && !cond.isPaused)
            {
                if (startBeat != -1)
                {
                    if (!legsHaveFallen) UpdateLimbPosition(cond, startBeat, legsTrans, legsPosY);
                    if (!bodyHasFallen) UpdateLimbPosition(cond, startBeat + 1, bodyTrans, bodyPosY);
                    if (!headHasFallen) UpdateLimbPosition(cond, startBeat + 2, headTrans, headPosY);
                    if (isStack) HandleStacking(cond);
                    if (headHasFallen && bodyHasFallen && legsHaveFallen) HandleConveyer(cond);
                }

                if (_botState is BotState.Holding)
                {
                    HandleHolding(cond);
                }
                else if (fullBody.gameObject.activeSelf)
                {
                    fillAnim.DoNormalizedAnimation("Fill", normalizedFill);
                }
            }
        }

        private void UpdateLimbPosition(Conductor cond, double targetBeat, Transform limbTrans, float limbPosY)
        {
            float normalizedBeat = cond.GetPositionFromBeat(targetBeat - 0.25, 0.25);
            float lerpedY = Mathf.Lerp(limbPosY + limbFallHeight, limbPosY, normalizedBeat);
            limbTrans.position = new Vector3(limbTrans.position.x, Mathf.Clamp(lerpedY, limbPosY, limbPosY + limbFallHeight));
        }

        private void HandleStacking(Conductor cond)
        {
            float normalizedBeat = cond.GetPositionFromBeat(stackBeat, stackLength);
            if (normalizedBeat >= 0 && normalizedBeat < 1)
            {
                MoveConveyer(normalizedBeat, lerpDistance * stackDistanceRate);
            }
            else if (normalizedBeat >= 1)
            {
                MoveConveyer(1, lerpDistance * stackDistanceRate);
                StopConveyer();
                isStack = false;
            }
        }

        private void HandleConveyer(Conductor cond)
        {
            if (this.conveyerStartBeat >= 0)
            {
                float normalizedBeat = cond.GetPositionFromBeat(this.conveyerStartBeat, conveyerLength);
                if (normalizedBeat >= 0)
                {
                    if (_botState is BotState.Ace) MoveConveyer(normalizedBeat, lerpDistance, flyDistance);
                    else if (_botState is BotState.Idle) MoveConveyer(normalizedBeat, lerpIdleDistance);
                    else MoveConveyer(normalizedBeat, lerpDistance);
                }
            }
            else
            {
                StopConveyer();
            }
        }

        private void HandleHolding(Conductor cond)
        {
            float normalizedBeat = cond.GetPositionFromBeat(startBeat + 4, holdLength);
            float normalizedExplodeBeat = cond.GetPositionFromBeat(startBeat + 4, holdLength + 0.25);

            if (!isExplode && beepEvent != null && beepEvent.enabled && ReportBeat(ref beepEvent.lastReportedBeat))
            {
                if (beepEvent.lastReportedBeat < beepEvent.startBeat + beepEvent.length)
                {
                    SoundByte.PlayOneShotGame("fillbots/beep");
                }
                fullBody.DoScaledAnimationAsync("HoldBeat", 1f);
                string sizeSuffix = game.fillerPosition switch
                {
                    BotSize.Small => "Small",
                    BotSize.Medium => "Medium",
                    BotSize.Large => "Large",
                    _ => throw new System.NotImplementedException()
                };
                game.filler.DoScaledAnimationAsync("HoldBeat" + sizeSuffix, 1f);
            }

            fillAnim.DoNormalizedAnimation("Fill", Mathf.Clamp(normalizedBeat, 0, 1));

            if (!isExplode && !game.IsExpectingInputNow(Fillbots.InputAction_BasicRelease) && normalizedExplodeBeat >= 1f)
            {
                HandleExplosion(cond);
            }
            else if (PlayerInput.GetIsAction(Fillbots.InputAction_BasicRelease) && !game.IsExpectingInputNow(Fillbots.InputAction_BasicRelease))
            {
                HandleRelease(cond, normalizedBeat);
            }
        }

        private void HandleExplosion(Conductor cond)
        {
            isExplode = true;
            fullBody.Play("Beyond", 0, 0);
            BeatAction.New(game, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + holdLength + 5.5, delegate {
                    game.fillerHolding = false;
                    SoundByte.PlayOneShotGame("fillbots/explosion");
                    fillSoundRelease();
                    game.currentBots.Remove(this);
                    Destroy(this.gameObject);
                }),
            });
        }

        private void HandleRelease(Conductor cond, float normalizedBeat)
        {
            if (normalizedBeat < 1)
            {
                fullBody.Play("Dead", 0, 0);
                SoundByte.PlayOneShotGame("fillbots/miss");
            }
            else if (!isExplode)
            {
                fullBody.DoScaledAnimationAsync("ReleaseLate", 0.5f);
                SoundByte.PlayOneShotGame("fillbots/miss");
            }
            fillSoundRelease();
            beepEvent.enabled = false;
            _botState = BotState.Ng;
            game.fillerHolding = false;
            normalizedFill = normalizedBeat;
            if (conveyerRestartLength >= 0)
            {
                this.conveyerStartBeat = cond.songPositionInBeats + conveyerRestartLength;
                if (game.conveyerStartBeat == -1) game.conveyerStartBeat = this.conveyerStartBeat;
            }
            else
            {
                this.conveyerStartBeat = -2;
                game.conveyerStartBeat = -1;
            }
            releaseEvent.Disable();
        }

        private void JustHold(PlayerActionEvent caller, float state)
        {
            string sizeSuffix = game.fillerPosition switch
            {
                BotSize.Small => "Small",
                BotSize.Medium => "Medium",
                BotSize.Large => "Large",
                _ => throw new System.NotImplementedException()
            };
            game.filler.DoScaledAnimationAsync("Hold" + sizeSuffix, 0.5f);
            SoundByte.PlayOneShotGame("fillbots/armExtension");
            
            if (state >= 1f || state <= -1f)
            {
                fullBody.Play("HoldBarely", 0, 0);
                return;
            }
            game.RenewConveyerNormalizedOffset();
            game.conveyerStartBeat = -1;
            conveyerLength = 1;

            transform.position = new Vector3(0, transform.position.y, 0);
            _botState = BotState.Holding;
            game.fillerHolding = true;
            fullBody.DoScaledAnimationAsync("Hold", 1f);
            SoundByte.PlayOneShotGame("fillbots/beep");

            float watarPitch = 3 / (float)(holdLength + 3) + 0.5f;
            fillSound = SoundByte.PlayOneShotGame("fillbots/water", -1, watarPitch, 1, true);
            fillSound.BendUp((float)holdLength * Conductor.instance.pitchedSecPerBeat / 0.5f,2*watarPitch);       // sorry

            releaseEvent = game.ScheduleInput(startBeat + 4, holdLength, Fillbots.InputAction_BasicRelease, JustRelease, Empty, Empty);
            beepEvent = new GameEvent()
            {
                startBeat = startBeat + 4,
                lastReportedBeat = startBeat + 4,
                length = (float)holdLength,
                enabled = true
            };
        }

        private void JustRelease(PlayerActionEvent caller, float state)
        {
            fillSoundRelease();
            beepEvent.enabled = false;
            if (conveyerRestartLength >= 0)
            {
                this.conveyerStartBeat = caller.timer + caller.startBeat + conveyerRestartLength;
                game.RenewConveyerNormalizedOffset();
                if (game.conveyerStartBeat is not -2) game.conveyerStartBeat = this.conveyerStartBeat;
            }
            else
            {
                this.conveyerStartBeat = -2;
                game.conveyerStartBeat = -1;
            }
 
            string sizeSuffix = game.fillerPosition switch
            {
                BotSize.Small => "Small",
                BotSize.Medium => "Medium",
                BotSize.Large => "Large",
                _ => throw new System.NotImplementedException()
            };
            if (state >= 1f)
            {
                _botState = BotState.Ng;
                SoundByte.PlayOneShotGame("fillbots/miss");
                game.filler.DoScaledAnimationAsync("ReleaseWhiff" + sizeSuffix, 0.5f);
                SoundByte.PlayOneShotGame("fillbots/armRetractionPop");
                fullBody.DoScaledAnimationAsync("ReleaseLate", 0.5f);
                return;
            }
            else if (state <= -1f)
            {
                _botState = BotState.Ng;
                SoundByte.PlayOneShotGame("fillbots/miss");
                game.filler.DoScaledAnimationAsync("ReleaseWhiff" + sizeSuffix, 0.5f);
                SoundByte.PlayOneShotGame("fillbots/armRetractionPop");
                fullBody.DoScaledAnimationAsync("ReleaseEarly", 0.5f);
                return;
            }
            
            if ( ((endAnim is EndAnim.Both && state == 0) || endAnim is EndAnim.Ace) && conveyerRestartLength >= 0 )
            {
                _botState = BotState.Ace;
                BeatAction.New(game, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(caller.startBeat + caller.timer + 0.5, delegate {
                        fullBody.DoScaledAnimationAsync("Fly", 0.5f);
                    }),
                });
            } 
            else
            {
                _botState = BotState.Just;
                if (size is BotSize.Small)
                {
                    BeatAction.New(game, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(caller.startBeat + caller.timer + 1, delegate {
                            fullBody.DoScaledAnimationAsync("Success", 0.5f);
                        }),
                    });
                }
                else
                {
                    BeatAction.New(game, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(caller.startBeat + caller.timer + 0.9, delegate {
                            _botState = BotState.Dance;
                        }),
                    });
                }

            }

            game.fillerHolding = false;
            game.filler.DoScaledAnimationAsync("Release" + sizeSuffix, 0.5f);
            SoundByte.PlayOneShotGame("fillbots/armRetraction");
            fullBody.DoScaledAnimationAsync("Release", 1f);
            string sizePrefix = size switch
            {
                BotSize.Small => "small",
                BotSize.Medium => "medium",
                BotSize.Large => "big",
                _ => throw new System.NotImplementedException()
            };
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("fillbots/" + sizePrefix + "Move", caller.startBeat + caller.timer + (altOK ? 0 : 0.5)),
                new MultiSound.Sound("fillbots/" + sizePrefix + "OK1", caller.startBeat + caller.timer + (altOK ? 0 : 0.5)),
                new MultiSound.Sound("fillbots/" + sizePrefix + "OK2", caller.startBeat + caller.timer + (altOK ? 0.5 : 1)),
            });
        }

        private void Empty(PlayerActionEvent caller) { }

        private void fillSoundRelease()
        {
            if (fillSound != null)
            {
                fillSound.KillLoop(0);
                fillSound = null;
            }
        }

        public void SuccessDance()
        {
            fullBody.DoScaledAnimationAsync("Success", 0.5f);
        }

        private bool ReportBeat(ref double lastReportedBeat)
        {
            var cond = Conductor.instance;
            bool result = cond.songPositionInBeats >= (lastReportedBeat) + 1f;
            if (result)
            {
                lastReportedBeat += 1f;
            }
            return result;
        }
    }
}

