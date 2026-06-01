# TRIAL:ACTUAL_END_FIELD

- **Type**: [Additional](../../additional.md)

- **Locked**: False

The last frame number in the C3D file is stored in the same way as two unsigned 16-bit integer values to form a 32-bit value. The first unsigned 16-bit integer is the least significant word and the second unsigned 16-bit integer is the most significant word.

Therefore the last frame number is calculated as:

$\text{Last frame number} = \text{ACTUAL\_END\_FIELD}[1] + \text{ACTUAL\_END\_FIELD}[2]*65535$