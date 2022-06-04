using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using System.Linq;

namespace HeavenStudio
{
    public class GameCamera : MonoBehaviour
    {
        public static GameCamera instance { get; private set; }
        public new Camera camera;

        private List<Beatmap.Entity> positionEvents = new List<Beatmap.Entity>();
        private List<Beatmap.Entity> rotationEvents = new List<Beatmap.Entity>();
        private List<Beatmap.Entity> scaleEvents = new List<Beatmap.Entity>();

        /**
            default cam position, for quick-resetting
        **/
        static Vector3 defaultPosition = new Vector3(0, 0, -10);
        static Vector3 defaultRotEluer = new Vector3(0, 0, 0);
        static Vector3 defaultScale = new Vector3(16, 9, 1);

        /**
            camera's current transformation
            TODO: stretching (the scale param) not working, will need help with this cause I don't understand Unity's camera
        **/
        private static Vector3 position;
        private static Vector3 rotEluer;
        private static Vector3 scale;

        /**
            camera's last transformation
            TODO: stretching (the scaleLast param) not working, will need help with this cause I don't understand Unity's camera
        **/
        private static Vector3 positionLast;
        private static Vector3 rotEluerLast;
        private static Vector3 scaleLast;

        /** 
            transformations to apply *after* the global transform, 
            to use in minigame scripts (Spaceball, Rhythm Rally, Built to Scale, etc.)
            and NOT in the editor
        **/
        public static Vector3 additionalPosition;
        public static Vector3 additionalRotEluer;
        public static Vector3 additionalScale;

        [Header("Components")]
        public Color baseColor;

        private void Awake()
        {
            instance = this;
            camera = this.GetComponent<Camera>();
        }

        private void Start()
        {
            GameManager.instance.onBeatChanged += OnBeatChanged;
            camera.backgroundColor = baseColor;

            ResetTransforms();
            ResetAdditionalTransforms();
            
            positionLast = defaultPosition;
            rotEluerLast = defaultRotEluer;
            scaleLast = defaultScale;
        }

        public void OnBeatChanged(float beat)
        {
            ResetTransforms();
            ResetAdditionalTransforms();

            positionLast = defaultPosition;
            rotEluerLast = defaultRotEluer;
            scaleLast = defaultScale;

            // this entire thing is a mess redo it later
            //pos
            positionEvents = EventCaller.GetAllInGameManagerList("vfx", new string[] { "move camera" });

            //rot
            rotationEvents = EventCaller.GetAllInGameManagerList("vfx", new string[] { "rotate camera" });

            //scale (TODO)
            // scaleEvents = EventCaller.GetAllInGameManagerList("vfx", new string[] { "scale camera" });

            UpdateCameraTranslate();
            UpdateCameraRotate();
        }

        private void Update()
        {
            UpdateCameraTranslate();
            UpdateCameraRotate();

            Camera cam = GetCamera();
            cam.transform.localPosition = position + additionalPosition;
            cam.transform.eulerAngles = rotEluer + additionalRotEluer;
            cam.transform.localScale = Vector3.Scale(scale, additionalScale);
        }

        private void UpdateCameraTranslate()
        {
            foreach (var e in positionEvents)
            {
                float prog = Conductor.instance.GetPositionFromBeat(e.beat, e.length);
                if (prog >= 0f)
                {
                    EasingFunction.Function func = EasingFunction.GetEasingFunction(e.ease);
                    float dx = func(positionLast.x, e.valA, Mathf.Min(prog, 1f));
                    float dy = func(positionLast.y, e.valB, Mathf.Min(prog, 1f));
                    float dz = func(positionLast.z, -e.valC, Mathf.Min(prog, 1f));
                    position = new Vector3(dx, dy, dz);
                }
                if (prog > 1f)
                {
                    positionLast = new Vector3(e.valA, e.valB, -e.valC);
                }
            }
        }

        private void UpdateCameraRotate()
        {
            foreach (var e in rotationEvents)
            {
                float prog = Conductor.instance.GetPositionFromBeat(e.beat, e.length);
                if (prog >= 0f)
                {
                    EasingFunction.Function func = EasingFunction.GetEasingFunction(e.ease);
                    float dx = func(rotEluerLast.x, e.valA, Mathf.Min(prog, 1f));
                    float dy = func(rotEluerLast.y, e.valB, Mathf.Min(prog, 1f));
                    float dz = func(rotEluerLast.z, e.valC, Mathf.Min(prog, 1f));
                    rotEluer = new Vector3(dx, dy, dz);
                }
                if (prog > 1f)
                {
                    rotEluerLast = new Vector3(e.valA, e.valB, -e.valC);
                }
            }
        }

        public static void ResetTransforms()
        {
            position = defaultPosition;
            rotEluer = defaultRotEluer;
            scale = defaultScale;
        }

        public static void ResetAdditionalTransforms()
        {
            additionalPosition = new Vector3(0, 0, 0);
            additionalRotEluer = new Vector3(0, 0, 0);
            additionalScale = new Vector3(1, 1, 1);
        }

        public static Camera GetCamera()
        {
            return instance.camera;
        }
    }
}