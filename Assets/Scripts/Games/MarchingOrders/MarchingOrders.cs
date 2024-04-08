using HeavenStudio.Common;
using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class AgbMarcherLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            RiqEntity FaceTurnUpdater(string datamodel, RiqEntity entity)
            {
                if (datamodel == "marchingOrders/face turn")
                {
                    if (entity["type2"] == (int)MarchingOrders.FaceTurnLength.Normal)
                    {
                        entity.datamodel = "marchingOrders/faceTurn";
                    }
                    else
                    {
                        entity.datamodel = "marchingOrders/faceTurnFast";
                    }
                    entity.CreateProperty("direction", entity["type"]);
                    entity.CreateProperty("point", false);
                    // don't actually need to do this here
                    //entity.version = 1;

                    entity.dynamicData.Remove("type");
                    entity.dynamicData.Remove("type2");
                    return entity;
                }
                return null;
            }
            RiqBeatmap.OnUpdateEntity += FaceTurnUpdater;

            RiqEntity MarchUpdater(string datamodel, RiqEntity entity)
            {
                if (datamodel == "marchingOrders/marching")
                {
                    entity.beat = double.NaN;
                    return entity;
                }
                return null;
            }
            RiqBeatmap.OnUpdateEntity += MarchUpdater;

            return new Minigame("marchingOrders", "Marching Orders", "ffb108", "ccc001", "f20016", false, false, new List<GameAction>()
                {
                    new GameAction("bop", "Bop")
                    {
                        function = delegate { var e = eventCaller.currentEntity; MarchingOrders.instance.BopAction(e.beat, e.length, e["bop"], e["autoBop"], e["clap"]); },
                        defaultLength = 1f,
                        resizable = true,
                        parameters = new List<Param>()
                        {
                            new Param("bop", true, "Bop", "Toggle if the cadets should bop for the duration of this event."),
                            new Param("autoBop", false, "Bop (Auto)", "Toggle if the cadets should automatically bop until another Bop event is reached."),
                            new Param("clap", false, "Clap", "Toggle if the cadets should clap instead of bop."),
                        }
                    },
                    new GameAction("attention", "Attention...")
                    {
                        function = delegate { var e = eventCaller.currentEntity; MarchingOrders.instance.SargeAttention(e.beat); },
                        defaultLength = 2f,
                        preFunction = delegate { var e = eventCaller.currentEntity; MarchingOrders.AttentionSound(e.beat);}
                    },
                    new GameAction("march", "March!")
                    {
                        function = delegate { var e = eventCaller.currentEntity; MarchingOrders.SargeMarch(e.beat, e["disableVoice"], e["shouldMarch"]); },
                        inactiveFunction = delegate { var e = eventCaller.currentEntity; MarchingOrders.SargeMarch(e.beat, e["disableVoice"], e["shouldMarch"]); },
                        defaultLength = 2f,
                        parameters = new List<Param>
                        {
                            new Param("disableVoice", false, "Mute Voice", "Toggle if the Sergeant's cue should play."),
                            new Param("shouldMarch", true, "March", "Toggle if the cadets should automatically march. If not, you should use a \"Force Marching\" event."),
                        },
                        priority = 5,
                    },
                    new GameAction("faceTurn", "Face Turn")
                    {
                        function = delegate {
                            var e = eventCaller.currentEntity;
                            MarchingOrders.instance.FaceTurn(e.beat, e["direction"], false, e["point"]);
                        },
                        defaultLength = 4f,
                        parameters = new List<Param>()
                        {
                            new Param("direction", MarchingOrders.Direction.Right, "Direction", "Choose the direction for the cadets to face."),
                            new Param("point", false, "Point", "Toggle if the cadets should both point and face the direction."),
                        }
                    },
                    new GameAction("faceTurnFast", "Fast Face Turn")
                    {
                        function = delegate {
                            var e = eventCaller.currentEntity;
                            MarchingOrders.instance.FaceTurn(e.beat, e["direction"], true, e["point"]);
                        },
                        defaultLength = 3f,
                        parameters = new List<Param>()
                        {
                            new Param("direction", MarchingOrders.Direction.Right, "Direction", "Choose the direction for the cadets to face."),
                            new Param("point", false, "Point", "Toggle if the cadets should both point and face the direction."),
                        }
                    },
                    new GameAction("halt", "Halt!")
                    {
                        function = delegate { MarchingOrders.instance.Halt(eventCaller.currentEntity.beat); },
                        defaultLength = 2f,
                        inactiveFunction = delegate { MarchingOrders.HaltSound(eventCaller.currentEntity.beat);}
                    },
                    new GameAction("go", "Go!")
                    {
                        function = delegate {
                            var e = eventCaller.currentEntity;
                            MarchingOrders.instance.MoveConveyor(e.length, e["start"], e["direction"]);
                        },
                        parameters = new List<Param>()
                        {
                            new Param("start", true, "Start Moving", "Toggle if the conveyor should start moving."),
                            new Param("direction", MarchingOrders.Direction.Right, "Direction", "Set the direction the conveyor will move."),
                        },
                        defaultLength = 7f,
                        resizable = true,
                    },
                    new GameAction("background", "Background Appearance")
                    {
                        function = delegate {
                            var e = eventCaller.currentEntity;
                            MarchingOrders.instance.BackgroundColorSet(e["preset"], e["colorFill"], e["colorTiles1"], e["colorTiles2"], e["colorTiles3"], e["colorPipes1"], e["colorPipes2"], e["colorPipes3"], e["colorConveyor1"], e["colorConveyor2"]);
                        },
                        defaultLength = 0.5f,
                        parameters = new List<Param>()
                        {
                            new Param("preset", MarchingOrders.BackgroundColor.Yellow, "Color", "Set the color of the background."),
                            new Param("colorFill", new Color(0.259f, 0.353f, 0.404f), "Wall Color", "Set the color of the wall."),
                            new Param("colorTiles1", new Color(1f, 0.76f, 0.52f), "Tile Outline Color", "Set the color of the tile outline."),
                            new Param("colorTiles2", new Color(1f, 0.6f, 0.2f), "Tile Shading Color", "Set the color of the tile shading."),
                            new Param("colorTiles3", new Color(1f, 0.675f, 0f), "Tile Fill Color", "Set the color of the tile's main color."),
                            new Param("colorPipes1", new Color(0.41f, 0.54f, 0.34f), "Pipe Outline Color", "Set the color of the pipes' outline."),
                            new Param("colorPipes2", new Color(0.43f, 0.8f, 0.45f), "Pipe Shading Color", "Set the color of the pipes' shading."),
                            new Param("colorPipes3", new Color(0.48f, 0.89f, 0.54f), "Pipe Fill Color", "Set the color of the pipes."),
                            new Param("colorConveyor1", new Color(0.157f, 0.25f, 0.3f), "Conveyor Fill Color", "Set the color of the conveyor belt."),
                            new Param("colorConveyor2", new Color(0.55f, 0.57f, 0.04f), "Conveyor Trim Color", "Set the conveyor's trim color."),
                        }
                    },

                    new GameAction("forceMarching", "Force Marching")
                    {
                        preFunction = delegate {
                            var e = eventCaller.currentEntity;
                            MarchingOrders.instance.ForceMarching(e.beat, e.length);
                        },
                        preFunctionLength = 0.2f,
                        resizable = true,
                    },
                    
                    // hidden in the editor but here cuz backwards compatibility
                    new GameAction("marching", "Start Marching (old)")
                    {
                        hidden = true,
                    },
                    new GameAction("face turn", "Direction to Turn (old)")
                    {
                        hidden = true,
                        parameters = new List<Param>()
                        {
                            new Param("type", MarchingOrders.Direction.Right, "Direction", "The direction the sergeant wants the cadets to face"),
                            new Param("type2", MarchingOrders.FaceTurnLength.Normal, "Length", "The duration of the turning event"),
                        }
                    },
                },
                new List<string>() { "agb", "normal" },
                "agbmarcher", "en",
                new List<string>() { "en", "jp" },
                chronologicalSortKey: 3
                );
        }
    }
}

