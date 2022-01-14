using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

using DG.Tweening;

namespace RhythmHeavenMania.Games.Spaceball
{
    public class SpaceballBall : MonoBehaviour
    {
        public float startBeat;
        private Animator anim;
        private int lastState;
        private bool inList = false;

        public bool high;

        private Minigame.Eligible e = new Minigame.Eligible();

        public GameObject Holder;
        public SpriteRenderer Sprite;

        private void Start()
        {
            anim = GetComponent<Animator>();

            e.gameObject = this.gameObject;

            float rot = Random.Range(0, 360);
            Sprite.gameObject.transform.eulerAngles = new Vector3(0, 0, rot);
        }

        private void Update()
        {
            float beatLength = 1f;
            if (high) beatLength = 2f;

            float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, beatLength + 0.15f);
            // print(normalizedBeatAnim + " " + Time.frameCount);

            if (high)
            {
                anim.Play("BallHigh", 0, normalizedBeatAnim);
            }
            else
            {
                anim.Play("BallLow", 0, normalizedBeatAnim);
            }

            anim.speed = 0;

            float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, beatLength);

            if (normalizedBeat > Minigame.EarlyTime() && normalizedBeat < Minigame.PerfectTime() && lastState == 0)
            {
                MakeEligible(true, false, false);
                lastState++;
            }
            // Perfect State
            else if (normalizedBeat > Minigame.PerfectTime() && normalizedBeat < Minigame.LateTime() && lastState == 1)
            {
                MakeEligible(false, true, false);
                lastState++;
            }
            // Late State
            else if (normalizedBeat > Minigame.LateTime() && normalizedBeat < Minigame.EndTime() && lastState == 2)
            {
                MakeEligible(false, false, true);
                lastState++;
            }
            else if (normalizedBeat < Minigame.EarlyTime() || normalizedBeat > Minigame.EndTime())
            {
                MakeInEligible();
            }

            // too lazy to make a proper fix for this
            float endTime = 1.2f;
            if (high) endTime = 1.1f;

            if (normalizedBeat > endTime)
            {
                Jukebox.PlayOneShotGame("spaceball/fall");
                Instantiate(Spaceball.instance.Dust, Spaceball.instance.Dust.transform.parent).SetActive(true);
                Destroy(this.gameObject);
            }

            if (PlayerInput.Pressed())
            {
                if (e.perfect)
                {
                    Jukebox.PlayOneShotGame("spaceball/hit");
                    Holder.transform.DOLocalMove(new Vector3(Random.Range(5, 18), 0, -600), 4f).SetEase(Ease.Linear);
                    Holder.transform.GetChild(0).gameObject.AddComponent<Rotate>().rotateSpeed = -245;

                    this.enabled = false;
                    gameObject.GetComponent<Animator>().enabled = false;
                }
                else if (e.late || e.early)
                {
                    Holder.transform.GetChild(0).gameObject.AddComponent<Rotate>().rotateSpeed = -55;

                    this.enabled = false;
                    gameObject.GetComponent<Animator>().enabled = false;

                    Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.AddForce(transform.up * 1100);
                    rb.AddForce(transform.right * 400);
                    rb.gravityScale = 9;

                    Jukebox.PlayOneShot("miss");
                }
            }
        }

        public void MakeEligible(bool early, bool perfect, bool late)
        {
            // print($"{early}, {perfect}, {late}");

            if (!inList)
            {
                e.early = early;
                e.perfect = perfect;
                e.late = late;

                SpaceballPlayer.instance.EligibleHits.Add(e);
                inList = true;
            }
            else
            {
                Minigame.Eligible es = SpaceballPlayer.instance.EligibleHits[SpaceballPlayer.instance.EligibleHits.IndexOf(e)];
                es.early = early;
                es.perfect = perfect;
                es.late = late;
            }
        }

        public void MakeInEligible()
        {
            if (!inList) return;

            SpaceballPlayer.instance.EligibleHits.Remove(e);
            inList = false;
        }
    }

}