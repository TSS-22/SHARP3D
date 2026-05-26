# ANALOG:FORMAT

- Type: Required ([see disclaimer](#disclaimer))

- Locked: True

> This part of the documentation in the C3D User Guide is unclear, [see disclaimer](#disclaimer). 

The ANALOG:FORMAT parameter is a character data array that consists of a single 7-bit ASCII (A-Z, 0-9) string that documents the [Analog Data](./../../../data/analog.md) format used within the C3D file. The parameter has two possible values: `SIGNED` or `UNSIGNED`. This
specifies whether the integer [Analog Data](./../../../data/analog.md) and associated integer values [Parameters](../../c3d-parameter-section.md) are stored as **signed** or **unsigned** 16-bit integer. It should normally be “locked”.

The original C3D file format defaulted to storing all data and parameters as Signed 16-bit integer values, with a range of -32767 to +32767. This is described as a **signed C3D file**. 

The parameter was invented originally because a manufacturer started storing [Analog Data](./../../../data/analog.md) as Unsigned 16-bit integer values
when 32-bit Floating-point became the default C3D file format. C3D files storing [Analog Data](./../../../data/analog.md) and [Offset](analog-offset.md) as Unsigned 16-bit integer (and probably other integer Analog Parameters, [see disclaimer](#disclaimer)) values are called **unsigned C3D file**. Storing [Analog Data](./../../../data/analog.md) and other Analog Parameters extends the range of possible values, as they can't feature negative values (e.g. point and analog channel counts). So even if the C3D format is floating-point, the C3D parameter integers will be read as unsigned integers resulting in the C3D file being described as unsigned.

> The parameter describes the analog data storage format, not the C3D file format. 

> The ANALOG:FORMAT parameter was first described about 2005, as a result software
applications created prior to that time will not read it. 

If the [ANALOG:FORMAT](./analog-format.md) parameter is `UNSIGNED` then the [ANALOG:OFFSET](./analog-offset.md) parameter, and other non-negative (all?) Analog Parameters must be interpreted as an Unsigned Int6.

If the [ANALOG:FORMAT](./analog-format.md) parameter does not exists then assume that its value is `SIGNED`. This will be correct most of the time.

If the ANALOG:FORMAT parameter contains the string `SIGNED` then the C3D 'storage' format for both the data samples and the ANALOG:OFFSET parameters must also be `SIGNED`. This is the default storage method for all analog data values, irrespective of resolution and allows data to be stored using signed integer values from -32767 to +32767 representing both positive and negative input signal excursions.

If the ANALOG:FORMAT parameter contains the string `UNSIGNED` then the [ANALOG:OFFSET](analog-offset.md) parameters must also be treated as `UNSIGNED` values. If the ANALOG:FORMAT parameter does not exist it should be assumed that its value is `SIGNED` unless the analog data contains 16-bit values, in which case `UNSIGNED` is a possibility.

## DISCLAIMER

This section is unclear in the C3D User Guide. It is not known if [ANALOG:FORMAT](./analog-format.md) concern all integer parameters, all integer parameters pertaining to analog data, or only the [ANALOG:OFFSET](./analog-offset.md). The only sure part is that [Analog Data](./../../../data/analog.md) are affected by it.

As most if not all Analog parameters seems to be positive value only, all integer analog parameters are affected by [ANALOG:FORMAT](./analog-format.md) values in SHARP3D, unless specified otherwise.

If [ANALOG:FORMAT](./analog-format.md) does not exist, nothing tells you that its value is `SIGNED` with 100% certainty but that is your best bet, and the one that SHARP3D is doing.


