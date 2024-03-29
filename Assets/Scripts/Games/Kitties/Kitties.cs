using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class CtrTeppanLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("kitties", "Kitties!", "ffffff", false, false, new List<GameAction>()
            {
                new GameAction("clap", "Cat Clap")
                {
                    function = delegate { Kitties.instance.Clap(eventCaller.currentEntity["toggle"], eventCaller.currentEntity["toggle1"], eventCaller.currentEntity["toggle2"],
                        eventCaller.currentEntity.beat,  eventCaller.currentEntity["type"]); },

                    defaultLength = 4f,
                    parameters = new List<Param>()
                    {
                        new Param("type", Kitties.SpawnType.Straight, "Direction", "Choose the direction that the kitties will spawn in."),
                        new Param("toggle", false, "Mice", "Toggle if the non-player kitties should be replaced with mice."),
                        new Param("toggle1", false, "Invert Direction", "Toggle if the spawn direction should be inverted."),
                        new Param("toggle2", false, "Keep Cats Spawned", "Toggle if the kitties should stay spawned after their cue. This is required for some other events."),
                    }
                },

                new GameAction("roll", "Roll")
                    {
                        function = delegate { Kitties.instance.Roll(eventCaller.currentEntity["toggle"], eventCaller.currentEntity.beat);  },

                        defaultLength = 4f,

                        parameters = new List<Param>()
                        {
                            new Param("toggle", false, "Keep Cats Spawned", "Toggle if the kitties should stay spawned after their cue. This is required for some other events."),
                        }
                    },

                new GameAction ("fish", "Fish")
                {
                    function = delegate { Kitties.instance.CatchFish(eventCaller.currentEntity.beat); },
                    defaultLength = 4f,
                },

                new GameAction("instantSpawn", "Instant Spawn")
                {
                    function = delegate { Kitties.instance.InstantSpawn(eventCaller.currentEntity["toggle"], eventCaller.currentEntity["toggle1"],
                        eventCaller.currentEntity.beat,  eventCaller.currentEntity["type"]); },

                    defaultLength = .5f,
                    parameters = new List<Param>()
                    {
                        new Param("type", Kitties.SpawnType.Straight, "Direction", "Choose the direction that the kitties will spawn in."),
                        new Param("toggle", false, "Mice", "Toggle if the non-player kitties should be replaced with mice."),
                        new Param("toggle1", false, "Invert Direction", "Toggle if the spawn direction should be inverted."),
                        new Param("toggle2", false, "Keep Cats Spawned", "Toggle if the kitties should stay spawned after their cue. This is required for some other events."),
                    }
                },

                new GameAction("bgcolor", "Background Appearance")
                {
                    function = delegate
                    {
                        var e = eventCaller.currentEntity;
                        Kitties.instance.BackgroundColor(e.beat, e.length, e["colorStart"], e["colorEnd"], e["ease"]);
                    },
                    defaultLength = 4,
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("colorStart", Color.white, "Start Color", "Set the color at the start of the event."),
                        new Param("colorEnd", Color.white, "End Color", "Set the color at the end of the event."),
                        new Param("ease", Util.EasingFunction.Ease.Linear, "Ease", "Set the easing of the action.")
                    }
                }
            },
            new List<string>() { "ctr", "normal" },
            "ctrteppan", "en",
            new List<string>() { },
            chronologicalSortKey: 74
            );
        }
    }
}
namespace HeavenStudio.Games
{
    using Scripts_Kitties;
    public class Kitties : Minigame
    {
        public CtrTeppanPlayer player;
        public Animator[] kitties;
        public GameObject[] Cats;

        public GameObject Fish;
        public bool isInverted;
        public SpriteRenderer background;

        public Vector3[] positions;
        public float[] rotationAngles;

        public enum SpawnType
        {
            Straight,
            DownDiagonal,
            UpDiagonal,
            CloseUp
        }

        public static Kitties instance;

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
        protected static bool IA_TouchAltPress(out double dt)
        {
            return PlayerInput.GetTouchDown(InputController.ActionsTouch.Tap, out dt)
                && instance.IsExpectingInputNow(InputAction_AltStart);
        }

        protected static bool IA_PadAltRelease(out double dt)
        {
            return PlayerInput.GetPadUp(InputController.ActionsPad.South, out dt);
        }
        protected static bool IA_BatonAltRelease(out double dt)
        {
            return PlayerInput.GetSqueezeUp(out dt);
        }

