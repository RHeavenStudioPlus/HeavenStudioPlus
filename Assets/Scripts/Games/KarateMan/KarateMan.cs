using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class KarateMan : Minigame
    {
        public GameObject Pot;
        public KarateJoe KarateJoe;

        public static KarateMan instance { get; set; }

        public Sprite[] ObjectSprites;

        private void Awake()
        {
            instance = this;
        }

        public void Shoot(float beat, int type)
        {
            GameObject pot = Instantiate(Pot);
            pot.transform.parent = Pot.transform.parent;

            Pot p = pot.GetComponent<Pot>();

            pot.SetActive(true);
            p.startBeat = beat;
            p.createBeat = beat;
            p.isThrown = true;
            p.type = type;
            p.Sprite.GetComponent<SpriteRenderer>().sprite = ObjectSprites[type];

            switch (type)
            {
                case 0:
                    Jukebox.PlayOneShotGame("karateman/objectOut");
                    p.hitSnd = "karateman/potHit";
                    break;
                case 1:
                    Jukebox.PlayOneShotGame("karateman/lightbulbOut");
                    p.hitSnd = "karateman/lightbulbHit";
                    break;
                case 2:
                    Jukebox.PlayOneShotGame("karateman/objectOut");
                    p.hitSnd = "karateman/rockHit";
                    break;
            }
        }

        public void Bop()
        {
            KarateJoe.anim.Play("Bop", 0, 0);
        }
    }
}