# ANALOG:OFFSET

- **Type**: [Required](../../required.md)

- **Locked**: True

> It is recommended that any offset adjustment of the C3D data is performed by the application reading the C3D file and does not alter the C3D file contents in any way. This approach preserves the original analog data measurements.

The ANALOG:OFFSET parameter is normally an array of integer values that are subtracted from each analog measurement before the individual [ANALOG:SCALE](./analog-scale.md) scaling factors are applied. By default a signed integer, the ANALOG:OFFSET values may be either positive or negative numbers in the range of –32768 to +32767 and can include the value of zero. However, if the [ANALOG:FORMAT](analog-format.md) parameter is “UNSIGNED” then the ANALOG:OFFSET parameter should be interpreted as unsigned integer numbers in the range of 0 to +65535.

One application of the ANALOG:OFFSET is to adjust the zero baselines for devices such as force plates that should return a zero when unloaded. In practice, force plates are notorious for drifting away from an unloaded zero value, which can lead to measurement errors in use. There are two common methods for “zeroing” these devices, each involves determining the measurement error during some period of unloaded sampling, by subtracting the sampled data values from the recorded ANALOG:OFFSET value. This result can then be used to reset the ANALOG:OFFSET parameters to new values (each analog channel will have a different “error” value here) or, can be used to adjust the sampled analog data values or correct the original offset measurement error. Both methods are in common use; both methods may run into problems if either the analog data or OFFSET parameters are close to their limits.

> The possibility of 16-bit integer overflow exists when applying the [ANALOG:OFFSET](./analog-offset.md) Parameter to the sampled 16-bit analog data. It is recommended that all applications perform internal scaling calculations with more than 16-bits of resolution (either 32-bit or floating-point) and check the results to ensure that internal math overflow has not occurred.

> There must always be a one to one correspondence between the [ANALOG:SCALE](./analog-scale.md) and ANALOG:OFFSET parameters. Both the [SCALE](./analog-scale.md) and OFFSET parameters must exist for every analog channel up to the value stored in the [ANALOG:USED](./analog-used.md) parameter.

The sampled [analog data is normally stored in the C3D file as signed integer values](../../../data/analog.md#signedunsigned) within the range of -32767 to +32767. It is worth noting at this point that the binary encoding method for analog data is not directly specified within the original C3D format specification which defaulted to using signed integers and, so long as the scaled results are correct, analog data can be stored anywhere within the range of the integer data type.

In general, the analog data is encoded over a symmetrical range (from a value of +v to –v) but this is not an absolute requirement. Software applications may write the analog data samples as unsigned values and use the OFFSET parameter to convert them to back to signed values when the data is scaled into physical world values.

> The ANALOG:OFFSET parameter may contain a negative value if an application has written it as an unsigned integer value in error.

The table shown below illustrates two common encoding methods used to represent both positive and negative values in C3D files.

| Scale | Offset Binary | Two's Complement |
| --- | --- | --- |
| + 1 * Full Scale | 1111 ... 1111 | 0111 ... 1111 |
| + 0.75 * Full Scale | 1110 ... 0000 | 0110 ... 0000 |
| + 0.50 * Full Scale | 1100 ... 0000 | 0100 ... 0000 |
| + 0.25 * Full Scale | 1010 ... 0000 | 0010 ... 0000 |
| 0 | 1000 ... 0000 | 0000 ... 0000 |
| - 0.25 * Full Scale | 0110 ... 0000 | 1110 ... 0000 |
| - 0.50 * Full Scale | 0100 ... 0000 | 1100 ... 0000 |
| - 0.75 * Full Scale | 0010 ... 0000 | 1010 ... 0000 |
| - Full Scale + 1 LSB | 0000 ... 0001 | 1000 ... 0001 |
| - 1 * Full Scale | 0000 ... 0000 | 1000 ... 0000 |

LSB: Least Significant Beat

Offset Binary is a simple binary count that is offset in order to represent equal magnitude over the positive and negative ranges. The maximum negative range being all zeros, while all ones represents the maximum positive range. The mid-range or zero value is represented by setting the most significant bit, with all other bits cleared. Two’s Complement Binary uses a simple binary count to represent all positive values while all negative values are stored with the most significant bit set. The Two’s Complement format simplifies the interface at a machine code level but offers no advantages within the C3D format or within high-level languages. It is a common output option for many Analog to Digital Converter (ADC) devices.

Software applications must always use the OFFSET and [SCALE](./analog-scale.md) parameters to determine data magnitude and must not assume that either OFFSET or [SCALE](./analog-scale.md) will take any particular value.

| ADC Resolution | Signed OFFSET | Unsigned OFFSET |
| --- | --- | --- |
| 8-bits | 0 | 127 |
| 12-bits | 0 | 2047 |
| 14-bits | 0 | 8191 |
| 16-bits | 0 | 32767 |

Typically, an analog-to-digital converter (ADC) has 12 to 16 bits of resolution, and can capture and store analog data using signed integer values from -32768 to +32767 representing both positive and negative input signal excursions. In order for software applications to correctly translate the analog data recorded in the C3D file into physical world values, the ANALOG:OFFSET and ANALOG:SCALE parameters must contain appropriate values. These are applied as shown:

`physical world value = (data value – ANALOG:OFFSET) * ANALOG:SCALE`

For example, a ±5 volt ADC with 12-bits of resolution can produce 4096 discreet samples values – these may be mapped as unsigned values using the range of 0 to +4095 (in which case the OFFSET would be +2047 for a symmetrical +5 to –5 volt range, translating the ADC samples into the signed integers). They could equally well be mapped directly as signed integers in the range of –2048 to +2047 in which case the OFFSET would be 0. If the [ANALOG:SCALE](./analog-scale.md) and OFFSET values are applied correctly, both configurations will return identical values covering the range of +5 to –5 volts.