namespace HeavenStudio.Games
{
    public class MarchingOrders : Minigame
    {
        public static MarchingOrders instance;

        static List<double> queuedMarches = new();

        [Header("Animators")]
        [SerializeField] Animator Sarge;
        [SerializeField] Animator Steam;
        [SerializeField] Animator[] Cadets = new Animator[3];
        [SerializeField] Animator[] CadetHeads = new Animator[3];
        [SerializeField] Animator CadetPlayer;
        [SerializeField] Animator CadetHeadPlayer;

        [Header("Colorable")]
        [SerializeField] Material[] RecolorMats;
        [SerializeField] SpriteRenderer[] BackgroundRecolorable;
        static Color BGColor1 = new Color(0.26f, 0.35f, 0.4f), BGColor2 = new Color(0.16f, 0.25f, 0.29f);

        [Header("Objects")]
        [SerializeField] ScrollObject[] ConveyorGo;

        [Header("Variables")]
        bool goBop;
        bool shouldClap;
        bool keepMarching;
        int marchOtherCount;
        int marchPlayerCount;
        double lastMissBeat;
        double lastReportedBeat;
        public static double wantMarch = double.MinValue;

        public enum Direction
        {
            Right,
            Left,
        }

        public enum FaceTurnLength
        {
            Normal,
            Fast,
        }

