namespace SHARP3D.Utils.Enum
{
    /// <summary>
    /// Specifies display states for a header event, indicating whether it is ON or OFF.
    /// </summary>
    public enum HeaderEventFlag : int
    {
        /// <summary>
        /// Represents the 'on' state with a value of 1.
        /// </summary>
        ON = 1,
        /// <summary>
        /// Indicates that the feature or setting is turned off.
        /// </summary>
        OFF = 0,
    }

    /// <summary>
    /// Provides helper methods for converting byte values to <see cref="HeaderEventFlag"/> enum values.
    /// </summary>
    public static class HeaderEventFlagHelper
    {
        /// <summary>
        /// Converts a byte value to its corresponding <see cref="HeaderEventFlag"/> enum value.
        /// </summary>
        /// <param name="b">The byte value to convert.</param>
        /// <returns>The corresponding <see cref="HeaderEventFlag"/> enum value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the byte value is not a valid <see cref="HeaderEventFlag"/> value.
        /// </exception>
        /// <example>
        /// <code>
        /// byte b = 1;
        /// HeaderEventFlag flag = HeaderEventFlagHelper.FromByte(b);
        /// Console.WriteLine(flag); // Output: ON
        /// </code>
        /// </example>
        public static HeaderEventFlag FromByte(byte b)
        {
            switch(b)
            {
                case 1:
                    return HeaderEventFlag.ON;
                case 0:
                    return HeaderEventFlag.OFF;
                default:
                    throw new ArgumentException($"Invalid byte value for HeaderEventFlag: {b}");
            }
        }
    }
}


