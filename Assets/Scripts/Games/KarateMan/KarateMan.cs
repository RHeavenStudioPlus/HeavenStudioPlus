using System;
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
            return new Minigame("karateman", "Karate Man [INDEV REWORK]", "70A8D8", false, false, new List<GameAction>()
            {
                new GameAction("bop",                   delegate { }, 0.5f, true),
                new GameAction("hit",                   delegate { var e = eventCaller.currentEntity; KarateMan.instance.CreateItem(e.beat, e.type); }, 2, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.HitType.Pot, "Object", "The object to fire")
                    }),
                new GameAction("bulb",                  delegate { var e = eventCaller.currentEntity; KarateMan.instance.CreateBulbSpecial(e.beat, e.type, e.colorA); }, 2, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.LightBulbType.Normal, "Type", "The preset bulb type. Yellow is used for kicks while Blue is used for combos"),
                        new Param("colorA", new Color(), "Custom Color", "The color to use when the bulb type is set to Custom")
                    }),
                new GameAction("kick",                  delegate { KarateMan.instance.Kick(eventCaller.currentEntity.beat); }, 4f),
                new GameAction("combo",                 delegate { KarateMan.instance.Combo(eventCaller.currentEntity.beat); }, 4f),
                new GameAction("hitX",                  delegate { var e = eventCaller.currentEntity; KarateMan.instance.DoWord(e.beat, e.type); }, 1f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.HitThree.HitThree, "Type", "The warning text to show")
                    }),
                new GameAction("prepare",               delegate { }, 1f, true),
                new GameAction("set background effects",  delegate {
                }, 0.5f, false, new List<Param>()
                {
                    new Param("type", KarateMan.BackgroundType.Yellow, "Background Type", "The preset background type"),
                    new Param("type2", KarateMan.ShadowType.Tinted, "Shadow Type", "The shadow type. If Tinted doesn't work with your background color try Custom"),
                    new Param("colorA", new Color(), "Custom Background Color", "The background color to use when background type is set to Custom"),
                    new Param("colorB", new Color(), "Custom Shadow Color", "The shadow color to use when shadow type is set to Custom"),
                    new Param("type3", KarateMan.BackgroundFXType.None, "FX Type", "The background effect to be displayed")

                }),

                // These are still here for backwards-compatibility but are hidden in the editor
                new GameAction("pot",                   delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.Pot); }, 2, hidden: true),
                new GameAction("rock",                  delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.Rock); }, 2, hidden: true),
                new GameAction("ball",                  delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.Ball); }, 2, hidden: true),
                new GameAction("tacobell",              delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.TacoBell); }, 2, hidden: true),
                new GameAction("hit4",                  delegate { KarateMan.instance.DoWord(eventCaller.currentEntity.beat, (int) KarateMan.HitThree.HitFour); }, hidden: true),
                new GameAction("bgfxon",                delegate { }, hidden: true),
                new GameAction("bgfxoff",               delegate { }, hidden: true),
                new GameAction("hit3",                  delegate { var e = eventCaller.currentEntity; KarateMan.instance.DoWord(e.beat, e.type); }, 1f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.HitThree.HitThree, "Type", "The warning text to show")
                    }, 
                hidden: true),
                new GameAction("set background color",  delegate { }, 0.5f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.BackgroundType.Yellow, "Background Type", "The preset background type"),
                        new Param("type2", KarateMan.ShadowType.Tinted, "Shadow Type", "The shadow type. If Tinted doesn't work with your background color try Custom"),
                        new Param("colorA", new Color(), "Custom Background Color", "The background color to use when background type is set to Custom"),
                        new Param("colorB", new Color(), "Custom Shadow Color", "The shadow color to use when shadow type is set to Custom"),

                    }),
                new GameAction("set background fx",  delegate {
                }, 0.5f, false, new List<Param>()
                {
                    new Param("type", KarateMan.BackgroundFXType.None, "FX Type", "The background effect to be displayed")
                },
                hidden: true)

            });
        }
    }
}

namespace HeavenStudio.Games
{
    using Scripts_KarateMan;
    public class KarateMan : Minigame
    {
        public static KarateMan instance;

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
            HitFour,
            Grr,
            Warning,
            Combo,
            HitOne,
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
        public KarateManJoe Joe;

