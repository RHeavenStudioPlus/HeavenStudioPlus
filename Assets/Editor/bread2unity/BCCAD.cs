using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace Bread2Unity
{
    public class BCCAD : IDataModel
    {

        public BCCAD Read(byte[] bytes)
        {
            sheetW = (ushort)bytes[4];
            sheetH = (ushort)bytes[6];

            ISprite spriteParts_ = new ISprite();

            int max = (bytes[8] * 2) + 12;
            int loopTimes = 0;

            // this is pretty bad spaghetti code, and I wrote this when I had the flu at 3 AM. so you're welcome --Starpelly

            for (int i = 12; i < max; i+=2) // 16 bit bytes, skip every 2nd byte
            {
                int ind = i + 4; // the first 4 contain the number of parts and an unknown number, I can skip these for now

                ISpritePart part = new ISpritePart();
                part.regionX    = BitConverter.ToUInt16(bytes, ind + 0);
                part.regionY    = BitConverter.ToUInt16(bytes, ind + 2);
                part.regionW    = BitConverter.ToUInt16(bytes,ind + 4);
                part.regionH    = BitConverter.ToUInt16(bytes, ind + 6);
                part.posX       = BitConverter.ToInt16(bytes, ind + 8);
                part.posY       = BitConverter.ToInt16(bytes, ind + 10);
                part.stretchX   = BitConverter.ToSingle(bytes, ind + 12);
                part.stretchY   = BitConverter.ToSingle(bytes, ind + 14);
                part.rotation   = BitConverter.ToSingle(bytes, ind + 16);
                part.flipX      = bytes[ind + 18] != (byte)0;
                part.flipY      = bytes[ind + 20] != (byte)0;
                // im sure the values between 20 and 28 are important so remind me to come back to these
                part.opacity    = bytes[ind + 28];

                // Debug.Log(part.regionX);

                spriteParts_.parts.Add(part);

                int compare = 32;
                if (loopTimes < 1)
                {
                    compare = 32;
                }
                else if (loopTimes >= 1)
                {
                    if (loopTimes % 2 == 0)
                    {
                        compare = 32;
                    }
                    else
                    {
                        compare = 34;
                    }
                }
                max += compare * 2;
                i += compare * 2;
                loopTimes++;

                Debug.Log("offset: " + (ind + (compare - loopTimes + 1) * 2) + ", val: " + BitConverter.ToUInt16(bytes, (ind + (compare - loopTimes + 1) * 2)));

            }
            sprites.Add(spriteParts_);

            return new BCCAD()
            {
            };
        }


        /// sprites length bytes start = 12

        /// 20 = 1
        /// 84 = 2
        /// 152 = 3
        /// 216 = 4
        /// 284 - 5
        /// 
        /// 
        /// 64
        /// 64
        /// -- Loop
        /// 68
        /// 64
        /// 68
        /// 64
    }
}