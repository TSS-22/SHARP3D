# POINT:SCALE_FACTOR

If a signed integer C3D file is converted to floating-point format then the original 3D scale factor should be simply negated and stored – this allows transparent conversion between signed integer and floating-point data types.

To retain the maximum resolution for signed integer data, the 3D scale factor is computed by dividing the maximum absolute coordinate value stored in the file, by 32000. This allow all of the 3D point coordinates to be safely stored within the range of a
signed 16-bit integer value with maximum accuracy.