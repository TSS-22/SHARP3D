# POINT:SCALE_FACTOR

If a signed integer C3D file is converted to floating-point format then the original 3D scale factor should be simply negated and stored – this allows transparent conversion between signed integer and floating-point data types.

To retain the maximum resolution for signed integer data, the 3D scale factor is computed by dividing the maximum absolute coordinate value stored in the file, by 32000. This allow all of the 3D point coordinates to be safely stored within the range of a
signed 16-bit integer value with maximum accuracy.

Note that the POINT:SCALE parameter
is one of the parameter section values that is copied to the C3D file header (words 7-
8) and can be quickly located and read by software applications without requiring a
detailed search of the parameter section.

Note that adding any additional data to the stored 3D data will
require that the POINT:SCALE factor is recalculated and all existing data
points stored as signed integers are rescaled if any new data values exceed
the existing maximum coordinate value.

It is important to realize that the sign of the POINT:SCALE parameter and the
magnitude of the parameter are treated as two independent factors in
floating-point data files. The sign simply indicates that the file is a floating-
point format, while the magnitude is used to scale the residual values and
should be set to an appropriate value (maximum coordinate/32000) in case the C3D file is converted to integer format. Failure to calculate and store a
valid POINT:SCALE parameter will cause corruption if the file data is format
is changed to an integer format for any reason.