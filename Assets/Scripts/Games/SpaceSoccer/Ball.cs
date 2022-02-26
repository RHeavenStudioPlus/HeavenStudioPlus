using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyBezierCurves;
using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.SpaceSoccer
{
    public class Ball : MonoBehaviour
    {
        public enum State { Dispensing, Kicked, HighKicked, Toe };
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
        public float startBeat;
        public State state;
        public float nextAnimBeat;
        public float highKickSwing = 0f;
        private float lastSpriteRot;
        public bool canKick;
        private bool lastKickLeft;

        public void Init(Kicker kicker, float dispensedBeat)
        {
            this.kicker = kicker;
            kicker.ball = this;
            kicker.dispenserBeat = dispensedBeat;
                state = State.Dispensing;
                startBeat = dispensedBeat;
                kicker.kickTimes = 0;
                if (kicker.player)
                {
                    MultiSound.Play(new MultiSound.Sound[]
                    {
                    new MultiSound.Sound("spaceSoccer/dispenseNoise",   dispensedBeat),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble1", dispensedBeat + 0.25f),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble2", dispensedBeat + 0.5f),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble2B",dispensedBeat + 0.5f),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble3", dispensedBeat + 0.75f),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble4", dispensedBeat + 1f),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble5", dispensedBeat + 1.25f),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble6", dispensedBeat + 1.5f),
                    new MultiSound.Sound("spaceSoccer/dispenseTumble6B",dispensedBeat + 1.75f),
                    });
                }
        }

        public void Kick(bool player)
        {
            if (player)
            Jukebox.PlayOneShotGame("spaceSoccer/ballHit");

            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            SetState(State.Kicked);

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
            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            SetState(State.HighKicked);

            highKickCurve.KeyPoints[0].transform.position = holder.transform.position;


            HitFX();
        }

        public void Toe()
        {

            lastSpriteRot = spriteHolder.transform.eulerAngles.z;

            SetState(State.Toe);

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
            switch (state)
            {
                case State.Dispensing:
                    {
                        float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(startBeat, 2.35f);

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
                        break;
                    }
                case State.Kicked:
                    {
                        float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(startBeat, 1.5f);

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
                        break;
                    }
                case State.HighKicked:
                    {
                        float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(startBeat, GetAnimLength(State.HighKicked) + 0.3f);

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
                        break;
                    }
                case State.Toe:
                    {
                        float normalizedBeatAnim = Conductor.instance.GetPositionFromBeat(startBeat, GetAnimLength(State.Toe) + 0.35f);

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
                        break;
                    }
            }

            holder.transform.position = new Vector3(holder.transform.position.x, holder.transform.position.y, kicker.transform.localPosition.z);
        }

        private void HitFX()
        {
            GameObject kickfx = Instantiate(kickFX.gameObject, SpaceSoccer.instance.transform);
            kickfx.SetActive(true);
            kickfx.transform.position = holder.transform.position;
        }

        private void SetState(State newState)
        {
            state = newState;
            startBeat = nextAnimBeat;
            nextAnimBeat += GetAnimLength(newState);
        }

        public float GetAnimLength(State anim)
        {
            switch(anim)
            {
                case State.Dispensing:
                    return 2f;
                case State.Kicked:
                    return 1f;
                case State.HighKicked:
                    return 2f - highKickSwing;
                case State.Toe:
                    return 2f - (1f - highKickSwing);
                default:
                    Debug.LogError("Ball has invalid state. State number: " + (int)anim);
                    return 0f;
            }
        }
    }
}