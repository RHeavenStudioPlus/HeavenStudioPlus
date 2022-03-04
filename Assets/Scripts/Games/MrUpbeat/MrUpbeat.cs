using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.MrUpbeat
{
    public class MrUpbeat : Minigame
    {
        [Header("References")]
        public GameObject metronome;
        public UpbeatMan man;

        public float nextBeat;
        public bool canGo = false;


        public static MrUpbeat instance;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            List<Beatmap.Entity> gos = GameManager.instance.Beatmap.entities.FindAll(c => c.datamodel == "mrUpbeat/go");
            for(int i=0; i<gos.Count; i++)
            {
                if ((gos[i].beat - 0.15f) <= Conductor.instance.songPositionInBeats && (gos[i].beat + gos[i].length) - 0.15f > Conductor.instance.songPositionInBeats)
                {
                    canGo = true;
                    break;
                } else
                {
                    canGo = false;
                }
            }

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(nextBeat, 0.5f);
            //StateCheck(normalizedBeat);
        }

        public void SetInterval(float beat)
        {
            nextBeat = beat;
        }
       

    }
}