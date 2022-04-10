using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyBezierCurves;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_TrickClass
{
    public class MobTrickObj : PlayerActionObject
    {
        public bool flyType;
        public float startBeat;
        bool flying = true;

        float flyBeats;

        [NonSerialized] public BezierCurve3D curve;

        private TrickClass game;

        private void Awake()
        {
            game = TrickClass.instance;
            flyBeats = flyType ? 4f : 2f;

            var cond = Conductor.instance;

            float flyPos = cond.GetPositionFromBeat(startBeat, flyBeats);
            transform.position = curve.GetPoint(flyPos);
        }

        // Update is called once per frame
        void Update()
        {
            if (flying)
            {
                var cond = Conductor.instance;

                float flyPos = cond.GetPositionFromBeat(startBeat, flyBeats);
                transform.position = curve.GetPoint(flyPos);

                if (flyPos > 1f)
                {
                    GameObject.Destroy(gameObject);
                    return;
                }
            }
        }
    }
}