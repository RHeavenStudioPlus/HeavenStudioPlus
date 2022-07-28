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
                new GameAction("hit",                   delegate { var e = eventCaller.currentEntity; KarateManNew.instance.CreateItem(e.beat, e.type); }, 2, false, 
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
                new GameAction("combo",                 delegate { var e = eventCaller.currentEntity; KarateManNew.instance.Combo(e.beat); }, 4f),
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
        public Transform ItemHolder;
        public GameObject Item;
        public KarateManJoeNew Joe;

        private void Awake()
        {
            instance = this;
            KarateManPotNew.ResetLastCombo();
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

        public void CreateItem(float beat, int type)
        {

            string outSound;

            switch (type)
            {
                case (int) HitType.Pot:
                    if (Starpelly.Mathp.GetDecimalFromFloat(beat) == 0f)
                        outSound = "karateman/objectOut";
                    else
                        outSound = "karateman/offbeatObjectOut";
                    CreateItemInstance(beat, "Item00");
                    break;
                default:
                    if (Starpelly.Mathp.GetDecimalFromFloat(beat) == 0f)
                        outSound = "karateman/objectOut";
                    else
                        outSound = "karateman/offbeatObjectOut";
                    CreateItemInstance(beat, "Item00");
                    break;
            }
            Jukebox.PlayOneShotGame(outSound, forcePlay: true);
        }

        public void Combo(float beat)
        {
            Jukebox.PlayOneShotGame("karateman/barrelOutCombos", forcePlay: true);

            int comboId = KarateManPotNew.GetNewCombo();

            BeatAction.New(gameObject, new List<BeatAction.Action>() 
            { 
                new BeatAction.Action(beat, delegate { CreateItemInstance(beat, "Item00", KarateManPotNew.ItemType.ComboPot1, comboId); }),
                new BeatAction.Action(beat + 0.25f, delegate { CreateItemInstance(beat + 0.25f, "Item00", KarateManPotNew.ItemType.ComboPot2, comboId); }),
                new BeatAction.Action(beat + 0.5f, delegate { CreateItemInstance(beat + 0.5f, "Item00", KarateManPotNew.ItemType.ComboPot3, comboId); }),
                new BeatAction.Action(beat + 0.75f, delegate { CreateItemInstance(beat + 0.75f, "Item00", KarateManPotNew.ItemType.ComboPot4, comboId); }),
                new BeatAction.Action(beat + 1f, delegate { CreateItemInstance(beat + 1f, "Item00", KarateManPotNew.ItemType.ComboPot5, comboId); }),
                new BeatAction.Action(beat + 1.5f, delegate { CreateItemInstance(beat + 1.5f, "Item05", KarateManPotNew.ItemType.ComboBarrel, comboId); }),
            });

            MultiSound.Play(new MultiSound.Sound[] 
            {
                new MultiSound.Sound("karateman/punchy1", beat + 1f), 
                new MultiSound.Sound("karateman/punchy2", beat + 1.25f), 
                new MultiSound.Sound("karateman/punchy3", beat + 1.5f), 
                new MultiSound.Sound("karateman/punchy4", beat + 1.75f), 
                new MultiSound.Sound("karateman/ko", beat + 2f), 
                new MultiSound.Sound("karateman/pow", beat + 2.5f) 
            }, forcePlay: true);
        }

        GameObject CreateItemInstance(float beat, string awakeAnim, KarateManPotNew.ItemType type = KarateManPotNew.ItemType.Pot, int comboId = -1)
        {
            GameObject mobj = GameObject.Instantiate(Item, ItemHolder);
            KarateManPotNew mobjDat = mobj.GetComponent<KarateManPotNew>();
            mobjDat.type = type;
            mobjDat.startBeat = beat;
            mobjDat.awakeAnim = awakeAnim;
            mobjDat.comboId = comboId;

            mobj.SetActive(true);
            
            return mobj;
        }
    }
}