using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Util;
using HeavenStudio.InputSystem;
using TMPro;
using Jukebox;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class CtrFirstContact
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("firstContact", "Second Contact", "1f3833", false, false, new List<GameAction>()
            {
                new GameAction("beat intervals", "Start Interval")
                {
                    preFunction = delegate { var e = eventCaller.currentEntity; FirstContact.PreInterval(e.beat, e.length, e["dialogue"], e["auto"]);   },
                    parameters = new List<Param>()
                    {
                        new Param("dialogue", "REPLACE THIS", "Mistranslation Dialogue", "Set the line to use when messing up the translation."),
                        new Param("auto", true, "Auto Pass Turn", "Toggle if the turn should be passed automatically at the end of the start interval.")
                    },
                    defaultLength = 3f,
                    resizable = true,
                    priority = 2,
                },
                new GameAction("alien speak", "Bob Speak")
                {
                    defaultLength = 0.5f,
                    parameters = new List<Param>()
                    {
                        new Param("spaceNum", new EntityTypes.Integer(0, 12, 0), "Amount Of Spaces", "Choose the amount of spaces to add before the icon."),
                        new Param("dotdotdot", false, "Ellipses", "Toggle if the symbol should have \"...\" before it."),
                        new Param("newline", false, "New line", "Toggle if this text starts a new line."),
                        new Param("dialogue", "", "Translation", "Set the text that this syllable will translate to. Spaces will not be automatically added.")
                    },
                    priority = 1
                },
                new GameAction("alien turnover", "Pass Turn")
                {
                    function = delegate { FirstContact.instance.PassTurnStandalone(eventCaller.currentEntity.beat);  },
                    resizable = true,
                },
                new GameAction("alien success", "Success")
                {
                    function = delegate { FirstContact.instance.AlienSuccess(eventCaller.currentEntity.beat);  },
                },
                new GameAction("mission control", "Show Mission Control")
                {
                    function = delegate { var e = eventCaller.currentEntity; FirstContact.instance.MissionControlDisplay(e.beat, e["toggle"], e.length);  },
                    resizable = true,
                    parameters = new List<Param>
                    {
                        new Param("toggle", false, "Stay", "Toggle if the camera should stay on mission control even after the event has ended. This is usually used for the end of a level.")
                    }
                },
                new GameAction("look at", "Look")
                {
                    function = delegate { FirstContact.instance.LookAtDirection(eventCaller.currentEntity["type"], eventCaller.currentEntity["type2"]);  },
                    defaultLength = .5f,
                    parameters = new List<Param>()
                    {
                        new Param("type", FirstContact.alienLookAt.lookAtAlien, "Farmer Bob", "Set where Farmer Bob will look."),
                        new Param("type2", FirstContact.translatorLookAt.lookAtBob, "Alien", "Set where the alien will look."),
                    }
                },
                new GameAction("live bar beat", "\"Live\" Bar")
                {
                    function = delegate { FirstContact.instance.LiveBarBeat(eventCaller.currentEntity["toggle"]);  },
                    defaultLength = .5f,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", true, "On Beat", "Toggle if the \"Live\" bar in the top-left will animate on onbeats or offbeats.")
                    }
                }
            },
            new List<string>() { "ctr", "repeat" },
            "ctrinterpreter", "en",
            new List<string>() { },
            chronologicalSortKey: 13
            );
        }
    }
}

namespace HeavenStudio.Games
{
    //using Scripts_FirstContact;

    public class FirstContact : Minigame
    {
        const string MID_MSG_MISS = "<color=\"red\"> ..? </color>";
        const string MSG_ALIEN = "<sprite name=\"AlienIcn\">";
        const string MSG_MAN = "<sprite name=\"ManIcn\">";
        const string MSG_MAN_DDD = "...<sprite name=\"ManIcn\">";
        // I should add a DonkTroll sprite 🫰🫰🫰🫰🫰

        public static FirstContact instance { get; private set; }

        //[Header("Properties")]
        private bool hasMissed;
        private double lastReportedBeat = 0;

        [Header("Components")]
        [SerializeField] Animator alien;
        [SerializeField] Animator translator;
        //[SerializeField] GameObject alienSpeech;
        [SerializeField] GameObject missionControl;
        [SerializeField] Animator liveBar;

