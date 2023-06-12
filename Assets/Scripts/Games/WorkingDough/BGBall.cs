using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;
using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_WorkingDough
{
    public class BGBall : SuperCurveObject
    {
        private double startBeat = double.MinValue;
        private Path path;

        public void Init(double beat)
        {
            startBeat = beat;
            path = WorkingDough.instance.GetPath("BGBall");
            Update();
        }

        private void Update()
        {
            var cond = Conductor.instance;

            if (cond.isPlaying && !cond.isPaused)
            {
                double beat = cond.songPositionInBeats;
                if (startBeat !=  double.MinValue)
                {
                    Vector3 pos = GetPathPositionFromBeat(path, Math.Max(startBeat, beat), startBeat);
                    transform.position = pos;
                    if (beat >= startBeat + 9) Destroy(gameObject);
                }
            }
        }
    }
}


