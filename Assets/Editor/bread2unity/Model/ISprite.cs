using System.Collections;
using System.Collections.Generic;

namespace Bread2Unity
{
    public class ISprite
    {
        public List<ISpritePart> parts = new List<ISpritePart>();
    }

    public class ISpritePart
    {
        public ushort regionX;
        public ushort regionY;
        public ushort regionW;
        public ushort regionH;

        public short posX;
        public short posY;

        public float stretchX;
        public float stretchY;

        public float rotation;

        public bool flipX;
        public bool flipY;

        public byte opacity;
    }
}