        public static PlayerInput.InputAction InputAction_AltStart =
            new("CtrTeppanAltStart", new int[] { IAAltDownCat, IAAltDownCat, IAAltDownCat },
            IA_PadAltPress, IA_TouchAltPress, IA_BatonAltPress);
        public static PlayerInput.InputAction InputAction_AltFinish =
            new("CtrTeppanAltFinish", new int[] { IAAltUpCat, IAFlickCat, IAAltUpCat },
            IA_PadAltRelease, IA_TouchFlick, IA_BatonAltRelease);
        public static PlayerInput.InputAction InputAction_TouchRelease =
            new("CtrTeppanTouchRelease", new int[] { IAEmptyCat, IAReleaseCat, IAEmptyCat },
            IA_Empty, IA_TouchBasicRelease, IA_Empty);

        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            BackgroundColorUpdate();
        }

        public void Clap(bool isMice, bool isInverse, bool keepSpawned, double beat, int type)
        {
            player.ScheduleClap(beat, type);
            MultiSound.Play(new MultiSound.Sound[] {
                new MultiSound.Sound("kitties/nya1", beat),
                new MultiSound.Sound("kitties/nya2", beat + .75f),
                new MultiSound.Sound("kitties/nya3", beat + 1.5f)
            });

            if (type == 3)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                    new BeatAction.Action(beat, delegate { Spawn(type, 0, isMice, isInverse, true, false);}),
                    new BeatAction.Action(beat + 2.5f, delegate { kitties[0].Play("FaceClap", 0, 0);}),
                    new BeatAction.Action(beat + 3f, delegate { kitties[0].Play("FaceClap", 0, 0);}),
                });

                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + .75f, delegate { Spawn(type, 1, isMice, isInverse, false, false);}),
                new BeatAction.Action(beat + 2.5f, delegate { kitties[1].Play("FaceClap", 0, 0);}),
                new BeatAction.Action(beat + 3f, delegate { kitties[1].Play("FaceClap", 0, 0);}),
                });

                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 1.5f, delegate { Spawn(type, 2, isMice, isInverse, false, false);}),
                new BeatAction.Action(beat + 1.5f, delegate { player.canClap = true;}),
                });
            }

            else if (!isMice)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                    new BeatAction.Action(beat, delegate { Spawn(type, 0, isMice, isInverse, true, false);}),
                    new BeatAction.Action(beat + 2.5f, delegate { kitties[0].Play("Clap1", 0, 0);}),
                    new BeatAction.Action(beat + 3f, delegate { kitties[0].Play("Clap2", 0, 0);}),
                });

                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + .75f, delegate { Spawn(type, 1, isMice, isInverse, false, false);}),
                new BeatAction.Action(beat + 2.5f, delegate { kitties[1].Play("Clap1", 0, 0);}),
                new BeatAction.Action(beat + 3f, delegate { kitties[1].Play("Clap2", 0, 0);}),
                });

                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 1.5f, delegate { Spawn(type, 2, isMice, isInverse, false, false);}),
                new BeatAction.Action(beat + 1.5f, delegate { player.canClap = true;}),
                });

            }
            else
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                    new BeatAction.Action(beat, delegate { Spawn(type, 0, isMice, isInverse, true, false);}),
                    new BeatAction.Action(beat + 2.5f, delegate { kitties[0].Play("MiceClap1", 0, 0);}),
                    new BeatAction.Action(beat + 3f, delegate { kitties[0].Play("MiceClap2", 0, 0);}),
                });

                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + .75f, delegate { Spawn(type, 1, isMice, isInverse, false, false);}),
                new BeatAction.Action(beat + 2.5f, delegate { kitties[1].Play("MiceClap1", 0, 0);}),
                new BeatAction.Action(beat + 3f, delegate { kitties[1].Play("MiceClap2", 0, 0);}),
                });

                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 1.5f, delegate { Spawn(type, 2, isMice, isInverse, false, false);}),
                new BeatAction.Action(beat + 1.5f, delegate { player.canClap = true;}),
                });

            }

            if (!keepSpawned)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                    new BeatAction.Action(beat + 3.5f, delegate { RemoveCats(false);})
                });
            }
        }

        public void Roll(bool keepSpawned, double beat)
        {
            if (!player.canClap)
                return;
            player.ScheduleRoll(beat);
            MultiSound.Play(new MultiSound.Sound[] {
                new MultiSound.Sound("kitties/roll1", beat),
                new MultiSound.Sound("kitties/roll2", beat + .5f),
                new MultiSound.Sound("kitties/roll3", beat + 1f),
                new MultiSound.Sound("kitties/roll4", beat + 1.5f)

            });
            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { kitties[0].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + .5f, delegate { kitties[0].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 1f, delegate { kitties[0].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 1.5f, delegate { kitties[0].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 2f, delegate { kitties[0].Play("Rolling", 0, 0); }),
                    new BeatAction.Action(beat + 2.75f, delegate { kitties[0].Play("RollEnd", 0, 0); })
                });

            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { kitties[1].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + .5f, delegate { kitties[1].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 1f, delegate { kitties[1].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 1.5f, delegate { kitties[1].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 2f, delegate { kitties[1].Play("Rolling", 0, 0); }),
                    new BeatAction.Action(beat + 2.75f, delegate { kitties[1].Play("RollEnd", 0, 0); })
                    });

            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat, delegate { kitties[2].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + .5f, delegate { kitties[2].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 1f, delegate { kitties[2].Play("RollStart", 0, 0); }),
                    new BeatAction.Action(beat + 1.5f, delegate { kitties[2].Play("RollStart", 0, 0); })
                });

            if (!keepSpawned)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
            {
                    new BeatAction.Action(beat + 3.5f, delegate { RemoveCats(false);})
                });
            }
        }

        public void CatchFish(double beat)
        {
            //if (!player.canClap)
            //    return;

            player.ScheduleFish(beat);
            MultiSound.Play(new MultiSound.Sound[] {
                new MultiSound.Sound("kitties/fish1", beat + 2f),
                new MultiSound.Sound("kitties/fish2", beat + 2.25f),
                new MultiSound.Sound("kitties/fish3", beat + 2.5f),

            });

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { if (isInverted)
                    Fish.transform.localScale = new Vector3(-1f, 1f, 1f);
                    else
                        Fish.transform.localScale = new Vector3(1f, 1f, 1f); }),
                new BeatAction.Action(beat, delegate { Fish.SetActive(true); }),
                new BeatAction.Action(beat, delegate { Fish.GetComponent<Animator>().DoScaledAnimationAsync("FishDangle", 0.5f); }),
                new BeatAction.Action(beat + 2f, delegate { kitties[0].Play("FishNotice", 0, 0);  }),
                new BeatAction.Action(beat + 2.25f, delegate { kitties[1].Play("FishNotice2", 0, 0);  }),
                new BeatAction.Action(beat + 2.5f, delegate { kitties[2].Play("FishNotice3", 0, 0);  })
                });

            BeatAction.New(instance, new List<BeatAction.Action>()
            {
                    new BeatAction.Action(beat + 4f, delegate { Fish.SetActive(false);})
                });


        }

        public void Spawn(int pos, int catNum, bool isMice, bool isInverse, bool firstSpawn, bool instant)
        {
            ResetRotation(catNum);
            if (firstSpawn)
            {
                isInverted = isInverse;
                switch (pos)
                {
                    case 0:

                        if (!isInverse)
                        {
                            positions[0] = new Vector3(-5.11f, -1.25f, 0f);
                            positions[1] = new Vector3(.32f, -1.25f, 0f);
                            positions[2] = new Vector3(5.75f, -1.25f, 0f);
                        }
                        else
                        {
                            positions[0] = new Vector3(5.75f, -1.25f, 0f);
                            positions[1] = new Vector3(.32f, -1.25f, 0f);
                            positions[2] = new Vector3(-5.11f, -1.5f, 0f);
                        }
                        break;

                    case 1:

                        if (!isInverse)
                        {
                            positions[0] = new Vector3(-6.61f, 1.75f, 6f);
                            positions[1] = new Vector3(.32f, -.25f, 2f);
                            positions[2] = new Vector3(4.25f, -1.75f, -2f);
                        }
                        else
                        {
                            positions[0] = new Vector3(6.61f, 1.75f, 6f);
                            positions[1] = new Vector3(.32f, -.25f, 2f);
                            positions[2] = new Vector3(-4.25f, -1.75f, -2f);
                        }
                        break;

                    case 2:

                        if (!isInverse)
                        {
                            positions[0] = new Vector3(4.25f, -1.75f, -2f);
                            positions[1] = new Vector3(.32f, -.25f, 2f);
                            positions[2] = new Vector3(-6.61f, 1.75f, 6f);
                        }
                        else
                        {
                            positions[0] = new Vector3(-4.25f, -1.75f, -2f);
                            positions[1] = new Vector3(.32f, -.25f, 2f);
                            positions[2] = new Vector3(6.61f, 1.75f, 6f);
                        }
                        break;

                    case 3:
                        if (firstSpawn)
                        {
                            rotationAngles = new float[] { -135f, 135f, 0f };
                            positions[0] = new Vector3(-8.21f, 3.7f, 0f);
                            positions[1] = new Vector3(7.51f, 4.2f, 0f);
                            positions[2] = new Vector3(.32f, -4.25f, 0f);
                        }
                        break;


                    default:
                        break;
                }
            }
            Cats[catNum].transform.position = positions[catNum];
            if (pos != 3)
            {
                if (!isInverse)
                    Cats[catNum].transform.localScale = new Vector3(1f, 1f, 1f);
                else
                    Cats[catNum].transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            else
            {
                var rotationVector = Cats[catNum].transform.rotation.eulerAngles;
                rotationVector.z = rotationAngles[catNum];
                Cats[catNum].transform.rotation = Quaternion.Euler(rotationVector);
                Cats[catNum].transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            Cats[catNum].transform.GetChild(0).gameObject.SetActive(true);

            if (!instant)
            {
                if (pos == 3)
                {
                    kitties[catNum].Play("FacePopIn", 0, 0);
                }
                else if (!isMice)
                    kitties[catNum].Play("PopIn", 0, 0);
                else if (catNum < 2)
                {
                    kitties[catNum].Play("MicePopIn", 0, 0);
                }
                else
                    kitties[catNum].Play("PopIn", 0, 0);
            }
            else
            {
                if (pos == 3)
                    kitties[catNum].Play("FaceIdle", 0, 0);
                else if (!isMice)
                    kitties[catNum].Play("Idle", 0, 0);
                else if (catNum < 2)
                    kitties[catNum].Play("MiceIdle", 0, 0);
                else
                    kitties[catNum].Play("Idle", 0, 0);
            }
        }

        public void ResetRotation(int catNum)
        {
            var rotationVector = Cats[0].transform.rotation.eulerAngles;
            rotationVector.z = 0;
            Cats[catNum].transform.rotation = Quaternion.Euler(rotationVector);
        }

        public void RemoveCats(bool fishing)
        {
            if (!fishing)
            {
                for (int x = 0; x < 3; x++)
                {
                    Cats[x].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
                Fish.SetActive(false);
            player.canClap = false;
        }

        public void InstantSpawn(bool isMice, bool isInverse, double beat, int pos)
        {
            BeatAction.New(instance, new List<BeatAction.Action>()
                {
                new BeatAction.Action(beat, delegate { Spawn(pos, 0, isMice, isInverse, true, true); }),
                new BeatAction.Action(beat, delegate { Spawn(pos, 1, isMice, isInverse, true, true); }),
                new BeatAction.Action(beat, delegate { Spawn(pos, 2, isMice, isInverse, true, true); })
            });
            player.canClap = true;
        }

        private ColorEase bgColorEase = new(Color.white);

        //call this in update
        private void BackgroundColorUpdate()
        {
            background.color = bgColorEase.GetColor();
        }

        public void BackgroundColor(double beat, float length, Color colorStartSet, Color colorEndSet, int ease)
        {
            bgColorEase = new ColorEase(beat, length, colorStartSet, colorEndSet, ease);
        }

        //call this in OnPlay(double beat) and OnGameSwitch(double beat)
        private void PersistColor(double beat)
        {
            var allEventsBeforeBeat = EventCaller.GetAllInGameManagerList("kitties", new string[] { "bgcolor" }).FindAll(x => x.beat < beat);
            if (allEventsBeforeBeat.Count > 0)
            {
                allEventsBeforeBeat.Sort((x, y) => x.beat.CompareTo(y.beat)); //just in case
                var lastEvent = allEventsBeforeBeat[^1];
                BackgroundColor(lastEvent.beat, lastEvent.length, lastEvent["colorStart"], lastEvent["colorEnd"], lastEvent["ease"]);
            }
        }

        public override void OnPlay(double beat)
        {
            PersistColor(beat);
        }

        public override void OnGameSwitch(double beat)
        {
            PersistColor(beat);
        }
    }
}