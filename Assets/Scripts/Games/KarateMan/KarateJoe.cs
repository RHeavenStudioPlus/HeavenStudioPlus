using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class KarateJoe : MonoBehaviour
    {
        public Animator anim;

        private int currentHitInList = 0;

        public GameObject HitEffect;

        [Header("Particles")]
        public ParticleSystem HitParticle;
        public ParticleSystem RockParticle;
        public GameObject BulbHit;

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

            bool punchLeft = true;

            if (canHit)
            {
                Pot p = EligibleHits[currentHitInList].gameObject.GetComponent<Pot>();

                if (p.type == 2 || p.type == 3)
                {
                    punchLeft = false;
                }
                else
                {
                    punchLeft = true;
                }

                if (KarateMan.instance.EligibleHits[currentHitInList].perfect)
                {
                    Jukebox.PlayOneShotGame(p.hitSnd);
                    p.Hit();

                    GameObject hit = Instantiate(HitEffect);
                    hit.transform.parent = HitEffect.transform.parent;
                    hit.SetActive(true);

                    switch (p.type)
                    {
                        case 0:
                            HitParticle.Play();
                            break;
                        case 1:
                            GameObject bulbHit = Instantiate(BulbHit);
                            bulbHit.transform.parent = BulbHit.transform.parent;
                            bulbHit.SetActive(true);
                            Destroy(bulbHit, 0.7f);
                            break;
                        case 2:
                            RockParticle.Play();
                            break;
                    }

                    Destroy(hit, 0.04f);
                }
                else
                {
                    Jukebox.PlayOneShot("miss");
                }
                p.isEligible = false;
                p.RemoveObject(currentHitInList, EligibleHits);
            }
            else
            {
                Jukebox.PlayOneShotGame("karateman/swingNoHit");
            }
            if (punchLeft)
                anim.Play("PunchLeft", 0, 0);
            else
                anim.Play("PunchRight", 0, 0);
        }
    }
}