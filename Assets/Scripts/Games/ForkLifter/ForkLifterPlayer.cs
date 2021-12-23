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

        public List<Eligible> EligibleHits = new List<Eligible>();
        private int currentHitInList = 0;


        public float timescale = 1;

        private int currentEarlyPeasOnFork;
        private int currentPerfectPeasOnFork;
        private int currentLatePeasOnFork;

        private bool isEating = false;

        // Burger shit

        private bool topbun, middleburger, bottombun;

        // -----------

        [System.Serializable]
        public class Eligible
        {
            public Pea pea;
            public bool early;
            public bool perfect;
            public bool late;
        }

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
            Time.timeScale = timescale;

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
            {
                Stab();
            }

            if (EligibleHits.Count == 0)
            {
                currentHitInList = 0;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Conductor.instance.musicSource.time += 3;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Conductor.instance.musicSource.time -= 3;
                GameManager.instance.SetCurrentEventToClosest();
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
                Jukebox.PlayOneShot("burger");
            }
            else
            {
                if (currentEarlyPeasOnFork > 0 || currentLatePeasOnFork > 0)
                {
                    Jukebox.PlayOneShot($"cough_{Random.Range(1, 3)}");
                }
                else
                {
                    Jukebox.PlayOneShot("gulp");
                }
            }

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

        public void Stab()
        {
            if (isEating) return;
            bool canHit = (EligibleHits.Count > 0) && (currentHitInList < EligibleHits.Count);

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
                    psprite.sprite = ForkLifter.instance.peaHitSprites[EligibleHits[currentHitInList].pea.type];
                    psprite.sortingOrder = 20;
                    switch (EligibleHits[currentHitInList].pea.type)
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

                    FastEffectHit(EligibleHits[currentHitInList].pea.type);

                    Jukebox.PlayOneShot("stab");

                    currentPerfectPeasOnFork++;

                    if (EligibleHits[currentHitInList].pea.type == 1)
                    {
                        topbun = true;
                    }
                    else if (EligibleHits[currentHitInList].pea.type == 2)
                    {
                        middleburger = true;
                    }
                    else if (EligibleHits[currentHitInList].pea.type == 3)
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
                    psprite.sprite = ForkLifter.instance.peaHitSprites[EligibleHits[currentHitInList].pea.type];
                    psprite.sortingOrder = 20;
                    HitFXMiss(new Vector2(1.0424f, -4.032f), new Vector2(1.129612f, 1.129612f));
                    HitFXMiss(new Vector2(0.771f, -3.016f), new Vector2(1.71701f, 1.71701f));
                    HitFXMiss(new Vector2(2.598f, -2.956f), new Vector2(1.576043f, 1.576043f));
                    HitFXMiss(new Vector2(2.551f, -3.609f), new Vector2(1.200788f, 1.200788f));

                    FastEffectHit(EligibleHits[currentHitInList].pea.type);

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
                    psprite.sprite = ForkLifter.instance.peaHitSprites[EligibleHits[currentHitInList].pea.type];
                    psprite.sortingOrder = 20;
                    HitFXMiss(new Vector2(1.0424f, -4.032f), new Vector2(1.129612f, 1.129612f));
                    HitFXMiss(new Vector2(0.771f, -3.016f), new Vector2(1.71701f, 1.71701f));
                    HitFXMiss(new Vector2(2.598f, -2.956f), new Vector2(1.576043f, 1.576043f));
                    HitFXMiss(new Vector2(2.551f, -3.609f), new Vector2(1.200788f, 1.200788f));

                    FastEffectHit(EligibleHits[currentHitInList].pea.type);

                    Jukebox.PlayOneShot("miss");

                    currentLatePeasOnFork++;

                    RemovePea();
                }
            }
            else
            {
                Jukebox.PlayOneShot("stabnohit");
            }

            anim.Play("Player_Stab", 0, 0);
        }

        private void FastEffectHit(int type)
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

        private void HitFXMiss(Vector2 pos, Vector2 size)
        {
            GameObject hitFXo = new GameObject();
            hitFXo.transform.localPosition = new Vector3(pos.x, pos.y);
            hitFXo.transform.localScale = new Vector3(size.x, size.y);
            SpriteRenderer hfxs = hitFXo.AddComponent<SpriteRenderer>();
            hfxs.sprite = hitFXMiss;
            hfxs.sortingOrder = 100;
            hfxs.DOColor(new Color(1, 1, 1, 0), 0.05f).OnComplete(delegate { Destroy(hitFXo); });
        }

        private void RemovePea()
        {
            if (currentHitInList < EligibleHits.Count)
            {
                Destroy(EligibleHits[currentHitInList].pea.gameObject);
                EligibleHits.Remove(EligibleHits[currentHitInList]);
                currentHitInList++;
            }
        }
    }
}