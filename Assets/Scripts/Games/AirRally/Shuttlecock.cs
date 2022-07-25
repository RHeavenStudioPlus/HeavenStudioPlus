using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;
using HeavenStudio.Util;


namespace HeavenStudio.Games.Scripts_AirRally
{
    public class Shuttlecock : PlayerActionObject
    {
        public float startBeat;
        private float flyBeats;

        public bool flyType;
        bool miss = false;
        public float flyPos;
        public bool isReturning;

        [NonReorderable] public BezierCurve3D curve;
        AirRally game;

        private void Awake()
        {
            game = AirRally.instance;
            flyBeats = isReturning ? 1.2f : 1.2f;
            var cond = Conductor.instance;
            flyPos = cond.GetPositionFromBeat(startBeat, flyBeats);
            transform.position = curve.GetPoint(flyPos);

        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var cond = Conductor.instance;
            float flyPos = cond.GetPositionFromBeat(startBeat, flyBeats);
            flyBeats = isReturning ? 1.2f : 1.2f;
            if (flyPos <= 1f)
            {
                if (!miss)
                {
                    flyPos *= 0.95f;
                }
                Vector3 lastPos = transform.position;
                Vector3 nextPos = curve.GetPoint(flyPos);

                if (flyType)
                {
                    Vector3 direction = (nextPos - lastPos).normalized;
                    float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    this.transform.eulerAngles = new Vector3(0, 0, rotation);
                }
                else
                {
                    if(!isReturning)
                        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (360f * Time.deltaTime));
                    else
                        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (-360f * Time.deltaTime));
                }

                transform.position = nextPos;
            }
            else
            {
                transform.position = curve.GetPoint(miss ? 1f : 0.95f);
            }

            //if (flyPos > 1f)
            //{
            //    if (Conductor.instance.GetPositionFromBeat(startBeat, flyBeats + 1f) >= 1f)
            //    {
            //        GameObject.Destroy(gameObject);
            //        return;
            //    }
            //}
            //if(flyPos > 1f)
            //{
            //    if(Conductor.instance.GetPositionFromBeat(startBeat, flyBeats + 1f) >= 1f)
            //    {
            //        if (!isReturning)
            //        {
            //            curve = game.MissCurve;
            //            transform.position = curve.GetPoint(flyPos);
            //        }

            //        else
            //        {
            //            curve = game.MissReturnCurve;
            //            transform.position = curve.GetPoint(flyPos);
            //        }

            //        return;
            //    }
            //}
            if (game.hasMissed && flyPos > 1f)
            {
                if (Conductor.instance.GetPositionFromBeat(startBeat, flyBeats + 1f) >= 1f)
                {
                    GameObject.Destroy(gameObject);
                    return;
                }
            }
        }
    }
}

