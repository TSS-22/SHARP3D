# ANALOG:BITS

- **Type**: [Required](../../required.md)

- **Locked**: True

> This parameter was added to the C3D format several years after its creation and may not be found in older C3D files.

The ANALOG:BITS parameter is a single integer value that describes the analog data sample resolution and will normally contain one of three values, 12, 14 or 16.

As this value directly affects the interpretation of the analog data it should normally be “locked”. If the parameter does not exist its value can be measured by reading every analog sample contained in the [analog data section](../../../data/analog.md) and
determining the effective resolution from the highest analog data value found. Alternatively, it is usually safe to assume that its value is 12. 

Software applications that change the resolution of analog data values for compatibility (i.e., down sampling 16-bit data to 12-bits) should always update this parameter to indicate the resolution of the data stored within the C3D file as it can be used to allow software applications to recalculate the [ANALOG:SCALE](./analog-scale.md) parameter values.

## Boundary Values

### Unsigned Integers

- 12-bit: 0 to 4,095
- 13-bit: 0 to 8,191
- 14-bit: 0 to 16,383
- 15-bit: 0 to 32,767
- 16-bit: 0 to 65,535

### Signed Integers (Two's Complement)

- 12-bit: -2,048 to 2,047
- 13-bit: -4,096 to 4,095
- 14-bit: -8,192 to 8,191
- 15-bit: -16,384 to 16,383
- 16-bit: -32,768 to 32,767


