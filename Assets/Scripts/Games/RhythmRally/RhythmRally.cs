using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Games.RhythmRally
{
    public class RhythmRally : MonoBehaviour
    {
        public Transform cameraPos;

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
        }
    }
}