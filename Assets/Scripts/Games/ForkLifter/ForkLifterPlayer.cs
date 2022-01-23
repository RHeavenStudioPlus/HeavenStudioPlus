using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.ForkLifter
{
    public class ForkLifterPlayer : MonoBehaviour
    {
        public static ForkLifterPlayer instance { get; set; }

        [Header("Objects")]
        public GameObject fork;
        public Sprite peaSprite;
        public Sprite hitFX;
        public Sprite hitFXG;
        public Sprite hitFXMiss;
        public Sprite hitFX2;
        public Transform early, perfect, late;

        [SerializeField]
        private BoxCollider2D col;

        private Animator anim;

        private int currentHitInList = 0;

        public int currentEarlyPeasOnFork;
        public int currentPerfectPeasOnFork;
        public int currentLatePeasOnFork;

        private bool isEating = false;

        // Burger shit

        public bool topbun, middleburger, bottombun;

        // -----------

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (PlayerInput.Pressed())
            {
                Stab(null);
            }

            if (ForkLifter.instance.EligibleHits.Count == 0)
            {
                currentHitInList = 0;
            }
        }

        public void Eat()
        {
            if (currentEarlyPeasOnFork != 0 || currentPerfectPeasOnFork != 0 || currentLatePeasOnFork != 0)
            {
                anim.Play("Player_Eat", 0, 0);
                isEating = true;
            }
        }

        public void EatConfirm()
        {
            if (topbun && middleburger && bottombun)
            {
                Jukebox.PlayOneShotGame("forkLifter/burger");
            }
            else
            {
                if (currentEarlyPeasOnFork > 0 || currentLatePeasOnFork > 0)
                {
                    Jukebox.PlayOneShotGame($"forkLifter/cough_{Random.Range(1, 3)}");
                }
                else
                {
                    Jukebox.PlayOneShotGame("forkLifter/gulp");
                }
            }

            RemoveObjFromFork();
        }

        public void RemoveObjFromFork()
        {
            for (int i = 0; i < early.transform.childCount; i++)
            {
                Destroy(early.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < perfect.transform.childCount; i++)
            {
                Destroy(perfect.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < late.transform.childCount; i++)
            {
                Destroy(late.transform.GetChild(i).gameObject);
            }
            currentEarlyPeasOnFork = 0;
            currentPerfectPeasOnFork = 0;
            currentLatePeasOnFork = 0;

            isEating = false;

            topbun = false; middleburger = false; bottombun = false;
        }

        public void Stab(Pea p)
        {
            if (isEating) return;
            /*var EligibleHits = ForkLifter.instance.EligibleHits;
            bool canHit = (ForkLifter.instance.EligibleHits.Count > 0) && (currentHitInList < ForkLifter.instance.EligibleHits.Count);

            int events = ForkLifter.instance.MultipleEventsAtOnce();

            for (int pt = 0; pt < events; pt++)
            {
                if (canHit)
                {
                    GameObject pea = new GameObject();

                    if (EligibleHits[currentHitInList].perfect)
                    {
                        pea.transform.parent = perfect.transform;
                        pea.transform.localScale = Vector2.one;

                        pea.transform.localPosition = Vector3.zero;

                        for (int i = 0; i < perfect.transform.childCount; i++)
                        {
                            perfect.transform.GetChild(i).transform.localPosition = new Vector3(0, (-1.67f - (0.15724f * i)) + 0.15724f * currentPerfectPeasOnFork);
                        }

                        SpriteRenderer psprite = pea.AddComponent<SpriteRenderer>();
                        psprite.sprite = ForkLifter.instance.peaHitSprites[EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type];
                        psprite.sortingOrder = 20;
                        switch (EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type)
                        {
                            case 0:
                                psprite.sortingOrder = 101;
                                break;
                            case 1:
                                psprite.sortingOrder = 104;
                                break;
                            case 2:
                                psprite.sortingOrder = 103;
                                break;
                            case 3:
                                psprite.sortingOrder = 102;
                                break;
                        }

                        GameObject hitFXo = new GameObject();
                        hitFXo.transform.localPosition = new Vector3(1.9969f, -3.7026f);
                        hitFXo.transform.localScale = new Vector3(3.142196f, 3.142196f);
                        SpriteRenderer hfxs = hitFXo.AddComponent<SpriteRenderer>();
                        hfxs.sprite = hitFX;
                        hfxs.sortingOrder = 100;
                        hfxs.DOColor(new Color(1, 1, 1, 0), 0.05f).OnComplete(delegate { Destroy(hitFXo); });

                        FastEffectHit(ForkLifter.instance.EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type);

                        Jukebox.PlayOneShotGame("forkLifter/stab");

                        currentPerfectPeasOnFork++;

                        if (EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type == 1)
                        {
                            topbun = true;
                        }
                        else if (EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type == 2)
                        {
                            middleburger = true;
                        }
                        else if (EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type == 3)
                        {
                            bottombun = true;
                        }

                        RemovePea();

                        GameProfiler.instance.IncreaseScore();
                    }
                    else if (EligibleHits[currentHitInList].early)
                    {
                        pea.transform.parent = early.transform;
                        pea.transform.localScale = Vector2.one;

                        pea.transform.localPosition = Vector3.zero;
                        pea.transform.localRotation = Quaternion.Euler(0, 0, 90);

                        for (int i = 0; i < early.transform.childCount; i++)
                        {
                            early.transform.GetChild(i).transform.localPosition = new Vector3(0, (-1.67f - (0.15724f * i)) + 0.15724f * currentEarlyPeasOnFork);
                        }

                        SpriteRenderer psprite = pea.AddComponent<SpriteRenderer>();
                        psprite.sprite = ForkLifter.instance.peaHitSprites[EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type];
                        psprite.sortingOrder = 20;
                        HitFXMiss(new Vector2(1.0424f, -4.032f), new Vector2(1.129612f, 1.129612f));
                        HitFXMiss(new Vector2(0.771f, -3.016f), new Vector2(1.71701f, 1.71701f));
                        HitFXMiss(new Vector2(2.598f, -2.956f), new Vector2(1.576043f, 1.576043f));
                        HitFXMiss(new Vector2(2.551f, -3.609f), new Vector2(1.200788f, 1.200788f));

                        FastEffectHit(ForkLifter.instance.EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type);

                        Jukebox.PlayOneShot("miss");

                        currentEarlyPeasOnFork++;

                        RemovePea();
                    }
                    else if (EligibleHits[currentHitInList].late)
                    {
                        pea.transform.parent = late.transform;
                        pea.transform.localScale = Vector2.one;

                        pea.transform.localPosition = Vector3.zero;
                        pea.transform.localRotation = Quaternion.Euler(0, 0, 90);

                        for (int i = 0; i < late.transform.childCount; i++)
                        {
                            late.transform.GetChild(i).transform.localPosition = new Vector3(0, (-1.67f - (0.15724f * i)) + 0.15724f * currentLatePeasOnFork);
                        }

                        SpriteRenderer psprite = pea.AddComponent<SpriteRenderer>();
                        psprite.sprite = ForkLifter.instance.peaHitSprites[EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type];
                        psprite.sortingOrder = 20;
                        HitFXMiss(new Vector2(1.0424f, -4.032f), new Vector2(1.129612f, 1.129612f));
                        HitFXMiss(new Vector2(0.771f, -3.016f), new Vector2(1.71701f, 1.71701f));
                        HitFXMiss(new Vector2(2.598f, -2.956f), new Vector2(1.576043f, 1.576043f));
                        HitFXMiss(new Vector2(2.551f, -3.609f), new Vector2(1.200788f, 1.200788f));

                        FastEffectHit(ForkLifter.instance.EligibleHits[currentHitInList].gameObject.GetComponent<Pea>().type);

                        Jukebox.PlayOneShot("miss");

                        currentLatePeasOnFork++;

                        RemovePea();
                    }
                }
            }*/

            if (p == null)
            {
                Jukebox.PlayOneShotGame("forkLifter/stabnohit");
            }

            anim.Play("Player_Stab", 0, 0);
        }

        public void FastEffectHit(int type)
        {
            GameObject hitFX2o = new GameObject();
            hitFX2o.transform.localPosition = new Vector3(0.11f, -2.15f);
            hitFX2o.transform.localScale = new Vector3(5.401058f, 1.742697f);
            hitFX2o.transform.localRotation = Quaternion.Euler(0, 0, -38.402f);
            SpriteRenderer hfx2s = hitFX2o.AddComponent<SpriteRenderer>();
            if (type == 2)
                hfx2s.sprite = hitFXG;
            else
                hfx2s.sprite = hitFX2;
            hfx2s.sortingOrder = -5;
            hfx2s.DOColor(new Color(1, 1, 1, 0), 0.07f).OnComplete(delegate { Destroy(hitFX2o); });
        }

        public void HitFXMiss(Vector2 pos, Vector2 size)
        {
            GameObject hitFXo = new GameObject();
            hitFXo.transform.localPosition = new Vector3(pos.x, pos.y);
            hitFXo.transform.localScale = new Vector3(size.x, size.y);
            SpriteRenderer hfxs = hitFXo.AddComponent<SpriteRenderer>();
            hfxs.sprite = hitFXMiss;
            hfxs.sortingOrder = 100;
            hfxs.DOColor(new Color(1, 1, 1, 0), 0.05f).OnComplete(delegate { Destroy(hitFXo); });
        }
    }
}