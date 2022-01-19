using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class Pot : PlayerActionObject
    {
        public float startBeat;
        public float createBeat;
        [HideInInspector] public Animator anim;

        public GameObject Holder;
        private GameObject newHolder;
        public GameObject Sprite;
        public GameObject Shadow;

        public bool isThrown;
        public bool isHit = false;

        public float hitBeat;

        private Vector3 lastPos;
        private float lastShadowX;

        public AnimationCurve hitCurve;
        public AnimationCurve hitCurveY;
        public AnimationCurve hitCurveX;
        public AnimationCurve missCurve;
        public AnimationCurve shadowCurve;
        public AnimationCurve shadowCurveScale;

        public int type;
        public string hitSnd;

        private float hitLength;

        private float lastRot;

        public bool kick;

        public float lastPotRot;

        private void Start()
        {
            anim = GetComponent<Animator>();

            Sprite.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            isEligible = true;

            if (type == 2)
                hitLength = 14f;
            else
                hitLength = 14f;

            PlayerActionInit(this.gameObject, createBeat, KarateMan.instance.EligibleHits);
        }

        private void Update()
        {
            float time2Destroy = Conductor.instance.GetLoopPositionFromBeat(createBeat, 4);

            if (time2Destroy >= 1)
                Destroy(this.gameObject);

            if (isThrown)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2.22000000002f);
                anim.Play("PotThrow", 0, normalizedBeatAnim);
                anim.speed = 0;

                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 1);

                StateCheck(normalizedBeat);

                Shadow.transform.localScale = Vector3.Lerp(new Vector3(4.12f, 4.12f), new Vector3(0.34f, 0.34f), shadowCurveScale.Evaluate(normalizedBeatAnim));
                Shadow.transform.localPosition = new Vector3(Mathf.Lerp(7.63f, -1.036f, shadowCurve.Evaluate(normalizedBeatAnim)), Mathf.Lerp(-12.26f, -2.822f, shadowCurve.Evaluate(normalizedBeatAnim)));

                lastPos = Holder.transform.localPosition;
                lastPotRot = Holder.transform.eulerAngles.z;
                lastShadowX = Shadow.transform.localPosition.x;
                lastRot = Holder.transform.GetChild(0).eulerAngles.z;
            }

            if (!isHit && !isThrown)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.5f);
                newHolder.transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0, 0.55f, Conductor.instance.GetLoopPositionFromBeat(hitBeat, 0.45f)));
                Holder.transform.localPosition = new Vector3(Mathf.Lerp(lastPos.x, 0.9f, normalizedBeatAnim), Mathf.Lerp(lastPos.y, -3.43f, missCurve.Evaluate(normalizedBeatAnim)));
                Holder.transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastRot, lastRot - 523.203f, normalizedBeatAnim));
                Shadow.transform.localPosition = new Vector3(Mathf.Lerp(lastShadowX, 0.9f, normalizedBeatAnim), Shadow.transform.localPosition.y);
            }

            if (kick == false)
            {
                if (isHit)
                {
                    float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.5f);
                    var y = Mathf.Lerp(lastPos.y, -3.27f, hitCurve.Evaluate(normalizedBeatAnim));
                    var x = Mathf.Lerp(lastPos.x, hitLength, hitCurveX.Evaluate(normalizedBeatAnim));
                    newHolder.transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0, 0.45f, hitCurveY.Evaluate(normalizedBeatAnim)));
                    Holder.transform.localPosition = new Vector3(x, y);
                    Shadow.transform.localPosition = new Vector3(Mathf.Lerp(lastShadowX, hitLength, hitCurveX.Evaluate(normalizedBeatAnim)), Shadow.transform.localPosition.y);
                    Holder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastPotRot, lastPotRot - 360, normalizedBeatAnim));
                    // anim.Play("PotHit", 0, normalizedBeatAnim);
                    // anim.speed = 0;
                }
            }
            else
            {
                if (isHit)
                {
                    float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.5f);
                    newHolder.transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0, 0.55f, Conductor.instance.GetLoopPositionFromBeat(hitBeat, 0.45f)));
                    Holder.transform.localPosition = new Vector3(Mathf.Lerp(lastPos.x, 0.9f, normalizedBeatAnim), Mathf.Lerp(lastPos.y, -3.43f, missCurve.Evaluate(normalizedBeatAnim)));
                    Holder.transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastRot, lastRot - 523.203f, normalizedBeatAnim));
                    Shadow.transform.localPosition = new Vector3(Mathf.Lerp(lastShadowX, 0.9f, normalizedBeatAnim), Shadow.transform.localPosition.y);
                }
            }
        }

        public void Hit()
        {
            if (!kick)
            {
                NewHolder();
            }
            else if (kick)
            {
                KarateMan.instance.CreateBomb(this.transform.parent, Holder.transform.localScale, ref Shadow);

                Destroy(this.gameObject);
            }

            hitBeat = Conductor.instance.songPositionInBeats;

            anim.enabled = false;
            isThrown = false;
            isHit = true;

            Sprite.GetComponent<SpriteRenderer>().sortingOrder = 49;
        }

        public void Miss()
        {
            NewHolder();
            Holder.transform.parent = newHolder.transform;

            hitBeat = Conductor.instance.songPositionInBeats;
            isHit = false;
            isThrown = false;
            anim.enabled = false;
            Sprite.GetComponent<SpriteRenderer>().sortingOrder = 49;
        }

        private void NewHolder()
        {
            newHolder = new GameObject();
            newHolder.transform.parent = this.gameObject.transform;
            Holder.transform.parent = newHolder.transform;
        }
    }
}