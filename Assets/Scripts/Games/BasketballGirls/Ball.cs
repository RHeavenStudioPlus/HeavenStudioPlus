using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_BasketballGirls
{
    public class Ball : MonoBehaviour
    {
        public double startBeat;
        [SerializeField] private Animator ballAnim;

        private BasketballGirls game;

        public void Init()
        {
            game = BasketballGirls.instance;
            ballAnim.DoScaledAnimationAsync("prepare", 0.5f);
            game.ScheduleInput(startBeat, 1, BasketballGirls.InputAction_Catch, JustCatch, MissCatch, Empty);
        }

        void JustCatch(PlayerActionEvent caller, float state)
        {
            game.girlRightNoBopIntervals.Add(new Interval(startBeat + 1, startBeat + 2));
            
            if (state <= -1f || state >= 1f)
            {
                SoundByte.PlayOneShotGame("basketballGirls/catch");
                MultiSound.Play(
                    new MultiSound.Sound[] {
                        new MultiSound.Sound("basketballGirls/throw", startBeat + 1.5),
                        new MultiSound.Sound("basketballGirls/6", startBeat + 2),
                    }
                );

                ballAnim.DoScaledAnimationAsync("catch", 0.5f);
                game.girlLeftAnim.DoScaledAnimationAsync("pass", 0.5f);
                game.girlRightAnim.DoScaledAnimationAsync("catch", 0.5f);
                BeatAction.New(this, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(startBeat + 1.5, delegate {
                        ballAnim.DoScaledAnimationAsync("shootBarely", 0.5f);
                        game.girlRightAnim.DoScaledAnimationAsync("shoot", 0.5f);
                    }),
                    new BeatAction.Action(startBeat + 2, delegate {
                        ballAnim.DoScaledAnimationAsync("barely", 0.5f);
                        game.goalAnim.DoScaledAnimationAsync("barely", 0.5f);
                    }),
                    new BeatAction.Action(startBeat + 3, delegate {
                        Destroy(gameObject);
                    }),
                });
                return;
            }
            SoundByte.PlayOneShotGame("basketballGirls/catch");
            MultiSound.Play(
                new MultiSound.Sound[] {
                    new MultiSound.Sound("basketballGirls/throw", startBeat + 1.5),
                    new MultiSound.Sound("basketballGirls/dunk", startBeat + 2),
                    new MultiSound.Sound("basketballGirls/ok1", startBeat + 2.5),
                    new MultiSound.Sound("basketballGirls/ok2", startBeat + 3),
                }
            );

            ballAnim.DoScaledAnimationAsync("catch", 0.5f);
            if (!game.girlLeftNoBopIntervals.Any(x => x.Start == (startBeat + 1))) game.girlLeftAnim.DoScaledAnimationAsync("pass", 0.5f);
            game.girlRightAnim.DoScaledAnimationAsync("catch", 0.5f);
            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + 1.5, delegate {
                    ballAnim.DoScaledAnimationAsync("shootJust", 0.5f);
                    game.girlRightAnim.DoScaledAnimationAsync("shoot", 0.5f);
                }),
                new BeatAction.Action(startBeat + 2, delegate {
                    ballAnim.DoScaledAnimationAsync("just", 0.5f);
                    game.goalAnim.DoScaledAnimationAsync("just", 0.5f);
                }),
                new BeatAction.Action(startBeat + 3, delegate {
                    Destroy(gameObject);
                }),
            });
        }

        void MissCatch(PlayerActionEvent caller)
        {
            SoundByte.PlayOneShotGame("basketballGirls/" + UnityEngine.Random.Range(1,2).ToString());
            ballAnim.DoScaledAnimationAsync("hit", 0.5f);
            game.girlLeftAnim.DoScaledAnimationAsync("shock", 0.5f);
            game.girlRightAnim.DoScaledAnimationAsync("hit", 0.5f);
            game.girlRightNoBopIntervals.Add(new Interval(startBeat + 1, startBeat + 2));
            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(startBeat + 3, delegate {
                    Destroy(gameObject);
                }),
            });
        }

        void Empty(PlayerActionEvent caller) { }
    }
}
