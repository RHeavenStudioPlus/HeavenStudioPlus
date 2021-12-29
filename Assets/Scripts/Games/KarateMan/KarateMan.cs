using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class KarateMan : Minigame
    {
        public GameObject Pot;

        public static KarateMan instance { get; set; }

        private void Awake()
        {
            instance = this;
        }

        public void Shoot(float beat)
        {
            GameObject pot = Instantiate(Pot);
            pot.transform.parent = Pot.transform.parent;
            pot.SetActive(true);
            pot.GetComponent<Pot>().startBeat = beat;

            Jukebox.PlayOneShotGame("karateman/objectOut");
        }
    }
}