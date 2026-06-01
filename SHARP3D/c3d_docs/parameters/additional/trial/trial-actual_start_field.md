# TRIAL:ACTUAL_START_FIELD

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The first frame number in the C3D file is stored in two unsigned 16-bit integer values to form a 32-bit value. The first unsigned 16-bit integer is the least significant word while the second unsigned 16-bit integer is the most significant word.

Therefore the first frame number is calculated as:

$\text{First frame number} = \text{ACTUAL\_START\_FIELD}[1] + \text{ACTUAL\_START\_FIELD}[2]*65535$
