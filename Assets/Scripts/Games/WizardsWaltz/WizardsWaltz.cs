using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.WizardsWaltz
{
    public class WizardsWaltz : Minigame
    {
        [Header("References")]
        public Wizard wizard;
        public GameObject plantHolder;
        public GameObject plantBase;

        public float beatInterval = 4f;
        float intervalStartBeat;
        bool intervalStarted;
        public float wizardBeatOffset = 0f;

        [NonSerialized] public int plantsLeft = 0;

        public static WizardsWaltz instance;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (!Conductor.instance.isPlaying && !Conductor.instance.isPaused && intervalStarted)
            {
                intervalStarted = false;
            }
        }

        public void SetIntervalStart(float beat, float interval = 4f)
        {
            // Don't do these things if the interval was already started.
            if (!intervalStarted)
            {
                plantsLeft = 0;
                intervalStarted = true;
            }

            intervalStartBeat = beat;
            beatInterval = interval;
        }

        public void SpawnFlower(float beat)
        {
            // If interval hasn't started, assume this is the first hair of the interval.
            if (!intervalStarted)
                SetIntervalStart(beat, beatInterval);

            Jukebox.PlayOneShotGame("wizardsWaltz/plant", beat);
            Plant plant = Instantiate(plantBase, plantHolder.transform).GetComponent<Plant>();

            var songPos = Conductor.instance.songPositionInBeats;
            var am = (beatInterval / 2f);
            var x = Mathf.Sin(Mathf.PI * songPos / am) * 6;
            var y = -2.5f + Mathf.Cos(Mathf.PI * songPos / am) * 1.5f;
            var scale = 1 - Mathf.Cos(Mathf.PI * songPos / am) * 0.25f;
            var xscale = scale;
            if (y > -2.5f) xscale *= -1;

            plant.transform.localPosition = new Vector3(x, y, -scale);
            plant.transform.localScale = new Vector3(xscale, scale, 1);

            plant.gameObject.SetActive(true);

            plant.createBeat = beat;
            plantsLeft++;
        }

    }
}