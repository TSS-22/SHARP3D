# POINT:RATE

- **Type**: Required

- **Locked**: True

> This parameter is locked. Extreme caution should be exercised when editing the value of this parameter as it affects the interpretation of the 3D/analog data storage sections.

The POINT:RATE parameter is always a single floating-point value that stores the 3D sample rate of the data contained within the C3D file in samples per second. If the 3D data points were recorded at a rate of 60 samples per second then RATE should be set to 60. Note that although most NTSC video based systems are described at 60Hz systems, many 60Hz video based systems are actually sampling at 59.94006 Hz and the failure to record the exact frame rate may lead to analog data synchronization errors. If the C3D file only contains 3D sample data for every fourth sample then the POINT:RATE value will be 15 (accurately 14.985 for NTSC video data).

It is important that the POINT:RATE parameter is accurately recorded as it is used to calculate timing for the 3D data samples and affects the analog sample rate which is always an integer multiple of the POINT:RATE parameter.

A copy of the POINT:RATE parameter value can also be found stored in floating-point format in [words 11-12 of the C3D file header](../../../c3d-header.md#word-11-12-3d-point-data-acquisition-rate). The POINT:RATE header value must always be an identical copy of the value stored in the parameter section

The same POINT:RATE value applies to all 3D samples: the C3D file format requires that all 3D points be recorded at a single rate. This means that if the C3D file is used to store 3D data from a variety of different sources, all 3D points (even fixed points) must be sampled at the rate required by the fastest moving 3D point.