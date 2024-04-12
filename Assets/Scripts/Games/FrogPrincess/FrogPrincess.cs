using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class RvlFrogPrincessLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("frogPrincess", "Frog Princess", "ffffff", false, false, new List<GameAction>()
            {
                new GameAction("jump", "Jump")
                {
                    function = delegate {var e = eventCaller.currentEntity; FrogPrincess.instance.Jump(e.beat); },
                    defaultLength = 4f,
                },
                new GameAction("background appearance", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        FrogPrincess.instance.BackgroundColorSet(e.beat, e.length, e["colorBGStart"], e["colorBGEnd"], e["ease"]);
                    },
                    defaultLength = 0.5f,
                    resizable = true, 
                    parameters = new List<Param>()
                    {
                        new Param("colorBGStart", new Color(0.482f, 0.74f, 0.87f), "Start BG Color", "Set the color at the start of the event."),
                        new Param("colorBGEnd", new Color(0.482f, 0.74f, 0.87f), "End BG Color", "Set the color at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Instant, "Ease", "Set the easing of the action."),
                    }
                },
            },
            new List<string>() { "rvl", "keep" },
            "rvlfrog", "en",
            new List<string>() {},
            chronologicalSortKey: 106
            );
        }
    }
}

namespace HeavenStudio.Games
{
    public class FrogPrincess : Minigame
    {
        [SerializeField] Animator frogAnim;
        [SerializeField] Animator princessAnim;
        [SerializeField] Transform Leaves;
        [SerializeField] Transform Lotuses;
        Animator[] LotusAnims;
        public ParticleSystem splashEffect;
        [SerializeField] private SpriteRenderer BGPlane;

        bool isPrepare, isHold, isGone;
        private ColorEase bgColorEase = new(new Color(0.482f, 0.74f, 0.87f));

        public static FrogPrincess instance;

        public void Awake()
        {
            instance = this;
            LotusAnims = new Animator[] {Lotuses.GetChild(1).GetComponent<Animator>(), Lotuses.GetChild(2).GetComponent<Animator>()};
        
        }
        private void Update()
        {
            var cond = Conductor.instance;
            if (cond.isPlaying && !cond.isPaused)
            {
                if (PlayerInput.GetIsAction(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_BasicPress))
                {
                    ScoreMiss();
                    HoldFastAnim(cond.songPositionInBeatsAsDouble);
                }
                if (PlayerInput.GetIsAction(InputAction_FlickRelease) && !IsExpectingInputNow(InputAction_FlickRelease))
                {
                    ScoreMiss();
                    JumpFastAnim(cond.songPositionInBeatsAsDouble);
                }
                if (PlayerInput.GetIsAction(InputAction_BasicRelease) && PlayerInput.PlayerHasControl() && PlayerInput.CurrentControlStyle is InputSystem.InputController.ControlStyles.Touch)
                {
                    ScoreMiss();
                    JumpFastAnim(cond.songPositionInBeatsAsDouble);
                }
                UpdateBackgroundColor();
            }
        }

        public void Jump(double beat)
        {
            if (!isGone)
            {
                isPrepare = true;
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { ReadyAnim();}),
                    new BeatAction.Action(beat + 1, delegate { ReadyAnim();}),
                });

