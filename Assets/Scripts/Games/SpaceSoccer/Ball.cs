using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyBezierCurves;
using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.SpaceSoccer
{
    public class Ball : MonoBehaviour
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
        public GameEvent kicked = new GameEvent();
        public GameEvent highKicked = new GameEvent();
        public GameEvent toe = new GameEvent();
        private bool lastKickLeft;

        public void Kick()
        {
            Jukebox.PlayOneShotGame("spaceSoccer/ballHit");

            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            dispensing = false;
            kicked.enabled = true;
            kicked.startBeat = Conductor.instance.songPositionInBeats;
            // kicked.startBeat = dispensedBeat + 2 + hitTimes;

            hitTimes++;

            lastKickLeft = kicker.kickLeft;

            if (kicker.kickLeft)
            {
                kickCurve.transform.localScale = new Vector3(-1, 1);
            }
            else
            {
                kickCurve.transform.localScale = new Vector3(1, 1);
            }
            kickCurve.KeyPoints[0].transform.position = holder.transform.position;
        }

        public void HighKick()
        {
            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            dispensing = false;
            kicked.enabled = false;
            highKicked.enabled = true;
            highKicked.startBeat = Conductor.instance.songPositionInBeats;

            highKickCurve.KeyPoints[0].transform.position = holder.transform.position;
        }

        public void Toe()
        {
            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            highKicked.enabled = false;
            kicked.enabled = false;

            toe.enabled = true;
            toe.startBeat = Conductor.instance.songPositionInBeats;
            
            toeCurve.KeyPoints[0].transform.position = holder.transform.position;
        }

        private void Update()
        {
            if (dispensing)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(dispensedBeat, 2.35f);
                holder.transform.position = dispenseCurve.GetPoint(normalizedBeatAnim);
                spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(0f, -1440f, normalizedBeatAnim));

                /*if (PlayerInput.Pressed())
                {
                    if (state.perfect)
                    {
                        Kick();
                    }
                }*/
            }
            else if (kicked.enabled)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(kicked.startBeat, 1.5f);
                holder.transform.position = kickCurve.GetPoint(normalizedBeatAnim);
                if (!lastKickLeft)
                {
                    spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, lastSpriteRot - 360f, normalizedBeatAnim));
                }
                else
                {
                    spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, lastSpriteRot + 360f, normalizedBeatAnim));
                }

                /*if (PlayerInput.Pressed())
                {
                    if (state.perfect)
                    {
                        if (kicker.canHighKick)
                        {
                            HighKick();
                        }
                        else if (kicker.canKick)
                        {
                            Kick();
                        }
                        // print(normalizedBeat);
                    }
                }*/
            }
            else if (highKicked.enabled)
            {
                float normalizedBeatAnim = Conductor.instance.GetLoopPositionFromBeat(highKicked.startBeat, 1.8f);
                holder.transform.position = highKickCurve.GetPoint(normalizedBeatAnim);
                spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, lastSpriteRot + 360f, normalizedBeatAnim));

                // if (state.perfect) Debug.Break();

                /*if (PlayerInput.Pressed())
                {
                    kickPrepare = true;
                    kicker.Kick(this);
                }
                if (kickPrepare)
                {
                    if (PlayerInput.PressedUp())
                    {
                        if (state.perfect)
                        {
                            Toe();
                        }
                    }
                }*/
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