# POINT:LABELS

- **Type**: [Required](../../required.md)

- **Locked**: False

The POINT:UNITS parameter is a four-character ASCII parameter that records the physical measurement environment used by the program that created the 3D Point data stored in the C3D file. The default for this parameter must be `mm` (millimeters) when C3D files contain 3D Point data samples to ensure data compatibility. The POINT:UNITS parameter is stored as 4 ASCII characters, UTF-8 encoding is not permitted to guarantee that the value can be read by all C3D applications.

> The POINT:UNITS parameter exists for documentation, its contents are not used in the 3D point scaling calculations. 

All C3D files containing 3D Point data must be scaled and written in millimeters to provide a standard environment allowing everyone to read and process the data in the file. While it is possible, in a restricted environment, to create and process C3D files containing data scaled in centimeters, decimeters, metres, feet, inches, yards, poles, etc., **only files stored in millimeters can be reliably read by other applications**.

Translating a C3D file that contains 3D point data stored in any units other than millimeters is extremely complex and any mistakes will render the file invalid. For example, the POINT:UNITS parameter documents the units that record both the 3D Point locations, the 3D residuals stored with each point, and may record the marker diameter too, as well as the location and orientation of force plates within the data collection volume stored in the [FORCE_PLATFORM:ORIGIN](../force_platform/force_platform-origin.md), [CORNERS](../force_platform/force_platform-corners.md), etc., and affects the scaling values stored in the [CAL_MATRIX](../../additional/force_platform/force_platform-cal_matrix.md), and [ANALOG:SCALE](../analog/analog-scale.md) parameters.

An application that reads a C3D file cannot simply rescale the point data by modifying the [POINT:SCALE](./point-scale_factor.md) parameter as the measurement units also control the location of force plates, marker dimensions, marker residuals, accelerometer data, and multiple other potential data values recorded in the C3D file.

> If you are creating C3D files to export data to any other application then you must store the 3D data in millimeters and record this parameter as mm.