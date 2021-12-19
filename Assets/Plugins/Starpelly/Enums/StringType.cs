namespace Starpelly.Enums.Strings
{
    public enum StringType : int
    {
        /// <summary>
        /// Both uppercase and lowercase letters in english.
        /// </summary>
        Alpha,
        /// <summary>
        /// All numbers ranging from 0-9
        /// </summary>
        Numeric,
        /// <summary>
        /// Both uppercase and lowercase letters with all numbers ranging from 0-9. (english)
        /// </summary>
        Alphanumeric,
        /// <summary>
        /// All uppercase letters in english.
        /// </summary>
        Uppercase,
        /// <summary>
        /// All lowercase letters in english.
        /// </summary>
        Lowercase,
        /// <summary>
        /// All punctuation marks on the QWERTY keyboard. (Also contains special keys)
        /// </summary>
        Punctuation,
        /// <summary>
        /// All of the above.
        /// </summary>
        ALL
    }
}