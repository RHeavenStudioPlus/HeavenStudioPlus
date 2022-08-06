using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

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
                        new Param("colorA", new Color(1f,1f,1f), "Custom Color", "The color to use when the bulb type is set to Custom")
                    }),
                new GameAction("kick",                  delegate { KarateMan.instance.Kick(eventCaller.currentEntity.beat); }, 4f),
                new GameAction("combo",                 delegate { KarateMan.instance.Combo(eventCaller.currentEntity.beat); }, 4f),
                new GameAction("hitX",                  delegate { var e = eventCaller.currentEntity; KarateMan.instance.DoWord(e.beat, e.type); }, 1f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.HitThree.HitThree, "Type", "The warning text to show")
                    }),
                new GameAction("prepare",               delegate { }, 1f, true),
                new GameAction("set background effects",  delegate { var e = eventCaller.currentEntity; KarateMan.instance.SetBgAndShadowCol(e.beat, e.length, e.type, e.type2, e.colorA, e.colorB, e.type3, e.colorC); }, 0.5f, true, new List<Param>()
                {
                    new Param("type", KarateMan.BackgroundType.Yellow, "Background Type", "The preset background type"),
                    new Param("type2", KarateMan.ShadowType.Tinted, "Shadow Type", "The shadow type. If Tinted doesn't work with your background color try Custom"),
                    new Param("colorA", new Color(), "Custom Background Color", "The background color to use when background type is set to Custom"),
                    new Param("colorB", new Color(), "Custom Shadow Color", "The shadow color to use when shadow type is set to Custom"),
                    new Param("colorC", new Color(), "Fading Shadow Color", "When using the Fade background effect, make shadow colour fade to this colour"),
                    new Param("type3", KarateMan.BackgroundFXType.None, "FX Type", "The background effect to be displayed. Fade uses the entity length to determine colour fading speed")

                }),
                new GameAction("set background texture",  delegate { var e = eventCaller.currentEntity; KarateMan.instance.SetBgTexture(e.type, e.type2, e.colorA, e.colorB); }, 0.5f, false, new List<Param>()
                {
                    new Param("type", KarateMan.BackgroundTextureType.Plain, "Texture", "The type of background texture to use"),
                    new Param("type2", KarateMan.ShadowType.Tinted, "Color Filter Type", "The method used to apply colour to the texture"),
                    new Param("colorA", new Color(), "Custom Filter Color", "The filter color to use when color filter type is set to Custom"),
                    new Param("colorB", new Color(), "Fading Filter Color", "When using the Fade background effect, make filter colour fade to this colour"),
                }),
                new GameAction("special camera",               delegate { var e = eventCaller.currentEntity; KarateMan.instance.DoSpecialCamera(e.beat, e.length, e.toggle); }, 8f, true, new List<Param>()
                {
                    new Param("toggle", true, "Return Camera", "Camera zooms back in?"),
                }),
                new GameAction("particle effects",  delegate { var e = eventCaller.currentEntity; KarateMan.instance.SetParticleEffect(e.beat, e.type, e.valA, e.valB); }, 0.5f, false, new List<Param>()
                {
                    new Param("type", KarateMan.ParticleType.None, "Particle Type", "The type of particle effect to spawn. Using \"None\" will stop all effects"),
                    new Param("valA", new EntityTypes.Float(0f, 64f, 1f), "Wind Strength", "The strength of the particle wind. (Does not work on the Rain particle.)"),
                    new Param("valB", new EntityTypes.Float(1f, 12f, 1f), "Particle Intensity", "The intensity of the particle effect")
                }),
                new GameAction("force facial expression",  delegate { KarateMan.instance.SetFaceExpression(eventCaller.currentEntity.type); }, 0.5f, false, new List<Param>()
                {
                    new Param("type", KarateMan.KarateManFaces.Normal, "Facial Expression", "The facial expression to force Joe to. Special moves may override this")
                }),

                // These are still here for backwards-compatibility but are hidden in the editor
                new GameAction("pot",                   delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.Pot); }, 2, hidden: true),
                new GameAction("rock",                  delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.Rock); }, 2, hidden: true),
                new GameAction("ball",                  delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.Ball); }, 2, hidden: true),
                new GameAction("tacobell",              delegate { KarateMan.instance.CreateItem(eventCaller.currentEntity.beat, (int) KarateMan.HitType.TacoBell); }, 2, hidden: true),
                new GameAction("hit4",                  delegate { KarateMan.instance.DoWord(eventCaller.currentEntity.beat, (int) KarateMan.HitThree.HitFour); }, hidden: true),
                new GameAction("bgfxon",                delegate { var e = eventCaller.currentEntity; KarateMan.instance.SetBgFx((int) KarateMan.BackgroundFXType.Sunburst, e.beat, e.length); }, hidden: true),
                new GameAction("bgfxoff",               delegate { var e = eventCaller.currentEntity; KarateMan.instance.SetBgFx((int) KarateMan.BackgroundFXType.None, e.beat, e.length); }, hidden: true),
                new GameAction("hit3",                  delegate { var e = eventCaller.currentEntity; KarateMan.instance.DoWord(e.beat, e.type); }, 1f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.HitThree.HitThree, "Type", "The warning text to show")
                    }, 
                hidden: true),
                new GameAction("set background color",  delegate { var e = eventCaller.currentEntity; KarateMan.instance.SetBgAndShadowCol(e.beat, e.length, e.type, e.type2, e.colorA, e.colorB, (int) KarateMan.instance.currentBgEffect, e.colorB); }, 0.5f, false, 
                    new List<Param>()
                    {
                        new Param("type", KarateMan.BackgroundType.Yellow, "Background Type", "The preset background type"),
                        new Param("type2", KarateMan.ShadowType.Tinted, "Shadow Type", "The shadow type. If Tinted doesn't work with your background color try Custom"),
                        new Param("colorA", new Color(), "Custom Background Color", "The background color to use when background type is set to Custom"),
                        new Param("colorB", new Color(), "Custom Shadow Color", "The shadow color to use when shadow type is set to Custom"),

                    },
                hidden: true),
                new GameAction("set background fx",  delegate { var e = eventCaller.currentEntity; KarateMan.instance.SetBgFx(e.type, e.beat, e.length); }, 0.5f, false, new List<Param>()
                {
                    new Param("type", KarateMan.BackgroundFXType.None, "FX Type", "The background effect to be displayed")
                },
                hidden: true),

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

        public enum BackgroundTextureType
        {
            Plain,
            Gradient,
            Radial,
            Blood,
            //ManMan?
        }

        public enum ShadowType
        {
            Tinted,
            Custom
        }

        public enum CameraAngle
        {
            Normal,
            Special
        }

        public enum ParticleType
        {
            None,
            Snow,
            Fire,
            Rain
        }
        
        public enum KarateManFaces
        {
            Normal,
            Smirk,
            Surprise,
            Sad,
            Lenny,
            Happy,
            VerySad,
            Blush
        }

        public Color[] LightBulbColors;
        public Color[] BackgroundColors;
        public Color[] ShadowColors;

        //camera positions (normal, special)
        [Header("Camera Positions")]
        public Transform[] CameraPosition;
        Vector3 cameraPosition;
        float startCamSpecial = Single.MinValue;
        float wantsReturn = Single.MinValue;
        float cameraReturnLength = 2f;
        CameraAngle cameraAngle = CameraAngle.Normal;

        //pot trajectory stuff
        [Header("References")]
        public Transform ItemHolder;
        public GameObject Item;
        public KarateManJoe Joe;

        [Header("Word")]
        public Animator Word;
        float wordClearTime = Single.MinValue;
        const float hitVoiceOffset = 0.042f;

        [Header("Backgrounds")]
        public SpriteRenderer BGPlane;
        public GameObject BGEffect;
        int bgType = (int) BackgroundType.Yellow;
        Color bgColour;
        Color bgColourLast;
        public BackgroundFXType currentBgEffect = BackgroundFXType.None;
        float bgFadeTime = Single.MinValue;
        float bgFadeDuration = 0f;

        Animator bgEffectAnimator;
        SpriteRenderer bgEffectSpriteRenderer;

        int textureFiltertype = (int) ShadowType.Tinted;
        Color filterColour;
        Color filterColourNext;
        public GameObject BGGradient;
        SpriteRenderer bgGradientRenderer;

        [Header("Shadows")]
        int currentShadowType = (int) ShadowType.Tinted;
        Color customShadowColour = Color.white;
        Color fadeShadowColour = Color.white;
        Color oldShadowColour;

        [Header("Particles")]
            //wind
        public WindZone Wind;
            //snow
        public ParticleSystem SnowEffect;
        public GameObject SnowEffectGO;
            //fire
        public ParticleSystem FireEffect;
        public GameObject FireEffectGO;
            //rain
        public ParticleSystem RainEffect;
        public GameObject RainEffectGO;

        private void Awake()
        {
            instance = this;
            KarateManPot.ResetLastCombo();
            cameraPosition = CameraPosition[0].position;
        }

        private void Start()
        {
            GameCamera.additionalPosition = cameraPosition - GameCamera.defaultPosition;
            bgEffectAnimator = BGEffect.GetComponent<Animator>();
            bgEffectSpriteRenderer = BGEffect.GetComponent<SpriteRenderer>();

            bgGradientRenderer = BGGradient.GetComponent<SpriteRenderer>();

            SetBgAndShadowCol(0f, 0f, bgType, (int) currentShadowType, BackgroundColors[bgType], customShadowColour, (int)currentBgEffect, customShadowColour);
        }

        private void Update()
        {
            var cond = Conductor.instance;
            switch (currentBgEffect)
            {
                case BackgroundFXType.Sunburst:
                    bgEffectAnimator.DoNormalizedAnimation("Sunburst", (cond.songPositionInBeats*0.5f) % 1f);
                    break;
                case BackgroundFXType.Rings:
                    bgEffectAnimator.DoNormalizedAnimation("Rings", (cond.songPositionInBeats*0.5f) % 1f);
                    break;
                default:
                    bgEffectAnimator.Play("NoPose", -1, 0);
                    break;
            }
            if (cond.songPositionInBeats >= wordClearTime)
            {
                Word.Play("NoPose");
            }

            if (cond.songPositionInBeats >= startCamSpecial && cond.songPositionInBeats <= wantsReturn)
            {
                float camX = 0f;
                float camY = 0f;
                float camZ = 0f;
                if (cond.songPositionInBeats <= startCamSpecial + cameraReturnLength)
                {
                    float prog = cond.GetPositionFromBeat(startCamSpecial, cameraReturnLength);
                    camX = EasingFunction.EaseOutCubic(CameraPosition[0].position.x, CameraPosition[1].position.x, prog);
                    camY = EasingFunction.EaseOutCubic(CameraPosition[0].position.y, CameraPosition[1].position.y, prog);
                    camZ = EasingFunction.EaseOutCubic(CameraPosition[0].position.z, CameraPosition[1].position.z, prog);
                    cameraPosition = new Vector3(camX, camY, camZ);
                }
                else if (cond.songPositionInBeats >= wantsReturn - cameraReturnLength)
                {
                    float prog = cond.GetPositionFromBeat(wantsReturn - cameraReturnLength, cameraReturnLength);
                    camX = EasingFunction.EaseOutQuad(CameraPosition[1].position.x, CameraPosition[0].position.x, prog);
                    camY = EasingFunction.EaseOutQuad(CameraPosition[1].position.y, CameraPosition[0].position.y, prog);
                    camZ = EasingFunction.EaseOutQuad(CameraPosition[1].position.z, CameraPosition[0].position.z, prog);
                    cameraPosition = new Vector3(camX, camY, camZ);
                }
                else
                {
                    cameraPosition = CameraPosition[1].position;
                }
            }
            else
            {
                if (cameraAngle == CameraAngle.Special)
                    cameraAngle = CameraAngle.Normal;
                cameraPosition = CameraPosition[0].position;
            }

            float fadeProg = cond.GetPositionFromBeat(bgFadeTime, bgFadeDuration);
            if (bgFadeTime != Single.MinValue && fadeProg >= 0)
            {
                if (fadeProg >= 1f)
                {
                    bgFadeTime = Single.MinValue;
                    bgFadeDuration = 0f;
                    BGPlane.color = bgColour;
                    filterColour = filterColourNext;
                    UpdateFilterColour(bgColour, filterColour);
                    oldShadowColour = GetShadowColor(true);
                }
                else
                {
                    Color col = Color.LerpUnclamped(bgColourLast, bgColour, fadeProg);
                    BGPlane.color = col;
                    UpdateFilterColour(col, Color.LerpUnclamped(filterColour, filterColourNext, fadeProg));
                }
            }

            GameCamera.additionalPosition = cameraPosition - GameCamera.defaultPosition;
            BGEffect.transform.position = new Vector3(GameCamera.instance.transform.position.x, GameCamera.instance.transform.position.y, 0);
        }

        public void DoSpecialCamera(float beat, float length, bool returns)
        {
            if (cameraAngle == CameraAngle.Normal)
            {
                startCamSpecial = beat;
                cameraAngle = CameraAngle.Special;
            }
            wantsReturn = returns ? beat + length : Single.MaxValue;
            cameraReturnLength = Mathf.Min(2f, length*0.5f);
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

        public GameObject CreateItemInstance(float beat, string awakeAnim, KarateManPot.ItemType type = KarateManPot.ItemType.Pot, int comboId = -1)
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

        public void SetBgAndShadowCol(float beat, float length, int bgType, int shadowType, Color a, Color b, int fx, Color c)
        {
            SetBgFx(fx, beat, length);
            UpdateShadowColour(shadowType, b, c);

            this.bgType = bgType;
            if (this.bgType == (int) BackgroundType.Custom)
                bgColour = a;
            else
                bgColour = BackgroundColors[this.bgType];
            BGPlane.color = bgColour;

            if (textureFiltertype == (int) ShadowType.Tinted)
                filterColour = Color.LerpUnclamped(bgColour, ShadowBlendColor, 0.45f);
        }

        public void SetBgFx(int fx, float beat, float length)
        {
            switch (fx)
            {
                case (int) BackgroundFXType.Fade:
                    bgColourLast = bgColour;
                    bgFadeTime = beat;
                    bgFadeDuration = length;
                    break;
                default:
                    currentBgEffect = (BackgroundFXType) fx;
                    break;
            }
        }

        public void SetBgTexture(int type, int filterType, Color filterColor, Color nextColor)
        {
            textureFiltertype = filterType;
            if (textureFiltertype == (int) ShadowType.Tinted)
                filterColour = Color.LerpUnclamped(bgColour, filterColor, 0.45f);
            else
            {
                filterColour = filterColor;
                filterColourNext = nextColor;
            }
            switch (type)
            {
                case (int) BackgroundTextureType.Gradient:
                    BGGradient.SetActive(true);
                    break;
                default:
                    BGGradient.SetActive(false);
                    break;
            }
            UpdateFilterColour(bgColour, filterColour);
        }

        void UpdateFilterColour(Color bgColor, Color filterColor)
        {
            Color col;
            if (textureFiltertype == (int) ShadowType.Tinted)
                col = Color.LerpUnclamped(bgColor, ShadowBlendColor, 0.45f);
            else
                col = filterColor;
            
            bgGradientRenderer.color = col;
        }

        public static Color ShadowBlendColor = new Color(195 / 255f, 48 / 255f, 2 / 255f);
        public Color GetShadowColor(bool next = false)
        {
            Color lastCol, nextCol;
            lastCol = oldShadowColour;

            if(currentShadowType == (int) ShadowType.Custom)
                nextCol = fadeShadowColour;
            else if(bgType < (int) BackgroundType.Custom)
                nextCol = ShadowColors[bgType];
            else
                nextCol = Color.LerpUnclamped(bgColour, ShadowBlendColor, 0.45f);

            float fadeProg = Conductor.instance.GetPositionFromBeat(bgFadeTime, bgFadeDuration);
            if (fadeProg <= 1f && fadeProg >= 0)
            {
                Debug.Log(fadeProg);
                return Color.LerpUnclamped(lastCol, nextCol, fadeProg);
            }
            return next ? nextCol : lastCol;
        }

        public void UpdateShadowColour(int type, Color colour, Color fadeColour)
        {

            if(currentShadowType == (int) ShadowType.Custom)
                oldShadowColour = customShadowColour;
            else if(bgType < (int) BackgroundType.Custom)
                oldShadowColour = ShadowColors[bgType];
            else
                oldShadowColour = Color.LerpUnclamped(bgColour, ShadowBlendColor, 0.45f);

            currentShadowType = type;
            customShadowColour = colour;
            fadeShadowColour = fadeColour;
        }

        public void SetParticleEffect(float beat, int type, float windStrength, float particleStrength)
        {
            ParticleSystem.EmissionModule emm = SnowEffect.emission;
            switch (type)
            {
                case (int) ParticleType.Snow:
                    SnowEffectGO.SetActive(true);
                    SnowEffect.Play();
                    emm = SnowEffect.emission;
                    emm.rateOverTime = particleStrength * 16f;
                    break;
                case (int) ParticleType.Fire:
                    FireEffectGO.SetActive(true);
                    FireEffect.Play();
                    emm = FireEffect.emission;
                    emm.rateOverTime = particleStrength * 8f;
                    break;
                case (int) ParticleType.Rain:
                    RainEffectGO.SetActive(true);
                    RainEffect.Play();
                    emm = RainEffect.emission;
                    emm.rateOverTime = particleStrength * 32f;
                    break;
                default:
                    SnowEffect.Stop();
                    FireEffect.Stop();
                    RainEffect.Stop();
                    break;
            }
            Wind.windMain = windStrength;
        }

        public void SetFaceExpression(int face)
        {
            Joe.SetFaceExpression(face);
        }
    }
}