        [SerializeField] GameObject alienTextbox;
        [SerializeField] TMP_Text alienText;
        [SerializeField] GameObject translateTextbox;
        [SerializeField] TMP_Text translateText;
        [SerializeField] GameObject translateFailTextbox;
        [SerializeField] TMP_Text translateFailText;

        [Header("Variables")]
        int currentVoicelineIndex = -1;
        public bool noHitOnce, isSpeaking;
        //public int version;
        public float lookAtLength = 1f;
        bool onBeat;
        float liveBarBeatOffset;

        string onOutDialogue = "YOU SUCK AT CHARTING";
        string callDiagBuffer = "";
        string respDiagBuffer = "";
        List<(string, bool)> callDiagList = new();
        int callDiagIndex = 0;
        private struct QueuedInterval
        {
            public double beat;
            public float interval;
            public bool autoPassTurn;
            public string outDialogue;
        }
        private static List<QueuedInterval> queuedIntervals = new List<QueuedInterval>();

        //public enum VersionOfContact
        //{
        //    FirstContact,
        //    CitrusRemix,
        //    SecondContact
        //}

        public enum alienLookAt
        {
            lookAtAlien,
            idle
        }

        public enum translatorLookAt
        {
            lookAtBob,
            idle
        }

        protected static bool IA_PadAny(out double dt)
        {
            return PlayerInput.GetPadDown(InputController.ActionsPad.East, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Up, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Down, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Left, out dt)
                    || PlayerInput.GetPadDown(InputController.ActionsPad.Right, out dt);
        }

        public static PlayerInput.InputAction InputAction_Press =
            new("CtrInterpreterPress", new int[] { IAPressCat, IAFlickCat, IAPressCat },
            IA_PadAny, IA_TouchBasicPress, IA_BatonBasicPress);

