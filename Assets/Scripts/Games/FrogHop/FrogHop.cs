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
    public static class NtrFrogHopLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("frogHop", "Frog Hop", "195A23", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Bop(e.beat, e.length, e["blue"], e["orange"], e["greens"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("blue", true, "Blue Bops", "Make Blue Frog bop during this event."),
                        new Param("orange", true, "Orange Bops", "Make Orange Frog bop during this event."),
                        new Param("greens", true, "Group Bops", "Make the frogs in the back bop during this event."),
                    },
                    resizable = true,
                },
                new GameAction("count", "Count In")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Count(e.beat, e["start"]);
                        }
                    },
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        FrogHop.CountVox(e.beat);
                    },
                    preFunctionLength = 0,
                    parameters = new List<Param>()
                    {
                        new Param("start", true, "Start Shaking", "Start shaking after the count in."),
                    },
                    defaultLength = 4.0f,
                },
                new GameAction("hop", "Start Shaking")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Hop(e.beat);
                        }
                    },
                    preFunctionLength = 1,
                },
                new GameAction("stop", "Stop Shaking")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Stop(e.beat);
                        }
                    },
                },
                new GameAction("twoshake", "Ya-hoo!")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.TwoHop(e.beat, e["spotlights"], e["jazz"]);
                        }
                    },
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        FrogHop.TwoHopVox(e.beat, e["enabled"]);
                    },
                    preFunctionLength = 0,
                    parameters = new List<Param>()
                    {
                        new Param("enabled", true, "Cue Sound", "Choose whether to play the cue sound for this event."),
                        new Param("spotlights", true, "Automatic Spotlights", "Handles spotlight switching automatically."),
                        new Param("jazz", false, "Jumpin' Jazz", "Mouth animations will be based on Frog Hop 2."),
                    },
                    defaultLength = 4.0f,
                },
                new GameAction("threeshake", "Yeah yeah yeah!")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.ThreeHop(e.beat, e["spotlights"], e["jazz"]);
                        }
                    },
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        FrogHop.ThreeHopVox(e.beat, e["enabled"]);
                    },
                    preFunctionLength = 0,
                    parameters = new List<Param>()
                    {
                        new Param("enabled", true, "Cue Sound", "Choose whether to play the cue sound for this event."),
                        new Param("spotlights", true, "Automatic Spotlights", "Handles spotlight switching automatically."),
                        new Param("jazz", false, "Jumpin' Jazz", "Mouth animations will be based on Frog Hop 2."),
                    },
                    defaultLength = 4.0f,
                },
                new GameAction("spin", "Spin it Boys!")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.SpinItBoys(e.beat, e["spotlights"], e["jazz"]);
                        }
                    },
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        FrogHop.SpinItBoysVox(e.beat, e["enabled"]);
                    },
                    preFunctionLength = 0,
                    parameters = new List<Param>()
                    {
                        new Param("enabled", true, "Cue Sound", "Choose whether to play the cue sound for this event."),
                        new Param("spotlights", true, "Automatic Spotlights", "Handles spotlight switching automatically."),
                        new Param("jazz", false, "Jumpin' Jazz", "Mouth animations will be based on Frog Hop 2."),
                    },
                    defaultLength = 4.0f,
                },
                new GameAction("thankyou", "Thank you... verrry much-a!")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.ThankYou(e.beat, e["pitched"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("pitched", false, "Enable Pitching", "Makes the frog voices pitch up and down based on the song's tempo."),
                    },
                    defaultLength = 6.0f,
                },
                new GameAction("mouthwide", "Open Mouth (Wide)")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Sing("Wide", e.beat + e.length - 0.5, e["blue"], e["orange"], e["greens"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("blue", true, "Blue Sings", "Make Blue Frog sing during this event."),
                        new Param("orange", false, "Orange Sings", "Make Orange Frog sing during this event."),
                        new Param("greens", false, "Group Sings", "Make the frogs in the back sing during this event."),
                    },
                    defaultLength = 0.5f,
                    resizable = true,
                },
                new GameAction("mouthnarrow", "Open Mouth (Narrow)")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Sing("Narrow", e.beat + e.length - 0.5, e["blue"], e["orange"], e["greens"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("blue", true, "Blue Sings", "Make Blue Frog sing during this event."),
                        new Param("orange", false, "Orange Sings", "Make Orange Frog sing during this event."),
                        new Param("greens", false, "Group Sings", "Make the frogs in the back sing during this event."),
                    },
                    defaultLength = 0.5f,
                    resizable = true,
                },
                new GameAction("spotlights", "Spotlights")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Spotlights(e["front"], e["back"], e["dark"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("front", true, "Front Lights", "Enables the spotlights on the front frogs."),
                        new Param("back", false, "Back Lights", "Enables the spotlights on the back frogs."),
                        new Param("dark", true, "Darken Stage", "Darkens the stage, allowing the spotlights to be seen."),
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("force", "Force Hop")
                {
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.ForceHop(e.beat, e.length, e["front"], e["back"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("front", true, "Front Frogs", "Make the frogs in the front sing during this event."),
                        new Param("back", true, "Back Frogs", "Make the frogs in the back sing during this event."),
                    },
                    resizable = true,
                    defaultLength = 4.0f,
                },
                new GameAction("pitching", "Enable Pitched Voices")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Pitching(e["enabled"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("enabled", true, "Enable Pitching", "Makes the frog voices pitch up and down based on the song's tempo."),
                    },
                    defaultLength = 0.5f,
                },
            }
            );
        }
    }
}

