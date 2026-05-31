# ANALOG:DESCRIPTIONSX 

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The parameters in these serie (i.e. DESCRIPTIONS2, DESCRIPTIONS3, etc) are an array of ASCII or UTF-8 encoded character strings that may be used to describe each corresponding [LABELSX](./analog-labels2.md) value. These parameters are synchronized with the corresponding POINT:LABELSX parameter and contains additional description strings with the same properties as the standard [ANALOG:DESCRIPTIONS](../../required/analog/analog-descriptions.md) parameter. For example, DESCRIPTIONS2, describre the trajectories named in LABELS2, DESCRIPTIONS3, the one in DESCRIPTIONS2, etc. When a DESCRIPTIONSX (i.e. DESCRIPTIONS2, DESCRIPTIONS3 etc) is found in a C3D file then the LABELS parameters of higher precedence (LABELSX-1 and onward) must always store 255 values. For example, if DESCRIPTIONS2 is present, DESCRIPTIONS will store 255 values. If DESCRIPTIONS3 is present, DESCRIPTIONS and DESCRIPTIONS2 store 255 values each.

These parameters describes the contents of a [LABELS<sub>n</sub>](./analog-labels2.md) parameters with the same array index to document the analog channel location or function for anyone reading the C3D file. 

> Any modifications to the C3D file analogs channels, by adding or deleting a channel, must maintain the descriptions stored in [DESCRIPTIONS](../../required/analog/analog-descriptions.md), DESCRIPTIONS2 etc., in synchronization with the identifiers stored in the [LABELS](../../required/analog/analog-labels.md) parameters.

Additional information and rules about these parameters can be found in [ANALOG:DESCRIPTIONS](../../required/analog/analog-descriptions.md) page.