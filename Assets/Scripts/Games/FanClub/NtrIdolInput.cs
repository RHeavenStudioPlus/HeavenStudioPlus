using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_FanClub
{
    public class NtrIdolInput : PlayerActionObject
    {
        public float startBeat;
        public int type;
        public bool doCharge = false;
        private bool hit = false;
        private bool hasHit = false;

        void Start()
        {
            PlayerActionInit(gameObject, startBeat);
        }

        public override void OnAce()
        {
            Hit(true, type, true);
        }

        void Update()
        {
            if (Conductor.instance.GetPositionFromBeat(startBeat, 1.25f) >= 1)
            {
                FanClub.instance.AngerOnMiss();
                CleanUp();
            }

            if (!hit && Conductor.instance.GetPositionFromBeat(startBeat, 1) >= 1)
            {
                hit = true;
                if (hasHit) CleanUp();
            }

            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, 1);
            StateCheck(normalizedBeat);

            if (PlayerInput.Pressed() && type == 0)
            {
                if (state.perfect)
                {
                    Hit(true);
                } else if (state.notPerfect())
                {
                    Hit(false);
                }
            }
            if (PlayerInput.PressedUp() && type == 1)
            {
                if (state.perfect)
                {
                    Hit(true, type);
                } else if (state.notPerfect())
                {
                    Hit(false, type);
                }
            }
        }

        public void Hit(bool _hit, int type = 0, bool fromAutoplay = false)
        {
            if (!hasHit)
            {
                if (type == 0)
                    FanClub.instance.Player.ClapStart(_hit, true, doCharge, fromAutoplay);
                else if (type == 1)
                    FanClub.instance.Player.JumpStart(_hit, true, fromAutoplay);

                hasHit = true;

                if (hit) CleanUp();
            }
        }

        public void CleanUp()
        {
            Destroy(gameObject);
        }
    }
}