namespace HeavenStudio.Games
{
    using HeavenStudio.Games.Loaders;
    using Scripts_FrogHop;
    public class FrogHop : Minigame
    {
        //definitions
        #region Definitions

        [SerializeField] public ntrFrog PlayerFrog;
        [SerializeField] public List<ntrFrog> OtherFrogs = new List<ntrFrog>();
        [SerializeField] public ntrFrog LeaderFrog;
        [SerializeField] public ntrFrog SingerFrog;
        [SerializeField] public GameObject Darkness;
        [SerializeField] public GameObject SpotlightFront;
        [SerializeField] public GameObject SpotlightBack;
        [SerializeField] public SpriteRenderer Mike;
        List<ntrFrog> AllFrogs = new();
        List<ntrFrog> FrontFrogs = new();
        List<ntrFrog> BackFrogs = new();
        List<ntrFrog> whoToInputKTB = new();

        int globalAnimSide = -1;

        double wantHop = double.MinValue;
        List<double> queuedHops = new();
        bool keepHopping;
        double startBackHop = double.MinValue;
        double startNoHop = double.MinValue;
        double startRegularHop = double.MinValue;

        static float globalPitch = 1;
        bool usesGlobalePitch = false; //oops i spelled global wrong lmao

        const int IAAltDownCat = IAMAXCAT;
        const int IAAltUpCat = IAMAXCAT + 1;

        protected static bool IA_PadAltPress(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.South, out dt);
        }
        protected static bool IA_BatonAltPress(out double dt)
        {
            return PlayerInput.GetSqueezeDown(out dt);
        }
        
        protected static bool IA_PadAltRelease(out double dt)
        {
            return PlayerInput.GetPadUp(InputController.ActionsPad.South, out dt);
        }
        protected static bool IA_BatonAltRelease(out double dt)
        {
            return PlayerInput.GetSqueezeUp(out dt);
        }

        public static PlayerInput.InputAction InputAction_AltPress =
            new("NtrFrogHopAltPress", new int[] { IAAltDownCat, IAAltDownCat, IAAltDownCat },
            IA_PadAltPress, IA_TouchBasicPress, IA_BatonAltPress);
        public static PlayerInput.InputAction InputAction_AltRelease =
            new("NtrFrogHopAltRelease", new int[] { IAAltUpCat, IAFlickCat, IAAltUpCat },
            IA_PadAltRelease, IA_TouchFlick, IA_BatonAltRelease);
        public static PlayerInput.InputAction InputAction_TouchRelease =
            new("NtrFrogHopTouchRelease", new int[] { IAEmptyCat, IAReleaseCat, IAEmptyCat },
            IA_Empty, IA_TouchBasicRelease, IA_Empty);

        #endregion

        //global methods
        #region Global Methods

