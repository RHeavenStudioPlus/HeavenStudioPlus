using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Common
{
    public class FollowMouse : MonoBehaviour
    {
        public Vector2 offset;

        private void Update()
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(pos.x - offset.x, pos.y - offset.y, 0);
        }
    }
}