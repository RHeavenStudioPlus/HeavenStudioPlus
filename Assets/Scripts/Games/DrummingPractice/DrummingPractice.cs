using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Starpelly;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.DrummingPractice
{
    public class DrummingPractice : Minigame
    {
        public enum MiiType
        {
            GuestA,
            GuestB,
            GuestC,
            GuestD,
            GuestE,
            GuestF,
            Matt,
            Tsunku,
            Marshal
        }

        [Header("References")]
        public SpriteRenderer backgroundGradient;
        public Drummer player;
        public Drummer leftDrummer;
        public Drummer rightDrummer;
        public GameObject hitPrefab;

        public GameEvent bop = new GameEvent();
        public int count = 0;

        public static DrummingPractice instance;

        private void Awake()
        {
            instance = this;
        }

        // TODO: Move this to OnGameSwitch() when functional?
        private void Start()
        {
            SetMiis(UnityEngine.Random.Range(0, player.miiFaces.Count));
        }

        private void Update()
        {
            if (Conductor.instance.ReportBeat(ref bop.lastReportedBeat, bop.startBeat % 1))
            {
                if (Conductor.instance.songPositionInBeats >= bop.startBeat && Conductor.instance.songPositionInBeats < bop.startBeat + bop.length)
                {
                    Bop();
                }
            }
        }

        public void SetBop(float beat, float length)
        {
            bop.startBeat = beat;
            bop.length = length;
        }

        public void Bop()
        {
            player.Bop();
            leftDrummer.Bop();
            rightDrummer.Bop();
        }

        public void Prepare(float beat, bool applause)
        {
            int type = count % 2;
            player.Prepare(type);
            leftDrummer.Prepare(type);
            rightDrummer.Prepare(type);
            count++;

            SetFaces(0);
            Jukebox.PlayOneShotGame("drummingPractice/prepare");

            GameObject hit = Instantiate(hitPrefab);
            hit.transform.parent = hitPrefab.transform.parent;
            hit.SetActive(true);
            DrummerHit h = hit.GetComponent<DrummerHit>();
            h.startBeat = beat;
            h.applause = applause;
        }

        public void SetFaces(int type)
        {
            player.SetFace(type);
            leftDrummer.SetFace(type);
            rightDrummer.SetFace(type);
        }

        public void SetMiis(int playerFace, bool all = false)
        {
            player.mii = playerFace;

            if (all)
            {
                leftDrummer.mii = playerFace;
                rightDrummer.mii = playerFace;
            }
            else
            {
                do
                {
                    leftDrummer.mii = UnityEngine.Random.Range(0, leftDrummer.miiFaces.Count);
                }
                while (leftDrummer.mii == player.mii);
                do
                {
                    rightDrummer.mii = UnityEngine.Random.Range(0, rightDrummer.miiFaces.Count);
                }
                while (rightDrummer.mii == leftDrummer.mii || rightDrummer.mii == player.mii);
            }

            SetFaces(0);
        }

    }
}