        public void Awake()
        {
            globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;

            PersistThings(Conductor.instance.songPositionInBeatsAsDouble);

            AllFrogs.Add(PlayerFrog);
            AllFrogs.AddRange(OtherFrogs);
            AllFrogs.Add(LeaderFrog);
            AllFrogs.Add(SingerFrog);

            FrontFrogs.Add(LeaderFrog);
            FrontFrogs.Add(SingerFrog);

            BackFrogs.Add(PlayerFrog);
            BackFrogs.AddRange(OtherFrogs);

            whoToInputKTB = AllFrogs;
        }

        public override void OnGameSwitch(double beat)
        {
            foreach (var entity in GameManager.instance.Beatmap.Entities)
            {
                if (entity.beat >= beat && entity.beat <= beat + 1)
                {
                    if (entity.datamodel == "frogHop/hop")
                    Hop(entity.beat);
                    continue;
                }

                if (entity.beat >= beat || entity.beat < beat - 4) continue;

                if (entity.datamodel == "frogHop/count")
                {
                    var e = entity;
                    Count(e.beat, e["start"]);
                    continue;
                }

                if (entity.beat < beat - 2) continue;

                switch (entity.datamodel)
                {
                    case "frogHop/twoshake":
                    {
                        var e = entity;
                        TwoHop(e.beat, e["spotlights"], e["jazz"], beat - e.beat);
                        continue;
                    }
                    case "frogHop/threeshake":
                    {
                        var e = entity;
                        ThreeHop(e.beat, e["spotlights"], e["jazz"], beat - e.beat);
                        continue;
                    }
                    case "frogHop/spinitboys":
                    {
                        var e = entity;
                        SpinItBoys(e.beat, e["spotlights"], e["jazz"], beat - e.beat);
                        continue;
                    }
                }
            }
        }

        public void Update()
        {
            //voice pitch stuff below

            globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;

            //whiff stuff below

            if (PlayerInput.GetIsAction(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_BasicPress))
            {
                if (PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch || !IsExpectingInputNow(InputAction_AltPress))
                {
                    PlayerFrog.Hop();
                    SoundByte.PlayOneShot("miss");
                    LightMiss(true);
                }
            }

            if (PlayerInput.GetIsAction(InputAction_AltPress) && !IsExpectingInputNow(InputAction_AltPress) && PlayerInput.CurrentControlStyle != InputController.ControlStyles.Touch)
            {
                PlayerFrog.Charge();
                SoundByte.PlayOneShot("miss");
                LightMiss(true);
            }

            if (PlayerInput.GetIsAction(InputAction_AltRelease) && !IsExpectingInputNow(InputAction_AltRelease))
            {
                PlayerFrog.Spin();
                SoundByte.PlayOneShotGame("frogHop/sigh", volume: 1.5f);
                LightMiss(true);
            }
        }