        //warning text
        public Animator Word;
        float wordClearTime = Single.MinValue;
        const float hitVoiceOffset = 0.042f;

        private void Awake()
        {
            instance = this;
            KarateManPot.ResetLastCombo();
            cameraPosition = CameraPosition[0].position;
        }

        private void Start()
        {
            GameCamera.additionalPosition = cameraPosition - GameCamera.defaultPosition;
        }

        private void Update()
        {
            GameCamera.additionalPosition = cameraPosition - GameCamera.defaultPosition;
            if (Conductor.instance.songPositionInBeats >= wordClearTime)
            {
                Word.Play("NoPose");
            }
        }

        public void DoWord(float beat, int type)
        {
            switch (type)
            {
                case (int) HitThree.HitTwo:
                    Word.Play("Word02");
                    wordClearTime = beat + 4f;
                    MultiSound.Play(new MultiSound.Sound[] 
                    {
                        new MultiSound.Sound("karateman/hit", beat + 0.5f, offset: hitVoiceOffset), 
                        new MultiSound.Sound("karateman/two", beat + 1f),
                    }, forcePlay: true);
                    break;
                case (int) HitThree.HitThree:
                    Word.Play("Word03");
                    wordClearTime = beat + 4f;
                    MultiSound.Play(new MultiSound.Sound[] 
                    {
                        new MultiSound.Sound("karateman/hit", beat + 0.5f, offset: hitVoiceOffset), 
                        new MultiSound.Sound("karateman/three", beat + 1f),
                    }, forcePlay: true);
                    break;
                case (int) HitThree.HitThreeAlt:
                    Word.Play("Word03");
                    wordClearTime = beat + 4f;
                    MultiSound.Play(new MultiSound.Sound[] 
                    {
                        new MultiSound.Sound("karateman/hitAlt", beat + 0.5f, offset: hitVoiceOffset), 
                        new MultiSound.Sound("karateman/threeAlt", beat + 1f),
                    }, forcePlay: true);
                    break;
                case (int) HitThree.HitFour:
                    Word.Play("Word04");
                    wordClearTime = beat + 4f;
                    MultiSound.Play(new MultiSound.Sound[] 
                    {
                        new MultiSound.Sound("karateman/hit", beat + 0.5f, offset: hitVoiceOffset), 
                        new MultiSound.Sound("karateman/four", beat + 1f),
                    }, forcePlay: true);
                    break;
                case (int) HitThree.Grr:
                    Word.Play("Word01");
                    wordClearTime = beat + 1f;
                    break;
                case (int) HitThree.Warning:
                    Word.Play("Word05");
                    wordClearTime = beat + 1f;
                    break;
                case (int) HitThree.Combo:
                    Word.Play("Word00");
                    wordClearTime = beat + 3f;
                    break;
                case (int) HitThree.HitOne: //really?
                    Word.Play("Word06");
                    wordClearTime = beat + 4f;
                    MultiSound.Play(new MultiSound.Sound[] 
                    {
                        new MultiSound.Sound("karateman/hit", beat + 0.5f, offset: hitVoiceOffset), 
                        new MultiSound.Sound("karateman/one", beat + 1f),
                    }, forcePlay: true);
                    break;
            }
        }

