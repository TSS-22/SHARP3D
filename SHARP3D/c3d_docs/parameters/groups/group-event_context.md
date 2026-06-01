# EVENT_CONTEXTS

This group provides names and descriptions for the contexts of named events in the C3D file that are stored in the [EVENT group](./group-event.md). The event context can be thought of as defining the class of event without limiting the user by defining the type of event. 
the typical event contexts are:

- Left side event
- Right side event
- General event
- Invalid event

Other contexts can easily be created.

This allows individual events to be created in the [EVENT group](./group-event.md) and then analyzed within their context. Thus, a Foot strike event can have a Left side event or Right side event context and can be organized and analyzed with other events with the same context. Multiple event contexts are supported giving applications the ability to define custom event contexts.

The EVENT_CONTEXT group, and an associated [EVENT group](./group-event.md), has been added to work around the limit of no more than 18 events in the [C3D file header](../../c3d-header.md#header-events). 

The descriptions of the parameters presented here are based on the descriptions provided by the manufacturer and from the direct examination of C3D data files. The event storage mechanism described here is completely separate from events stored in the C3D header and there is no requirement that these events duplicate or match the events stored in the C3D file header.