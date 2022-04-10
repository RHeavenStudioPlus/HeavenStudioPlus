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
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}