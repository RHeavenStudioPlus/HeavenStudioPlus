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
            sheetW = BitConverter.ToUInt16(bytes, 4);
            sheetH = BitConverter.ToUInt16(bytes, 6);


            // int max = (bytes[8] * 2) + 12;
            int max = 64 * bytes[8] + 12;
            
            // note this doesn't account for empty sprites, but I'll get there when i get there
            for (int i = 12; i < max; i += 2) // 16 bit bytes, skip every 2nd byte
            {
                ISprite spriteParts_ = new ISprite();
                int compare = 0;
                for (int j = 0; j < bytes[i]; j++)
                {
                    int ind = i + 4 + (64 * j);

                    ISpritePart part = new ISpritePart();
                    part.regionX = BitConverter.ToUInt16(bytes, ind + 0);
                    part.regionY = BitConverter.ToUInt16(bytes, ind + 2);
                    part.regionW = BitConverter.ToUInt16(bytes, ind + 4);
                    part.regionH = BitConverter.ToUInt16(bytes, ind + 6);
                    part.posX = BitConverter.ToInt16(bytes, ind + 8);
                    part.posY = BitConverter.ToInt16(bytes, ind + 10);
                    part.stretchX = BitConverter.ToSingle(bytes, ind + 12);
                    part.stretchY = BitConverter.ToSingle(bytes, ind + 14);
                    part.rotation = BitConverter.ToSingle(bytes, ind + 16);
                    part.flipX = bytes[ind + 18] != (byte)0;
                    part.flipY = bytes[ind + 20] != (byte)0;
                    // im sure the values between 20 and 28 are important so remind me to come back to these
                    part.opacity = bytes[ind + 28];

                    Debug.Log("offset: " + ind + ", val: " + part.regionX);

                    spriteParts_.parts.Add(part);

                    compare += 64;
                }

                sprites.Add(spriteParts_);

                i += compare;

            }

            return new BCCAD()
            {
            };
        }


        /// sprites length bytes start = 12
    }
}