        public void LateUpdate()
        {
            //ktb stuff below

            if (wantHop != double.MinValue)
            {
                queuedHops.Add(wantHop);
                keepHopping = true;
                wantHop = double.MinValue;
            }

            if (Conductor.instance.isPlaying && !Conductor.instance.isPaused)
            {
                if (queuedHops.Count > 0)
                {
                    foreach (var hop in queuedHops)
                    {
                        var actions = new List<BeatAction.Action>();

                        bool betweenHopValues = hop + 1 < startRegularHop && hop + 1 >= startNoHop;
                        if (!betweenHopValues) ScheduleInput(hop, 1, InputAction_BasicPress, PlayerHopNormal, PlayerMiss, Nothing);

                        betweenHopValues = hop + 1 < startRegularHop && hop + 1 >= startNoHop;
                        if (!betweenHopValues) actions.Add(new BeatAction.Action(hop + 1, delegate { NPCHop(BackFrogs); }));

                        betweenHopValues = hop + 1 < startRegularHop && hop + 1 >= startBackHop;
                        if (!betweenHopValues) actions.Add(new BeatAction.Action(hop + 1, delegate { 
                            betweenHopValues = hop + 1 < startRegularHop && hop + 1 >= startBackHop;
                            if (!betweenHopValues) { NPCHop(FrontFrogs); SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_E_BEAT"); }
                        }));

                        if (keepHopping) actions.Add(new BeatAction.Action(hop, delegate { queuedHops.Add(hop + 1); }));

                        BeatAction.New(this, actions);
                    }
                    queuedHops.Clear();
                }
            }
        }

        #endregion

        //frog hop methods
        #region Frog Hop Methods

        public void Bop(double beat, float length, bool blue, bool orange, bool greens)
        {
            var FrogsToBop = new List<ntrFrog>();

            if (blue) FrogsToBop.Add(SingerFrog);
            if (orange) FrogsToBop.Add(LeaderFrog);
            if (greens) FrogsToBop.AddRange(BackFrogs);

            var actions = new List<BeatAction.Action>();
            
            for (int i = 0; i < length; i++)
            { 
                actions.Add(new(beat + i, delegate { BopAnimation(FrogsToBop); }));
            }

            BeatAction.New(this, actions);
        }

        public void BopAnimation(List<ntrFrog> FrogsToBop)
        {
            foreach (var a in FrogsToBop) { a.Bop(); }
        }

        public void Count(double beat, bool start)
        {
            var actions = new List<BeatAction.Action>();

            actions.Add(new(beat + 0.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
            actions.Add(new(beat + 1.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
            actions.Add(new(beat + 2.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
            actions.Add(new(beat + 3.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));

            BeatAction.New(this, actions);

            if (start) Hop(beat + 4.0);
        }

        public static void CountVox(double beat)
        {
            globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;
            bool usesGlobalePitch = GetPitched(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT1", beat + 0.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT2", beat + 1.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT3", beat + 2.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT4", beat + 3.0, usesGlobalePitch ? globalPitch : 1));

            MultiSound.Play(sounds, forcePlay: true);
        }

        public void Hop (double beat)
        {
            wantHop = beat - 1;
        }

        public void Stop (double beat)
        {
            keepHopping = false;
        }

        public void Pitching(bool enabled)
        {
            usesGlobalePitch = enabled;
        }

        public void ForceHop(double beat, double length, bool front, bool back)
        {
            var actions = new List<BeatAction.Action>();

            for (int i = 0; i < length; i++)
            {
                if (front)
                {
                    actions.Add(new BeatAction.Action(beat + i, delegate { NPCHop(FrontFrogs); }));
                }
                if (back)
                {
                    actions.Add(new BeatAction.Action(beat + i, delegate { 
                        NPCHop(BackFrogs); 
                        SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_E_BEAT");
                    }));
                    ScheduleInput(beat - 1, i + 1, InputAction_BasicPress, PlayerHopNormal, PlayerMiss, Nothing);
                }
            }

            BeatAction.New(this, actions);
        }

        public void TwoHop (double beat, bool spotlights, bool jumpinJazz, double start = 0)
        {
            CueCommon(beat, spotlights);

            var actions = new List<BeatAction.Action>();
            var sounds = new List<MultiSound.Sound>();

            //call
            if (start <= 0.0) actions.Add(new(beat + 0.0, delegate { NPCHop(FrontFrogs); Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
            if (start <= 0.5) actions.Add(new(beat + 0.5, delegate { NPCHop(FrontFrogs, true); Talk(new List<ntrFrog>() { LeaderFrog }, "Narrow", jumpinJazz ? beat + 2.5 : beat + 1.5); }));

            //response
            actions.Add(new(beat + 2.0, delegate { NPCHop(BackFrogs); Talk(BackFrogs, "Wide", beat); }));
            actions.Add(new(beat + 2.5, delegate { NPCHop(BackFrogs, true); Talk(BackFrogs, "Narrow", jumpinJazz ? beat + 4.5 : beat + 3.5); }));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_HA", beat + 2.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_HAAI", beat + 2.5, usesGlobalePitch ? globalPitch : 1));

            //inputs
            ScheduleInput(beat, 2.0, InputAction_BasicPress, PlayerHopYa, PlayerMiss, Nothing);
            ScheduleInput(beat, 2.5, InputAction_BasicPress, PlayerHopHoo, PlayerMiss, Nothing);

            BeatAction.New(this, actions);
            MultiSound.Play(sounds);
        }

        public static void TwoHopVox(double beat, bool enabled)
        {
            if (!enabled) return;
            globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;
            bool usesGlobalePitch = GetPitched(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HA", beat, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_DEFAULT", beat));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAAI", beat + 0.5, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_HAAI", beat + 0.5));

            MultiSound.Play(sounds, forcePlay: true);
        }

        public void ThreeHop (double beat, bool spotlights, bool jumpinJazz, double start = 0)
        {
            CueCommon(beat, spotlights);

            var actions = new List<BeatAction.Action>();
            var sounds = new List<MultiSound.Sound>();

            //call
            if (start <= 0.0) actions.Add(new(beat + 0.0, delegate { NPCHop(FrontFrogs); Talk(new List<ntrFrog>() { LeaderFrog }, "Narrow", jumpinJazz ? beat + 2.5 : beat); }));
            if (start <= 0.5) actions.Add(new(beat + 0.5, delegate { NPCHop(FrontFrogs); if (!jumpinJazz) Talk(new List<ntrFrog>() { LeaderFrog }, "Narrow", beat); }));
            if (start <= 1.0) actions.Add(new(beat + 1.0, delegate { NPCHop(FrontFrogs, true); if (!jumpinJazz) Talk(new List<ntrFrog>() { LeaderFrog }, "Narrow", beat); }));

            //response
            actions.Add(new(beat + 2.0, delegate { NPCHop(BackFrogs); Talk(BackFrogs, "Narrow", jumpinJazz ? beat + 4.5 : beat); }));
            actions.Add(new(beat + 2.5, delegate { NPCHop(BackFrogs); if (!jumpinJazz) Talk(BackFrogs, "Narrow", beat); }));
            actions.Add(new(beat + 3.0, delegate { NPCHop(BackFrogs, true); if (!jumpinJazz) Talk(BackFrogs, "Narrow", beat); }));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_HAI", beat + 2.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_HAI", beat + 2.5, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_HAI", beat + 3.0, usesGlobalePitch ? globalPitch : 1));

            //inputs
            ScheduleInput(beat, 2.0, InputAction_BasicPress, PlayerHopYeah, PlayerMiss, Nothing);
            ScheduleInput(beat, 2.5, InputAction_BasicPress, PlayerHopYeah, PlayerMiss, Nothing);
            ScheduleInput(beat, 3.0, InputAction_BasicPress, PlayerHopYeahAccent, PlayerMiss, Nothing);

            BeatAction.New(this, actions);
            MultiSound.Play(sounds);
        }

        public static void ThreeHopVox(double beat, bool enabled)
        {
            if (!enabled) return;
            globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;
            bool usesGlobalePitch = GetPitched(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAI", beat, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_DEFAULT", beat));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAI", beat + 0.5, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_DEFAULT", beat + 0.5));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAI", beat + 1.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_DEFAULT", beat + 1.0));

