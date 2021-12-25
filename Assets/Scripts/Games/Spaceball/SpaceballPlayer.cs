using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.Spaceball
{
    public class SpaceballPlayer : MonoBehaviour
    {
        private Animator anim;

        public List<Minigame.Eligible> EligibleHits = new List<Minigame.Eligible>();
        [SerializeField] private int currentHitInList = 0;

        public static SpaceballPlayer instance { get; set; }

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
            if (EligibleHits.Count == 0)
                currentHitInList = 0;

            if (PlayerInput.Pressed())
            {
                Swing();
            }
        }

        public void Swing()
        {
            bool canHit = (EligibleHits.Count > 0) && (currentHitInList < EligibleHits.Count);
            if (canHit)
            {
                if (EligibleHits[currentHitInList].perfect)
                {
                    Jukebox.PlayOneShotGame("spaceball/hit");
                    EligibleHits[currentHitInList].gameObject.GetComponent<SpaceballBall>().Holder.transform.DOMove(new Vector3(Random.Range(5, 25), 0, -600), 5f);

                    EligibleHits[currentHitInList].gameObject.GetComponent<SpaceballBall>().enabled = false;
                    EligibleHits[currentHitInList].gameObject.GetComponent<Animator>().enabled = false;

                }
                else
                {
                    EligibleHits[currentHitInList].gameObject.GetComponent<SpaceballBall>().Holder.transform.GetChild(0).gameObject.AddComponent<Rotate>().rotateSpeed = -55;

                    EligibleHits[currentHitInList].gameObject.GetComponent<SpaceballBall>().enabled = false;
                    EligibleHits[currentHitInList].gameObject.GetComponent<Animator>().enabled = false;

                    Rigidbody2D rb = EligibleHits[currentHitInList].gameObject.AddComponent<Rigidbody2D>();
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.AddForce(transform.up * 1100);
                    rb.AddForce(transform.right * 400);
                    rb.gravityScale = 9;

                    Jukebox.PlayOneShot("miss");
                }
                RemoveBall();
            }
            else
                Jukebox.PlayOneShotGame("spaceball/swing");
            anim.Play("Swing", 0, 0);
        }

        private void RemoveBall()
        {
            if (currentHitInList < EligibleHits.Count)
            {
                EligibleHits.Remove(EligibleHits[currentHitInList]);
                currentHitInList++;
            }
        }
    }
}