using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class KarateMan : Minigame
    {
        public GameObject Pot, Bomb;
        public KarateJoe KarateJoe;

        public static KarateMan instance { get; set; }

        public Sprite[] ObjectSprites;

        public List<BGSpriteC> BGSprites;
        public SpriteRenderer BGSprite;

        private bool bgEnabled;
        private int newBeat;

        [System.Serializable]
        public class BGSpriteC
        {
            public List<Sprite> Sprites;
        }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (bgEnabled)
            {
                if (Conductor.instance.songPositionInBeats > newBeat)
                {
                    if (newBeat % 2 == 0)
                        BGSprite.sprite = BGSprites[0].Sprites[1];
                    else
                        BGSprite.sprite = BGSprites[0].Sprites[2];

                    newBeat++;
                }
            }
        }

        public void BGFXOn()
        {
            bgEnabled = true;
            newBeat = Mathf.RoundToInt(Conductor.instance.songPositionInBeats);
        }

        public void BGFXOff()
        {
            bgEnabled = false;
            BGSprite.sprite = BGSprites[0].Sprites[0];
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
                case 3:
                    Jukebox.PlayOneShotGame("karateman/objectOut");
                    p.hitSnd = "karateman/soccerHit";
                    break;
                case 4:
                    p.kick = true;
                    Jukebox.PlayOneShotGame("karateman/barrelOutKicks");
                    p.hitSnd = "karateman/barrelOutKicks";
                    break;
            }
        }

        public void Bop()
        {
            KarateJoe.anim.Play("Bop", 0, 0);
        }
    }
}