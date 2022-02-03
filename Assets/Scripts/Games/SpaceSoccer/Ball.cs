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
        [HideInInspector] public Kicker kicker;
        [SerializeField] private GameObject holder;
        [SerializeField] private GameObject spriteHolder;
        [SerializeField] private GameObject kickFX;
        [Space(10)]
        [SerializeField] private BezierCurve3D dispenseCurve;
        [SerializeField] private BezierCurve3D kickCurve;
        [SerializeField] private BezierCurve3D highKickCurve;
        [SerializeField] private BezierCurve3D toeCurve;

        [Header("Properties")]
        public float dispensedBeat = 0;
        public bool dispensing;
        public float hitTimes;
        private float lastSpriteRot;
        public bool canKick;
        public GameEvent kicked = new GameEvent();
        public GameEvent highKicked = new GameEvent();
        public GameEvent toe = new GameEvent();
        private bool lastKickLeft;

        private void Start()
        {
        }

        public void Kick(bool player)
        {
            if (player)
            Jukebox.PlayOneShotGame("spaceSoccer/ballHit");

            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            dispensing = false;
            kicked.enabled = true;
            // kicked.startBeat = Conductor.instance.songPositionInBeats;
            kicked.startBeat = dispensedBeat + 2 + hitTimes;

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

            HitFX();
        }

        public void HighKick()
        {
            hitTimes += 1.5f;

            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            dispensing = false;
            kicked.enabled = false;
            highKicked.enabled = true;
            highKicked.startBeat = Conductor.instance.songPositionInBeats;

            highKickCurve.KeyPoints[0].transform.position = holder.transform.position;


            HitFX();
        }

        public void Toe()
        {
            hitTimes += 1.5f;

            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            highKicked.enabled = false;
            kicked.enabled = false;

            toe.enabled = true;
            toe.startBeat = Conductor.instance.songPositionInBeats;

            toeCurve.KeyPoints[0].transform.position = holder.transform.position;
            if (lastKickLeft)
            {
                toeCurve.KeyPoints[1].transform.localPosition = new Vector3(5.39f, 0);
            }
            else
            {
                toeCurve.KeyPoints[1].transform.localPosition = new Vector3(6.49f, 0);
            }


            HitFX();
        }

        private void Update()
        {
            if (dispensing)
            {
                float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(dispensedBeat, 2.35f);

                dispenseCurve.KeyPoints[0].transform.position = new Vector3(kicker.transform.position.x - 6f, kicker.transform.position.y - 6f);
                dispenseCurve.KeyPoints[1].transform.position = new Vector3(kicker.transform.position.x - 1f, kicker.transform.position.y - 6f);

                holder.transform.localPosition = dispenseCurve.GetPoint(normalizedBeatAnim);
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
                float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(kicked.startBeat, 1.5f);

                if (!lastKickLeft)
                {
                    kickCurve.KeyPoints[1].transform.position = new Vector3(kicker.transform.position.x + 0.5f, kicker.transform.position.y - 6f);
                    spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, lastSpriteRot - 360f, normalizedBeatAnim));
                }
                else
                {
                    kickCurve.KeyPoints[1].transform.position = new Vector3(kicker.transform.position.x - 2.5f, kicker.transform.position.y - 6f);
                    spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, lastSpriteRot + 360f, normalizedBeatAnim));
                }

                holder.transform.localPosition = kickCurve.GetPoint(normalizedBeatAnim);

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
                float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(highKicked.startBeat, 1.8f);

                highKickCurve.KeyPoints[1].transform.position = new Vector3(kicker.transform.position.x - 3.5f, kicker.transform.position.y - 6f);

                holder.transform.localPosition = highKickCurve.GetPoint(normalizedBeatAnim);
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
                float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(toe.startBeat, 1.85f);

                if (!lastKickLeft)
                {
                    toeCurve.KeyPoints[1].transform.position = new Vector3(kicker.transform.position.x + 0.5f, kicker.transform.position.y - 6f);
                }
                else
                {
                    toeCurve.KeyPoints[1].transform.position = new Vector3(kicker.transform.position.x - 1.0f, kicker.transform.position.y - 6f);
                }

                holder.transform.localPosition = toeCurve.GetPoint(normalizedBeatAnim);
                spriteHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(lastSpriteRot, -860f, normalizedBeatAnim));
            }

            holder.transform.position = new Vector3(holder.transform.position.x, holder.transform.position.y, kicker.transform.localPosition.z);
        }

        private void HitFX()
        {
            GameObject kickfx = Instantiate(kickFX.gameObject, SpaceSoccer.instance.transform);
            kickfx.SetActive(true);
            kickfx.transform.position = holder.transform.position;
        }
    }
}