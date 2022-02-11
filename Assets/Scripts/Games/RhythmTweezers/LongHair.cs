using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.RhythmTweezers
{
    public class LongHair : PlayerActionObject
    {
        public float createBeat;
        public GameObject hairSprite;
        public GameObject stubbleSprite;
        private RhythmTweezers game;
        private Tweezers tweezers;

        private bool isHolding = false;
        private float holdBeat = 0f;

        public GameObject holder;

        private void Awake()
        {
            game = RhythmTweezers.instance;
            tweezers = game.Tweezers;
        }

        private void Update()
        {
            float stateBeat = Conductor.instance.GetPositionFromBeat(createBeat + game.tweezerBeatOffset, game.beatInterval);
            StateCheck(stateBeat);

            if (PlayerInput.Pressed() && tweezers.hitOnFrame == 0)
            {
                if (state.perfect)
                {
                    Jukebox.PlayOneShotGame($"rhythmTweezers/longPull{UnityEngine.Random.Range(1, 5)}");
                    isHolding = true;
                    holdBeat = Conductor.instance.songPositionInBeats;
                }
            }

            if (isHolding && Conductor.instance.songPositionInBeats >= holdBeat + 0.5f)
            {
                Destroy(holder.transform.GetChild(0).gameObject);
                isHolding = false;
                Ace();
            }


            if (isHolding)
            {
                holder.transform.eulerAngles = new Vector3(0, 0, tweezers.transform.eulerAngles.z * 1.056f);
                holder.transform.GetChild(0).transform.localScale = Vector2.one / holder.transform.localScale;

                float normalizedBeat = Conductor.instance.GetPositionFromBeat(holdBeat, 0.5f);
                GetComponent<Animator>().Play("LoopPull", 0, normalizedBeat);
                tweezers.anim.Play("Tweezers_LongPluck", 0, normalizedBeat);
                // float angleBetweenTweezersAndHair = angleBtw2Points(tweezers.transform.position, holder.transform.position);
                // holder.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleBetweenTweezersAndHair));
            }
        }


        float angleBtw2Points(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        public void Ace()
        {
            Jukebox.PlayOneShotGame("rhythmTweezers/longPullEnd");
            tweezers.LongPluck(true, this);

            tweezers.hitOnFrame++;
        }
    }
}