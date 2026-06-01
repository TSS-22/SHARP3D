# ANALOG:LABELS

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The parameters in these serie (i.e. LABELS2, LABELS3, etc) are an array of ASCII [ANALOG:LABELS](../../required/analog/analog-labels.md) entries that will only be seen in C3D files that contain more than 255 analog channels and extends support for an additional 255 additional analog channels per additional parameter. When a LABELSX (i.e. LABELS2, LABELS3 etc) is found in a C3D file then the LABELS parameters of higher precedence (LABELSX-1 and onward) must always store 255 values. For example, if LABELS2 is present, LABELS will store 255 values. If LABELS3 is present, LABELS and LABELS2 store 255 values each. The function of the LABELS is to provide a means of identifying and referencing analog channels so all [ANALOG:LABELS](../../required/analog/analog-labels.md) must be concise and unique. While UTF-8 encoding is permitted for LABELSX entries, it is recommended that all LABELS entries should use ASCII. Refer to [ANALOG:LABELS](../../required/analog/analog-labels.md) page for more information on labelling rules. 
<!--All LABELS parameter characteristics
must be identical, defined as CHAR strings with the same lengths.-->

> To keep it consistent with [ANALOG:LABELS](../../required/analog/analog-labels.md) rules, SHARP3D consider using UTF-8 characters in ANALOG:LABELSX a malpractice.