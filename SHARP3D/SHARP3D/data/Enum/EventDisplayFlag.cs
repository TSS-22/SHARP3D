namespace SHARP3D
{
    /// <summary>
    /// Specifies display states for a header event, indicating whether it is ON or OFF.
    /// </summary>
    public enum EventDisplayFlag : int
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

    public static class EventDisplayFlagExtensions
    {
        public static EventDisplayFlag FromByte(byte b)
        {
            switch(b)
            {
                case 1:
                    return EventDisplayFlag.ON;
                case 0:
                    return EventDisplayFlag.OFF;
                default:
                    throw new ArgumentException($"Invalid byte value for EventDisplayFlag: {b}");
            }
        }
    }
}


