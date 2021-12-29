using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class KarateJoe : MonoBehaviour
    {
        private Animator anim;

        private int currentHitInList = 0;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (PlayerInput.Pressed())
            {
                Swing();
            }
        }

        private void Swing()
        {
            var EligibleHits = KarateMan.instance.EligibleHits;
            bool canHit = (EligibleHits.Count > 0) && (currentHitInList < EligibleHits.Count);

            if (canHit)
            {
                if (KarateMan.instance.EligibleHits[currentHitInList].perfect)
                {
                    Jukebox.PlayOneShotGame("karateman/potHit");
                }
                else
                {
                    Jukebox.PlayOneShot("miss");
                }
                EligibleHits[currentHitInList].gameObject.GetComponent<Pot>().enabled = false;
                EligibleHits[currentHitInList].gameObject.GetComponent<Pot>().RemoveObject(currentHitInList, EligibleHits);
            }
            else
            {
                Jukebox.PlayOneShotGame("karateman/swingNoHit");
            }
            anim.Play("PunchLeft", 0, 0);
        }
    }
}