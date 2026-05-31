# ANALOG:SCALEX

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The parameters in these serie (i.e. SCALE2, SCALE3, etc) are an array of 255 additional [ANALOG:SCALE](../../required/analog/analog-scale.md) values. These parameters are synchronized with the corresponding nalog channels. An individual [SCALE](../../required/analog/analog-scale.md) parameter is required for every analog channel supported by the C3D file. Refer to the page on [ANALOG:SCALE](../../required/analog/analog-scale.md) for more information.

When a SCALESX (i.e. SCALES2, SCALES3 etc) is found in a C3D file then the LABELS parameters of higher precedence (LABELSX-1 and onward) must always store 255 values. For example, if SCALES2 is present, SCALES will store 255 values. If SCALES3 is present, SCALES and SCALES2 store 255 values each.

> Any modifications to the C3D file analogs channels, by adding or deleting a channel, must maintain consistency with related Analog parameters: [LABELS](./analog-labels2.md), [DESCRIPTIONS](analog-descriptions2.md), [OFFSET](analog-offset2.md), SCALE, [UNITS](./analog-units2.md).