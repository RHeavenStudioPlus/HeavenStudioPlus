using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania
{
    public class EntityTypes
    {
        public struct Integer
        {
            public int min;
            public int val;
            public int max;

            public Integer(int min, int max, int val = 0)
            {
                this.min = min;
                this.val = val;
                this.max = max;
            }
        }
    }
}