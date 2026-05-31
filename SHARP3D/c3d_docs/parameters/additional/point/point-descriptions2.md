# POINT:DESCRIPTIONSX

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The parameters in these serie (i.e. DESCRIPTIONS2, DESCRIPTIONS3, etc) are an array of ASCII or UTF-8 encoded character strings that may be used to describe each corresponding [LABELSX](./point-labels2.md) value. These parameters are synchronized with the corresponding POINT:LABELSX parameter and contains additional description strings with the same properties as the standard [POINT:DESCRIPTIONS](../../required/point/point-descriptions.md) parameter. For example, DESCRIPTIONS2, describre the trajectories named in LABELS2, DESCRIPTIONS3, the one in LABELS3, etc.

These parameters describes the contents of a [LABELS<sub>n</sub>](./point-labels2.md) parameters with the same array index to document the point location or function for anyone reading the C3D file. 

> Any modifications to the C3D file points, by adding or deleting a point, must maintain the descriptions stored in DESCRIPTIONS, DESCRIPTIONS2 etc., in synchronization with the identifiers stored in the [LABELS](../../required/point/point-labels.md) parameters.

Additional information and rules about these parameters can be found in [POINT:DESCRIPTIONS](../../required/point/point-descriptions.md) page.