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
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.TryGetMinigame(out DressYourBest instance)) {
                            DressYourBest.Characters characters = (DressYourBest.Characters)e["characters"];
                            instance.ToggleBopping(characters, e["bop"]);
                            if (e["auto"]) instance.DoBopping(e.beat, e.length, characters);
                        }
                    },
                    defaultLength = 1f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new("characters", DressYourBest.Characters.Both, "Characters", "Choose the characters to toggle bopping."),
                        new("bop", true, "Bop", "Toggle if the selected characters should bop for the duration of this event."),
                        new("auto", true, "Bop (Auto)", "Toggle if the selected characters should automatically bop until another Bop event is reached."),
                    }
                },
                new GameAction("start interval", "Start Interval")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.TryGetMinigame(out DressYourBest instance)) {
                            instance.QueueStartInterval(e.beat, e.length, e["auto"]);
                        }
                    },
                    defaultLength = 3f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new("auto", true, "Auto Pass Turn", "Toggle if the turn should be passed automatically at the end of the start interval.")
                    }
                },
                new GameAction("monkey call", "Monkey Call")
                {
                    inactiveFunction = delegate {
                        var e = eventCaller.currentEntity;
                        SoundByte.PlayOneShotGame("dressYourBest/monkey_call_" + (e["callSfx"] + 1), e.beat, forcePlay: true);
                    },
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new("callSfx", DressYourBest.CallSFX.Long, "Call SFX", "Set the type of sound effect to use for the call.")
                    }
                },
                new GameAction("pass turn", "Pass Turn")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.TryGetMinigame(out DressYourBest instance)) {
                            instance.PassTurn(e.beat);
                        }
                    },
                    defaultLength = 1f,
                    // parameters = new List<Param>()
                    // {
                    //     new("auto", true, "Auto Pass Turn", "Toggle if the turn should be passed automatically at the end of the start interval.")
                    // }
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
            Both,
        }

        public enum CallSFX
        {
            Long,
            Short,
        }

        // LightState is mainly used to get different colors from a list
        public enum LightState
        {
            IdleOrListening,
            Repeating,
            Correct,
            Wrong,
        }

        [Header("Animators")]
        [SerializeField] private Animator girlAnim;
        [SerializeField] private Animator monkeyAnim;
        [SerializeField] private Animator sewingAnim;

        [Header("Renderers")]
        [SerializeField] private SpriteRenderer bgSpriteRenderer;
        [SerializeField] private Renderer lightRenderer;

        [Header("Material(s)")]
        [SerializeField] private Material lightMaterialTemplate;

        [Header("Variables")]
        [SerializeField] private ColorPair[] lightStates;
        [Serializable] // can't serialize tuples :/
        private struct ColorPair
        {
            public Color inside;
            public Color outside;
        }

        // can't make a reference type a const, this is the next best thing
        public readonly static Color DefaultBGColor = new(0.84f, 0.58f, 0.87f);

        // i set variables to null when they are not initialized by default 👍
        private ColorEase bgColorEase = new(DefaultBGColor);
        private Material lightMaterialCurrent;
        private Sound whirringSfx = null;
        private List<RiqEntity> callEntities;

        // if characters should bop automatically
        private bool girlBop = true;
        private bool monkeyBop = true;

        private void Awake()
        {
            // instantiate the material so it doesn't persist between game switches
            lightRenderer.material = Instantiate(lightMaterialTemplate);
            SetLightFromState(LightState.IdleOrListening); // default
        }

        private void Update()
        {
            bgSpriteRenderer.color = bgColorEase.GetColor();
        }

        public override void OnLateBeatPulse(double beat)
        {
            if (girlBop && !girlAnim.IsPlayingAnimationNames()) {
                girlAnim.DoScaledAnimationAsync("Bop", 0.5f);
            }
            if (monkeyBop && !monkeyAnim.IsPlayingAnimationNames("Call")) {
                monkeyAnim.DoScaledAnimationAsync("Bop", 0.5f);
            }
        }

        // OnGameSwitch and OnPlay do very similar things, but it's better to keep them separate if they need to do different things 
        public override void OnGameSwitch(double beat)
        {
            StoreAllCallEntities();
            PersistBackgroundAppearance(beat);
        }

        public override void OnPlay(double beat)
        {
            StoreAllCallEntities();
            PersistBackgroundAppearance(beat);
        }

        private void StoreAllCallEntities()
        {
            // just makes more sense to go through like 50-100 entities max when going through the game instead of like 2000 max
            callEntities = gameManager.Beatmap.Entities.FindAll(e => e.datamodel == "dressYourBest/monkey call");
        }

        private void PersistBackgroundAppearance(double beat)
        {
            // find the last background appearance from the current beat
            // this uses only beat, not length. earlier events will be completely ignored
            RiqEntity bgEntity = gameManager.Beatmap.Entities.FindLast(e => e.beat < beat && e.datamodel == "dressYourBest/background appearance");
            if (bgEntity != null) {
                RiqEntity e = bgEntity;
                ChangeBackgroundAppearance(e.beat, e.length, e["startColor"], e["endColor"], e["ease"]);
            }
        }

        private void SetLightFromState(LightState state)
        {
            ColorPair colorPair = lightStates[(int)state];
            lightRenderer.material.SetColor("_ColorAlpha", colorPair.outside);
            lightRenderer.material.SetColor("_ColorBravo", colorPair.inside);
        }

        public void ChangeBackgroundAppearance(double beat, float length, Color startColor, Color endColor, int ease)
        {
            bgColorEase = new ColorEase(beat, length, startColor, endColor, ease);
        }

        public void ToggleBopping(Characters characters, bool toggle)
        {
            if (characters is Characters.Girl or Characters.Both) {
                girlBop = toggle;
            }
            if (characters is Characters.Monkey or Characters.Both) {
                monkeyBop = toggle;
            }
        }

        public void DoBopping(double beat, float length, Characters characters)
        {
            // not super necessary, but just creating one callback that gets added to, then assigned to a beataction is just simpler
            BeatAction.EventCallback bopAction = delegate { };
            if (characters is Characters.Girl or Characters.Both) {
                bopAction += () => girlAnim.DoScaledAnimationAsync("Bop", 0.5f);
            }
            if (characters is Characters.Monkey or Characters.Both) {
                bopAction += () => monkeyAnim.DoScaledAnimationAsync("Bop", 0.5f);
            }

            List<BeatAction.Action> actions = new();
            for (int i = 0; i < length; i++) {
                actions.Add(new(beat + i, bopAction));
            }
            _ = BeatAction.New(this, actions);
        }

        // startBeat exists so actions that happened when inactive aren't done again. that would suck
        public void QueueStartInterval(double beat, float length, bool auto, double startBeat = double.MinValue)
        {
            List<RiqEntity> neededCalls = GetNeededCalls(beat, length);
            if (neededCalls.Count <= 0) return;

            if (startBeat < beat + length) {
                List<MultiSound.Sound> sounds = new();
                List<BeatAction.Action> actions = new() { // first beat of start interval stuff
                    // might not be necessary?
                    // new(beat, delegate {
                    //     monkeyAnimator.DoScaledAnimationAsync("StartCalling", 0.5f, animLayer: 1);
                    // })
                };
                foreach (RiqEntity call in neededCalls)
                {
                    Debug.Log("call.beat : " + call.beat);
                    if (call.beat < startBeat) continue;
                    sounds.Add(new("dressYourBest/monkey_call_" + (call["callSfx"] + 1), call.beat));
                    actions.Add(new(call.beat, () => monkeyAnim.DoScaledAnimationAsync("Call", 0.5f)));
                }
                // have to add this after all the other actions as actions are done in order of beat
                if (auto) {
                    actions.Add(new(beat + length, delegate {
                        PassTurn(beat + length, beat, length, neededCalls);
                    }));
                }
                _ = MultiSound.Play(sounds);
                _ = BeatAction.New(this, actions);
            }
        }

        public void PassTurn(double beat, double startIntervalBeat = double.NaN, float startIntervalLength = float.NaN, List<RiqEntity> neededCalls = null)
        {
            if (double.IsNaN(startIntervalBeat) || double.IsNaN(startIntervalLength)) {
                RiqEntity startInterval = gameManager.Beatmap.Entities.FindLast(e => e.beat + e.length < beat);
                if (startInterval == null) return;
                startIntervalBeat = startInterval.beat;
                startIntervalLength = startInterval.length;
            }
            neededCalls ??= GetNeededCalls(startIntervalBeat, startIntervalLength);
            if (neededCalls.Count <= 0) return; // do the actual stuff under here

            SoundByte.PlayOneShotGame("dressYourBest/pass_turn");
            // "Any" check instead of just checking the last one?
            if (neededCalls[^1].beat != beat) {
                // might wanna check if if a bop is already playing, too?
                // down to visual preference.
                monkeyAnim.DoScaledAnimationAsync("Idle", 0.5f);
            }
            hitCount = 0;
            foreach (RiqEntity call in neededCalls)
            {
                double relativeBeat = call.beat - startIntervalBeat;
                _ = ScheduleInput(beat, relativeBeat + 1, InputAction_BasicPress, OnHit, OnMiss, null);
            }
        }


        private List<RiqEntity> GetNeededCalls(double beat, float length)
        {
            return callEntities.FindAll(e => e.beat >= beat && e.beat <= beat + length);
        }

        private int hitCount = 0; // resets every pass turn
        private void OnHit(PlayerActionEvent caller, float state)
        {
            SoundByte.PlayOneShotGame("dressYourBest/hit_1");
            SoundByte.PlayOneShotGame("dressYourBest/hit_2", pitch: SoundByte.GetPitchFromSemiTones(hitCount, false));
            sewingAnim.DoScaledAnimationAsync("Hit", 0.5f);
            hitCount++;
        }
        private void OnMiss(PlayerActionEvent caller)
        {
            
        }
    }
}