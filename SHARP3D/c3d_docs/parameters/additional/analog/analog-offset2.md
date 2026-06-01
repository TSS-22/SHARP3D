# ANALOG:OFFSETX

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The parameters in these serie (i.e. OFFSET2, OFFSET3, etc) are an array of 255 additional [ANALOG:OFFSET](../../required/analog/analog-offset.md) values. These parameters are synchronized with the corresponding nalog channels. An individual [OFFSET](../../required/analog/analog-offset.md) parameter is required for every analog channel supported by the C3D file. Refer to the page on [ANALOG:OFFSET](../../required/analog/analog-offset.md) for more information.

When a OFFSET (i.e. OFFSETS2, OFFSETS3 etc) is found in a C3D file then the LABELS parameters of higher precedence (LABELSX-1 and onward) must always store 255 values. For example, if OFFSETS2 is present, OFFSETS will store 255 values. If OFFSETS3 is present, OFFSETS and OFFSETS2 store 255 values each.

> Any modifications to the C3D file analogs channels, by adding or deleting a channel, must maintain consistency with related Analog parameters: [LABELS](./analog-labels2.md), [DESCRIPTIONS](analog-descriptions2.md), OFFSET, [SCALE](./analog-scale2.md), [UNITS](./analog-units2.md).