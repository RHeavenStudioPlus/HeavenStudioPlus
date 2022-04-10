using DG.Tweening;
using NaughtyBezierCurves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games
{
    /**
        mob_Trick
    **/

    using Scripts_TrickClass;
    public class TrickClass : MonoBehaviour
    {
        public enum TrickObjType {
            Plane,
            Shock,
            Ball,
            Chair,
            Phone
        }

        [Header("References")]
        public GameObject ballPrefab;
        public GameObject planePrefab;
        public GameObject shockPrefab;
        public Transform objHolder;

        [Header("Curves")]
        public BezierCurve3D ballTossCurve;
        public BezierCurve3D ballMissCurve;
        public BezierCurve3D planeTossCurve;
        public BezierCurve3D planeMissCurve;
        public BezierCurve3D shockTossCurve;

        public static TrickClass instance;

        private void Awake()
        {
            instance = this;
        }

        public void TossObject(float beat, int type)
        {
            switch (type)
            {
                case (int) TrickObjType.Plane:
                    Jukebox.PlayOneShotGame("trickClass/girl_toss_plane");
                    break;
                default:
                    Jukebox.PlayOneShotGame("trickClass/girl_toss_ball");
                    break;
            }
            SpawnObject(beat, type);
        }

        public void SpawnObject(float beat, int type)
        {
            GameObject objectToSpawn;
            BezierCurve3D curve;
            bool isPlane = false;
            switch (type)
            {
                case (int) TrickObjType.Plane:
                    objectToSpawn = planePrefab;
                    curve = planeTossCurve;
                    isPlane = true;
                    break;
                default:
                    objectToSpawn = ballPrefab;
                    curve = ballTossCurve;
                    break;
            }
            var mobj = GameObject.Instantiate(objectToSpawn, objHolder);
            var thinker = mobj.GetComponent<MobTrickObj>();

            thinker.startBeat = beat;
            thinker.flyType = isPlane;
            thinker.curve = curve;

            mobj.SetActive(true);
        }
    }
}