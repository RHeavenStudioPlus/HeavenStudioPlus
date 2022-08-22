using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace HeavenStudio
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

        public struct Float
        {
            public float min;
            public float val;
            public float max;

            public Float(float min, float max, float val = 0f)
            {
                this.min = min;
                this.val = val;
                this.max = max;
            }
        }

        // https://answers.unity.com/questions/772235/cannot-serialize-color.html
        // i am crying
        [System.Serializable]
        public class SerializableColor
        {
            public float[] colorStore = new float[4] { 1F, 1F, 1F, 1F };
            public UnityEngine.Color Color
            {
                get { return new Color(colorStore[0], colorStore[1], colorStore[2], colorStore[3]); }
                set { colorStore = new float[4] { value.r, value.g, value.b, value.a }; }
            }

            //makes this class usable as Color, Color normalColor = mySerializableColor;
            public static implicit operator UnityEngine.Color(SerializableColor instance)
            {
                return instance.Color;
            }

            //makes this class assignable by Color, SerializableColor myColor = Color.white;
            public static implicit operator SerializableColor(UnityEngine.Color color)
            {
                return new SerializableColor { Color = color };
            }
        }
    }
}