using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

//THIS CLASS IS TO BE RENAMED

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class RvlNewKarateLoader
    {
        public static Minigame AddGame(EventCaller eventCaller) {
            return new Minigame("karateManNew", "Karate Man [INDEV REWORK]", "70A8D8", false, false, new List<GameAction>()
            {
                new GameAction("bop",                   delegate { }, 0.5f, true),
                new GameAction("hit",                   delegate{}, 2, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateManNew.HitType.Pot, "Object", "The object to fire")
                    }),
                new GameAction("bulb",                  delegate {}, 2, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateManNew.LightBulbType.Normal, "Type", "The preset bulb type. Yellow is used for kicks while Blue is used for combos"),
                        new Param("colorA", new Color(), "Custom Color", "The color to use when the bulb type is set to Custom")
                    }),
                new GameAction("kick",                  delegate { }, 4.5f),
                new GameAction("combo",                 delegate { }, 4f),
                new GameAction("hit3",                  delegate { }, 1f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateManNew.HitThree.HitThree, "Type", "What should be called out")
                    }),
                new GameAction("prepare",               delegate { }, 1f, true),
                new GameAction("set background color",  delegate { }, 0.5f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateManNew.BackgroundType.Yellow, "Background Type", "The preset background type"),
                        new Param("type2", KarateManNew.ShadowType.Tinted, "Shadow Type", "The shadow type. If Tinted doesn't work with your background color try Custom"),
                        new Param("colorA", new Color(), "Custom Background Color", "The background color to use when background type is set to Custom"),
                        new Param("colorB", new Color(), "Custom Shadow Color", "The shadow color to use when shadow type is set to Custom"),

                    }),
                new GameAction("set background fx",  delegate { }, 0.5f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateManNew.BackgroundFXType.None, "FX Type", "The background effect to be displayed")

                    }),
                // These are still here for backwards-compatibility but are hidden in the editor
                new GameAction("pot",                   delegate { }, 2, hidden: true),
                new GameAction("rock",                  delegate { }, 2, hidden: true),
                new GameAction("ball",                  delegate { }, 2, hidden: true),
                new GameAction("tacobell",              delegate { }, 2, hidden: true),
                new GameAction("hit4",                  delegate { }, hidden: true),
                new GameAction("bgfxon",                delegate { }, hidden: true),
                new GameAction("bgfxoff",               delegate { }, hidden: true),

            });
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_KarateMan;
    public class KarateManNew : Minigame
    {
        public static KarateManNew instance;

        public enum HitType
        {
            Pot = 0,
            Lightbulb = 1,
            Rock = 2,
            Ball = 3,
            CookingPot = 6,
            Alien = 7,

            TacoBell = 999
        }

        public enum HitThree
        {
            HitTwo,
            HitThree,
            HitThreeAlt,
            HitFour
        }

        public enum LightBulbType
        {
            Normal,
            Blue,
            Yellow,
            Custom 
        }

        public enum BackgroundType
        {
            Yellow,
            Fuchsia,
            Blue,
            Red,
            Orange,
            Pink,
            Custom
        }

        public enum BackgroundFXType
        {
            None,
            Sunburst,
            Rings,
            Fade
        }

        public enum ShadowType
        {
            Tinted,
            Custom
        }

        public Color[] LightBulbColors;
        public Color[] BackgroundColors;
        public Color[] ShadowColors;

        //camera positions (normal, special)
        public Transform[] CameraPosition;

        Vector3 cameraPosition;
        
        //pot trajectory stuff
        public Transform[] HitPosition;
        static Vector2 StartPositionOffset = new Vector2(-3f, -8f);
        //https://www.desmos.com/calculator/ycn9v62i4f

        private void Awake()
        {
            instance = this;
            cameraPosition = CameraPosition[0].position;
        }

        private void Start()
        {
            GameCamera.additionalPosition = cameraPosition - GameCamera.defaultPosition;
        }

        private void Update()
        {
            GameCamera.additionalPosition = cameraPosition - GameCamera.defaultPosition;
        }
    }
}