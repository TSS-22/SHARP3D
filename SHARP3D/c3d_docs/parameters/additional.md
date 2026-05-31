# Additional Parameters

> The additional parameters and groups described here are not formally required for a C3D file to meet the C3D standard but may be required for compatibility in different situations.

They have been created by manufacturers to allow them to extend the original format to accommodate larger numbers of 3D frames in a C3D file, synchronize the 3D data with external film images, support more than 255 3D points, more than 18 events, and more than 255 analog channels. 

All C3D applications should check for these parameters and be prepared to handle them appropriately but they are optional and may not exist. These parameters have been added as technology advances and motion capture systems create larger C3D files with more points and analog channels so they may be required under specific circumstances.

## Additional Parameters List

### POINT

- [LONG_FRAMES](./additional/point/point-long_frames.md)
- [POINT:LABELSX](./additional/point/point-labels2.md)
- [POINT:DESCRIPTIONSX](./additional/point/point-descriptions2.md)

### ANALOG

- [ANALOG:LABELSX](./additional/analog/analog-labels2.md)
- [ANALOG:DESCRIPTIONSX](./additional/analog/analog-descriptions2.md)
- [ANALOG:SCALEX](./additional/analog/analog-scale2.md)
- [ANALOG:OFFSETX](./additional/analog/analog-offset2.md)
- [ANALOG:UNITSX](./additional/analog/analog-units2.md)

### FORCE_PLATFORM

- [FORCE_PLATFORM:CAL_MATRIX](./additional/force_platform/force_platform-cal_matrix.md)

### TRIAL

- [TRIAL:ACTUAL_START_FIELD](./additional/trial/trial-actual_start_field.md)
- [TRIAL:ACTUAL_END_FIELD](./additional/trial/trial-actual_end_field.md)
- [TRIAL:CAMERA_RATE](./additional/trial/trial-camera_rate.md)

### EVENT

- [EVENT:USED](./additional/event/event-used.md)
- [EVENT:CONTEXTS](./additional/event/event-contexts.md)
- [EVENT:LABELS](./additional/event/event-labels.md)
- [EVENT:DESCRIPTIONS](./additional/event/event-descriptions.md)
- [EVENT:TIMES](./additional/event/event-times.md)
- [EVENT:SUBJECTS](./additional/event/event-subjects.md)
- [EVENT:ICON_IDS](./additional/event/event-icon_ids.md)
- [EVENT:GENERIC_FLAGS](./additional/event/event-generic_flags.md)

### EVENT_CONTEXT

- [EVENT_CONTEXT:USED](./additional/event_context/event_context-used.md)
- [EVENT_CONTEXT:ICON_IDS](./additional/event_context/event_context-icon_ids.md)
- [EVENT_CONTEXT:LABELS](./additional/event_context/event_context-labels.md)
- [EVENT_CONTEXT:COLOURS](./additional/event_context/event_context-colours.md)
