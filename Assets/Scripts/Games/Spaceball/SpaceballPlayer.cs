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

        private int currentHitInList = 0;

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
            if (Spaceball.instance.EligibleHits.Count == 0)
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
            var EligibleHits = Spaceball.instance.EligibleHits;
            bool canHit = (Spaceball.instance.EligibleHits.Count > 0) && (currentHitInList < Spaceball.instance.EligibleHits.Count);

            int events = Spaceball.instance.MultipleEventsAtOnce();

            for (int eventI = 0; eventI < events; eventI++)
            {
                if (canHit)
                {
                    SpaceballBall ball = EligibleHits[currentHitInList].gameObject.GetComponent<SpaceballBall>();

                    if (EligibleHits[currentHitInList].perfect)
                    {
                        ball.hit = true;
                        ball.hitBeat = Conductor.instance.songPositionInBeats;
                        ball.hitPos = ball.Holder.transform.localPosition;
                        ball.hitRot = ball.Holder.transform.eulerAngles.z;

                        Jukebox.PlayOneShotGame("spaceball/hit");

                        ball.randomEndPosX = Random.Range(40f, 55f);

                        ball.anim.enabled = false;
                    }
                    else if (EligibleHits[currentHitInList].late || EligibleHits[currentHitInList].early)
                    {
                        ball.Holder.transform.GetChild(0).gameObject.AddComponent<Rotate>().rotateSpeed = -55;

                        ball.enabled = false;
                        ball.anim.enabled = false;

                        Rigidbody2D rb = ball.gameObject.AddComponent<Rigidbody2D>();
                        rb.bodyType = RigidbodyType2D.Dynamic;
                        rb.AddForce(transform.up * 1100);
                        rb.AddForce(transform.right * 400);
                        rb.gravityScale = 9;

                        Jukebox.PlayOneShot("miss");
                    }

                    ball.RemoveObject(currentHitInList);
                }
            }

            if (!canHit)
                Jukebox.PlayOneShotGame("spaceball/swing");

            anim.Play("Swing", 0, 0);
        }

        public void SetSprite(int id)
        {
            PlayerSprite.sprite = PlayerSpriteSheets[costume].sprites[id];
        }
    }
}