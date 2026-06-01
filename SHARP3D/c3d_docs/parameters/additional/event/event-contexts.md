# EVENT:CONTEXTS

- **Type**: [Additional](../../additional.md)

- **Locked**: False

This is an array of user defined strings, typically with a size of \[[EVENT:USED](event-used.md),16\]. It is used to record a “context” for each event: e.g. Left, Right, General etc. The string used for each event is chosen from a list stored in the [EVENT_CONTEXT:LABELS](../event_context/event_context-labels.md) parameter. This enables a “side” to be assigned to bipedal events where the observer is interested in left versus right side data or could just as easily describe “up” versus “down” events too.