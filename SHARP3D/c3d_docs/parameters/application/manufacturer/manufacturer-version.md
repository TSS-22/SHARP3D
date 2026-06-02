# MANUFACTURER:VERSION

- **Type**: [Application](../../application.md)

- **Locked**: True

Originally described as an ASCII character string, the VERSION parameter is intended to identify the version of the software that created the C3D file. Some manufacturers have redefined this as an array of integers that record the version identification, while other manufacturers store their software version as an ASCII string using a different parameter name [MANUFACTURER:VERSION_LABEL](./manufacturer-version.md).

Since all parameter headers describe the parameter type, applications that follow the C3D specification accurately can read the VERSION parameter type and determine if it is an ASCII string, an integer, or an array before correctly reading and displaying the version data stored in the parameter. 

> This parameter should be locked and must not be changed by other software applications if they modify the C3D file because it is provided to document the creation of the C3D file.