                ScheduleInput(beat, 2, InputAction_BasicPress, JustHold, MissHold, Empty, CanHold);
            }
        }

        void JustHold(PlayerActionEvent caller, float state)
        {
            var currentBeat = caller.timer + caller.startBeat; 
            isJust = false;
            ScheduleInput(currentBeat, 1, InputAction_FlickRelease, JustJump, MissJump, Empty, CanJump);

            if (state >= 1f || state <= -1f)
            {
                HoldBarelyAnim();
                return;
            }

            HoldAnim();
        }

        void MissHold(PlayerActionEvent caller)
        {
            HoldMissAnim(caller.timer + caller.startBeat);
        }

        bool CanHold() { return isPrepare;}

        void JustJump(PlayerActionEvent caller, float state)
        {
            var currentBeat = caller.timer + caller.startBeat; 
            isJust = true;

            if (state >= 1f || state <= -1f)
            {
                JumpBarelyAnim(currentBeat);
                return;
            }

            JumpAnim(currentBeat);
        }

        void MissJump(PlayerActionEvent caller)
        {
            JumpMissAnim(caller.timer + caller.startBeat);
        }

        bool isJust = false;    // not fundamental solution
        bool CanJump() { return isHold && !isGone || isJust;}

        void Empty(PlayerActionEvent caller) { }

        void ReadyAnim()
        {
            if (!isGone)
            {
                SoundByte.PlayOneShotGame("frogPrincess/ready");
                if (!frogAnim.IsPlayingAnimationNames("jump"))
                {
                    frogAnim.DoScaledAnimationAsync("ready", 0.5f);
                    princessAnim.DoScaledAnimationAsync("ready", 0.5f);
                    princessAnim.DoScaledAnimationAsync("wary", 0.5f);
                }
            }
        }

        void HoldAnim()
        {
            isHold = true;

            UpdatePos();
            LotusAnims[0].DoScaledAnimationAsync("hold", 0.5f);
            frogAnim.DoScaledAnimationAsync("hold", 0.5f);

            SoundByte.PlayOneShotGame("frogPrincess/lean");
            princessAnim.DoScaledAnimationAsync("hold", 0.5f);
            princessAnim.Play("idle", 1, 0);
        }

        void HoldBarelyAnim()
        {
            isHold = true;

            UpdatePos();
            LotusAnims[0].DoScaledAnimationAsync("hold", 0.5f);
            frogAnim.DoScaledAnimationAsync("hold", 0.5f);

            SoundByte.PlayOneShotGame("frogPrincess/lean");
            SoundByte.PlayOneShotGame("frogPrincess/7");
            princessAnim.DoScaledAnimationAsync("holdBarely", 0.5f);
            princessAnim.DoScaledAnimationAsync("surpriseHoldBarely", 0.5f);
        }

        void HoldMissAnim(double beat)
        {
            if (isPrepare)
            {
                isGone = true;

                UpdatePos();
                SoundByte.PlayOneShotGame("frogPrincess/A");
                LotusAnims[0].DoScaledAnimationAsync("fall", 0.5f);
                frogAnim.DoScaledAnimationAsync("fall", 0.5f);
                princessAnim.DoScaledAnimationAsync("fallBackward", 0.5f);

                Appear(beat + 0.5, false);
            }
        }

        void HoldFastAnim(double beat)
        {
            if (!isHold && !isGone)
            {
                isGone = true;
                isPrepare = false;

                UpdatePos();
                LotusAnims[0].DoScaledAnimationAsync("hold", 0.5f);
                frogAnim.DoScaledAnimationAsync("hold", 0.5f);
                SoundByte.PlayOneShotGame("frogPrincess/lean");
                SoundByte.PlayOneShotGame("frogPrincess/A");
                princessAnim.DoScaledAnimationAsync("fallForward", 0.5f);
                princessAnim.DoScaledAnimationAsync("surpriseFall", 0.5f);

                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat + 0.75, delegate
                    {
                        LotusAnims[0].DoScaledAnimationAsync("release", 0.5f);
                        frogAnim.DoScaledAnimationAsync("release", 0.5f);
                    }),
                });

                Appear(beat, false);
            }
        }

        void JumpAnim(double beat)
        {
            isHold = false;

            UpdatePos();
            LotusAnims[0].DoScaledAnimationAsync("release", 0.5f);
            StartCoroutine(MoveCo(Lotuses, beat, moveTime, moveDistance));
            StartCoroutine(MoveCo(Leaves, beat, moveTime, moveDistance));

            SoundByte.PlayOneShotGame("frogPrincess/jump");
            LotusAnims[1].DoScaledAnimationAsync("jump", 0.5f);
            frogAnim.DoScaledAnimationAsync("jump", 0.5f);
            princessAnim.DoScaledAnimationAsync("jump", 0.5f);
            princessAnim.DoScaledAnimationAsync("happy", 0.5f);
        }

        void JumpBarelyAnim(double beat)
        {
            isHold = false;

            UpdatePos();
            LotusAnims[0].DoScaledAnimationAsync("release", 0.5f);
            StartCoroutine(MoveCo(Lotuses, beat, moveTime, moveDistance));
            StartCoroutine(MoveCo(Leaves, beat, moveTime, moveDistance));

            SoundByte.PlayOneShotGame("frogPrincess/jump");
            SoundByte.PlayOneShotGame("frogPrincess/7", beat + 0.5);
            LotusAnims[1].DoScaledAnimationAsync("jumpBarely", 0.5f);
            frogAnim.DoScaledAnimationAsync("jumpBarely", 0.5f);
            princessAnim.DoScaledAnimationAsync("jumpBarely", 0.5f);
            princessAnim.DoScaledAnimationAsync("surpriseJumpBarely", 0.5f);
        }

        void JumpMissAnim(double beat)
        {
            if (isHold && !isGone)
            {
                isHold = false;
                isGone = true;

                UpdatePos();
                SoundByte.PlayOneShotGame("frogPrincess/A");
                LotusAnims[0].DoScaledAnimationAsync("fall", 0.5f);
                frogAnim.DoScaledAnimationAsync("fall", 0.5f);
                princessAnim.DoScaledAnimationAsync("fallForward", 0.5f);

                Appear(beat, false);
            }
        }

        void JumpFastAnim(double beat)
        {
            if (isHold && !isGone)
            {
                isHold = false;
                isGone = true;

                UpdatePos();
                SoundByte.PlayOneShotGame("frogPrincess/jump");
                LotusAnims[0].DoScaledAnimationAsync("release", 0.5f);
                StartCoroutine(MoveCo(Lotuses, beat, moveTime, moveDistance));
                StartCoroutine(MoveCo(Leaves, beat, moveTime, moveDistance));
                frogAnim.DoScaledAnimationAsync("jumpFast", 0.5f);
                princessAnim.DoScaledAnimationAsync("jumpFast", 0.5f);
                princessAnim.Play("idle", 1, 0);

                ParticleSystem spawnedParticle = Instantiate(splashEffect, transform);
                BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat + 0.5, delegate
                    {
                        SoundByte.PlayOneShotGame("frogPrincess/A");
                        spawnedParticle.PlayScaledAsync(0.5f);
                    }),
                    new BeatAction.Action(beat + 1, delegate
                    {
                        Destroy(spawnedParticle);
                    }),
                });

                Appear(beat, true);
            }
        }

        public float moveDistance;
        public float moveTime;
        IEnumerator MoveCo(Transform thing, double beat, float length, float xValue)
        {
            float xPos = thing.localPosition.x;
            if (length > 0)
            {
                float normalized = Conductor.instance.GetPositionFromBeat(beat, length, false);
                while (normalized <= 1f)
                {
                    normalized = Conductor.instance.GetPositionFromBeat(beat, length);
                    thing.localPosition = new Vector2(Mathf.SmoothStep(xPos, xPos + xValue, normalized), thing.localPosition.y);
                    yield return null;
                }
            }
            thing.localPosition = new Vector2(xPos + xValue, thing.localPosition.y);
            yield break;
        }
        void UpdatePos()
        {
            Vector3 lotusesPos = Lotuses.localPosition, leavesPos = Leaves.localPosition;
            
            Lotuses.localPosition = new Vector2(0, lotusesPos.y);
            float newPosX = (leavesPos.x - 3) % 10.7f + 3;
            Leaves.localPosition = new Vector2(newPosX, lotusesPos.y);
        }

        void Appear(double beat, bool frog = false)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 0.9, delegate
                {
                    isGone = false;
                    princessAnim.Play("idle", 0, 0);
                    princessAnim.Play("idle", 1, 0);
                    princessAnim.DoScaledAnimationAsync("appear", 0.5f);
                    if (frog) 
                    {
                        frogAnim.Play("idle", 0, 0);
                        frogAnim.DoScaledAnimationAsync("appear", 0.5f);
                    }
                }),
            });
        }

        public void BackgroundColorSet(double beat, float length, Color BGStart, Color BGEnd, int colorEaseSet)
        {
            bgColorEase = new(beat, length, BGStart, BGEnd, colorEaseSet);

            UpdateBackgroundColor();
        }
        private void UpdateBackgroundColor()
        {
            BGPlane.color = bgColorEase.GetColor();
        }
    }
}