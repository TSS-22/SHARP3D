namespace SHARP3D
{
    // TODO: Implement methods to curate the eventLabel. Don't know where to put it.
    /// <summary>
    /// Represents an event in a C3D file header, including its time, display flag, and label.
    /// </summary>
    public struct C3dHeaderEvent
    {
        /// <summary>
        /// The time at which the event occurs.
        /// </summary>
        public float EventTime;
        /// <summary>
        /// Specifies the <see cref="EventDisplayFlag"/> for the event. 
        /// </summary>
        public EventDisplayFlag DisplayFlag;
        /// <summary>
        /// Specifies the label associated with the event. It is either a 2 or 4 character string of ASCII character depending on the C3D file version.
        /// </summary>
        public string EventLabel;

        // TODO: Entierity
        public static C3dHeaderEvent EventFromBinaries(byte[] binaries, int offset)
        {
            C3dHeaderEvent headerEvent = new C3dHeaderEvent();
            headerEvent.EventTime = BitConverter.ToSingle(binaries, offset);
            headerEvent.DisplayFlag = (EventDisplayFlag)BitConverter.ToInt32(binaries, offset + 4);
            // Assuming the label is a fixed length string of 16 bytes
            headerEvent.EventLabel = System.Text.Encoding.ASCII.GetString(binaries, offset + 8, 16).TrimEnd('\0');
            return headerEvent;
        }

        // TODO: Entierity
        public static C3dHeaderEvent[] EventsFromBinaries(byte[] binaries, int definedEventsNb)
        {
            C3dHeaderEvent[] events = new C3dHeaderEvent[definedEventsNb];
            for (int i = 0; i < definedEventsNb; i++)
            {
                events[i] = EventFromBinaries(binaries, i * 24); // Each event takes 24 bytes
            }
            return events;
        }
    }

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
}