            MultiSound.Play(sounds, forcePlay: true);
        }

        public void SpinItBoys (double beat, bool spotlights, bool jumpinJazz, double start = 0)
        {
            CueCommon(beat, spotlights);

            var actions = new List<BeatAction.Action>();
            var sounds = new List<MultiSound.Sound>();

            //call
            if (start <= 0.0) actions.Add(new(beat + 0.0, delegate { NPCCharge(FrontFrogs); Talk(new List<ntrFrog>() { LeaderFrog }, "Narrow", beat); }));
            if (start <= 1.0) actions.Add(new(beat + 1.0, delegate { NPCSpin(FrontFrogs); Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));

            //response
            actions.Add(new(beat + 2.0, delegate { NPCCharge(BackFrogs); Talk(BackFrogs, "Narrow", jumpinJazz ? beat + 3.0 : beat); }));
            actions.Add(new(beat + 3.0, delegate { NPCSpin(BackFrogs); Talk(BackFrogs, "Wide", beat); }));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_KURU_1", beat + 2.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_KURU_2", beat + 2.5, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_LIN", beat + 3.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_E_SPIN", beat + 3.0));

            //inputs
            ScheduleInput(beat, 2.0, InputAction_AltPress, PlayerHopCharge, PlayerMiss, Nothing);
            ScheduleInput(beat, 3.0, InputAction_AltRelease, PlayerSpin, PlayerMissNoFlip, Nothing);

            BeatAction.New(this, actions);
            MultiSound.Play(sounds);
        }

        public static void SpinItBoysVox(double beat, bool enabled)
        {
            if (!enabled) return;
            globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;
            bool usesGlobalePitch = GetPitched(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_KURU_1", beat, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_KURU_2", beat + 0.5, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_LIN", beat + 1.0, usesGlobalePitch ? globalPitch : 1));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_SPIN", beat + 1.0));

            MultiSound.Play(sounds, forcePlay: true);
        }

        public void CueCommon(double beat, bool spotlights = true)
        {
            startBackHop = beat;
            startNoHop = beat + 2;
            startRegularHop = beat + 4;

            if (!spotlights) return;

            var actions = new List<BeatAction.Action>();

            actions.Add(new(beat + 1.5, delegate { Spotlights(false, true); }));
            actions.Add(new(beat + 3.5, delegate { Spotlights(true, false); }));

            BeatAction.New(this, actions);
        }

        public void Spotlights(bool front, bool back, bool dark = true)
        {
            foreach (var a in FrontFrogs) { a.Darken(front || !dark); }

            if (front || !dark) Mike.color = new Color(1, 1, 1, 1);
            else Mike.color = new Color(0.5f, 0.5f, 0.5f, 1);

            Darkness.SetActive(dark);
            SpotlightFront.SetActive(front);
            SpotlightBack.SetActive(back);
        }

        public void ThankYou(double beat, bool stretchToTempo)
        {
            float pitch = stretchToTempo ? globalPitch * Conductor.instance.TimelinePitch : 1;
            double offset = stretchToTempo ? (.2 / ((Conductor.instance.GetBpmAtBeat(beat) * Conductor.instance.TimelinePitch) / 156)) : .2;

            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("frogHop/tyvm", beat, pitch: pitch, offset: offset),
            });

            var actions = new List<BeatAction.Action>();
            var BlueFrog = new List<ntrFrog>() { SingerFrog };
            double stretch = stretchToTempo ? 1 : 1 / (globalPitch * Conductor.instance.TimelinePitch);

            actions.Add(new(beat, delegate { BopAnimation(BlueFrog); }));

            actions.Add(new(beat + (0.00 / stretch), delegate { Talk(BlueFrog, "Narrow", beat); })); //"thank"
            actions.Add(new(beat + (0.50 / stretch), delegate { Talk(BlueFrog, "Narrow", beat); })); //"you"
            actions.Add(new(beat + (2.00 / stretch), delegate { Talk(BlueFrog, "Wide", beat + (4.00 / stretch)); })); //"verrry"
            actions.Add(new(beat + (4.50 / stretch), delegate { Talk(BlueFrog, "Narrow", beat); })); //"much"
            actions.Add(new(beat + (5.50 / stretch), delegate { Talk(BlueFrog, "Narrow", beat); })); //"-a!"

            BeatAction.New(this, actions);

            Debug.Log(offset);
        }

        public void Talk(List<ntrFrog> FrogsToTalk, string syllable, double animEnd)
        {
            foreach (var a in FrogsToTalk) { a.Talk(syllable, animEnd); }
        }

        public void Sing(string syllable, double animEnd, bool blue, bool orange, bool greens)
        {
            var FrogsToTalk = new List<ntrFrog>();

            if (blue) FrogsToTalk.Add(SingerFrog);
            if (orange) FrogsToTalk.Add(LeaderFrog);
            if (greens) FrogsToTalk.AddRange(BackFrogs);

            Talk(FrogsToTalk, syllable, animEnd);
        }

        public void NPCHop(List<ntrFrog> FrogsToHop, bool isThisLong = false)
        {
            foreach (var a in FrogsToHop) { if (a != PlayerFrog) a.Hop(isLong: isThisLong); }
        }

        public void NPCCharge(List<ntrFrog> FrogsToHop)
        {
            foreach (var a in FrogsToHop) { if (a != PlayerFrog) a.Charge(); }
        }

        public void NPCSpin(List<ntrFrog> FrogsToHop)
        {
            foreach (var a in FrogsToHop) { if (a != PlayerFrog) a.Spin(); }
        }

        public void PlayerHopNormal(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f) { SoundByte.PlayOneShotGame("frogHop/miss2", volume: 1.5f); LightMiss(sweat: true); }
            else SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_P_BEAT");
            PlayerHop();
        }

        public void PlayerHopYa(PlayerActionEvent caller, float state)
        {
            SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_P_HA", pitch: usesGlobalePitch ? globalPitch : 1);
            if (state >= 1f || state <= -1f) { SoundByte.PlayOneShotGame("frogHop/miss2", volume: 1.5f); LightMiss(sweat: true); }
            else SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_POP_DEFAULT");
            PlayerHop();
        }

        public void PlayerHopHoo(PlayerActionEvent caller, float state)
        {
            SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_P_HAAI", pitch: usesGlobalePitch ? globalPitch : 1);
            if (state >= 1f || state <= -1f) { SoundByte.PlayOneShotGame("frogHop/miss2", volume: 1.5f); LightMiss(sweat: true); }
            else SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_POP_HAAI");
            PlayerHop(true);
        }

        public void PlayerHopYeah(PlayerActionEvent caller, float state)
        {
            SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_P_HAI", pitch: usesGlobalePitch ? globalPitch : 1);
            if (state >= 1f || state <= -1f) { SoundByte.PlayOneShotGame("frogHop/miss2", volume: 1.5f); LightMiss(sweat: true); }
            else SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_POP_DEFAULT");
            PlayerHop();
        }

        public void PlayerHopYeahAccent(PlayerActionEvent caller, float state)
        {
            SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_P_HAI", pitch: usesGlobalePitch ? globalPitch : 1);
            if (state >= 1f || state <= -1f) { SoundByte.PlayOneShotGame("frogHop/miss2", volume: 1.5f); LightMiss(sweat: true); }
            else SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_POP_DEFAULT");
            PlayerHop(true);
        }

        public void PlayerHop(bool isLong = false)
        {
            globalAnimSide *= -1;
            PlayerFrog.Hop(globalAnimSide, isLong);
        }

        public void PlayerHopCharge(PlayerActionEvent caller, float state)
        {
            double beat = caller.startBeat + caller.timer;

            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_P_KURU_1", beat, usesGlobalePitch ? globalPitch : 1),
                new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_P_KURU_2", beat + 0.5, usesGlobalePitch ? globalPitch : 1)
            });

            if (state >= 1f || state <= -1f) { SoundByte.PlayOneShotGame("frogHop/miss2", volume: 1.5f); LightMiss(sweat: true); }
            globalAnimSide *= -1;
            PlayerFrog.Charge(globalAnimSide);
        }

        public void PlayerSpin(PlayerActionEvent caller, float state)
        {
            SoundByte.PlayOneShotGame("frogHop/SE_NTR_FROG_EN_P_LIN", pitch: usesGlobalePitch ? globalPitch : 1);
            if (state >= 1f || state <= -1f) { SoundByte.PlayOneShotGame("frogHop/miss2", volume: 1.5f); LightMiss(); }
            PlayerFrog.Spin();
        }

        public void PlayerMiss(PlayerActionEvent caller)
        {
            globalAnimSide *= -1;
            LightMiss();

            if (globalAnimSide > 0) PlayerFrog.Bump();
        }

        public void PlayerMissNoFlip(PlayerActionEvent caller)
        {
            LightMiss();
            PlayerFrog.Bump();
        }

        public void LightMiss(bool whiff = false, bool sweat = false)
        {
            if (whiff) ScoreMiss(0.5f);
            if (sweat) PlayerFrog.Sweat();
            else { foreach (var a in OtherFrogs) { a.Glare(); } }
        }

        public void Nothing(PlayerActionEvent caller) { }

        private void PersistThings(double beat)
        {
            var allEvents = GameManager.instance.Beatmap.Entities.FindAll(e => e.datamodel.Split('/')[0] is "frogHop");
            var eventsBefore = allEvents.FindAll(e => e.beat < beat);

            var lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/pitching");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                Pitching(e["enabled"]);
            }
        }

        public static bool GetPitched(double beat)
        {
            var allEvents = GameManager.instance.Beatmap.Entities.FindAll(e => e.datamodel.Split('/')[0] is "frogHop");
            var eventsBefore = allEvents.FindAll(e => e.beat < beat);

            bool isPitched = false;

            var lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/pitching");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                isPitched = e["enabled"];
            }

            return isPitched;
        }

        #endregion
    }
}