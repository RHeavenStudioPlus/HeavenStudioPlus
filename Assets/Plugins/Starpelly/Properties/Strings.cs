using Starpelly.Enums.Strings;

namespace Starpelly.Properties
{
    public class Strings
    {
        /// <summary>
        /// Chooses a string based on the StringType chosen.
        /// </summary>
        /// <param name="stringType">The string type eg. (uppercase, lowercase, numeric)</param>
        /// <returns>A list of chars because enums don't support strings. :(</returns>
        public static string Chars(StringType stringType)
        {
            const string alpha = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string numeric = @"0123456789";
            const string alphanumeric = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string uppercase = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = @"abcdefghijklmnopqrstuvwxyz";
            const string punctuation = @"!@#$%^&*()_+{}:|<>?/.,;'\[]-=`~ ";
            const string all = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+{}:|<>?/.,;'\[]-=`~ ";

            string returnString;

            // I wish C# had the ability to use strings with enums
            switch (stringType)
            {
                case StringType.Alpha:
                    returnString = alpha;
                    break;
                case StringType.Numeric:
                    returnString = numeric;
                    break;
                case StringType.Alphanumeric:
                    returnString = alphanumeric;
                    break;
                case StringType.Uppercase:
                    returnString = uppercase;
                    break;
                case StringType.Lowercase:
                    returnString = lowercase;
                    break;
                case StringType.Punctuation:
                    returnString = punctuation;
                    break;
                case StringType.ALL:
                    returnString = all;
                    break;
                default:
                    returnString = all;
                    break;
            }

            return returnString;
        }
    }
}
