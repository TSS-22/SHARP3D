# FORCE_PLATFORM:ZERO

- **Type**: [Required](../../required.md)

- **Locked**: False

> The ZERO parameter is specified in terms of **3D data frame numbers** in the C3D file, not analog
samples.

The FORCE_PLATFORM:ZERO parameter is an array that normally contains two non-zero integer values. These specify the range of 3D data frame numbers that may be used to provide a baseline for the force platform measurements. The default array values are is 1,10. Some applications may specify a range of 0,10 which should be interpreted as a range of 1 to 10 since the C3D file does not have a 3D frame number 0, the first frame in a C3D file is frame 1.

This parameter is intended to define the range of analog data that is expected to have no physical forces or moment present and allows an application that reads the force plate data to read the stored analog data for the given frames, find the mean for each channel, and then subtract it from the recorded analog data for the corresponding channel as it is accessed. This process will remove any DC offsets from the recorded force plate data. If the two frame numbers provided are both zero then no baseline-offset correction should be applied - any other value defines a range of 3D frames.

> Note that the presence of this parameter does not mean that any baseline correction has been performed, only that if it is to be performed, then the specified values by FORCE_PLATFORM:ZERO should be used.

Motion Capture systems that record the 3D points and force place data can set the ZERO frame numbers based on the data collection conditions, defining them as a range of frames where no 3D points are recorded above the force plates. 

> Any post processing editing of the C3D file that deletes frames at the start of the data collection will affect the ZERO frame numbers.

Applications that implement baseline correction in an application must be careful to ensure that the baseline correction is only applied to the specified force plate channels and that the force plates were unloaded during the frame rage specified by the parameter. 

An application that processes the force plate data to remove any offsets might set both FORCE_PLATFORM:ZERO frame number to 0 after the C3D file data has been modified to indicate that the data has been processed. 