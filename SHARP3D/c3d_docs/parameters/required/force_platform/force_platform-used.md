# FORCE_PLATFORM:USED

- **Type**: [Required](../../required.md)

- **Locked**: True

> The value of USED sets the minimum array size of other parameters in the [FORCE_PLATFORM](../group-force_platform.md) group.

The FORCE_PLATFORM:USED parameter is normally a single unsigned integer value that stores the number of force platforms for which analog data and parameters exist in the C3D file. When stored as an integer this may contain any value between 0 and 65535 although in practice the C3D format limits the size of the arrays describing the [FORCE_PLATFORM group parameters](../group-force_platform.md) to 255 so effectively the default C3D file format could support 255 force plates.

If FORCE_PLATFORM:USED is set to zero, then any remaining force platform parameters are not valid.

> It is important that the USED parameter exists even when the C3D file does not contain any force platform information, so that applications reading the C3D file can determine that force platform information does not exist.