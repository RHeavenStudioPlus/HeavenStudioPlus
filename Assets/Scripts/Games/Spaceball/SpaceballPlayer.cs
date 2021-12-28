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

        public int costume;

        public SpriteRenderer PlayerSprite;
        public List<SpriteSheet> PlayerSpriteSheets = new List<SpriteSheet>();

        [System.Serializable]
        public class SpriteSheet
        {
            public List<Sprite> sprites;
        }

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

        public void SetCostume(int costume)
        {
            this.costume = costume;
            anim.Play("Idle", 0, 0);
        }

        public void Swing()
        {
            bool canHit = (EligibleHits.Count > 0) && (currentHitInList < EligibleHits.Count);

            if (canHit)
            {
                if (EligibleHits[currentHitInList].perfect)
                {
                    Jukebox.PlayOneShotGame("spaceball/hit");
                    EligibleHits[currentHitInList].gameObject.GetComponent<SpaceballBall>().Holder.transform.DOLocalMove(new Vector3(Random.Range(0, 25), 0, -600), 7f).SetEase(Ease.Linear);
                    EligibleHits[currentHitInList].gameObject.GetComponent<SpaceballBall>().Holder.transform.GetChild(0).gameObject.AddComponent<Rotate>().rotateSpeed = -95;

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

        public void SetSprite(int id)
        {
            PlayerSprite.sprite = PlayerSpriteSheets[costume].sprites[id];
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