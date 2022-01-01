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
        [SerializeField] private GameObject Shadow;

        public bool isThrown;
        public bool isHit = false;

        public float hitBeat;

        private Vector3 lastPos;
        private float lastShadowX;

        public AnimationCurve hitCurve;
        public AnimationCurve hitCurveX;
        public AnimationCurve missCurve;

        public int type;
        public string hitSnd;

        private float hitLength;

        private float lastRot;

        public bool kick;

        private void Start()
        {
            PlayerActionInit(this.gameObject);
            anim = GetComponent<Animator>();

            Sprite.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            isEligible = true;

            if (type == 2)
                hitLength = 23.45f;
            else
                hitLength = 16f;
        }

        private void Update()
        {
            float time2Destroy = Conductor.instance.GetLoopPositionFromBeat(createBeat, 4);

            if (time2Destroy >= 1)
                Destroy(this.gameObject);

            if (isThrown)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(startBeat, 2.15f);
                anim.Play("PotThrow", 0, normalizedBeatAnim);
                anim.speed = 0;

                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(startBeat, 1);

                StateCheck(normalizedBeat, KarateMan.instance.EligibleHits);

                lastPos = Holder.transform.localPosition;
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
                    float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(hitBeat, 1.15f);
                    var y = Mathf.Lerp(lastPos.y, -3.27f, hitCurve.Evaluate(normalizedBeatAnim));
                    var x = Mathf.Lerp(lastPos.x, hitLength, hitCurveX.Evaluate(normalizedBeatAnim));
                    newHolder.transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(0, 0.55f, Conductor.instance.GetLoopPositionFromBeat(hitBeat, 0.45f)));
                    Holder.transform.localPosition = new Vector3(x, y);
                    Shadow.transform.localPosition = new Vector3(Mathf.Lerp(lastShadowX, hitLength, hitCurveX.Evaluate(normalizedBeatAnim)), Shadow.transform.localPosition.y);
                    // anim.Play("PotHit", 0, normalizedBeatAnim);
                    // anim.speed = 0;
                }
            }
        }

        public void Hit()
        {
            if (!kick)
            {
                newHolder = new GameObject();
                newHolder.transform.parent = this.gameObject.transform;
                Holder.transform.parent = newHolder.transform;
                Holder.transform.GetChild(0).gameObject.AddComponent<Rotate>().rotateSpeed = -7 * Conductor.instance.songBpm;

                hitBeat = Conductor.instance.songPositionInBeats;

                anim.enabled = false;
                isThrown = false;
                isHit = true;

                Sprite.GetComponent<SpriteRenderer>().sortingOrder = 49;
            }
            else if (kick)
            {
                Instantiate(KarateMan.instance.Bomb, this.transform.parent).SetActive(true);

                Destroy(this.gameObject);
            }
        }

        public void Miss()
        {
            newHolder = new GameObject();
            newHolder.transform.parent = this.gameObject.transform;
            Holder.transform.parent = newHolder.transform;

            hitBeat = Conductor.instance.songPositionInBeats;
            isHit = false;
            isThrown = false;
            anim.enabled = false;
            Sprite.GetComponent<SpriteRenderer>().sortingOrder = 49;
        }
    }
}