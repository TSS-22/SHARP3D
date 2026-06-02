# Reading the C3D Frame Count

When a C3D file is opened, the number of frames stored in the file is normally read from the C3D parameter [POINT:FRAMES](../../parameters/required/point/point-frames.md). It is important that applications determine the parameter storage type when they open a parameter, before reading the parameter value. Traditionally the C3D frame count stored in the [POINT:FRAMES](../../parameters/required/point/point-frames.md) parameter was a signed integer, but it may also be stored as an unsigned integer, or a floating-point value. So applications opening the C3D file must be written correctly and must determine the parameter type before reading the parameter.

When a C3D file is opened, the following rules need to apply to reading the frame count from file that may have different vendors’ interpretations of the C3D standard:

1. If the value stored in the [POINT:FRAMES](../../parameters/required/point/point-frames.md) parameter is not 65535, then the value
stored in [POINT:FRAMES](../../parameters/required/point/point-frames.md) is the C3D frame count. Note that the [POINT:FRAMES](../../parameters/required/point/point-frames.md) parameter may have been written as a positive signed 16-bit integer, an unsigned
16-bit integer, or a floating-point value.

2. If the [POINT:FRAMES](../../parameters/required/point/point-frames.md) parameter value is 65535 then it is likely that the C3D file contains more than 65535 frames, with the actual frame count stored in one of two different vendor defined parameters. If the [POINT:LONG_FRAMES](../../parameters/additional/point/point-long_frames.md) and [TRIAL parameters](../../parameters/groups/group-trial.md) do not exist, then 65535 is the actual C3D frame count.

3. If the [POINT:FRAMES](../../parameters/required/point/point-frames.md) parameter value is 65535 and the [TRIAL parameters](../../parameters/groups/group-trial.md) exist, then the C3D frame count must be calculated from the two values stored in the [TRIAL:ACTUAL_START_FIELD](../../parameters/additional/trial/trial-actual_start_field.md) and the [TRIAL:ACTUAL_END_FIELD](../../parameters/additional/trial/trial-actual_end_field.md) parameters.

4. If the [POINT:FRAMES](../../parameters/required/point/point-frames.md) parameter value is 65535 and the [POINT:LONG_FRAMES](../../parameters/additional/point/point-long_frames.md) parameter exists, then the [POINT:LONG_FRAMES](../../parameters/additional/point/point-long_frames.md) contains the frame count stored as a floating-point value.

5. If the [POINT:FRAMES](../../parameters/required/point/point-frames.md) parameter value is 65535 and both the [TRIAL parameters](../../parameters/groups/group-trial.md) and the [POINT:LONG_FRAMES](../../parameters/additional/point/point-long_frames.md) parameters exist, then they should both contain identical frame counts.

While these rules describe the actual C3D data frame count, a potential complication is that the C3D file header stores the frame range of the raw data that generated the 3D data stored in the C3D file. The original raw data frame range is stored as two 16-bit integers in header [words 4](../../c3d-header.md#word-4-first-frame-number-of-the-raw-data) and [word 5](../../c3d-header.md#word-5-end-frame-number-of-the-raw-data) that record the first and last frame numbers of the raw data that created the 3D data values stored in the file. While there is a potential relationship between the frame numbers stored in the header, and the frames stored as C3D data, the header values do not define the C3D file frame count.

Some applications may incorrectly expect the header frame values to define the C3D file frame range but in normal circumstances, when reading or processing data in a C3D file, the C3D file header raw frame range values should be ignored because they do not define the C3D file frame count and any changes to the C3D file frame count may not change the header values.

Once the C3D file frame count has been determined using the methods described above, it is recommended that all applications processing data from C3D files work with their internal 3D frame indexes defined as 32-bit unsigned integers to avoid internal application integer overflow problems when processing data from large files.  