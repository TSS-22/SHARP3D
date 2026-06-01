# ANALOG:UNITSX

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The parameters in these serie (i.e. UNIT2, UNIT3, etc) are an array of 255 additional [ANALOG:UNIT](../../required/analog/analog-units.md) values. These parameters are synchronized with the corresponding nalog channels. An individual [UNIT](../../required/analog/analog-units.md) parameter is
required for every analog channel supported by the C3D file. Refer to the page on [ANALOG:UNIT](../../required/analog/analog-units.md) for more information.

When a UNITSX (i.e. UNITS2, UNITS3 etc) is found in a C3D file then the LABELS parameters of higher precedence (LABELSX-1 and onward) must always store 255 values. For example, if UNITS2 is present, UNITS will store 255 values. If UNITS3 is present, UNITS and UNITS2 store 255 values each.

> Any modifications to the C3D file analogs channels, by adding or deleting a channel, must maintain consistency with related Analog parameters: [LABELS](./analog-labels2.md), [DESCRIPTIONS](analog-descriptions2.md), [OFFSET](analog-offset2.md), [SCALE](./analog-scale2.md), UNITS.