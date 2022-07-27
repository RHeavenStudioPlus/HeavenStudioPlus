using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_KarateMan
{
    public class KarateManPotNew : PlayerActionObject
    {
        public float startBeat;
        public ItemType type;
        public int path = 1;
        int status = 0;

        public enum ItemType {
            Pot,        // path 1
            Bulb,       // path 1
            Rock,       // path 1
            Ball,       // path 1
            Cooking,    // path 1
            Alien,      // path 1
            TacoBell,   // path 1

            KickBarrel, // path 1
            KickBomb,   // no path

            ComboPot1,  // path 1
            ComboPot2,  // path 1
            ComboPot3,  // path 2
            ComboPot4,  // path 3
            ComboPot5,  // path 4
            ComboBarrel // path 5
        }

        public enum FlyStatus {
            Fly,
            Hit,
            NG,
            HitWeak
        }

        //pot trajectory stuff
        public Transform[] HitPosition;
        public float[] HitPositionOffset;
        static Vector3 StartPositionOffset = new Vector3(3f, 0f, -8f);

        float ProgressToHitPosition(float progress) {
            return progress + (HitPositionOffset[path] -0.5f);
        }

        Vector3 ProgressToFlyPosition()
        {
            var cond = Conductor.instance;
            float progress = Mathf.Min(cond.GetPositionFromBeat(startBeat, 2f), 1f);
            float progressToHitPosition = ProgressToHitPosition(progress);


            //https://www.desmos.com/calculator/ycn9v62i4f
            float offset = HitPositionOffset[path];
            float flyHeight = (progressToHitPosition*(progressToHitPosition-1f))/(offset*(offset-1f));
            float floorHeight = HitPosition[0].position.y;

            Vector3 startPosition = transform.position + StartPositionOffset;
            Vector3 endPosition = transform.position - StartPositionOffset;
            Vector3 flyPosition = new Vector3(
                Mathf.Lerp(startPosition.x, endPosition.x, progress),
                floorHeight + (HitPosition[path].position.y - floorHeight) * flyHeight,
                Mathf.Lerp(startPosition.z, endPosition.z, progress)
            );

            if (progress >= 0.5f && flyPosition.y < HitPosition[0].position.y) {
                flyPosition.y = floorHeight;
            }
            return flyPosition;
        }
    }
}