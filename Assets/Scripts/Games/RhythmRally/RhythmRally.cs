using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.RhythmRally
{
    public class RhythmRally : MonoBehaviour
    {
        public Transform cameraPos;

        public GameObject ball;
        public NaughtyBezierCurves.BezierCurve3D curve3D;

        // Start is called before the first frame update
        void Start()
        {
            GameCamera.instance.camera.transform.position = cameraPos.position;
            GameCamera.instance.camera.transform.rotation = cameraPos.rotation;
            GameCamera.instance.camera.fieldOfView = 41f;
            GameCamera.instance.camera.backgroundColor = Color.white;
        }

        // Update is called once per frame
        void Update()
        {
            ball.transform.position = curve3D.GetPoint(Mathf.Clamp(Conductor.instance.GetPositionFromBeat(0, 2f), 0, 1));
            ball.transform.GetChild(0).transform.position = new Vector3(ball.transform.position.x, -0.399f, ball.transform.position.z);
        }
    }
}