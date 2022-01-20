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

        private bool inCombo;
        private bool hitCombo;
        private float comboBeat;

        public List<Pot> currentComboPots = new List<Pot>();
        private int comboPotIndex;
        private int currentComboHitInList;
        private int comboIndex;

        public static KarateJoe instance { get; set; }

        private void Start()
        {
            instance = this;
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (inCombo)
            {
                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(comboBeat, 1);

                if (hitCombo)
                {
                    if (currentComboPots[comboPotIndex] == null) return;
                }
                else
                {
                    normalizedBeat += 1;
                }

                if (normalizedBeat >= 1 && comboIndex < 1)
                {
                    if (hitCombo)
                    {
                        currentComboPots[comboPotIndex].Hit();
                        HitEffectF(currentComboPots[comboPotIndex].Holder.transform.localPosition);
                        comboPotIndex++;
                        Jukebox.PlayOneShotGame("karateman/comboHit1");
                    }
                    comboIndex++;
                    anim.Play("PunchLeft", 0, 0);
                }
                else if (normalizedBeat >= 1.25f && comboIndex < 2)
                {
                    if (hitCombo)
                    {
                        currentComboPots[comboPotIndex].Hit();
                        HitEffectF(currentComboPots[comboPotIndex].Holder.transform.localPosition);
                        comboPotIndex++;
                        Jukebox.PlayOneShotGame("karateman/comboHit1");
                    }
                    comboIndex++;
                    anim.Play("PunchRight", 0, 0);
                }
                else if (normalizedBeat >= 1.5f && comboIndex < 3)
                {
                    if (hitCombo)
                    {
                        currentComboPots[comboPotIndex].Hit();
                        HitEffectF(currentComboPots[comboPotIndex].Holder.transform.localPosition);
                        comboPotIndex++;
                        Jukebox.PlayOneShotGame("karateman/comboHit2");
                    }
                    comboIndex++;
                    anim.Play("ComboCrouch", 0, 0);
                }
                else if (normalizedBeat >= 1.75f && comboIndex < 4)
                {
                    if (hitCombo)
                    {
                        currentComboPots[comboPotIndex].Hit();
                        HitEffectF(currentComboPots[comboPotIndex].Holder.transform.localPosition);
                        comboPotIndex++;
                        Jukebox.PlayOneShotGame("karateman/comboHit3");
                    }
                    comboIndex++;
                    anim.Play("ComboKick", 0, 0);
                }
                else if (normalizedBeat >= 2f && comboIndex < 5)
                {
                    if (hitCombo)
                    {
                        currentComboPots[comboPotIndex].Hit();
                        HitEffectF(currentComboPots[comboPotIndex].Holder.transform.localPosition);
                        comboPotIndex++;
                        Jukebox.PlayOneShotGame("karateman/comboHit3");
                    }
                    comboIndex++;
                    anim.Play("ComboCrouchPunch", 0, 0);
                }
                else if (normalizedBeat >= 2.05f)
                {
                    if (hitCombo)
                    {
                        if (PlayerInput.AltPressedUp())
                        {
                            ComboPow();
                        }
                    }
                    else
                    {
                        // fail anim
                        anim.Play("Idle");
                        ResetCombo();
                    }
                }
            }
            else
            {
                if (PlayerInput.AltPressed())
                {
                    Combo();
                }
            }

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
                if (PlayerInput.Pressed() && !inCombo)
                {
                    Swing();
                }
            }
        }

        private void Combo()
        {
            var EligibleHits = KarateMan.instance.EligibleCombos;
            bool canHit = (EligibleHits.Count > 0) && (currentComboHitInList < EligibleHits.Count);

            if (canHit)
            {
                if (KarateMan.instance.EligibleCombos[currentComboHitInList].perfect)
                {
                    comboBeat = EligibleHits[currentComboHitInList].createBeat;
                    hitCombo = true;
                // Debug.Break();
                }
                else
                {
                    comboBeat = Conductor.instance.songPositionInBeats;
                    hitCombo = false;
                }
            }
            else
            {
                comboBeat = Conductor.instance.songPositionInBeats;
                hitCombo = false;
            }

            inCombo = true;
        }

        private void ComboPow()
        {
            if (!hitCombo || !inCombo || !hitCombo && !inCombo) return;

            anim.Play("Pow", 0, 0);

            if (currentComboPots[comboPotIndex].state.perfect)
            {
                BarrelDestroy(currentComboPots[comboPotIndex], true);
                HitEffectF(currentComboPots[comboPotIndex].Holder.transform.localPosition);
                Destroy(currentComboPots[comboPotIndex].gameObject);
                Jukebox.PlayOneShotGame("karateman/comboHit4");
            }
            else
            {
                Jukebox.PlayOneShot("miss");
                currentComboPots[comboPotIndex].Miss();
            }

            ResetCombo();
        }

        private void ResetCombo()
        {
            hitCombo = false;
            inCombo = false;
            comboPotIndex = 0;
            comboIndex = 0;
            currentComboHitInList = 0;
            currentComboPots.Clear();
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
                                BarrelDestroy(p, false);
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

        private void BarrelDestroy(Pot p, bool combo)
        {
            for (int i = 0; i < 8; i++)
            {
                GameObject be = new GameObject();
                be.transform.localPosition = p.Holder.transform.localPosition;
                be.transform.parent = this.transform.parent;
                be.transform.localScale = p.Holder.transform.localScale;
                BarrelDestroyEffect bde = be.AddComponent<BarrelDestroyEffect>();
                Vector3 pos = be.transform.localPosition;
                SpriteRenderer sprite = be.AddComponent<SpriteRenderer>();

                bde.shadow = Instantiate(p.Shadow, transform.parent);
                bde.shadow.transform.position = p.Shadow.transform.position;
                bde.shadow.transform.localScale = p.Shadow.transform.lossyScale;
                bde.index = i;
                bde.combo = combo;

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
        }
    }
}