        public void CreateItem(float beat, int type)
        {

            string outSound;
            if (Starpelly.Mathp.GetDecimalFromFloat(beat + 0.5f) == 0f)
                outSound = "karateman/offbeatObjectOut";
            else
                outSound = "karateman/objectOut";

            switch (type)
            {
                case (int) HitType.Pot:
                    CreateItemInstance(beat, "Item00");
                    break;
                case (int) HitType.Lightbulb:
                    if (Starpelly.Mathp.GetDecimalFromFloat(beat + 0.5f) == 0f)
                        outSound = "karateman/offbeatLightbulbOut";
                    else
                        outSound = "karateman/lightbulbOut";
                    var mobj = CreateItemInstance(beat, "Item01", KarateManPot.ItemType.Bulb);
                    mobj.GetComponent<KarateManPot>().SetBulbColor(LightBulbColors[0]);
                    break;
                case (int) HitType.Rock:
                    CreateItemInstance(beat, "Item02", KarateManPot.ItemType.Rock);
                    break;
                case (int) HitType.Ball:
                    CreateItemInstance(beat, "Item03", KarateManPot.ItemType.Ball);
                    break;
                case (int) HitType.CookingPot:
                    CreateItemInstance(beat, "Item06", KarateManPot.ItemType.Cooking);
                    break;
                case (int) HitType.Alien:
                    CreateItemInstance(beat, "Item07", KarateManPot.ItemType.Alien);
                    break;
                case (int) HitType.TacoBell:
                    CreateItemInstance(beat, "Item99", KarateManPot.ItemType.TacoBell);
                    break;
                default:
                    CreateItemInstance(beat, "Item00");
                    break;
            }
            Jukebox.PlayOneShotGame(outSound, forcePlay: true);
        }

        public void CreateBulbSpecial(float beat, int type, Color c)
        {
            string outSound;
            if (Starpelly.Mathp.GetDecimalFromFloat(beat + 0.5f) == 0f)
                outSound = "karateman/offbeatLightbulbOut";
            else
                outSound = "karateman/lightbulbOut";
            var mobj = CreateItemInstance(beat, "Item01", KarateManPot.ItemType.Bulb);

            if (type == (int) LightBulbType.Custom)
                mobj.GetComponent<KarateManPot>().SetBulbColor(c);
            else
                mobj.GetComponent<KarateManPot>().SetBulbColor(LightBulbColors[type]);
            Jukebox.PlayOneShotGame(outSound, forcePlay: true);
        }

        public void Combo(float beat)
        {
            Jukebox.PlayOneShotGame("karateman/barrelOutCombos", forcePlay: true);

            int comboId = KarateManPot.GetNewCombo();

            BeatAction.New(gameObject, new List<BeatAction.Action>() 
            { 
                new BeatAction.Action(beat, delegate { CreateItemInstance(beat, "Item00", KarateManPot.ItemType.ComboPot1, comboId); }),
                new BeatAction.Action(beat + 0.25f, delegate { CreateItemInstance(beat + 0.25f, "Item00", KarateManPot.ItemType.ComboPot2, comboId); }),
                new BeatAction.Action(beat + 0.5f, delegate { CreateItemInstance(beat + 0.5f, "Item00", KarateManPot.ItemType.ComboPot3, comboId); }),
                new BeatAction.Action(beat + 0.75f, delegate { CreateItemInstance(beat + 0.75f, "Item00", KarateManPot.ItemType.ComboPot4, comboId); }),
                new BeatAction.Action(beat + 1f, delegate { CreateItemInstance(beat + 1f, "Item00", KarateManPot.ItemType.ComboPot5, comboId); }),
                new BeatAction.Action(beat + 1.5f, delegate { CreateItemInstance(beat + 1.5f, "Item05", KarateManPot.ItemType.ComboBarrel, comboId); }),
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

        public void Kick(float beat)
        {
            Jukebox.PlayOneShotGame("karateman/barrelOutKicks", forcePlay: true);

            CreateItemInstance(beat, "Item05", KarateManPot.ItemType.KickBarrel);

            MultiSound.Play(new MultiSound.Sound[] 
            {
                new MultiSound.Sound("karateman/punchKick1", beat + 1f), 
                new MultiSound.Sound("karateman/punchKick2", beat + 1.5f), 
                new MultiSound.Sound("karateman/punchKick3", beat + 1.75f), 
                new MultiSound.Sound("karateman/punchKick4", beat + 2.5f),
            }, forcePlay: true);
        }

        GameObject CreateItemInstance(float beat, string awakeAnim, KarateManPot.ItemType type = KarateManPot.ItemType.Pot, int comboId = -1)
        {
            GameObject mobj = GameObject.Instantiate(Item, ItemHolder);
            KarateManPot mobjDat = mobj.GetComponent<KarateManPot>();
            mobjDat.type = type;
            mobjDat.startBeat = beat;
            mobjDat.awakeAnim = awakeAnim;
            mobjDat.comboId = comboId;

            mobj.SetActive(true);
            
            return mobj;
        }
    }
}