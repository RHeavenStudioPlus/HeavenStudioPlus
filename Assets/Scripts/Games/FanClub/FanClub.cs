using HeavenStudio.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; //don't ask

namespace HeavenStudio.Games
{
    // none yet
    using Scripts_FanClub;

    public class FanClub : Minigame
    {
        // userdata here
        [Header("Animators")]

        [Header("Objects")]
        public GameObject Arisa;
        public ParticleSystem idolClapEffect;
        public GameObject spectator;
        public GameObject spectatorAnchor;

        // end userdata

        //arisa's animation controller
        private Animator idolAnimator;
        //spectators
        private List<GameObject> Spectators;
        private GameObject Player;

        //bop-type animations
        public GameEvent bop = new GameEvent();
        public GameEvent specBop = new GameEvent();
        public GameEvent handCrap = new GameEvent();
        public GameEvent response = new GameEvent();
        //game scene
        public static FanClub instance;

        private void Awake()
        {
            instance = this;
        }

        const int FAN_COUNT = 12;
        const float RADIUS = 1.25f;
        private void Start()
        {
            Spectators = new List<GameObject>();
            idolAnimator = Arisa.GetComponent<Animator>();

            // procedurally spawning the spectators
            // from middle of viewport:

            //  |========A========|
            //  f==f==f==0==p==f==f
            //  =f==f==0==0==f==f==

            //spawn 10, the 4th is our player (idx 3)
            Vector3 origin = spectatorAnchor.transform.localPosition;
            int sortOrigin = spectatorAnchor.GetComponent<SortingGroup>().sortingOrder;
            Vector3 spawnPos = new Vector3(origin.x, origin.y, origin.z);
            spawnPos.x -= RADIUS * 2 * 3;
            for (int i = 0; i < FAN_COUNT; i++)
            {
                GameObject mobj = Instantiate(spectator, spectatorAnchor.transform.parent);
                mobj.transform.localPosition = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
                mobj.GetComponent<SortingGroup>().sortingOrder = i + sortOrigin;
                if (i == 3)
                    Player = mobj;
                Spectators.Add(mobj);

                //prepare spawn point of next spectator
                spawnPos.x += RADIUS * 2;
                if (i == 2)
                    spawnPos.x += RADIUS * 2;
                if (i == 8)
                    spawnPos.x += RADIUS * 4;
                if (i == 5)
                {
                    spawnPos = new Vector3(origin.x, origin.y, origin.z);
                    spawnPos.x -= RADIUS * 2 * 4 - RADIUS;
                    spawnPos.y -= RADIUS;
                    spawnPos.z -= RADIUS/4;
                }
            }
        }

        private void Update()
        {
            if (Conductor.instance.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (Conductor.instance.songPositionInBeats >= specBop.startBeat && Conductor.instance.songPositionInBeats < specBop.startBeat + specBop.length)
                    PlayAnimationAll("FanBeat");
                
                if (Conductor.instance.songPositionInBeats >= response.startBeat && Conductor.instance.songPositionInBeats < response.startBeat + response.length)
                {
                    idolAnimator.Play("IdolResponse", 0, 0);
                }
                else if (Conductor.instance.songPositionInBeats >= handCrap.startBeat && Conductor.instance.songPositionInBeats < handCrap.startBeat + handCrap.length)
                {
                    idolAnimator.Play("IdolCrap", 0, 0);
                    idolClapEffect.Play();
                }
                else if (Conductor.instance.songPositionInBeats >= bop.startBeat && Conductor.instance.songPositionInBeats < bop.startBeat + bop.length)
                {
                    idolAnimator.Play("IdolBeat", 0, 0);
                    PlayAnimationAll("FanBeat");
                }
            }
        }

        public void Bop(float beat, float length)
        {
            bop.length = length;
            bop.startBeat = beat;
        }

        public void SpecBop(float beat, float length)
        {
            specBop.length = length;
            specBop.startBeat = beat;
        }

        public void CallHai(float beat, int type = 0)
        {
            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/arisa_hai_1_jp", beat), 
                new MultiSound.Sound("fanClub/arisa_hai_2_jp", beat + 1f),
                new MultiSound.Sound("fanClub/arisa_hai_3_jp", beat + 2f),
            });

            BeatAction.New(Arisa, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); PlayAnimationAll("FanBeat");}),
                new BeatAction.Action(beat + 1f, delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); PlayAnimationAll("FanBeat");}),
                new BeatAction.Action(beat + 2f, delegate { Arisa.GetComponent<Animator>().Play("IdolPeace", 0, 0); PlayAnimationAll("FanBeat");}),
                new BeatAction.Action(beat + 3f, delegate { Arisa.GetComponent<Animator>().Play("IdolPeaceNoSync", 0, 0); PlayAnimationAll("FanPrepare");}),

                new BeatAction.Action(beat + 4f, delegate { PlayAnimationAll("FanClap", true);}),
                new BeatAction.Action(beat + 5f, delegate { PlayAnimationAll("FanClap", true);}),
                new BeatAction.Action(beat + 6f, delegate { PlayAnimationAll("FanClap", true);}),
                new BeatAction.Action(beat + 7f, delegate { PlayAnimationAll("FanClap", true);}),
            });

            handCrap.length = 4f;
            handCrap.startBeat = beat + 4f;
        }

        const float CALL_LENGTH = 2f;
        public void CallKamone(float beat, int type = 0)
        {
            // clip certain events to the start of this cue if needed
            if (bop.startBeat + bop.length > beat && bop.startBeat < beat)
            {
                //let bopping continue *after* this cue
                if (bop.startBeat + bop.length > beat + CALL_LENGTH)
                {
                    bop.length -= (beat + CALL_LENGTH - bop.startBeat);
                    bop.startBeat = beat + CALL_LENGTH;
                }
                else
                    bop.length = beat - bop.startBeat;
            }
            // interrupt clapping completely, no need to continue
            if (handCrap.startBeat + handCrap.length > beat && handCrap.startBeat < beat)
                handCrap.length = beat - handCrap.startBeat;
            // same with responses
            if (response.startBeat + response.length > beat && response.startBeat < beat)
                response.length = beat - response.startBeat;

            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound("fanClub/arisa_ka_jp", beat), 
                new MultiSound.Sound("fanClub/arisa_mo_jp", beat + 0.5f),
                new MultiSound.Sound("fanClub/arisa_ne_jp", beat + 1f),
            });

            BeatAction.New(Arisa, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate { Arisa.GetComponent<Animator>().Play("IdolCall0", 0, 0); PlayAnimationAll("FanBeat");}),
                new BeatAction.Action(beat + 0.75f, delegate { Arisa.GetComponent<Animator>().Play("IdolCall1", 0, 0); }),
                new BeatAction.Action(beat + 1f, delegate { PlayAnimationAll("FanPrepare");}),
            });

            response.length = 4f;
            response.startBeat = beat + CALL_LENGTH;
        }

        private void PlayAnimationAll(string anim, bool noPlayer = false, bool doForced = false)
        {
            for (int i = 0; i < Spectators.Count; i++)
            {
                if (i == 3 && noPlayer)
                    continue;
                
                if (!Spectators[i].GetComponent<Animator>().IsAnimationNotPlaying() && !doForced)
                    continue;
                Spectators[i].GetComponent<Animator>().Play(anim, -1, 0);
            }
        }
    }
}