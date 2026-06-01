# POINT:LONG_FRAMES

- **Type**: [Additional](../../additional.md)

- **Locked**: True

> This parameter should be locked and caution should be exercised when editing the value of this parameter.

The original C3D documentation described the [POINT:FRAMES](../../required/point/point-frames.md) parameter as a signed 16-bit integer, limiting the maximum frame count to 32767 frames. As C3D files started to approach this limit, the interpretation was changed to read [POINT:FRAMES](../../required/point/point-frames.md) as an unsigned integer, extending the maximum frame range to 65535 frames but when C3D files passed this limit, [Vicon Motion Systems](https://www.vicon.com/) started storing the frame count as two complex [TRIAL](../../groups/group-trial.md) parameters, apparently storing the frame count as video fields; the POINT:LONG_FRAMES was independently created by C-Motion<sup id="fnref1"><a href="#fn1">1</a></sup> to store the total frame count as a single floating-point value.

Both these alternate methods ignored the option of storing the total frame count as a floating-point POINT:FRAMES parameter, an option for all software applications that read C3D parameters correctly, by determining the parameters type before reading the stored values. This was probably a result of the original C3D user guide describing the way that data was stored in C3D files at the time that the C3D user guide was written, instead of the way that data could be stored in C3D files.

The POINT:LONG_FRAMES parameter is described as a floating-point value containing the total frame count for C3D files that contain 65535 or more frames of data. If the POINT:LONG_FRAMES parameter exists, it must override any value stored by the [POINT:FRAMES](../../required/point/point-frames.md) parameter which **should be set to 65535 when the actual frame count is larger the 65535**. The LONG_FRAMES parameter may not be found in C3D files that store [POINT:FRAMES](../../required/point/point-frames.md) as a floating-point value, or C3D files that expect the user to calculate the total C3D frame count by using frame numbers stored in the [TRIAL](../../groups/group-trial.md) group parameters.

The POINT:LONG_FRAMES is not required in a C3D file that contains less than 65535 frames but if it is present, then the POINT:LONG_FRAMES parameter should contain a copy of the [POINT:FRAMES](../../required/point/point-frames.md) parameter. In general the POINT:LONG_FRAMES parameter is only seen in files created by C-Motion<sup id="fnref1"><a href="#fn1">1</a></sup> applications that contain more than 65535 frames.

<sup id="fn1">1</sup>C-Motion is now [Has-Motion](https://www.has-motion.ca/) [↩](#fnref1)