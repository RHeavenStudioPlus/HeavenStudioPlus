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
                            instance.Count(e.beat, e["start"], e["leader"], e["backup"]);
                        }
                    },
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        FrogHop.CountVox(e.beat, e["leader"], e["backup"]);
                    },
                    preFunctionLength = 0,
                    parameters = new List<Param>()
                    {
                        new Param("start", true, "Start Shaking", "Start shaking after the count in."),
                        new Param("leader", true, "Orange Frog Counts", "Make Orange Frog count during this event."),
                        new Param("backup", false, "Group Counts", "Make the frogs in the back count during this event."),
                    },
                    defaultLength = 4.0f,
                },
                new GameAction("countforce", "Count")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.CountForce(e.beat, e["leader"], e["backup"]);
                        }
                    },
                    preFunction = delegate {
                        var e = eventCaller.currentEntity;
                        FrogHop.CountForceVox(e.beat, e["syllable"], e["leader"], e["backup"]);
                    },
                    preFunctionLength = 0,
                    parameters = new List<Param>()
                    {
                        new Param("syllable", FrogHop.Number.One, "Type", "Which number the frog(s) should say."),
                        new Param("leader", true, "Orange Frog Counts", "Make Orange Frog count during this event."),
                        new Param("backup", false, "Group Counts", "Make the frogs in the back count during this event."),
                    },
                    defaultLength = 1.0f,
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
                            instance.ThankYou(e.beat, e["pitched"], e["override"], e["overPitch"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("override", false, "Pitch Override", "Whether the frog voice pitch will be determined automatically.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "overPitch" }),
                            new Param.CollapseParam((x, _) => !(bool)x, new string[] { "pitched" }),
                        }),
                        new Param("overPitch", new EntityTypes.Float(0.25f, 4, 1), "Pitch", "Changes the frog voice pitch manually."),
                        new Param("pitched", false, "Enable Pitching", "Makes the frog voice pitch up and down based on the song's tempo."),
                    },
                    defaultLength = 6.0f,
                },
                new GameAction("mouthwide", "Mouth Animation (Open Wide)")
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
                new GameAction("mouthnarrow", "Mouth Animation (Open Narrow)")
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
                new GameAction("mouthspecial", "Mouth Animation (Special)")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.Wink("Special", e.beat + e.length, e["blue"], e["orange"], e["greens"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("blue", true, "Blue Winks", "Make Blue Frog wink during this event."),
                        new Param("orange", false, "Orange Smirks", "Make Orange Frog smirk during this event."),
                        new Param("greens", false, "Group Pogs", "Make the frogs in the back pog during this event."),
                    },
                    defaultLength = 1f,
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
                new GameAction("colorSingerFrog", "Blue Frog Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.RecolorFrog(0, e["color1"], e["color2"], e["color3"], e["color4"], e["color5"], e["color6"], e["lipstick"], e["belt"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("color1", FrogHop.singerFrogColors[0], "Skin Color", "The color to set Blue Frog's skin to."),
                        new Param("color2", FrogHop.singerFrogColors[1], "Tummy Color", "The color to set Blue Frog's tummy to."),
                        new Param("color3", FrogHop.singerFrogColors[2], "Pants Color", "The color to set Blue Frog's pants to."),
                        new Param("color5", FrogHop.singerFrogColors[4], "Sclera Color", "The color to set Blue Frog's scleras to."),
                        new Param("belt", true, "Frog Has Belt", "Make Blue Frog wear a belt.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "color4" })
                        }),
                        new Param("color4", FrogHop.singerFrogColors[3], "Belt Color", "The color to set Blue Frog's belt to."),
                        new Param("lipstick", false, "Frog Has Lipstick", "Make Blue Frog wear lipstick.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "color6" })
                        }),
                        new Param("color6", FrogHop.singerFrogColors[5], "Lipstick Color", "The color to set Blue Frog's lipstick to."),
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("colorLeaderFrog", "Orange Frog Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.RecolorFrog(2, e["color1"], e["color2"], e["color3"], e["color4"], e["color5"], e["color6"], e["lipstick"], e["belt"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("color1", FrogHop.leaderFrogColors[0], "Skin Color", "The color to set Orange Frog's skin to."),
                        new Param("color2", FrogHop.leaderFrogColors[1], "Tummy Color", "The color to set Orange Frog's tummy to."),
                        new Param("color3", FrogHop.leaderFrogColors[2], "Pants Color", "The color to set Orange Frog's pants to."),
                        new Param("color5", FrogHop.leaderFrogColors[4], "Sclera Color", "The color to set Orange Frog's scleras to."),
                        new Param("belt", true, "Frog Has Belt", "Make Orange Frog wear a belt.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "color4" })
                        }),
                        new Param("color4", FrogHop.leaderFrogColors[3], "Belt Color", "The color to set Orange Frog's belt to."),
                        new Param("lipstick", true, "Frog Has Lipstick", "Make Orange Frog wear lipstick.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "color6" })
                        }),
                        new Param("color6", FrogHop.leaderFrogColors[5], "Lipstick Color", "The color to set Orange Frog's lipstick to."),
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("colorBackupFrog", "Green Frogs Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.RecolorFrog(4, e["color1"], e["color2"], e["color3"], e["color4"], e["color5"], e["color6"], e["lipstick"], e["belt"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("color1", FrogHop.backupFrogColors[0], "Skin Color", "The color to set Green Frogs' skin to."),
                        new Param("color2", FrogHop.backupFrogColors[1], "Tummy Color", "The color to set Green Frogs' tummy to."),
                        new Param("color3", FrogHop.backupFrogColors[2], "Pants Color", "The color to set Green Frogs' pants to."),
                        new Param("color5", FrogHop.backupFrogColors[4], "Sclera Color", "The color to set Green Frogs' scleras to."),
                        new Param("belt", false, "Frog Has Belt", "Make Green Frogs wear a belt.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "color4" })
                        }),
                        new Param("color4", FrogHop.backupFrogColors[3], "Belt Color", "The color to set Green Frogs' belt to."),
                        new Param("lipstick", false, "Frog Has Lipstick", "Make Green Frogs wear lipstick.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "color6" })
                        }),
                        new Param("color6", FrogHop.backupFrogColors[5], "Lipstick Color", "The color to set Green Frogs' lipstick to."),
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("changeBgColor", "Background Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.ChangeBGColor(e.beat, e.length, e["colorFrom"], e["colorTo"], e["colorFrom2"], e["colorTo2"], e["ease"]);
                        }
                    },
                    defaultLength = 4f,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("colorFrom", FrogHop.defaultBGColor, "Color A Start", "Set the top-most color of the background gradient at the start of the event."),
                        new Param("colorTo", FrogHop.defaultBGColor, "Color A End", "Set the top-most color of the background gradient at the end of the event."),
                        new Param("colorFrom2", FrogHop.defaultBGColorBottom, "Color B Start", "Set the bottom-most color of the background gradient at the start of the event."),
                        new Param("colorTo2", FrogHop.defaultBGColorBottom, "Color B End", "Set the bottom-most color of the background gradient at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    }
                },
                new GameAction("colorStage", "Stage Appearance")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.StageAppearance(e["color1"], e["color2"], e["color3"], e["color4"], e["mikeL"], e["mikeR"], e["color5"], e["color6"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("color1", FrogHop.stageColors[0], "Stage Top Color", "The color to set the stage's top to."),
                        new Param("color2", FrogHop.stageColors[1], "Stage Rim Color", "The color to set the stage's rim to."),
                        new Param("color3", FrogHop.stageColors[2], "Stage Trim Color", "The color to set the stage's trim to."),
                        new Param("color4", FrogHop.stageColors[3], "Stage Base Color", "The color to set the stage's base to."),
                        new Param("mikeL", true, "Left Microphone", "Enables the microphone in front of Blue Frog."),
                        new Param("mikeR", false, "Right Microphone", "Enables the microphone in front of Orange Frog."),
                        new Param("color5", Color.white, "Front Spotlight Color", "The color to set the front spotlights to."),
                        new Param("color6", Color.white, "Back Spotlight Color", "The color to set the back spotlights to."),
                    },
                    defaultLength = 0.5f,
                },
                new GameAction("disableBlue", "Toggle Blue Frog")
                {
                    function = delegate {
                        var e = eventCaller.currentEntity;
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out FrogHop instance)) {
                            instance.DisableBlue(e["disable"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("disable", true, "Disable", "Makes blue frog disappear."),
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
                            instance.Pitching(e["pitched"], e["override"], e["overPitch"]);
                        }
                    },
                    parameters = new List<Param>()
                    {
                        new Param("override", false, "Pitch Override", "Whether the frog voice pitch will be determined automatically.", new List<Param.CollapseParam>()
                        {
                            new Param.CollapseParam((x, _) => (bool)x, new string[] { "overPitch" }),
                            new Param.CollapseParam((x, _) => !(bool)x, new string[] { "pitched" }),
                        }),
                        new Param("overPitch", new EntityTypes.Float(0.25f, 4, 1), "Pitch", "Changes the frog voice pitch manually."),
                        new Param("pitched", false, "Enable Pitching", "Makes the frog voices pitch up and down based on the song's tempo."),
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

        //general purpose stuff below

        [SerializeField] ntrFrog PlayerFrog;
        [SerializeField] List<ntrFrog> OtherFrogs = new List<ntrFrog>();
        [SerializeField] ntrFrog LeaderFrog;
        [SerializeField] ntrFrog SingerFrog;
        [SerializeField] GameObject Darkness;
        [SerializeField] GameObject SpotlightFront;
        [SerializeField] GameObject SpotlightBack;
        [SerializeField] SpriteRenderer SpotlightFrontColor;
        [SerializeField] SpriteRenderer SpotlightBackColor;
        [SerializeField] SpriteRenderer Mike;
        [SerializeField] SpriteRenderer Mike2;
        [SerializeField] SpriteRenderer Stage;
        [SerializeField] SpriteRenderer StageTop;
        [SerializeField] List<Material> _FrogColors = new List<Material>();
        List<Material> FrogColors = new();
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
        float globalPitchOverride = -1;
        bool usesGlobalePitch = false; //oops i spelled global wrong lmao

        public enum Number
        {
            One,
            Two,
            Three,
            Four,
        }

        //recolor stuff below

        private static readonly Dictionary<int, string> singerFrogColorLookup = new() {
            { 0, "#69A6FF" }, //skin
            { 1, "#ACEEE5" }, //tummy
            { 2, "#0C59FF" }, //pants
            { 3, "#F92D5F" }, //belt
            { 4, "#FFFFFF" }, //sclera
            { 5, "#8B42C0" }, //lipstick
        };
        private static List<Color> _singerFrogColors = new List<Color>();
        public static List<Color> singerFrogColors
        {
            get
            {
                for ( int i = 0; i < singerFrogColorLookup.Count; i++ )
                {
                    Color colorTemp;
                    ColorUtility.TryParseHtmlString(singerFrogColorLookup[i], out colorTemp);
                    _singerFrogColors.Add(colorTemp);
                }
                return _singerFrogColors;
            }
        }

        private static readonly Dictionary<int, string> leaderFrogColorLookup = new() {
            { 0, "#FF954E" }, //skin
            { 1, "#F9D7C4" }, //tummy
            { 2, "#F92D5F" }, //pants
            { 3, "#0C59FF" }, //belt
            { 4, "#FFFFFF" }, //sclera
            { 5, "#EB3600" }, //lipstick
        };
        private static List<Color> _leaderFrogColors = new List<Color>();
        public static List<Color> leaderFrogColors
        {
            get
            {
                for ( int i = 0; i < leaderFrogColorLookup.Count; i++ )
                {
                    Color colorTemp;
                    ColorUtility.TryParseHtmlString(leaderFrogColorLookup[i], out colorTemp);
                    _leaderFrogColors.Add(colorTemp);
                }
                return _leaderFrogColors;
            }
        }

        private static readonly Dictionary<int, string> backupFrogColorLookup = new() {
            { 0, "#3DDF30" }, //skin
            { 1, "#FFF769" }, //tummy
            { 2, "#165423" }, //pants
            { 3, "#1E6F18" }, //belt
            { 4, "#FFFFFF" }, //sclera
            { 5, "#EB3600" }, //lipstick
        };
        private static List<Color> _backupFrogColors = new List<Color>();
        public static List<Color> backupFrogColors
        {
            get
            {
                for ( int i = 0; i < backupFrogColorLookup.Count; i++ )
                {
                    Color colorTemp;
                    ColorUtility.TryParseHtmlString(backupFrogColorLookup[i], out colorTemp);
                    _backupFrogColors.Add(colorTemp);
                }
                return _backupFrogColors;
            }
        }

        private static readonly Dictionary<int, string> stageColorLookup = new() {
            { 0, "#FFFFFF" }, //top
            { 1, "#C0F36D" }, //rim
            { 2, "#D5F65A" }, //trim
            { 3, "#94C539" }, //base
        };
        private static List<Color> _stageColors = new List<Color>();
        public static List<Color> stageColors
        {
            get
            {
                for ( int i = 0; i < stageColorLookup.Count; i++ )
                {
                    Color colorTemp;
                    ColorUtility.TryParseHtmlString(stageColorLookup[i], out colorTemp);
                    _stageColors.Add(colorTemp);
                }
                return _stageColors;
            }
        }

        //bg stuff below

        [SerializeField] SpriteRenderer gradient;
        [SerializeField] SpriteRenderer bgLow;
        [SerializeField] SpriteRenderer bgHigh;

        double bgColorStartBeat = -1;
        float bgColorLength = 0;
        Util.EasingFunction.Ease lastEase;
        Color colorFrom;
        Color colorTo;
        Color colorFrom2;
        Color colorTo2;

        private static Color _defaultBGColor;
        public static Color defaultBGColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#5A9C28", out _defaultBGColor);
                return _defaultBGColor;
            }
        }
        private static Color _defaultBGColorBottom;
        public static Color defaultBGColorBottom
        {
            get
            {
                ColorUtility.TryParseHtmlString("#D6EEA4", out _defaultBGColorBottom);
                return _defaultBGColorBottom;
            }
        }

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
            if (globalPitchOverride < 0) globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;
            else globalPitch = globalPitchOverride;

            AllFrogs.Add(PlayerFrog);
            AllFrogs.AddRange(OtherFrogs);
            AllFrogs.Add(LeaderFrog);
            AllFrogs.Add(SingerFrog);

            BackFrogs.Add(PlayerFrog);
            BackFrogs.AddRange(OtherFrogs);

            FrontFrogs.Add(LeaderFrog);
            FrontFrogs.Add(SingerFrog);

            Material tempMat;
            foreach (var mat in _FrogColors)
            {
                FrogColors.Add(new(mat));
            }
            SingerFrog.AssignMaterials(FrogColors[0], FrogColors[1], FrogColors[6]);
            LeaderFrog.AssignMaterials(FrogColors[2], FrogColors[3], FrogColors[6]);
            SingerFrog.beltColor = Color.red;
            LeaderFrog.beltColor = Color.green;
            foreach (var a in BackFrogs)
            {
                a.AssignMaterials(FrogColors[4], FrogColors[5], FrogColors[6]);
                a.beltColor = Color.blue;
            }
            Stage.material = FrogColors[7];

            PersistThings(Conductor.instance.songPositionInBeatsAsDouble);

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
                    Count(e.beat, e["start"], e["leader"], e["backup"]);
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
            //bg stuff below

            BackgroundColorUpdate(Conductor.instance);

            //voice pitch stuff below

            if (globalPitchOverride < 0) globalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;
            else globalPitch = globalPitchOverride;

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

        public void Count(double beat, bool start, bool leaderCounts, bool backupCounts)
        {
            var actions = new List<BeatAction.Action>();

            if (leaderCounts)
            {
                actions.Add(new(beat + 0.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
                actions.Add(new(beat + 1.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
                actions.Add(new(beat + 2.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
                actions.Add(new(beat + 3.0, delegate { Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat); }));
            }

            if (backupCounts)
            {
                actions.Add(new(beat + 0.0, delegate { Talk(BackFrogs, "Wide", beat); }));
                actions.Add(new(beat + 1.0, delegate { Talk(BackFrogs, "Wide", beat); }));
                actions.Add(new(beat + 2.0, delegate { Talk(BackFrogs, "Wide", beat); }));
                actions.Add(new(beat + 3.0, delegate { Talk(BackFrogs, "Wide", beat); }));
            }

            actions.Sort((x, y) => x.beat.CompareTo(y.beat));
            BeatAction.New(this, actions);

            if (start) Hop(beat + 4.0);
        }

        public static void CountVox(double beat, bool leaderCounts, bool backupCounts)
        {
            float pitchToUse = GetPitch(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            if (leaderCounts)
            {
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT1", beat + 0.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT2", beat + 1.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT3", beat + 2.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT4", beat + 3.0, pitchToUse));
            }

            if (backupCounts)
            {
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT1_EXTRAS_CUSTOM", beat + 0.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT2_EXTRAS_CUSTOM", beat + 1.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT3_EXTRAS_CUSTOM", beat + 2.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT4_EXTRAS_CUSTOM", beat + 3.0, pitchToUse));

                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT1_PLAYER_CUSTOM", beat + 0.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT2_PLAYER_CUSTOM", beat + 1.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT3_PLAYER_CUSTOM", beat + 2.0, pitchToUse));
                sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_COUNT4_PLAYER_CUSTOM", beat + 3.0, pitchToUse));
            }

            MultiSound.Play(sounds, forcePlay: true);
        }

        public void CountForce(double beat, bool leaderCounts, bool backupCounts)
        {
            var actions = new List<BeatAction.Action>();

            if (leaderCounts) Talk(new List<ntrFrog>() { LeaderFrog }, "Wide", beat);

            if (backupCounts) Talk(BackFrogs, "Wide", beat);
        }

        public static void CountForceVox(double beat, int Number, bool leaderCounts, bool backupCounts)
        {
            float pitchToUse = GetPitch(Conductor.instance.songPositionInBeatsAsDouble);

            if (leaderCounts) SoundByte.PlayOneShotGame($"frogHop/SE_NTR_FROG_EN_COUNT" + (Number + 1));

            if (backupCounts)
            {
                SoundByte.PlayOneShotGame($"frogHop/SE_NTR_FROG_EN_COUNT" + (Number + 1) + $"_EXTRAS_CUSTOM");
                SoundByte.PlayOneShotGame($"frogHop/SE_NTR_FROG_EN_COUNT" + (Number + 1) + $"_PLAYER_CUSTOM");
            }
        }

        public void Hop (double beat)
        {
            wantHop = beat - 1;
        }

        public void Stop (double beat)
        {
            keepHopping = false;
        }

        public void Pitching(bool enabled, bool manualPitch, float pitchValue)
        {
            usesGlobalePitch = enabled | manualPitch;
            if (manualPitch) globalPitchOverride = pitchValue;
            else globalPitchOverride = -1;
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
            float pitchToUse = GetPitch(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HA", beat, pitchToUse));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_DEFAULT", beat));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAAI", beat + 0.5, pitchToUse));
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
            float pitchToUse = GetPitch(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAI", beat, pitchToUse));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_DEFAULT", beat));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAI", beat + 0.5, pitchToUse));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_POP_DEFAULT", beat + 0.5));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_HAI", beat + 1.0, pitchToUse));
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
            float pitchToUse = GetPitch(Conductor.instance.songPositionInBeatsAsDouble);

            var sounds = new List<MultiSound.Sound>();

            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_KURU_1", beat, pitchToUse));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_KURU_2", beat + 0.5, pitchToUse));
            sounds.Add(new MultiSound.Sound("frogHop/SE_NTR_FROG_EN_T_LIN", beat + 1.0, pitchToUse));
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

            if (front || !dark) { Mike.color = new Color(1, 1, 1, 1); Mike2.color = new Color(1, 1, 1, 1); }
            else { Mike.color = new Color(0.5f, 0.5f, 0.5f, 1); Mike2.color = new Color(0.5f, 0.5f, 0.5f, 1); }

            Darkness.SetActive(dark);
            SpotlightFront.SetActive(front);
            SpotlightBack.SetActive(back);
        }

        public void ThankYou(double beat, bool stretchToTempo, bool manualPitch, float pitchValue)
        {
            float pitch;
            double offset;
            double stretch;

            if (!manualPitch)
            {
                pitch = stretchToTempo ? globalPitch * Conductor.instance.TimelinePitch : 1;
                offset = stretchToTempo ? (.2 / ((Conductor.instance.GetBpmAtBeat(beat) * Conductor.instance.TimelinePitch) / 156)) : .2;
                stretch = stretchToTempo ? 1 : 1 / (globalPitch * Conductor.instance.TimelinePitch);
            }
            else
            {
                pitch = pitchValue;
                offset = .2 / (pitchValue / 1);
                stretch = (pitchValue / Conductor.instance.TimelinePitch) / (Conductor.instance.GetBpmAtBeat(beat) / 156);
            }

            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("frogHop/tyvm", beat, pitch: pitch, offset: offset),
            });

            var actions = new List<BeatAction.Action>();
            var BlueFrog = new List<ntrFrog>() { SingerFrog };

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

        public void Wink(string syllable, double animEnd, bool blue, bool orange, bool greens)
        {
            var FrogsToTalk = new List<ntrFrog>();

            if (blue) FrogsToTalk.Add(SingerFrog);
            if (orange) FrogsToTalk.Add(LeaderFrog);
            if (greens) FrogsToTalk.AddRange(BackFrogs);

            foreach (var a in FrogsToTalk) { a.Wink(syllable, animEnd); }
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

        public void DisableBlue(bool disable)
        {
            SingerFrog.gameObject.SetActive(!disable);
        }

        public void RecolorFrog(int whichFrog, Color skinColor, Color tummyColor, Color pantsColor, Color beltColor, Color scleraColor, Color lipstickColor, bool lipstickEnabled, bool beltEnabled)
        {
            FrogColors[whichFrog + 0].SetColor("_ColorAlpha", pantsColor);
            FrogColors[whichFrog + 0].SetColor("_ColorBravo", tummyColor);
            FrogColors[whichFrog + 0].SetColor("_ColorDelta", skinColor);

            FrogColors[whichFrog + 1].SetColor("_ColorAlpha", skinColor);
            FrogColors[whichFrog + 1].SetColor("_ColorBravo", lipstickEnabled ? lipstickColor : skinColor);
            FrogColors[whichFrog + 1].SetColor("_ColorDelta", scleraColor);

            string beltToModify;
            List<ntrFrog> beltToEnable;
            switch (whichFrog) {
                case 0: beltToModify = "_ColorAlpha"; beltToEnable = new List<ntrFrog>() { SingerFrog }; break;
                case 2: beltToModify = "_ColorBravo"; beltToEnable = new List<ntrFrog>() { LeaderFrog }; break;
                default: beltToModify = "_ColorDelta"; beltToEnable = BackFrogs; break;
            }

            FrogColors[6].SetColor(beltToModify, beltColor);
            foreach (var a in beltToEnable) { a.Belt.gameObject.SetActive(beltEnabled); }
        }

        public void StageAppearance(Color stageColor1, Color stageColor2, Color stageColor3, Color stageColor4, bool leftMike, bool rightMike, Color frontSpotlightColor, Color backSpotlightColor)
        {
            StageTop.color = stageColor1;
            FrogColors[7].SetColor("_ColorAlpha", stageColor4);
            FrogColors[7].SetColor("_ColorBravo", stageColor2);
            FrogColors[7].SetColor("_ColorDelta", stageColor3);

            Mike.enabled = leftMike;
            Mike2.enabled = rightMike;

            Color transparent = new Color(1, 1, 1, 0.5f);
            SpotlightFrontColor.color = frontSpotlightColor * transparent;
            SpotlightBackColor.color = backSpotlightColor * transparent;
        }

        private void PersistThings(double beat)
        {
            var allEvents = GameManager.instance.Beatmap.Entities.FindAll(e => e.datamodel.Split('/')[0] is "frogHop");
            var eventsBefore = allEvents.FindAll(e => e.beat < beat);

            var lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/changeBgColor");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                ChangeBGColor(e.beat, e.length, e["colorFrom"], e["colorTo"], e["colorFrom2"], e["colorTo2"], e["ease"]);
            }
            else
            {
                colorFrom = defaultBGColor;
                colorTo = defaultBGColor;
                colorFrom2 = defaultBGColorBottom;
                colorTo2 = defaultBGColorBottom;
            }

            BackgroundColorUpdate(Conductor.instance);

            lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/pitching");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                Pitching(e["pitched"], e["override"], e["overPitch"]);
            }

            lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/colorSingerFrog");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                RecolorFrog(0, e["color1"], e["color2"], e["color3"], e["color4"], e["color5"], e["color6"], e["lipstick"], e["belt"]);
            }

            lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/colorLeaderFrog");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                RecolorFrog(2, e["color1"], e["color2"], e["color3"], e["color4"], e["color5"], e["color6"], e["lipstick"], e["belt"]);
            }

            lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/colorBackupFrog");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                RecolorFrog(4, e["color1"], e["color2"], e["color3"], e["color4"], e["color5"], e["color6"], e["lipstick"], e["belt"]);
            }

            lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/colorStage");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                StageAppearance(e["color1"], e["color2"], e["color3"], e["color4"], e["mikeL"], e["mikeR"], e["color5"], e["color6"]);
            }

            lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/spotlights");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                Spotlights(e["front"], e["back"], e["dark"]);
            }

            lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/disableBlue");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                DisableBlue(e["disable"]);
            }
        }

        public void ChangeBGColor(double beat, float length, Color color1, Color color2, Color color3, Color color4, int ease)
        {
            bgColorStartBeat = beat;
            bgColorLength = length;
            colorFrom = color1;
            colorTo = color2;
            colorFrom2 = color3;
            colorTo2 = color4;
            lastEase = (Util.EasingFunction.Ease)ease;
        }

        private void BackgroundColorUpdate(Conductor cond)
        {
            float normalizedBeat = Mathf.Clamp01(cond.GetPositionFromBeat(bgColorStartBeat, bgColorLength));
            Util.EasingFunction.Function func = Util.EasingFunction.GetEasingFunction(lastEase);
            float newColorR = func(colorFrom.r, colorTo.r, normalizedBeat);
            float newColorG = func(colorFrom.g, colorTo.g, normalizedBeat);
            float newColorB = func(colorFrom.b, colorTo.b, normalizedBeat);
            bgHigh.color = new Color(newColorR, newColorG, newColorB);
            gradient.color = new Color(newColorR, newColorG, newColorB);
            newColorR = func(colorFrom2.r, colorTo2.r, normalizedBeat);
            newColorG = func(colorFrom2.g, colorTo2.g, normalizedBeat);
            newColorB = func(colorFrom2.b, colorTo2.b, normalizedBeat);
            bgLow.color = new Color(newColorR, newColorG, newColorB);
        }

        public static float GetPitch(double beat)
        {
            var allEvents = GameManager.instance.Beatmap.Entities.FindAll(e => e.datamodel.Split('/')[0] is "frogHop");
            var eventsBefore = allEvents.FindAll(e => e.beat < beat);

            float finalPitch = 1;

            var lastPersistEvent = eventsBefore.FindLast(e => e.datamodel == "frogHop/pitching");
            if (lastPersistEvent != null)
            {
                var e = lastPersistEvent;
                if (!e["override"])
                {
                    if (e["pitched"]) finalPitch = (float)Conductor.instance.GetBpmAtBeat(Conductor.instance.songPositionInBeatsAsDouble) / 156;
                }
                else
                {
                    finalPitch = e["overPitch"];
                }
            }

            return finalPitch;
        }

        #endregion
    }
}