        public enum BackgroundColor
        {
            Yellow,
            Blue,
            Custom,
        }

        const int IAAltDownCat = IAMAXCAT;
        const int IALeft = IAMAXCAT + 1;
        const int IARight = IAMAXCAT + 2;
        protected static bool IA_PadLeft(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.Left, out dt);
        }
        protected static bool IA_BatonLeft(out double dt)
        {
            return PlayerInput.GetBatonDown(InputController.ActionsBaton.West, out dt);
        }
        protected static bool IA_TouchLeft(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Left, out dt)
                && (instance.IsExpectingInputNow(InputAction_Left) || instance.IsExpectingInputNow(InputAction_Right));
        }

        protected static bool IA_PadRight(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.Right, out dt);
        }
        protected static bool IA_BatonRight(out double dt)
        {
            return PlayerInput.GetBatonDown(InputController.ActionsBaton.East, out dt);
        }
        protected static bool IA_TouchRight(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Right, out dt)
                && (instance.IsExpectingInputNow(InputAction_Left) || instance.IsExpectingInputNow(InputAction_Right));
        }

        protected static bool IA_TouchAltPress(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Tap, out dt)
                && instance.IsExpectingInputNow(InputAction_Alt);
        }
        protected static bool IA_PadAltPress(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.South, out dt);
        }
        protected static bool IA_BatonAltPress(out double dt)
        {
            return PlayerInput.GetSqueezeDown(out dt);
        }

        public static PlayerInput.InputAction InputAction_Left =
            new("AgbMarcherLeft", new int[] { IALeft, IALeft, IALeft },
            IA_PadLeft, IA_TouchLeft, IA_BatonLeft);

        public static PlayerInput.InputAction InputAction_Right =
            new("AgbMarcherRight", new int[] { IARight, IARight, IARight },
            IA_PadRight, IA_TouchRight, IA_BatonRight);

        public static PlayerInput.InputAction InputAction_Alt =
            new("AgbMarcherAlt", new int[] { IAAltDownCat, IAAltDownCat, IAAltDownCat },
            IA_PadAltPress, IA_TouchAltPress, IA_BatonAltPress);

        void Awake()
        {
            instance = this;

            for (int i = 0; i < BackgroundRecolorable.Length; i++) BackgroundRecolorable[i].color = i == 0 ? BGColor1 : BGColor2;
        }

        void Update()
        {
            for (int i = 0; i < BackgroundRecolorable.Length; i++) BackgroundRecolorable[i].color = i == 0 ? BGColor1 : BGColor2;

            if (wantMarch != double.MinValue)
            {
                queuedMarches.Add(wantMarch);
                marchOtherCount =
                marchPlayerCount = 0;
                keepMarching = true;
                wantMarch = double.MinValue;
            }

            if (goBop && Conductor.instance.ReportBeat(ref lastReportedBeat))
            {
                foreach (var cadet in Cadets) cadet.DoScaledAnimationAsync(shouldClap ? "Clap" : "Bop", 0.5f);
                CadetPlayer.DoScaledAnimationAsync(shouldClap ? "Clap" : "Bop", 0.5f);
            }

            if (Conductor.instance.isPlaying && !Conductor.instance.isPaused)
            {
                if (queuedMarches.Count > 0)
                {
                    foreach (var march in queuedMarches)
                    {
                        BeatAction.New(instance, new List<BeatAction.Action>() {
                            new BeatAction.Action(march, delegate {
                                ScheduleInput(march, 1f, InputAction_BasicPress, MarchHit, GenericMiss, Empty);
                            }),
                            new BeatAction.Action(march + 1, delegate {
                                marchOtherCount++;
                                foreach (var cadet in Cadets) cadet.DoScaledAnimationAsync(marchOtherCount % 2 != 0 ? "MarchR" : "MarchL", 0.5f);
                                SoundByte.PlayOneShotGame("marchingOrders/stepOther");
                                if (keepMarching) queuedMarches.Add(march + 1);
                            }),
                        });
                    }
                    queuedMarches.Clear();
                }
            }

            if (ConveyorGo[0].AutoScroll && (ConveyorGo[1].gameObject.transform.position.x < 0))
            {
                foreach (var scroll in ConveyorGo) scroll.AutoScroll = false;
                ConveyorGo[0].gameObject.transform.position = new Vector3(6.181f, -3.37f);
                ConveyorGo[1].gameObject.transform.position = new Vector3(0, 0);
            }

            // input stuff below

            if (PlayerInput.GetIsAction(InputAction_BasicPress) && !IsExpectingInputNow(InputAction_BasicPress))
            {
                if (PlayerInput.CurrentControlStyle == InputController.ControlStyles.Touch)
                {
                    if (!(IsExpectingInputNow(InputAction_Alt)
                        || IsExpectingInputNow(InputAction_Left)
                        || IsExpectingInputNow(InputAction_Right)))
                    {
                        Miss();
                        marchPlayerCount++;
                        CadetPlayer.DoScaledAnimationAsync(marchPlayerCount % 2 != 0 ? "MarchR" : "MarchL", 0.5f);
                    }
                }
                else
                {
                    Miss();
                    marchPlayerCount++;
                    CadetPlayer.DoScaledAnimationAsync(marchPlayerCount % 2 != 0 ? "MarchR" : "MarchL", 0.5f);
                }
            }

            if (PlayerInput.GetIsAction(InputAction_Alt) && !IsExpectingInputNow(InputAction_Alt))
            {
                Miss();
                CadetPlayer.DoScaledAnimationAsync("Halt", 0.5f);
            }

            if (PlayerInput.GetIsAction(InputAction_Left) && !IsExpectingInputNow(InputAction_Left))
            {
                Miss();
                CadetHeadPlayer.DoScaledAnimationAsync("FaceL", 0.5f);
            }

            if (PlayerInput.GetIsAction(InputAction_Right) && !IsExpectingInputNow(InputAction_Right))
            {
                Miss();
                CadetHeadPlayer.DoScaledAnimationAsync("FaceR", 0.5f);
            }
        }

        public void LeftSuccess(PlayerActionEvent caller, float state)
        {
            TurnSuccess(state, "L");
        }

        public void RightSuccess(PlayerActionEvent caller, float state)
        {
            TurnSuccess(state, "R");
        }

        public void LeftPointSuccess(PlayerActionEvent caller, float state)
        {
            TurnSuccess(state, "L", true);
        }

        public void RightPointSuccess(PlayerActionEvent caller, float state)
        {
            TurnSuccess(state, "R", true);
        }

        void TurnSuccess(float state, string dir, bool shouldPoint = false)
        {
            if (state <= -1f || state >= 1f) SoundByte.PlayOneShot("nearMiss");
            else SoundByte.PlayOneShotGame("marchingOrders/turnActionPlayer");

            CadetHeadPlayer.DoScaledAnimationAsync("Face" + dir, 0.5f);
            if (shouldPoint) CadetPlayer.DoScaledAnimationAsync("Point" + dir, 0.5f);
        }

        public void GenericMiss(PlayerActionEvent caller)
        {
            if (Conductor.instance.songPositionInBeatsAsDouble - lastMissBeat <= 1.1f) return;
            Miss();
        }

        public void Miss()
        {
            lastMissBeat = Conductor.instance.songPositionInBeatsAsDouble;
            SoundByte.PlayOneShot("miss");
            Sarge.DoScaledAnimationAsync("Anger", 0.5f);
            Steam.DoScaledAnimationAsync("Steam", 0.5f);
        }

        public void MarchHit(PlayerActionEvent caller, float state)
        {
            if (state <= -1f || state >= 1f) SoundByte.PlayOneShot("nearMiss");
            else SoundByte.PlayOneShotGame("marchingOrders/stepPlayer", volume: 0.25f);
            marchPlayerCount++;
            CadetPlayer.DoScaledAnimationAsync(marchPlayerCount % 2 != 0 ? "MarchR" : "MarchL", 0.5f);
        }

        public void HaltHit(PlayerActionEvent caller, float state)
        {
            if (state <= -1f || state >= 1f) SoundByte.PlayOneShot("nearMiss");
            else SoundByte.PlayOneShotGame("marchingOrders/stepPlayer", volume: 0.25f);

            CadetPlayer.DoScaledAnimationAsync("Halt", 0.5f);
        }

        public void BopAction(double beat, float length, bool shouldBop, bool autoBop, bool clap)
        {
            goBop = autoBop;
            shouldClap = clap;
            if (shouldBop)
            {
                for (int i = 0; i < length; i++)
                {
                    BeatAction.New(instance, new List<BeatAction.Action>() {
                        new BeatAction.Action(beat + i, delegate {
                            foreach (var cadet in Cadets) cadet.DoScaledAnimationAsync(shouldClap ? "Clap" : "Bop", 0.5f);
                            CadetPlayer.DoScaledAnimationAsync(shouldClap ? "Clap" : "Bop", 0.5f);
                        })
                    });
                }
            }
        }

        public void SargeAttention(double beat)
        {
            BeatAction.New(this, new List<BeatAction.Action>() {
                new BeatAction.Action(beat + 0.25f, delegate { Sarge.DoScaledAnimationAsync("Talk", 0.5f);}),
            });
        }

        public static void SargeMarch(double beat, bool noVoice, bool march)
        {
            if (march) MarchingOrders.wantMarch = beat + 1;
            if (!noVoice) PlaySoundSequence("marchingOrders", "susume", beat);

            if (GameManager.instance.currentGame == "marchingOrders")
            {
                MarchingOrders.instance.PreMarch(beat);
                if (!noVoice) MarchingOrders.instance.Sarge.DoScaledAnimationAsync("Talk", 0.5f);
            }
        }

        public void ForceMarching(double beat, float length)
        {
            for (int i = 0; i < length; i++)
            {
                ScheduleInput(beat + i - 0.2f, 0.2f, InputAction_BasicPress, MarchHit, GenericMiss, Empty);
                BeatAction.New(instance, new List<BeatAction.Action>() {
                    new BeatAction.Action(beat + i, delegate {
                        marchOtherCount++;
                        foreach (var cadet in Cadets) cadet.DoScaledAnimationAsync(marchOtherCount % 2 != 0 ? "MarchR" : "MarchL", 0.5f);
                        SoundByte.PlayOneShotGame("marchingOrders/stepOther");
                    }),
                });
            }
        }

        public void PreMarch(double beat)
        {
            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 1f, delegate {
                    foreach (var cadet in Cadets) cadet.DoScaledAnimationAsync("MarchL", 0.5f);
                    CadetPlayer.DoScaledAnimationAsync("MarchL", 0.5f);
                }),
            });
        }

        public void Halt(double beat)
        {
            keepMarching = false;
            HaltSound(beat);

            ScheduleInput(beat, 1f, InputAction_Alt, HaltHit, GenericMiss, Empty);
            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { Sarge.DoScaledAnimationAsync("Talk", 0.5f);}),
                new BeatAction.Action(beat + 1f, delegate { foreach (var cadet in Cadets) cadet.DoScaledAnimationAsync("Halt", 0.5f);}),
            });
        }

        public void FaceTurn(double beat, int direction, bool isFast, bool shouldPoint)
        {
            // x is true if the direction is right
            bool x = (direction == 0);
            int turnLength = (isFast ? 0 : 1);

            ScheduleInput(beat, turnLength + 2f, x ? InputAction_Right : InputAction_Left, x ? (shouldPoint ? RightPointSuccess : RightSuccess) : (shouldPoint ? LeftPointSuccess : LeftSuccess), GenericMiss, Empty);
            MultiSound.Play(new MultiSound.Sound[] {
                new MultiSound.Sound($"marchingOrders/{(x ? "right" : "left")}FaceTurn1{(isFast ? "fast" : "")}", beat),
                new MultiSound.Sound($"marchingOrders/{(x ? "right" : "left")}FaceTurn2{(isFast ? "fast" : "")}", beat + 0.5f),
                new MultiSound.Sound($"marchingOrders/{(x ? "right" : "left")}FaceTurn3", beat + turnLength + 1f),
                new MultiSound.Sound("marchingOrders/turnAction", beat + turnLength + 2f),
            }, forcePlay: true);

            BeatAction.New(this, new List<BeatAction.Action>() {
                new BeatAction.Action(beat + turnLength + 2f,delegate {
                    if (shouldPoint) foreach (var cadet in Cadets) cadet.DoScaledAnimationAsync($"Point{(x ? "R" : "L")}", 0.5f);
                    foreach (var head in CadetHeads) head.DoScaledAnimationAsync($"Face{(x ? "R" : "L")}", 0.5f);
                })
            });

            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { Sarge.DoScaledAnimationAsync("Talk", 0.5f);}),
                new BeatAction.Action(beat + turnLength + 1f, delegate { Sarge.DoScaledAnimationAsync("Talk", 0.5f);}),
            });
        }

        public void BackgroundColorSet(int preset, Color fill, Color tiles1, Color tiles2, Color tiles3, Color pipes1, Color pipes2, Color pipes3, Color conveyor1, Color conveyor2)
        {
            if (preset == 2) UpdateMaterialColor(fill, tiles1, tiles2, tiles3, pipes1, pipes2, pipes3, conveyor1, conveyor2);
            else
            {
                bool x = preset == 0;
                UpdateMaterialColor(
                    x ? new Color(0.26f, 0.36f, 0.39f) : new Color(0.25f, 0.45f, 0.52f),
                    x ? new Color(1f, 0.76f, 0.52f) : new Color(0.45f, 0.71f, 0.81f),
                    x ? new Color(1f, 0.6f, 0.2f) : new Color(0.65f, 0.87f, 0.94f),
                    x ? new Color(1f, 0.68f, 0f) : new Color(0.65f, 0.87f, 0.94f),
                    x ? new Color(0.41f, 0.54f, 0.34f) : new Color(0.36f, 0.58f, 0.64f),
                    x ? new Color(0.43f, 0.8f, 0.45f) : new Color(0.48f, 0.65f, 0.71f),
                    x ? new Color(0.48f, 0.89f, 0.54f) : new Color(0.48f, 0.65f, 0.71f),
                    x ? new Color(0.16f, 0.25f, 0.3f) : new Color(0.32f, 0.55f, 0.62f),
                    x ? new Color(0.55f, 0.57f, 0.04f) : new Color(0.17f, 0.31f, 0.35f)
                );
            }
        }

        public void UpdateMaterialColor(Color fill, Color tiles1, Color tiles2, Color tiles3, Color pipes1, Color pipes2, Color pipes3, Color conveyor1, Color conveyor2)
        {
            BGColor1 = fill;
            BGColor2 = conveyor1;

            Recolor(0, tiles3, tiles2, tiles1);
            Recolor(1, pipes2, pipes1, pipes3);
            Recolor(2, new Color(0, 0, 0), conveyor1, conveyor2);

            void Recolor(int i, Color color1, Color color2, Color color3)
            {
                RecolorMats[i].SetColor("_ColorAlpha", color1);
                RecolorMats[i].SetColor("_ColorBravo", color2);
                RecolorMats[i].SetColor("_ColorDelta", color3);
            }
        }

        public static void AttentionSound(double beat)
        {
            PlaySoundSequence("marchingOrders", "zentai", beat - 1);
        }

        public static void HaltSound(double beat)
        {
            PlaySoundSequence("marchingOrders", "tomare", beat);
        }

        public void MoveConveyor(float length, bool go, int direction)
        {
            foreach (var scroll in ConveyorGo)
            {
                scroll.SpeedMod = ((direction == 0 ? 20 : -20) / length) * (Conductor.instance.songBpm / 100);
                scroll.AutoScroll = go;
            }
        }

        static void Empty(PlayerActionEvent caller) { }
    }
}

