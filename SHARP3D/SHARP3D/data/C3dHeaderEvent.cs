namespace SHARP3D
{
    // TODO: Implement methods to curate the eventLabel. Don't know where to put it.
    public struct C3dHeaderEvent
    {
        public float EventTime;
        public EventDisplayFlag DisplayFlag;
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

    public enum EventDisplayFlag : int
    {
        ON = 1,
        OFF = 0,
    }
}