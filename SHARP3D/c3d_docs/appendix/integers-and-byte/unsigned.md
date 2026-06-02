# Unsigned Numbers

Many of the traditional C3D file storage limits have been eliminated by interpreting many of the stored integer values as unsigned integers instead of signed integers in situations where a negative value would not be possible. The use of unsigned integers and unsigned bytes in the C3D file format since 2004 has effectively doubled the amount of parameter and data storage that is available within the C3D file as compared to the original specification. This affects the interpretation of the C3D file data but does not change the content of the C3D file at a binary level so all older C3D files created prior to the interpretation change remain compatible and readable because the file format is unchanged at a binary level.

By interpreting the stored frame count<sup id="fnref1"><a href="#fn1">1</a></sup>, [POINT:FRAMES](../../parameters/required/point/point-frames.md), as an unsigned integer, the maximum number of 3D frames that can be stored in a C3D file is 65535, which increases the length of 3D data that can be recorded in the C3D file to just over 18 minutes at 60Hz e.g. $65535 / (60 * 60)$. When the 65535 integer frame limit is exceeded, the [POINT:FRAMES](../../parameters/required/point/point-frames.md) count can be written as a floating-point value. In addition, interpreting the parameter ([POINT:DATA_START](../../parameters/required/point/point-data_start.md)) as an unsigned integer allows the 3D data storage section to start anywhere within the first 32Mb ($65535*512/1024$) of the C3D file.

The length of most parameter items, pointers, and indexes can be interpreted as an unsigned byte within the parameter records without making any change to the contents of a C3D file at a binary level. This extends the amount of parameter storage available from 127 characters per value to 255 characters and allows many parameter dimensions to have up to 255 separate values (the signed limit was 127). The C3D group and parameter name lengths must always be interpreted as a signed integer because the C3D format uses the sign of the stored name length to indicate the locked, or unlocked, status of the parameter.

*Binary and hex integers translated into unsigned and signed integer values.*
| 16-bit Binary Integer | Unsigned | Signed |
| --- | --- | --- |
| 11111111 11111111 | 65535 | -0 |
| 11111111 11111110 | 65534 | -1 |
| 11111110 11111111 | 65279 | -256 |
| 10000000 00000000 | 32768 | -32767 |
| 01111111 11111111 | 32767 | 32767 |
| 00000001 00000000 | 256 | 256 |
| 00000000 00000001 | 1 | 1 |
| 00000000 00000000 | 0 | 0 |

While the use of unsigned integers and unsigned bytes within the parameters of a C3D file could create problems for older C3D applications that interpret large unsigned values as negative values, this is does not affect the interpretation of data within the various sections. For instance, when using the C3D integer file formats, point data within the 3D data section is always stored as signed integer values and analog data within the 3D data section is also stored as signed integers by default. As a result, almost all older C3D applications will remain able to read, write, and process C3D files created in the modern environment so long as the new files sizes do not exceed the traditional size limits which are normally adequate for most clinical and biomechanical data collection environments.

Regardless of the integer format interpretation, for all practical purposes, files that store pointers as signed, or unsigned integers are identical at a binary level. It is the interpretation of the data values by the application that defines whether the file is considered to be signed or unsigned. Both types of file use the same format for storing 3D and analog data values, which are always stored as signed integers or floating-point values. The result is that when viewed at a binary level there is no structural difference between the C3D files, the only difference is in the range of numbers that each format supports which is determined by the application that reads and interprets the binary values stored in the file.

The determination as to whether a C3D file is using signed integers or unsigned integers is simply a matter of how the application interprets the stored values<sup id="fnref2"><a href="#fn2">1</a></sup>(sic). For example arrays do not have negative indexes, a file cannot contain a negative number of data points or channels, and cannot store a negative number of frames.

While in the majority of instances, interpreting the stored integers as unsigned integers removes this as an issue, certain parameter integer bytes (group ID’s, parameter ID’s, group and parameter lengths etc.) remain signed values.

<sup id="fn1">1</sup>. See the [3D Frame Count appendix](../3d-frame-count/main.md) for more details on calculating the frame count of a C3D file. [↩](#fnref1)

<sup id="fn2">1</sup>. You read that right.[↩] (#fnref2)