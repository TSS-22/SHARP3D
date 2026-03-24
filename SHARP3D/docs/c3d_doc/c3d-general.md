# General informations

## Reading process of a C3D File

The endian format used in the C3D file must be determined in order to read the C3D file contents because the endian format affects the interpretation of all 16-bit integer and 32-bit floating-point formats. All applications opening a C3D file must determine the endian type before reading any integer or floating-point values.