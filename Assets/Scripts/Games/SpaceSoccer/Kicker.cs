using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.SpaceSoccer
{
    public class Kicker : MonoBehaviour
    {
        private GameEvent kickEvents = new GameEvent();

        [Header("Properties")]
        public bool canKick;
        public bool canHighKick;

        [Header("Components")]
        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void KeepUp(float beat, float length)
        {
            kickEvents.startBeat = beat;
            kickEvents.length = length;
        }

        public void Kick(Ball b)
        {
            if (b.hitTimes % 2 == 0)
            {
                anim.Play("KickRight", 0, 0);
            }
            else
            {
                anim.Play("KickLeft", 0, 0);
            }
            Jukebox.PlayOneShotGame("spaceSoccer/kick");
        }

        public void HighKick(float beat)
        {
            canHighKick = true;
            // Jukebox.PlayOneShotGame("spaceSoccer/highKickToe1");
        }

        private void Update()
        {
            if (Conductor.instance.songPositionInBeats >= kickEvents.startBeat && Conductor.instance.songPositionInBeats < kickEvents.startBeat + kickEvents.length)
            {
                canKick = true;
            }
            else
            {
                canKick = false;
            }
        }
    }
}