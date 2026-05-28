# ANALOG:USED

- **Type**: [Required](../../required.md)

- **Locked**: True

> This parameter is locked. Extreme caution should be exercised when editing this parameter as it affects the way that 3D/analog data is stored.

The ANALOG:USED parameter is normally an unsigned integer value that stores the number of analog channels that are contained within the C3D file. The value stored in ANALOG:USED is used to compute the analog data frame rate from the total number of analog data words collected during each 3D frame. The total number of analog samples stored per 3D frame must be an integer multiple of ANALOG:USED. The value of the ANALOG:USED parameter is not stored in the C3D file header but can be calculated from two values that are stored in the C3D file header: [C3D header word 3](../../../c3d-header.md#word-3-total-number-of-analog-samples-per-data-frames) divided by [C3D header Word 10](../../../c3d-header.md#word-10-number-of-analog-frame-per-data-frame).

As an unsigned integer, the ANALOG:USED parameter supports a maximum of 65535 analog channels, although it is unusual to find analog hardware systems collecting more than a few hundred channels of analog data. In practice the C3D format is limited to 255 analog channels due to limitations imposed by the parameters [LABELS](./analog-labels.md), [DESCRIPTIONS](./analog-descriptions.md), [SCALE](./analog-scale.md), [OFFSET](./analog-offset.md), and [UNITS](./analog-units.md). To circumvent these limitations, additional parameters can be created to extend the storage of said parameters: [LABELS2](./analog-labels.md), [DESCRIPTIONS2](./analog-descriptions.md), [SCALE2](./analog-scale.md), [OFFSET2](./analog-offset.md), and [UNITS2](./analog-units.md), etc. Refer to the documentation of each of those parameters to know more about this.

> If the ANALOG:USED parameter is zero then the C3D file does not contain any analog data values and all other ANALOG parameters should be ignored.

## Disclaimer

Users occasionally create files with different numbers of [LABELS](./analog-labels.md) and [DESCRIPTIONS](./analog-descriptions.md) parameters. This can create unwanted behavior using SHARP3D or any other C3D tool in general due to the inconsistencies it produce. **SHARP3D consider this a malpractice**. There should always be a one to one relationship between the number of [LABELS](./analog-labels.md), the number of [DESCRIPTIONS](./analog-descriptions.md), and [POINT:USED](./analog-used.md). 