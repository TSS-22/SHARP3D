# ANALOG

The ANALOG parameters group stores information about the analog data recorded within a C3D file. As a result, the parameter [ANALOG:USED](./../required/analog/analog-used.md) should be stored in all C3D files even if the file does not contain any analog data. C3D files that do not contain analog data should set the value of the [USED](./../required/analog/analog-used.md) parameter to zero.

The original specification for [analog data](../../data/analog.md) storage within the C3D file assumed that
data values were sampled by an Analog to Digital Converter (ADC) and then written to the C3D file as binary samples. The assumption was that the binary value would be stored in the C3D file as a signed 16-bit integer unless the C3D file used floating-point format, in which case the signed 16-bit value would be converted to a floating-pint value before being written to the file.

This method worked well for many years because the majority of analog data was sampled at 12-bit resolution and programmers implementing analog storage functions did not have to think too hard about the differences between storing signed offset, or unsigned offset data. The sampled values obtained from the ADC could
simply be written to the file, stored as a positive signed integer value, and any necessary scaling or format conversions could be handled by creating, and applying, the appropriate [OFFSET](./../required/analog/analog-offset.md) and [SCALE](./../required/analog/analog-scale.md) values. It made no difference whether 12-bit or 14-bit data samples were considered to be a signed integer or an unsigned integer as all the possible unsigned values could be stored within the range of a signed 16-bit integer without any risk of integer overflow errors.

|  | 12-bit ADC | 14-bit ADC | 16-bit ADC |
| --- | --- | --- | --- |
| Maximum value | 4096 | 16384 | 65536 |
| Midrange (zero) | 2047 | 8191 | 32767 |
| Minimum value | 0 | 0 | 0 |

This situation changed in two ways with the introduction of 16-bit resolution Analog Data Convertor (ADC) samples:

- The potential for integer overflow exists when the [ANALOG:OFFSET](./../required/analog/analog-offset.md) parameter is applied to 16-bit resolution data. This requires that all math operations on analog data be performed with at least 32-bit resolution to handle any potential overflow when large analog data values are encountered with any significant [OFFSET](./../required/analog/analog-offset.md) values because any positive offset applied to the maximum sample value causes an overflow error, potentially inverting the data sample.

- The interpretation of the format used to store the analog data sample is significant. Before the introduction of 16-bit ADCs, most analog data samples contained 12-bit data values with a range of 4096 discreet values, stored as positive numbers from 0 to 4095 as a signed 16-bit integer and converted to a scaled voltage measurement by the application of the [ANALOG:SCALE](./../required/analog/analog-scale.md) and [ANALOG:OFFSET](./../required/analog/analog-offset.md) parameters associated with individual analog channels. The introduction of 16-bit analog data samples changed this and requires that the analog values are interpreted as signed integer values.

The first C3D application to implement 16-bit analog data stored the analog data as unsigned 16-bit integer values, thus rendering the analog data unreadable to standard C3D applications that expect to read signed integers from the C3D file. The programmer was unwilling to correct this, as the problems were only discovered
after the software had been widely distributed and users started complaining that other C3D applications could not read the new format.

In order to work around this problem two additional parameters ([ANALOG:FORMAT](./../required/analog/analog-format.md) and [ANALOG:BITS](./../required/analog/analog-bits.md)) were added to the C3D file format description to document the analog sample format and measurement resolution. **These two parameters are “optional” in the sense that they are unnecessary unless the analog values have been stored as unsigned integers**. Some applications will not read these parameters and will fail to read unsigned 16-bit analog data although. 

The choice of SIGNED or UNSIGNED analog data and the ADC or data resolution can be determined by simply interpreting the [ANALOG:OFFSET](./../required/analog/analog-offset.md) value.

> The default storage format for all analog data in a C3D file is as a 16-bit signed integer.

It is strongly recommended that anyone storing 16-bit analog data in integer format C3D files follow the original C3D format description and store their data using signed integers wherever possible. Care is needed when writing code to convert between signed and unsigned formats or reading/writing all format variants.

The parameters listed below must always be provided if the C3D file does contain analog data. Other ANALOG parameters may be required by particular software applications. Consult your manufacturer’s documentation for details of application specific parameters.

## Additional ANALOG Parameters

These additional parameters document the extension of the ANALOG group to support more than 255 “analog channels”, enabling the storage of digital data values in the same manner that the C3D file uses to store more than 255 3D points. 

This method remains compatible with older applications which may be limited to only displaying and processing less than 255 analog channels but the extension to add more analog channels does not change the internal C3D format. Therefore implementation is relatively easy for most applications working with the C3D file format and makes it easy to maintain compatibility with older C3D files.

As with the extension to the [POINT group](./group-point.md#additional-point-parameters), the additional parameters described here must be each treated as a single array, the contents of all of the associated [LABELS](../additional/analog/analog-labels2.md), [DESCRIPTIONS](../additional/analog/analog-descriptions2.md), [SCALE](../additional/analog/analog-scale2.md), [OFFSET](../additional/analog/analog-offset2.md), and [UNITS](../additional/analog/analog-units2.md) parameters must all be manipulated in synchronization with each other.