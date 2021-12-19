using System.Linq;

using Starpelly.Enums.Strings;

namespace Starpelly.Random
{
    public class Strings
    {
        private static System.Random random = new System.Random();

        /// <summary>
        /// Function used to get a random string using the StringType and length provided.
        /// </summary>
        /// <param name="stringType">The string type. e.g, (uppercase, lowercase, numeric)</param>
        /// <param name="length">The length you want the string to be.</param>
        /// <returns>A random string of characters in a random order.</returns>
        public static string RandomString(StringType stringType, int length)
        {
            string chars = Properties.Strings.Chars(stringType);

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}