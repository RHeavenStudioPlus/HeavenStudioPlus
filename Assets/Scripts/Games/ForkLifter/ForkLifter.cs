using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

using DG.Tweening;

namespace RhythmHeavenMania.Games.ForkLifter
{
    public class ForkLifter : Minigame
    {
        public static ForkLifter instance;

        [Header("References")]
        public ForkLifterHand ForkLifterHand;

        [Header("Objects")]
        public Animator handAnim;
        public GameObject flickedObject;
        public SpriteRenderer peaPreview;

        public Sprite[] peaSprites;
        public Sprite[] peaHitSprites;

        private void Awake()
        {
            instance = this;
        }

        public override void OnGameSwitch()
        {
            ForkLifterHand.CheckNextFlick();
        }

        public void Flick(float beat, int type)
        {
            Jukebox.PlayOneShotGame("forkLifter/flick");
            handAnim.Play("Hand_Flick", 0, 0);
            GameObject fo = Instantiate(flickedObject);
            fo.transform.parent = flickedObject.transform.parent;
            fo.GetComponent<Pea>().startBeat = beat;
            fo.GetComponent<Pea>().type = type;
            fo.SetActive(true);
        }
    }

}