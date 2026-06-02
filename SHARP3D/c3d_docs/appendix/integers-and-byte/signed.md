# Signed Numbers

The use of signed 16-bit integer and signed 8-bit byte numbers for parameter and data storage in the early days of C3D file creation was a consequence of using [FORTRAN](https://fortran-lang.org/) to create most of the early C3D applications.

> Traditionally C3D files used signed bytes and integers by default. The range of signed byte is -127 to 127 while the signed integer range is -32767 to 32767.

The use of signed numbers places some limits the amount of data that can be stored in the C3D file. According to the original C3D specification, the maximum number of 3D frames that could be stored in a signed C3D file was 32767 frames, a result of the use of a signed 16-bit signed integer parameter to record the number of 3D frames stored in the file. This limited the length of 3D data that can be recorded in the C3D file to just over 9 minutes at 60Hz ($32767 / (60 * 60)$) or correspondingly less at higher 3D frame rates.

The use of an 8-bit signed pointer to locate the start of the parameter section and a 16-bit signed integer to record the start of the 3D data section also placed limits on the C3D file structure:

- The 8-bit pointer to the start of the parameter section limits the placement of the start of the [parameter section](../../parameters/c3d-parameter-section.md) to any 512-byte block within the first range of 1 to 127, effectively within 64kB ($127*512/1024$) of the start of the C3D file.

- The start of the 3D data section is recorded by a signed 16-bit integer parameter ([POINT:DATA_START](../../parameters/required/point/point-data_start.md)) that points to the location of the first 512-byte block used to store [3D point and/or analog data](../../data/c3d-data-section.md). This limits the location of the start of 3D data storage to any 512-byte block boundary within the first 16Mb (32767*512/1024) of the C3D file.

Internally, the size of individual C3D parameter dimensions is limited by the use of a signed byte as a pointer, or index within the parameter records. Parameters could not contain more than 127 characters nor have more than 127 separate values, in any one dimension etc.