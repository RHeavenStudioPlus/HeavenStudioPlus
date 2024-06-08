using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class PcoDressYourBestLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("dressYourBest", "Dress Your Best", "d593dd", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new("characters", DressYourBest.Characters.Girl, "Bop", "Toggle if the selected characters should bop for the duration of this event."),
                        new("bop", true, "Bop", "Toggle if the selected characters should bop for the duration of this event."),
                        new("auto", true, "Bop (Auto)", "Toggle if the selected characters should automatically bop until another Bop event is reached."),
                    }
                },
                new GameAction("start interval", "Start Interval")
                {
                    defaultLength = 4f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new("auto", true, "Auto Pass Turn", "Toggle if the turn should be passed automatically at the end of the start interval.")
                    }
                },
                new GameAction("monkey call", "Monkey Call")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        SoundByte.PlayOneShotGame("dressYourBest/monkey_call_" + e["callSfx"], e.beat, forcePlay: true);
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new("callSfx", DressYourBest.CallSFX.Long, "Call SFX", "Set the type of sound effect to use for the call.")
                    }
                },
                new GameAction("background appearance", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.TryGetMinigame(out DressYourBest instance)) {
                            instance.ChangeBackgroundAppearance(e.beat, e.length, e["start"], e["end"], e["ease"]);
                        }
                    },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new("start", DressYourBest.DefaultBGColor, "Start Color", "Set the color at the start of the event."),
                        new("end", DressYourBest.DefaultBGColor, "End Color", "Set the color at the end of the event."),
                        new("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    },
                },
            }
            );
        }
    }
}

namespace HeavenStudio.Games
{
    public class DressYourBest : Minigame
    {
        public enum Characters
        {
            Girl,
            Monkey,
        }

        public enum CallSFX
        {
            Long,
            Short,
        }

        public enum LightState
        {
            IdleOrListening,
            Repeating,
            Correct,
            Wrong,
        }

        [Header("Animators")]
        [SerializeField] private Animator girlAnimator;
        [SerializeField] private Animator monkeyAnimator;

        [Header("Renderers")]
        [SerializeField] private SpriteRenderer bgSpriteRenderer;
        [SerializeField] private Renderer lightRenderer;

        [Header("Material(s)")]
        [SerializeField] private Material lightMaterialTemplate;

        [Header("Variables")]
        [SerializeField] private ColorPair[] lightStates;
        [Serializable]
        private struct ColorPair
        {
            public Color inside;
            public Color outside;
        }

        public readonly static Color DefaultBGColor = new(0.84f, 0.58f, 0.87f);

        private ColorEase bgColorEase = new(DefaultBGColor);
        private Material lightMaterialCurrent;
        private Sound whirringSfx = null;

        private bool girlBop = true;
        private bool monkeyBop = true;


        private void Awake()
        {
            // lightMaterialCurrent = Instantiate(lightMaterialTemplate);
            lightRenderer.material = Instantiate(lightMaterialTemplate);
        }

        private void Update()
        {
            bgSpriteRenderer.color = bgColorEase.GetColor();
        }

        public override void OnLateBeatPulse(double beat)
        {
            if (girlBop && !girlAnimator.IsPlayingAnimationNames()) {
                girlAnimator.DoScaledAnimationAsync("Bop", 0.5f);
            }
            if (monkeyBop && !monkeyAnimator.IsPlayingAnimationNames()) {
                monkeyAnimator.DoScaledAnimationAsync("Bop", 0.5f);
            }
        }

        public override void OnGameSwitch(double beat)
        {
            PersistBackgroundAppearance(beat);
        }

        public override void OnPlay(double beat)
        {
            PersistBackgroundAppearance(beat);
        }

        private void PersistBackgroundAppearance(double beat)
        {
            RiqEntity bgEntity = gameManager.Beatmap.Entities.FindLast(e => e.beat < beat && e.datamodel == "dressYourBest/background appearance");
            if (bgEntity != null) {
                RiqEntity e = bgEntity;
                ChangeBackgroundAppearance(e.beat, e.length, e["startColor"], e["endColor"], e["ease"]);
            }
        }

        public void ChangeBackgroundAppearance(double beat, float length, Color startColor, Color endColor, int ease)
        {
            bgColorEase = new ColorEase(beat, length, startColor, endColor, ease);
        }
    }
}