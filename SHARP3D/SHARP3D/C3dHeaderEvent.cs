// TODO: Implement methods to curate the eventLabel. Don't know where to put it.
struct C3dHeaderEvent
{
    public float eventTime;
    public eventDisplayFlag displayFlag;
    public string eventLabel;
}

enum eventDisplayFlag : int
{
    ON = 1,
    OFF = 0,
}