using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bread2Unity
{
    public class IAnimation
    {
        public List<IAnimationStep> steps;
    }

    public class IAnimationStep
    {
        public ushort spriteIndex;
        public ushort delay;

        public float stretchX;
        public float stretchY;

        public float rotation;

        public byte opacity;
    }
}