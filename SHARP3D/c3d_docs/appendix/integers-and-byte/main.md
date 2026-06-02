# Integers and Bytes

Some of the [parameters](../../parameters/c3d-parameter-section.md) in C3D files store data values using 16-bit integers, while various parameter [header](../../c3d-header.md) values and arrays use an 8-bit byte as an index. In the original C3D specification all integers and bytes in the [parameter section](../../parameters/c3d-parameter-section.md) were one's complement signed integers with a range of –32767 to +32767, and all 8-bit integers were one's complement signed bytes with a range of –127 to +127. Thus, in the original C3D format description every integer and byte in a parameter could store both positive and negative values and arrays could potentially have both positive and negative indexes.

Some 16-bit integer parameters in a C3D file never need to store a negative value; for example the number of 3D points and the number of analog channels are always positive values. In addition, arrays within the C3D file (which use an 8-bit index) never use a negative index – the array index values are always positive, while other parameters (analog data sample values, parameter indexes, group IDs, parameter IDs, and name lengths) require that the stored value is signed.

The C3D format expects that all integer parameters that can never have a negative value are interpreted as unsigned values and that arrays use an unsigned 8-bit byte index which doubles the potential array storage available. The stored binary format of the C3D files remains unchanged but the interpretation of the stored values in older C3D files is now occasionally dependent on the value that is being read.

Binary and hex bytes as unisgned and one's complement signed byte values:
| Binary | Hex | Unsigned | Signed |
| --- | --- | --- | --- |
| 11111111 | FF | 255 | -0 |
| 11111110 | FE | 254 | -1 |
| 10101111 | AF | 175 | -80 |
| 10000000 | 80 | 128 | -127 |
| 01111111 | 7F | 127 | 127 |
| 01010000 | 50 | 80 | 80 |
| 00000001 | 1 | 1 | 1 |
| 00000000 | 0 | 0 | 0 |

> Storing unsigned values in C3D files does not change the contents of the file at a binary level and remains compatible with older applications written to process C3D files that did not exceed the original file size limits, e.g. 32,767 frames of data.

This change in the interpretation of the integer parameter values has the potential to cause problems for some older software applications that will read negative values for frame ranges and array indexes in larger files created with the new interpretation of the C3D standard but should not cause problems for modern applications. The majority of older applications will fail to read the newer C3D files for other reasons.

While the larger arrays created by the use of unsigned bytes as array indexes can significantly increase the size of the parameter section, many modern applications use create larger parameter labels and store empty descriptions which can add a significant amount of empty space to the parameter storage. Many of the older software applications, written using signed integers throughout, allocate fixed amounts of parameter storage (generally about 10kB) and any C3D file that uses unsigned array indexes, or poorly designed label names and descriptions, is very likely to overflow this allocation, usually with fatal results for the application.

Since the discussion above does not change the C3D file format at a binary level
there is no flag to indicate that a C3D file uses unsigned integers in the [parameter section](../../parameters/c3d-parameter-section.md). The use of unsigned integers can only be determined by finding unsigned values that exceed the signed range values in certain parameter or index values as
shown in the table below. For example, if the [POINT:FRAMES](../../parameters/required/point/point-frames.md) value is greater than
32767 then unsigned integers are used because a negative frame count (if read as a
signed integer) cannot exist:

| Parameters | Signed Integer | Unsigned Integer |
| --- | --- | --- |
| [POINT:FRAMES](../../parameters/required/point/point-frames.md) | Data value 1 to 32767 | Data value 1 to 65535 |
| [POINT:LABELS](../../parameters/required/point/point-labels.md) | Array index 1 to 127 | Array index 1 to 255 |
| [ANALOG:LABELS](../../parameters/required/analog/analog-labels.md) | Array index 1 to 127 | Array index 1 to 255 |
*Typical signed integer vs unsigned integer parameter changes.*

It is worth pointing out at this stage that it is unlikely that most parameters will ever be required to exceed the ranges supported by the original signed value. In general, the [POINT:LABELS](../../parameters/required/point/point-labels.md) and [ANALOG:LABELS](../../parameters/required/analog/analog-labels.md) are the most likely to exceed the signed range of 127 array entries, while old applications may read the frame count stored in [POINT:FRAMES](../../parameters/required/point/point-frames.md) as a negative integer value if the actual frame count is unsigned and exceeds 32767 resulting in an error.

Other parameter entries could exceed the signed integer limits but it is unlikely to be seen in most environments. While it is theoretically possible that almost all of the force plate parameters could take unsigned values, the only ones that are likely to be unsigned are the parameters [FORCE_PLATFORM:CHANNEL](../../parameters/required/force_platform/force_platform-channel.md) which would have to use an unsigned array if the C3D file contained more than 127 analog channels.