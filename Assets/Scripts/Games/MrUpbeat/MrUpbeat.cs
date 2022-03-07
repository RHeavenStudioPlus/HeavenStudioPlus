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
        public GameObject bt;

        public GameEvent beat = new GameEvent();
        public bool canGo = false;
        public int beatCount = 0;

        public static MrUpbeat instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            canGo = false;
            man.stepTimes = 0;
            SetInterval(0);
            var pos = Conductor.instance.songPositionInBeats;
            StartCoroutine(Upbeat(pos - Mathf.Round(pos)));
        }

        private void Update()
        {
            List<Beatmap.Entity> gos = GameManager.instance.Beatmap.entities.FindAll(c => c.datamodel == "mrUpbeat/go");
            for (int i = 0; i < gos.Count; i++)
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

            if (canGo)
            {
                metronome.transform.eulerAngles = new Vector3(0, 0, 270 - Mathf.Cos(Mathf.PI * Conductor.instance.songPositionInBeats) * 75);
            }

            if (Conductor.instance.ReportBeat(ref beat.lastReportedBeat))
            {
                StartCoroutine(Upbeat());
                if (canGo)
                {
                    if (beatCount % 2 == 0)
                        Jukebox.PlayOneShotGame("mrUpbeat/metronomeRight");
                    else
                        Jukebox.PlayOneShotGame("mrUpbeat/metronomeLeft");

                    Beat(Mathf.Round(Conductor.instance.songPositionInBeats));
                }
            }
        }

        public void SetInterval(float beat)
        {
            beatCount = 0;
            man.targetBeat = beat + 320f;
            man.Idle();
        }

        public void Go(float beat)
        {
            beatCount = 0;
        }

        public void Ding(bool applause)
        {
            Jukebox.PlayOneShotGame("mrUpbeat/ding");
            if (applause)
                Jukebox.PlayOneShot("applause");
        }

        public void Beat(float beat)
        {
            beatCount++;

            GameObject _beat = Instantiate(bt);
            _beat.transform.parent = bt.transform.parent;
            _beat.SetActive(true);
            UpbeatStep s = _beat.GetComponent<UpbeatStep>();
            s.startBeat = beat;
        }

        private IEnumerator Upbeat(float offset = 0)
        {
            yield return new WaitForSeconds(Conductor.instance.secPerBeat * 0.5f - offset);
            man.Blip();
        }
    }
}