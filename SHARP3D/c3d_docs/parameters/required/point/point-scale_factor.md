# POINT:SCALE

- **Type**: [Required](../../required.md)

- **Locked**: True

> This parameter is locked and should not be changed. Extreme caution should be exercised when editing this parameter as it affects all stored 3D data values.

The POINT:SCALE parameter is a single floating-point value that stores the scaling factor that is applied to convert each of the signed integer 3D point values into the reference coordinate system values recorded by the POINT:UNITS parameter. A positive SCALE factor indicates that the 3D data is stored as signed 16-bit integer values, while a negative SCALE factor indicates that the C3D file contains 3D points that have been pre-scaled and stored as floating-point values.

The POINT:SCALE parameter effectively documents the resolution of the stored 3D point values recorded in the C3D file and is used by both integer and floating point storage methods to generate each 3D co-ordinate residual value that documents the accuracy of each 3D measurement. A copy of the SCALE parameter value can also be found stored in words 7-8 of the C3D file header.

The 3D scale factor is the maximum coordinate value present in the 3D data divided by 32000, thus a POINT:SCALE parameter value of 1.0 (or -1.0) indicates a 3D data resolution of 1mm. This allows the 3D point coordinates to be recorded within the range of a 16-bit signed integer. Since the POINT:SCALE value is required to interpret the 3D residual it is important that the correct SCALE value is calculated if the 3D information stored in floating-point format. Failing to calculate the correct SCALE value can cause data loss if the file contains coordinates stored as floating-point values and is converted to an integer file for user application compatibility. 

> Note that adding any additional data to the stored 3D data will require that the POINT:SCALE factor is recalculated and all existing data points stored as signed integers are rescaled if any new data values exceed the existing maximum coordinate value.

If a signed integer C3D file is converted to floating-point format then the original 3D scale factor should be negated and stored. Note that if an integer formatted C3D file is converted to a floating-point C3D file then it is important to preserve the absolute POINT:SCALE value, as this documents the C3D data resolution and will allow the file to be transparently converted back into an integer form if needed at any time for other applications. It is essential that the correct POINT:SCALE value is calculated and saved for all storage formats as it is used to scale the 3D residual information when a C3D file is stored any format. Failing to calculate the POINT:SCALE parameter value violates the C3D format definition because the value is required if the C3D file is stored in integer format. 

It is important to realize that the sign of the POINT:SCALE parameter and the magnitude of the parameter are treated as two independent factors in floating-point data files. The sign simply indicates that the file is a floating-point format, while the magnitude is used to scale the residual values and should be set to an appropriate value (maximum coordinate/32000) in case the C3D file is converted to integer format. Failure to calculate and store a valid POINT:SCALE parameter will cause corruption if the file data is format is changed to an integer format for any reason.

A negative POINT:SCALE value indicates that the file is already scaled and stored as floating-point values. The absolute POINT:SCALE value, ignoring the sign, is used to scale the 3D residual value for all formats but is only used to scale 3D data stored as signed integer values, it is not used to scale 3D floating-point data. As a result, the POINT:SCALE factor must always be accurately calculated, and stored as a locked value, because any casual changes have the potential for 3D data corruption.

Note that the POINT:SCALE parameter is one of the [parameter section](../../c3d-parameter-section.md) values that is copied to the [C3D file header (words 7-8)](../../../c3d-header.md#word-7-8-3d-sacle-factor) and can be quickly located and read by software applications without requiring a detailed search of the [parameter section](../../c3d-parameter-section.md).

The scaling factor is dependent upon the calibration volume and is calculated when the data is stored such that the greatest precision is allowed over the entire volume of interest.

> By default all physical 3D coordinates and marker residual measurements in a C3D file are stored in millimeters. The effective resolution of the recorded data is the POINT:SCALE value.

When C3D files with the POINT:SCALE value set to -1 are seen, it indicates that the application creating the file does not fully support the C3D file format and as a result the file cannot be saved as an integer format C3D file, and probably only contains processed values, not actual measurements.