        void OnDestroy()
        {
            foreach (var evt in scheduledInputs)
            {
                evt.Disable();
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            callDiagBuffer = "";
            respDiagBuffer = "";
            callDiagList.Clear();
            callDiagIndex = 0;

            alienTextbox.SetActive(false);
            translateTextbox.SetActive(false);
            translateFailTextbox.SetActive(false);
        }

        private List<RiqEntity> GetAllSpeaksInBetweenBeat(double beat, double endBeat)
        {
            List<RiqEntity> speakEvents = EventCaller.GetAllInGameManagerList("firstContact", new string[] { "alien speak" });
            List<RiqEntity> tempEvents = new();

            foreach (var entity in speakEvents)
            {
                if (entity.beat >= beat && entity.beat < endBeat)
                {
                    tempEvents.Add(entity);
                }
            }
            return tempEvents;
        }

        private RiqEntity GetLastIntervalBeforeBeat(double beat)
        {
            List<RiqEntity> intervalEvents = EventCaller.GetAllInGameManagerList("firstContact", new string[] { "beat intervals" });
            if (intervalEvents.Count == 0) return null;
            var tempEvents = intervalEvents.FindAll(x => x.beat <= beat);
            tempEvents.Sort((x, y) => x.beat.CompareTo(y.beat));
            return tempEvents[^1];
        }

        public static void PreInterval(double beat, float interval, string outDialogue, bool autoPassTurn)
        {
            if (GameManager.instance.currentGame == "firstContact")
            {
                instance.SetIntervalStart(beat, interval, outDialogue, beat, autoPassTurn);
            }
            else
            {
                queuedIntervals.Add(new QueuedInterval
                {
                    beat = beat,
                    interval = interval,
                    outDialogue = outDialogue,
                    autoPassTurn = autoPassTurn
                });
            }
        }

        private void SetIntervalStart(double beat, float interval, string outDialogue, double gameSwitchBeat, bool autoPassTurn)
        {
            List<RiqEntity> relevantSpeakEvents = GetAllSpeaksInBetweenBeat(beat, beat + interval);
            relevantSpeakEvents.Sort((x, y) => x.beat.CompareTo(y.beat));
            List<BeatAction.Action> queuedSpeaks = new()
            {
                new BeatAction.Action(beat, delegate
                {
                    translator.Play("translator_lookAtAlien", 0, 0);

                    onOutDialogue = outDialogue;
                    callDiagBuffer = "";
                    respDiagBuffer = "";
                    callDiagList.Clear();
                    callDiagIndex = 0;

                    alienText.text = "";
                    translateText.text = "";
                    translateFailText.text = "";

                    alienTextbox.SetActive(false);
                    translateTextbox.SetActive(false);
                    translateFailTextbox.SetActive(false);
                    if (autoPassTurn)
                    {
                        AlienTurnOver(beat + interval, beat, beat + interval, 1);
                    }
                })
            };
            for (int i = 0; i < relevantSpeakEvents.Count; i++)
            {
                RiqEntity speakEventToCheck = relevantSpeakEvents[i];
                if (speakEventToCheck.beat >= gameSwitchBeat)
                {
                    queuedSpeaks.Add(new BeatAction.Action(speakEventToCheck.beat, delegate
                    {
                        AlienSpeak(speakEventToCheck.beat, speakEventToCheck["spaceNum"], speakEventToCheck["dotdotdot"]);
                    }));
                }
                else
                {
                    AlienSpeakInactive(speakEventToCheck["spaceNum"]);
                }
            }
            BeatAction.New(this, queuedSpeaks);
        }

        public override void OnGameSwitch(double beat)
        {
            if (Conductor.instance.isPlaying && !Conductor.instance.isPaused)
            {
                if (queuedIntervals.Count > 0)
                {
                    foreach (var interval in queuedIntervals)
                    {
                        SetIntervalStart(interval.beat, interval.interval, interval.outDialogue, beat, interval.autoPassTurn);
                    }
                    queuedIntervals.Clear();
                }
            }
        }

        private void Update()
        {
            if (Conductor.instance.ReportBeat(ref lastReportedBeat, offset: liveBarBeatOffset))
            {
                liveBar.Play("liveBar", 0, 0);
            }
            else if (Conductor.instance.songPositionInBeatsAsDouble < lastReportedBeat)
            {
                lastReportedBeat = Math.Round(Conductor.instance.songPositionInBeatsAsDouble);
            }

            if (PlayerInput.GetIsAction(InputAction_Press) && !IsExpectingInputNow(InputAction_Press))
            {
                if (isSpeaking)
                {
                    if (noHitOnce || callDiagIndex == 0)
                    {
                        FailContact();
                    }
                    else
                    {
                        SoundByte.PlayOneShotGame("firstContact/ALIEN_PLAYER_A", -1, SoundByte.GetPitchFromSemiTones(UnityEngine.Random.Range(-3, 3), false));
                        TrailingContact();
                        ScoreMiss();
                    }
                }
                else if (!noHitOnce && !missionControl.activeInHierarchy)
                {
                    translator.DoScaledAnimationAsync("translator_eh", 0.5f);
                    SoundByte.PlayOneShotGame("firstContact/ALIEN_PLAYER_MISS2_A", -1, SoundByte.GetPitchFromSemiTones(UnityEngine.Random.Range(-2, 1), false));
                    ScoreMiss();
                }
            }
        }

        public void LiveBarBeat(bool onBeat)
        {
            if (onBeat)
            {
                liveBarBeatOffset = 0;
            }
            else
            {
                liveBarBeatOffset = .5f;
            }
        }

        public void LookAtDirection(int alienLookAt, int translatorLookAt)
        {
            switch (alienLookAt)
            {
                case 0:
                    alien.Play("alien_lookAt", 0, 0);
                    break;
                case 1:
                    alien.Play("alien_idle", 0, 0);
                    break;
            }

            switch (translatorLookAt)
            {
                case 0:
                    translator.Play("translator_lookAtAlien", 0, 0);
                    break;
                case 1:
                    translator.Play("translator_idle", 0, 0);
                    break;
            }

        }

        private void AlienSpeak(double beat, int spaceNum, bool hasDDD)
        {
            int voiceline = UnityEngine.Random.Range(1, 11);
            if (voiceline == currentVoicelineIndex) voiceline++;
            if (voiceline > 10) voiceline = 1;
            currentVoicelineIndex = voiceline;
            SoundByte.PlayOneShotGame("firstContact/Bob" + voiceline, beat, SoundByte.GetPitchFromCents(UnityEngine.Random.Range(-100, 0), false));
            SoundByte.PlayOneShotGame("firstContact/BobB");
            alien.DoScaledAnimationAsync("alien_talk", 0.5f);
            if (UnityEngine.Random.Range(0, 5) == 0) translator.DoScaledAnimationAsync("translator_lookAtAlien_nod", 0.5f);

            alienTextbox.SetActive(true);
            for (int i = 0; i < spaceNum * 2; i++)
            {
                callDiagBuffer += " ";
            }
            callDiagBuffer += hasDDD ? MSG_MAN_DDD : MSG_MAN;
            UpdateAlienTextbox();
        }

        private void AlienSpeakInactive(int spaceNum)
        {
            alienTextbox.SetActive(true);
            for (int i = 0; i < spaceNum * 2; i++)
            {
                callDiagBuffer += " ";
            }
            callDiagBuffer += MSG_MAN;
            UpdateAlienTextbox();
        }

        public void PassTurnStandalone(double beat)
        {
            RiqEntity lastInterval = GetLastIntervalBeforeBeat(beat);
            float length = EventCaller.GetAllInGameManagerList("firstContact", new string[] { "alien turnover" }).Find(x => x.beat == beat).length;
            if (lastInterval == null) return;
            AlienTurnOver(beat, lastInterval.beat, lastInterval.beat + lastInterval.length, length);
        }

        private void AlienTurnOver(double beat, double intervalBeat, double endBeat, float length)
        {
            var inputs = GetAllSpeaksInBetweenBeat(intervalBeat, endBeat);
            inputs.Sort((x, y) => x.beat.CompareTo(y.beat));
            for (int i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                double relativeBeat = input.beat - intervalBeat;
                ScheduleInput(beat, length + relativeBeat, InputAction_Press, AlienTapping, AlienOnMiss, AlienEmpty, CanAlienTapping);
                callDiagList.Add((input["dialogue"], input["newline"]));
            }
            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate
                {
                    isSpeaking = true;
                    hasMissed = false;
                    SoundByte.PlayOneShotGame("firstContact/turnover");
                    alienTextbox.SetActive(false);
                    alien.Play("alien_point", 0, 0);
                }),
                new BeatAction.Action(beat + (length / 2), delegate
                {
                    alien.Play("alien_idle", 0, 0);
                })
            });
        }

        public void AlienSuccess(double beat)
        {
            string animString = "";
            List<MultiSound.Sound> sound = new List<MultiSound.Sound>();
            if (!(hasMissed || noHitOnce))
            {
                sound = new List<MultiSound.Sound>()
                {
                    new MultiSound.Sound("firstContact/successCrowd", beat),
                    new MultiSound.Sound("firstContact/nod", beat),
                    new MultiSound.Sound("firstContact/nod", beat + 0.5f),
                    new MultiSound.Sound("firstContact/successExtra" + UnityEngine.Random.Range(1, 3), beat + 0.5f, SoundByte.GetPitchFromCents(UnityEngine.Random.Range(-50, 50), false)),
                    new MultiSound.Sound("firstContact/whistle", beat + UnityEngine.Random.Range(0.5f, 1.5f), SoundByte.GetPitchFromCents(UnityEngine.Random.Range(-50, 100), false), UnityEngine.Random.Range(0.4f, 1f)),
                };
                animString = "alien_success";
            }
            else
            {
                sound = new List<MultiSound.Sound>()
                {
                    new MultiSound.Sound("firstContact/fail", beat),
                    new MultiSound.Sound("firstContact/shakeHead", beat),
                    new MultiSound.Sound("firstContact/shakeHead", beat + 0.5f),
                };
                animString = "alien_fail";
            }


            MultiSound.Play(sound.ToArray());

            BeatAction.New(this, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { alien.Play(animString, 0, 0); }),
                new BeatAction.Action(beat + .5f, delegate { alien.Play(animString, 0, 0); }),
                new BeatAction.Action(beat + 1, delegate { alienTextbox.SetActive(false); translateTextbox.SetActive(false); translateFailTextbox.SetActive(false); })
            });

            isSpeaking = false;
            hasMissed = false;
            noHitOnce = false;
        }

        void UpdateAlienTextbox()
        {
            alienText.text = callDiagBuffer;
        }

        string GetMessageFromCallDialogue(int callDiagIndex = 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = callDiagIndex; i < callDiagList.Count; i++)
            {
                (string s, bool showNewline) = callDiagList[i];
                if (showNewline && callDiagIndex != i)
                {
                    break;
                }
                sb.Append(s);
            }
            return sb.ToString();
        }

        void UpdateTranslateTextbox()
        {
            if (callDiagIndex == 0 && !hasMissed && !noHitOnce)
            {
                ResetTranslateTextbox();
            }
            translateText.text = respDiagBuffer;
            translateFailText.text = respDiagBuffer;
        }

        void ResetTranslateTextbox(bool destroyText = false)
        {
            // shift the textbox to centre the message
            string message = GetMessageFromCallDialogue(callDiagIndex);
            // many hardcoded values there'll be a better way to do this
            Vector2 size = translateText.GetPreferredValues(message, 10.95f, 2);
            translateText.rectTransform.anchoredPosition = new Vector2(Mathf.Max((10.95f / 2f) + (-size.x / 2 - 0.25f), -0.25f), Mathf.Max((2.11f / 2f) + (-size.y / 2) + 0.2f, 0.2f));
            if (destroyText)
            {
                respDiagBuffer = "";
                translateText.text = "";
            }
        }

        public void MissionControlDisplay(double beat, bool stay, float length)
        {
            missionControl.SetActive(true);

            alienTextbox.SetActive(false);
            translateTextbox.SetActive(false);
            translateFailTextbox.SetActive(false);

            string textToPut = "";

            if (!(hasMissed || noHitOnce))
            {
                textToPut = "missionControl_success";
            }
            else
            {
                textToPut = "missionControl_fail";
            }

            missionControl.GetComponentInParent<Animator>().Play(textToPut, 0, 0);
            alien.Play("alien_idle", 0, 0);
            translator.Play("translator_idle", 0, 0);

            if (!stay)
            {
                BeatAction.New(instance, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + length, delegate { missionControl.SetActive(false); }),
                    });
            }
            else
            {
                missionControl.SetActive(true);
            }
            isSpeaking = false;
        }

        void FailContact()
        {
            SoundByte.PlayOneShotGame("firstContact/failContact");
            translator.DoScaledAnimationAsync("translator_speak", 0.5f);
            if (!hasMissed && callDiagIndex == 0)
            {
                translateFailTextbox.SetActive(true);
                respDiagBuffer = onOutDialogue;
                UpdateTranslateTextbox();
                ScoreMiss();
            }
            hasMissed = true;
        }

        void TrailingContact()
        {
            SoundByte.PlayOneShotGame("firstContact/slightlyFail");
            translator.Play("translator_eh", 0, 0);
            if (!hasMissed)
            {
                respDiagBuffer += MID_MSG_MISS;
                UpdateTranslateTextbox();
            }
            hasMissed = true;
        }

        public bool CanAlienTapping()
        {
            return !(hasMissed || noHitOnce);
        }

        public void AlienTapping(PlayerActionEvent caller, float state) //OnHit
        {
            (string dialogue, bool showNewline) = callDiagList[callDiagIndex];
            translateTextbox.SetActive(true);
            if (showNewline)
            {
                ResetTranslateTextbox(true);
            }
            if (state >= 1f || state <= -1f)
            {
                SoundByte.PlayOneShotGame("firstContact/ALIEN_PLAYER_A", -1, SoundByte.GetPitchFromSemiTones(UnityEngine.Random.Range(-3, 3), false));
                TrailingContact();
                callDiagIndex++;
                return;
            }

            translator.DoScaledAnimationAsync("translator_speak", 0.5f);
            SoundByte.PlayOneShotGame("firstContact/ALIEN_PLAYER_A", -1, SoundByte.GetPitchFromSemiTones(UnityEngine.Random.Range(-3, 3), false));
            SoundByte.PlayOneShotGame("firstContact/ALIEN_PLAYER_B");
            respDiagBuffer += dialogue;
            UpdateTranslateTextbox();
            callDiagIndex++;
        }

        public void AlienOnMiss(PlayerActionEvent caller) //OnMiss
        {
            if (!noHitOnce && !hasMissed)
            {
                SoundByte.PlayOneShotGame("firstContact/alienNoHit");
                noHitOnce = true;
            }

            if (callDiagIndex > 0 && !hasMissed)
            {
                respDiagBuffer += MID_MSG_MISS;
                UpdateTranslateTextbox();
                hasMissed = true;
            }

            alien.Play("alien_noHit", 0, 0);
        }

        public void AlienEmpty(PlayerActionEvent caller) { } //OnEmpty
    }
}