# POINT:USED

- **Type**: Required

- **Locked**: True

> This parameter is locked and should not be changed unless an application has added or deleted 3D points as value of this parameter affects the interpretation of the 3D/analog data storage records.

The POINT:USED parameter is normally an unsigned integer that contains the number of 3D point coordinates that are written to each frame of data in the C3D file data section. This parameter describes the number of points that are stored in every frame of data in the C3D file and is used to enable the 3D data section of the file to be interpreted. Every point in the C3D file should normally have an associated entry in the [POINT:LABELS](./point-labels.md) and [POINT:DESCRIPTIONS](./point-descriptions.md) parameters.

For example: if it is wished to store coordinates for ten 3D points, then POINT:USED should be set to 10, and every [3D Point Frame](../../../data/3d-point.md) will have space for POINT:USED number of 3D points.

The importance of the POINT:USED parameter lies in the fact that an application reading the 3D data section directly uses this value to determine the number of 3D coordinate points stored in each frame. 

> The points do not have to be valid, they just have to have storage allocated. 

Invalid points should be stored with a [negative residual](../../../data/3d-point.md#camera-mask-and-residual) if no valid trajectory data exists. When an application has read *POINT:USED number* of 3D coordinate points then it has read the entire frame of 3D data.

A copy of the USED parameter value can also be found [in word 2 of the C3D file header](../../../c3d-header.md#word-2-number-of-3d-markerstrajectories-per-data-frame). The POINT:USED header value must always be identical to the parameter value.

While the use of an unsigned integer to store the number of points in a C3D file means that a maximum of 65535 points can be stored, the associated [POINT:LABELS](./point-labels.md) and [POINT:DESCRIPTIONS](./point-descriptions.md) parameters are limited to a maximum of 255 entries. This limit can be bypassed by creating additional LABELS2 and DESCRIPTIONS2 parameters.