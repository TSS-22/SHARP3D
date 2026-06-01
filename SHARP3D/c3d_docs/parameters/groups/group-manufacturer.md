# MANUFACTURER

> This group is optional and exists to document the C3D file creation, any changes or modifications.

The MANUFACTURER group is used to record information about the software or hardware used to create the C3D file. This group is intended to provide information that can be used to identify the hardware or software that generated the C3D file to document the original creation of each C3D file. The existence of this group provides information that may be useful when debugging individual user issues and can help applications and users determine the correct interpretation of any unique parameters (e.g. [ANALYSIS group](./group-analysis.md) parameters) stored in the C3D file whose values and interpretation are specific to the source of the original data.

There are no requirements that this group exists in a C3D file or that it contains any specific parameters. The following parameters are common but manufacturers can create any values within the group. This group and, any parameters contained within the group, are optional and should contain values specific to the C3D file creator. The intention is to make it easy for anyone who works with C3D files to determine the original source of the file in case any problems are encountered.

Since this is a user defined group, the parameter values may be stored in 7-bit ASCII or UTF-8 encoding, but since UTF-8 encoded characters are longer than 8-bits, the use of UTF-8 limits the size of the stored entries.