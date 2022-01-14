using UnityEngine;

namespace Starpelly
{
    public static class Colors
    {
        public static string Color2Hex(this Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        /// Converts a Hexadecimal Color to an RGB Color.
        /// </summary>
        public static Color Hex2RGB(this string hex)
        {
            try
            {
                hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
                hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
                byte a = 255;//assume fully visible unless specified in hex
                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                //Only use alpha if the string has enough characters
                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                }
                return new Color32(r, g, b, a);
            }
            catch
            {
                return Color.black;
            }
        }
    }
}
