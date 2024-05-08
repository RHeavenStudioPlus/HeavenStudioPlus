using System;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;


namespace HeavenStudio.Games.Scripts_LoveLab
{
    public class LoveLabFlask : SuperCurveObject
    {
        private LoveLab game;
        [SerializeField] private SuperCurveObject.Path path;
        private double pathStartBeat = double.MinValue;
        private Conductor conductor;
        public LoveLab.flaskHeart heartType;

        void Awake()
        {
            game = LoveLab.instance;
            conductor = Conductor.instance;
        }

        void Update()
        {
            double beat = conductor.songPositionInBeatsAsDouble;
            double height = 0f;
            if (pathStartBeat > double.MinValue)
            {
                Vector3 pos = GetPathPositionFromBeat(path, Math.Max(beat, pathStartBeat), out height, pathStartBeat);
                transform.position = pos;
                float rot = GetPathValue("rot");
                transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z - (rot * Time.deltaTime * (1f / conductor.pitchedSecPerBeat)));
            }
        }

        public void customShakes(double beat, string reqArc)
        {
            path = game.GetPath(reqArc);
            UpdateLastRealPos();
            pathStartBeat = beat - 1f;

            Vector3 pos = GetPathPositionFromBeat(path, pathStartBeat, pathStartBeat);
            transform.position = pos;

            gameObject.SetActive(true);

            BeatAction.New(game, new List<BeatAction.Action>
            {
                new BeatAction.Action(beat, delegate
                {
                    Destroy(this.gameObject);
                }),
            });
        }

        public void girlArc(double beat, string reqArc)
        {
            path = game.GetPath(reqArc);
            UpdateLastRealPos();
            pathStartBeat = beat - 1f;

            Vector3 pos = GetPathPositionFromBeat(path, pathStartBeat, pathStartBeat);
            transform.position = pos;

            gameObject.SetActive(true);
        }

        public void ForWeirdInit(double beat)
        {
            path = game.GetPath("WeirdFlaskIn");
            UpdateLastRealPos();
            pathStartBeat = beat - 1f;

            Vector3 pos = GetPathPositionFromBeat(path, pathStartBeat, pathStartBeat);
            transform.position = pos;

            gameObject.SetActive(true);

            game.labWeirdEndState(beat, 1f, this.gameObject);
        }


        public void onMissWhenHold(double beat)
        {
            path = game.GetPath("GirlFlaskMiss");
            UpdateLastRealPos();
            pathStartBeat = beat - 1f;

            Vector3 pos = GetPathPositionFromBeat(path, pathStartBeat, pathStartBeat);
            transform.position = pos;

            gameObject.SetActive(true);

            BeatAction.New(game, new List<BeatAction.Action>()
            {
                new BeatAction.Action(beat, delegate {
                    game.playFlaskBreak(1);
                    Destroy(this.gameObject); })
            });
        }

        public void destroyThisObj()
        {
            LoveLab.instance.girlInstantiatedFlask.RemoveAt(0);
            Destroy(this.gameObject);
        }
    }
}