namespace SHARP3D
{
    // TODO: Implement methods to curate the eventLabel. Don't know where to put it.
    public struct C3dHeaderEvent
    {
        public float EventTime;
        public EventDisplayFlag DisplayFlag;
        public string EventLabel;
    }

    public enum EventDisplayFlag : int
    {
        ON = 1,
        OFF = 0,
    }
}
