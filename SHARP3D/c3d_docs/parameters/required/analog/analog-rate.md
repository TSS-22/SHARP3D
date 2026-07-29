# ANALOG:RATE

- **Type**: [Required](../../required.md)

- **Locked**: True

> This parameter is Locked. Extreme caution should be exercised when editing its value as it affects the way that [data](../../../data/c3d-data-section.md) is stored.

The ANALOG:RATE parameter is a single floating-point value that stores the sample rate at which the analog data was collected in samples per second. From This can be calculated the number of analog samples that exist in each analog channel for the given [POINT:RATE](../point/point-rate.md) value. For example, an ANALOG:RATE value of 600 for a C3D file that contains data with a POINT:RATE of 60.00 has 10 analog samples per 3D sample (60 x 10).

The RATE parameter value is not stored in the C3D file header. However, the header does record the 3D sample frame rate in [words 11-12](../../../c3d-header.md#word-11-12-3d-point-data-acquisition-rate) as well as the number of analog samples per 3D frame in [word 10](../../../c3d-header.md#word-10-number-of-analog-frame-per-data-frame). The ANALOG:RATE parameter value should always be identical to the value:

`3D_frame_rate * analog samples per frame`

For example, an ANALOG:RATE will have a value of 600 in a C3D file with a [POINT:RATE](../point/point-rate.md) value of 60 that contains 10 samples of analog data per 3D frame. 

> Note that although the C3D format specified that the number of analog samples per 3D frame must be an integer number, the actual 3D frame rate is a floating-point value since it may not be exact. Therefore, the ANALOG:RATE (from the above calculation) must also be stored as a floating-point value.

Note that a single ANALOG:RATE value applies to all analog channels – the C3D file format requires that all analog channels be recorded at a single rate. This means that if the C3D file is used to store analog data from a variety of different sources, all analog signals must be sampled at the rate required by the source with the highest frequency components.