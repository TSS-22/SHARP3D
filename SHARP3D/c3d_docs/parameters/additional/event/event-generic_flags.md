# EVENT:GENERIC_FLAGS

- **Type**: [Additional](../../additional.md)

- **Locked**: False

This is an array of size: \[[EVENT:USED](./event-used.md)\] which contains the flags associated with the corresponding [labels](./event-labels.md), indicating whether the event is general purpose (value non-zero) or has specific purpose (value zero). General-purpose events may have free-entry text [labels](./event-labels.md) and [descriptions](./event-descriptions.md), whereas those of specialized events tend to be fixed. 

> The values used are defined by the application that creates and processes the data, their values are not described as part of the C3D file format because events are normally application and trial environment specific.