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

        public bool hitBarrel = false;
        private Coroutine kickC;

        private float barrelBeat;

        public static KarateJoe instance { get; set; }

        private void Start()
        {
            instance = this;
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (hitBarrel)
            {
                if (PlayerInput.PressedUp())
                {
                    if (kickC != null) StopCoroutine(kickC);
                    hitBarrel = false;
                    anim.Play("Kick", 0, 0);
                }

                if (Conductor.instance.songPositionInBeats > barrelBeat + 3)
                {
                    if (kickC != null) StopCoroutine(kickC);
                    hitBarrel = false;
                    // should be inebetween for this
                    anim.Play("Idle", 0, 0);
                }
            }
            else
            {
                if (PlayerInput.Pressed())
                {
                    Swing();
                }
            }
        }

        private IEnumerator PrepareKick()
        {
            barrelBeat = Conductor.instance.songPositionInBeats;
            hitBarrel = true;
            yield return new WaitForSeconds(0.17f);
            anim.Play("KickPrepare", 0, 0);
        }

        private void Swing()
        {
            var EligibleHits = KarateMan.instance.EligibleHits;
            bool canHit = (EligibleHits.Count > 0) && (currentHitInList < EligibleHits.Count);

            bool punchLeft = true;

            int events = KarateMan.instance.MultipleEventsAtOnce();

            for (int pt = 0; pt < events; pt++)
            {
                if (canHit)
                {
                    Pot p = EligibleHits[currentHitInList].gameObject.GetComponent<Pot>();

                    if (p.type == 2 || p.type == 3 || p.type == 4)
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

                        HitEffectF(HitEffect.transform.localPosition);

                        switch (p.type)
                        {
                            case 0:
                                // HitParticle.Play();
                                break;
                            case 1:
                                GameObject bulbHit = Instantiate(BulbHit);
                                bulbHit.transform.parent = BulbHit.transform.parent;
                                bulbHit.SetActive(true);
                                Destroy(bulbHit, 0.7f);
                                break;
                            case 2:
                                // RockParticle.Play();
                                break;
                            case 4:
                                if (kickC != null) StopCoroutine(kickC);
                                kickC = StartCoroutine(PrepareKick());
                                for (int i = 0; i < 8; i++)
                                {
                                    GameObject be = new GameObject();
                                    be.transform.localPosition = p.transform.localPosition;
                                    be.transform.parent = this.transform.parent;
                                    be.transform.localScale = p.Holder.transform.localScale;
                                    BarrelDestroyEffect bde = be.AddComponent<BarrelDestroyEffect>();
                                    Vector3 pos = be.transform.localPosition;
                                    SpriteRenderer sprite = be.AddComponent<SpriteRenderer>();

                                    bde.shadow = Instantiate(p.Shadow, transform.parent);
                                    bde.shadow.transform.position = p.Shadow.transform.position;
                                    bde.shadow.transform.localScale = p.Shadow.transform.lossyScale;
                                    bde.index = i;

                                    switch (i)
                                    {
                                        case 0:
                                            be.transform.localPosition = new Vector3(pos.x, pos.y + 1.25f);
                                            sprite.sortingOrder = 35;
                                            bde.spriteIndex = 3;
                                            break;
                                        case 1:
                                            be.transform.localPosition = new Vector3(pos.x, pos.y + -0.55f);
                                            sprite.sortingOrder = 31;
                                            bde.spriteIndex = 3;
                                            break;
                                        case 2:
                                            be.transform.localPosition = new Vector3(pos.x - 0.8f, pos.y + 0.45f);
                                            sprite.sortingOrder = 32;
                                            bde.spriteIndex = 0;
                                            break;
                                        case 3:
                                            be.transform.localPosition = new Vector3(pos.x - 0.5f, pos.y + 0.45f);
                                            sprite.sortingOrder = 33;
                                            bde.spriteIndex = 1;
                                            break;
                                        case 4:
                                            be.transform.localPosition = new Vector3(pos.x, pos.y + 0.45f);
                                            sprite.sortingOrder = 34;
                                            bde.spriteIndex = 2;
                                            break;
                                        case 5:
                                            be.transform.localPosition = new Vector3(pos.x + 0.5f, pos.y + 0.45f);
                                            sprite.sortingOrder = 33;
                                            sprite.flipX = true;
                                            bde.spriteIndex = 1;
                                            break;
                                        case 6:
                                            be.transform.localPosition = new Vector3(pos.x + 0.8f, pos.y + 0.45f);
                                            sprite.sortingOrder = 32;
                                            sprite.flipX = true;
                                            bde.spriteIndex = 0;
                                            break;
                                        case 7:
                                            be.transform.localPosition = new Vector3(pos.x, pos.y + 1.25f);
                                            sprite.sortingOrder = 39;
                                            bde.spriteIndex = 4;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        Jukebox.PlayOneShot("miss");
                        p.Miss();
                    }
                    p.isEligible = false;
                    p.RemoveObject(currentHitInList);
                }
            }

            if (!canHit)
                Jukebox.PlayOneShotGame("karateman/swingNoHit");

            if (punchLeft)
                anim.Play("PunchLeft", 0, 0);
            else
                anim.Play("PunchRight", 0, 0);
        }

        public void HitEffectF(Vector3 pos)
        {
            GameObject hit = Instantiate(HitEffect);
            hit.transform.parent = HitEffect.transform.parent;
            hit.transform.localPosition = pos;
            hit.SetActive(true);
            Destroy(hit, 0.06f);
        }
    }
}