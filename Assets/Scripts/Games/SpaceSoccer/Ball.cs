using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyBezierCurves;
using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.SpaceSoccer
{
    public class Ball : PlayerActionObject
    {
        [Header("Components")]
        [SerializeField] private Kicker kicker;
        [SerializeField] private GameObject holder;
        [SerializeField] private GameObject spriteHolder;
        [Space(10)]
        [SerializeField] private BezierCurve3D dispenseCurve;
        [SerializeField] private BezierCurve3D kickCurve;
        [SerializeField] private BezierCurve3D highKickCurve;
        [SerializeField] private BezierCurve3D toeCurve;

        [Header("Properties")]
        public float dispensedBeat = 0;
        public bool dispensing;
        public int hitTimes;
        private float lastSpriteRot;
        public bool canKick;
        private GameEvent kicked = new GameEvent();
        private GameEvent highKicked = new GameEvent();
        private GameEvent toe = new GameEvent();
        private bool kickPrepare = false;

        private void Start()
        {
            PlayerActionInit(this.gameObject, dispensedBeat);
        }

        public override void OnAce()
        {
            kicker.Kick(this);
        }

        public void Kick()
        {
            Jukebox.PlayOneShotGame("spaceSoccer/ballHit");
            kicker.Kick(this);

            dispensing = false;
            kicked.enabled = true;
            kicked.startBeat = Conductor.instance.songPositionInBeats;
            // kicked.startBeat = dispensedBeat + 2 + hitTimes;

            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            hitTimes++;

            if (hitTimes % 2 == 0)
            {
                kickCurve.transform.localScale = new Vector3(-1, 1);
            }
            else
            {
                kickCurve.transform.localScale = new Vector3(1, 1);
            }
            kickCurve.KeyPoints[0].transform.position = holder.transform.position;

            ResetState();
        }

        public void HighKick()
        {
            Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe1_hit");

            kicked.enabled = false;
            highKicked.enabled = true;
            highKicked.startBeat = Conductor.instance.songPositionInBeats;

            highKickCurve.KeyPoints[0].transform.position = holder.transform.position;
        }

        public void Toe()
        {
            Jukebox.PlayOneShotGame("spaceSoccer/highkicktoe3_hit");

            highKicked.enabled = false;
            kicked.enabled = false;
            kickPrepare = false;

            toe.enabled = true;
            toe.startBeat = Conductor.instance.songPositionInBeats;
            
            toeCurve.KeyPoints[0].transform.position = holder.transform.position;
        }

        private void Update()
        {

            if (dispensing)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(dispensedBeat, 2.5f);
                holder.transform.position = dispenseCurve.GetPoint(normalizedBeatAnim);
                spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(0f, -1440f, normalizedBeatAnim));

                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(dispensedBeat, 2f);
                StateCheck(normalizedBeat);

                if (PlayerInput.Pressed())
                {
                        Kick();
                    if (state.perfect)
                    {
                    }
                }
            }
            else if (kicked.enabled)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(kicked.startBeat, 1.5f);
                holder.transform.position = kickCurve.GetPoint(normalizedBeatAnim);
                if (hitTimes % 2 == 0)
                {
                    spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, lastSpriteRot + 360f, normalizedBeatAnim));
                }
                else
                {
                    spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, lastSpriteRot - 360f, normalizedBeatAnim));
                }

                float normalizedBeat = Conductor.instance.GetLoopPositionFromBeat(kicked.startBeat, 1f);
                StateCheck(normalizedBeat);

                if (PlayerInput.Pressed())
                {
                    if (kicker.canHighKick)
                    {
                        HighKick();
                    }
                    else if (kicker.canKick)
                    {
                        Kick();
                    }
                    if (state.perfect)
                    {
                        // print(normalizedBeat);
                    }
                }
            }
            else if (highKicked.enabled)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(highKicked.startBeat, 1.85f);
                holder.transform.position = highKickCurve.GetPoint(normalizedBeatAnim);
                spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, -460f, normalizedBeatAnim));

                if (PlayerInput.Pressed())
                {
                    kickPrepare = true;
                    kicker.Kick(this);
                }
                if (kickPrepare)
                {
                    if (PlayerInput.PressedUp())
                    {
                        Toe();
                    }
                }
            }
            else if (toe.enabled)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(toe.startBeat, 1.85f);
                holder.transform.position = toeCurve.GetPoint(normalizedBeatAnim);
                spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, -860f, normalizedBeatAnim));

            }
        }
    }
}