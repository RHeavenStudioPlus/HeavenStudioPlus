using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.DJSchool
{
    public class DJSchool : Minigame
    {
        [Header("Components")]
        [SerializeField] private Student student;
        [SerializeField] private GameObject djYellow;
        private Animator djYellowAnim;
        [SerializeField] private SpriteRenderer headSprite;
        [SerializeField] private Sprite[] headSprites;

        [Header("Properties")]
        public GameEvent bop = new GameEvent();
        private bool djYellowHolding;

        public static DJSchool instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            djYellowAnim = djYellow.GetComponent<Animator>();
        }

        private void Update()
        {
            if (Conductor.instance.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (Conductor.instance.songPositionInBeats >= bop.startBeat && Conductor.instance.songPositionInBeats < bop.startBeat + bop.length)
                {
                    if (student.anim.IsAnimationNotPlaying())
                    {
                        if (student.isHolding)
                        {
                            student.anim.Play("HoldBop", 0, 0);
                        }
                        else
                        {
                            student.anim.Play("IdleBop", 0, 0);
                        }
                    }
                    if (djYellowAnim.IsAnimationNotPlaying())
                    {
                        if (djYellowHolding)
                        {
                            djYellowAnim.Play("HoldBop", 0, 0);
                        }
                        else
                        {
                            // there is no sprite for the alternate idle bop, oh well
                            djYellowAnim.Play("IdleBop", 0, 0);
                        }
                    }
                }
            }
        }

        public void Bop(float beat, float length)
        {
            bop.startBeat = beat;
            bop.length = length;
        }

        public void BreakCmon(float beat, int type)
        {
            if (djYellowHolding) return;

            string[] sounds = new string[] { };

            if (type == 0)
            {
                sounds = new string[] { "djSchool/breakCmon1", "djSchool/breakCmon2", "djSchool/ooh" };
            }
            else if (type == 1)
            {
                sounds = new string[] { "djSchool/breakCmonAlt1", "djSchool/breakCmonAlt2", "djSchool/oohAlt" };
            }
            else if (type == 2)
            {
                SetDJYellowHead(2);
                sounds = new string[] { "djSchool/breakCmonLoud1", "djSchool/breakCmonLoud2", "djSchool/oohLoud" };
            }


            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound(sounds[0],   beat),
                new MultiSound.Sound(sounds[1], beat + 1f - (0.030f/Conductor.instance.secPerBeat)*Conductor.instance.musicSource.pitch),
                new MultiSound.Sound(sounds[2], beat + 2f),
            });

            BeatAction.New(djYellow, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { djYellow.GetComponent<Animator>().Play("BreakCmon", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { djYellow.GetComponent<Animator>().Play("BreakCmon", 0, 0); }),
                new BeatAction.Action(beat + 2f, delegate 
                { 
                    djYellow.GetComponent<Animator>().Play("Hold", 0, 0); 
                    djYellowHolding = true;
                    SetDJYellowHead(1);
                }),
            });
            
            student.holdBeat = beat;
            student.ResetState();
        }

        public void AndStop(float beat)
        {
            if (djYellowHolding) return;

            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("djSchool/andStop1",   beat),
                new MultiSound.Sound("djSchool/andStop2",   beat + .5f - (0.1200f/Conductor.instance.secPerBeat)*Conductor.instance.musicSource.pitch),
                new MultiSound.Sound("djSchool/oohAlt",     beat + 1.5f),
            });

            BeatAction.New(djYellow, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat + 0.5f, delegate { djYellow.GetComponent<Animator>().Play("BreakCmon", 0, 0); }),
                new BeatAction.Action(beat + 1.5f, delegate
                {
                    djYellow.GetComponent<Animator>().Play("Hold", 0, 0);
                    djYellowHolding = true;
                    SetDJYellowHead(1);
                }),
            });

            student.holdBeat = beat - 0.5f;
            student.ResetState();
        }

        public void ScratchoHey(float beat, int type)
        {
            string[] sounds = new string[] { };

            if (type == 0)
            {
                sounds = new string[] { "djSchool/scratchoHey1", "djSchool/scratchoHey2", "djSchool/scratchoHey3", "djSchool/scratchoHey4", "djSchool/hey" };
            }
            else if (type == 1)
            {
                sounds = new string[] { "djSchool/scratchoHeyAlt1", "djSchool/scratchoHeyAlt2", "djSchool/scratchoHeyAlt3", "djSchool/scratchoHeyAlt4", "djSchool/heyAlt" };
            }
            else if (type == 2)
            {
                sounds = new string[] { "djSchool/scratchoHeyLoud1", "djSchool/scratchoHeyLoud2", "djSchool/scratchoHeyLoud3", "djSchool/scratchoHeyLoud4", "djSchool/heyLoud" };
            }

            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound(sounds[0],   beat),
                new MultiSound.Sound(sounds[1], beat + .25f),
                new MultiSound.Sound(sounds[2], beat + .5f),
                new MultiSound.Sound(sounds[3], beat + 1f - (0.0500f/Conductor.instance.secPerBeat)*Conductor.instance.musicSource.pitch),
                new MultiSound.Sound(sounds[4], beat + 2f - (0.070f/Conductor.instance.secPerBeat)*Conductor.instance.musicSource.pitch),
            });

            BeatAction.New(djYellow, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { djYellow.GetComponent<Animator>().Play("Scratcho", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { djYellow.GetComponent<Animator>().Play("Scratcho2", 0, 0); }),
                new BeatAction.Action(beat + 2.05f, delegate 
                { 
                    djYellow.GetComponent<Animator>().Play("Hey", 0, 0);
                    djYellowHolding = false;
                }),
            });

            student.swipeBeat = beat;
            student.ResetState();
        }

        private void SetDJYellowHead(int type)
        {
            headSprite.sprite = headSprites[type];
        }
    }
}