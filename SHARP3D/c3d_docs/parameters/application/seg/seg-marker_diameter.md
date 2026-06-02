# SEG:MARKER_DIAMETER

- **Type**: [Application](../../application.md)

- **Locked**: False

A floating-point value that contains the diameter of the markers, or largest marker used, in the collection of 3D data. This parameter is measured using the units recorded in the [POINT:UNITS](../../required/point/point-units.md) parameter, which is the same unit as used in the reference coordinate system.

This is a good example of a parameter that is defined in terms of the value of a standard C3D parameter. Since marker based photogrammetry software generally calculates the center locations of spherical markers it is important to know the marker size in order to accurately measure the position of the object to which the marker was attached.

An improved version of this would be to create an array that store the marker diameter of each marker used, in regard to their index in the [USED](../../required/point/point-used.md) parameter. This would allow the use of marker of different size. 