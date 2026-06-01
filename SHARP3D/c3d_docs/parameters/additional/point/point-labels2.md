# POINT:LABELSX

- **Type**: [Additional](../../additional.md)

- **Locked**: False

> This serie of parameter is found in many larger C3D files that contain more than 255 3D points.

This serie of parameter, (i.e. LABELS2, LABELS3, etc), is used to store additional point identifying labels beyond the 255 limit in the default [POINT:LABELS](../../required/point/point-labels.md) parameter. Each of these parameters are an array of up to 255 character strings, and therefore expands the maximum number of labels by 255. 

When a LABELSX (i.e. LABELS2, LABELS3 etc) is found in a C3D file then the LABELS parameters of higher precedence (LABELSX-1 and onward) must always store 255 values. For example, if LABELS2 is present, LABELS will store 255 values. If LABELS3 is present, LABELS and LABELS2 store 255 values each.

 Some software applications can generate a great many 3D trajectories. Since the C3D parameter array used to store the [POINT:LABELS](../../required/point/point-labels.md) names have a maximum dimension of 255, the use of a single label array would limit the number of 3D markers that could be stored in a C3D file. The solution here is to create additional LABELS parameters by adding a number e.g., LABELS2. If required, additional parameters like this could exist such as LABELS3, LABELS4, etc., to store even more 3D point labels.

UTF-8 encoding is permitted for the LABELS but ASCII characters are recommended as most user localization requirements can be satisfied by defining a UTF-8 encoded [DESCRIPTIONS](../../required/point/point-descriptions.md) string with the same array index. It is important that all [POINT:LABELS](../../required/point/point-labels.md) and POINT:LABELS2 names are concise and unique as they are used by software applications to identify, reference, and track individual 3D points recorded in the C3D file. Refer to [POINT:LABELS](../../required/point/point-labels.md) page for more information on labelling rules.

> To keep it consistent with [POINT:LABELS](../../required/point/point-labels.md) rules, SHARP3D consider using UTF-8 characters in POINT:LABELSX a malpractice.