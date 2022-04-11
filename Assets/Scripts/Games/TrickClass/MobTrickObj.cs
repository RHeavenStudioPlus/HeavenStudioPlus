using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NaughtyBezierCurves;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_TrickClass
{
    public class MobTrickObj : PlayerActionObject
    {
        public bool flyType;
        public float startBeat;
        bool flying = true;
        bool dodged = false;
        bool miss = false;

        float flyBeats;
        float dodgeBeats;
        public int type;

        [NonSerialized] public BezierCurve3D curve;

        private TrickClass game;

        private void Awake()
        {
            game = TrickClass.instance;
            flyBeats = flyType ? 4f : 2f;
            dodgeBeats = flyType ? 2f : 1f;

            var cond = Conductor.instance;

            float flyPos = cond.GetPositionFromBeat(startBeat, flyBeats);
            transform.position = curve.GetPoint(flyPos);
        }

        // Update is called once per frame
        void Update()
        {
            if (flying)
            {
                var cond = Conductor.instance;

                float flyPos = cond.GetPositionFromBeat(startBeat, flyBeats);
                if (flyPos <= 1f) 
                {
                    if (!miss)
                    {
                        flyPos *= 0.9f;
                    }
                    Vector3 lastPos = transform.position;
                    Vector3 nextPos = curve.GetPoint(flyPos);

                    if (flyType)
                    {
                        Vector3 direction = (nextPos - lastPos).normalized;
                        float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        this.transform.eulerAngles = new Vector3(0, 0, rotation);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (360f * Time.deltaTime));
                    }

                    transform.position = nextPos;
                }
                else
                {
                    transform.position = curve.GetPoint(1f);
                }

                if (flyPos > 1f)
                {
                    if (Conductor.instance.GetPositionFromBeat(startBeat, flyBeats + 1f) >= 1f)
                    {
                        GameObject.Destroy(gameObject);
                        return;
                    }
                }

                if (!(dodged || miss))
                {
                    float normalizedBeat = cond.GetPositionFromMargin(startBeat + dodgeBeats, 1f);
                    StateCheck(normalizedBeat);

                    if (PlayerInput.Pressed())
                    {
                        if (state.perfect)
                        {
                            dodged = true;
                            MultiSound.Play(new MultiSound.Sound[] { 
                                new MultiSound.Sound("trickClass/ball_impact", startBeat + flyBeats, volume: 0.75f), 
                            });
                        }
                    }
                    // no input?
                    if (Conductor.instance.GetPositionFromBeat(startBeat, dodgeBeats) >= Minigame.EndTime())
                    {
                        Jukebox.PlayOneShotGame(GetDodgeSound());
                        miss = true;
                        switch (type)
                        {
                            case (int) TrickClass.TrickObjType.Plane:
                                curve = TrickClass.instance.planeMissCurve;
                                flyBeats = 4f;
                                break;
                            default:
                                curve = TrickClass.instance.ballMissCurve;
                                flyBeats = 1.25f;
                                break;
                        }
                        startBeat += dodgeBeats;
                    }
                }
            }
        }

        public override void OnAce()
        {
            TrickClass.instance.PlayerDodge();
            dodged = true;
            MultiSound.Play(new MultiSound.Sound[] { 
                new MultiSound.Sound(GetDodgeSound(), startBeat + flyBeats, volume: 0.5f), 
            });
        }

        public string GetDodgeSound()
        {
            switch (type)
            {
                default:
                    return "trickClass/ball_impact";
            }
        }
    }
}