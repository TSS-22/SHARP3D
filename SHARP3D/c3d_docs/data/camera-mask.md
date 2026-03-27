# Camera Mask

In addition to a 3D residual value, the 3D coordinate format can also provide information about which observers provided the information used to calculate the associated 3D point location. This information is called the “camera contribution” or “camera mask” and is stored, together with the 3D residual, in the fourth word of the [3D Point record](./3d-point.md#camera-mask-and-residual). 

> Residual values are not limited to optical motion capture systems.

The camera mask can be very useful, particularly when used in conjunction with the residual data as it provided information that can allow the user to evaluate the data quality. Since the camera mask tells us which cameras, or observers, were used to
construct any given 3D point, is can be quite easy to identify a poor, or poorly calibrated, observer simply by noticing that the residuals increase when a particular camera is used to calculate the 3D point. Typically, this shows up as a sudden jump in the point trajectory data when the offending observer contributes faulty positional information. Careful observation of noise levels of individual trajectories within the data collection volume can lead to improvements in the overall system accuracy by enabling the photogrammetry software to eliminate cameras or observation sources that are not performing well.

Improvements in the automation of data collection, together with an increase in the number of cameras in motion capture systems make the routine evaluation of the camera mask an essential part of quality control. In addition, engineers configuring an automated motion capture environment for the first time or changing an existing laboratory configuration can directly assess the entire data collection process (data collection, trajectory identification and generation) by careful evaluation of the camera mask and residual values within a C3D file.

| Bit-8 | Bit-7 | Bit-6 | Bit-5 | Bit-4 | Bit-3 | Bit-2 | Bit-1 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Residual Sign | Camera #7 | Camera #6 | Camera #5 | Camera #4 | Camera #3 | Camera #2 | Camera #1 |

The camera contribution mask example shown above is found in Word 4 of the [3D Point record](./3d-point.md). In the camera contribution mask, byte 1 of word 4 contains eight bits, seven of which are set corresponding to the observers that contributed to the points measurement. Bit-1 refers to the first camera, bit-2 to the
second etc.

> When 3D data collection systems have more than 7 cameras the camera mask is still valid and can record the camera contributions from the first 7 cameras, or from specific cameras that are defined by parameter values to aid in debugging 3D observational issues.

The camera contribution byte is [part of the 16-bit signed integer used to store the 3D residual](./3d-point.md#camera-mask-and-residual) and as a result, bit-8 is not available to store camera mask information as it stores the sign of the 16-bit signed integer. Note that, for compatibility, and to simplify data access functions, the same signed-integer format is retained internally even when the [3D points are stored using the floating-point format](./3d-point.md#camera-mask-and-residual).

> While the C3D format defines the camera mask in an optical 3D environment, other sensor based data collection systems could use the camera mask to record the IMU sensor or markerless contributions to the 3D point location calculation.

In the original C3D format there was no provision for recording the contribution of more than seven observers or the requirement that these bits are actually used when a C3D file is created. As a result, as the number of cameras used to record motion has
increased, many manufacturers have abandoned recording any camera contribution data even for systems with less than eight cameras. This is unfortunate because documenting the observer (or potentially sensor) contribution to each calculated 3D location is very helpful when setting up a 3D motion capture environment or debugging problems with any camera or sensor configuration.

The camera mask enables users to determine which cameras failed to contribute data to the calculated 3D location, frame by frame throughout the entire data collection so that if a marker always disappears when the subject walks through the data collection
volume it is very easy to identify which cameras that are failing to record the marker on a frame by frame basis while documenting that other cameras always record the marker. Knowing the camera contributions means that marker visibility problems can usually be solved by moving a camera location a few inches, or replacing a faulty camera instead of buying more cameras. Reviewing the camera contributions in a typical operating environment make it easy to demonstrate which cameras are ideally located and which camera locations should be moved.

The camera contribution bits are usually cleared if the associated 3D point has been modeled, interpolated, filtered, or otherwise modified. As a result, the presence of an active camera contribution mask will normally indicate that the associated 3D data point is a raw measurement and has not been filtered or modified in any way. In practice, this convention is not strictly observed and cannot be counted on.