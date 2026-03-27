# C3D Parameter Section

Information about the data stored in the file are stored in this section, so that any user opening the file can process the contents once the Parameter Section has been read and the user understands the Parameters.

Information that describes the data within the C3D file, or the data collection environment, is stored in the file as Parameters. 

All Parameters are organized into Groups so that related parameters (e.g., all the parameters containing information about the analog data in a C3D file) can be located and their associations identified. This concept allows users to have a simple, easy to remember, name for a parameter and then use the name in several different places.

When described and documented, the Group name and Parameter name are separated by a colon (:), so that when the parameter “SCALE” belongs to the “ANALOG” group
will be written as ANALOG:SCALE – the group name always precedes the parameter
name. The ability to reference parameters in this way allows similar parameters with
different functions to exist in the same file without any risk of confusion. Thus, the
SCALE parameter ANALOG:SCALE is different from the parameter POINT:SCALE, the
function of each parameter should be described by the associated parameter
description string to help document the file contents.

While there is a minimum set of Parameter information required to process or read a
C3D file, the Parameter and Group concept is flexible and allows users to create
groups and parameters to store relevant information. This information is then
available to any other application that reads the C3D file.

## Storing Order logic

It is useful to understand the logic that results in the apparent random assignment of Group and Parameter numbers, and the random ordering within the Parameter Section of Groups and Parameters.

If a parameter or group is added to the parameter section then the new item will usually be appended after the last entry in the Parameter Section. When a Parameter is deleted, it is first located and then all of its contents are packed out of the vector. This approach provides much flexibility, but means that the order of Groups and Parameters within the section will be random.

When writing the Parameter section, the total vector will be written. This ensures that all parameters that were read in, but were not changed, will be written out accurately. As a result the order in which parameters are found within the parameter section may be random.

To find a Parameter within the Parameter Section, the Parameter Section vector is searched
sequentially for the Parameter’s Group name, which then yields the group ID number.
The Parameter Section vector is then searched again from the beginning for parameters belonging to the appropriate group ID and having the required name. The Parameter’s data can then
be accessed.

All information stored in the Parameter Section is organized into Groups even though related items may be stored in various areas of the Parameter Section. A Group is simply a collection of related Parameters. 

Each Parameter is a member of a single Group thus allowing two Parameters to have the same name if they belong to different groups. For example, there may be two parameters called SCALE: one SCALE Parameter applies to 3D Point Data, while the other SCALE Parameter applies to
Analog Data. The two Parameters are stored in separate Groups called POINT and
ANALOG. Thus, the 3D Point Parameter can be referenced as [POINT:SCALE](./required/point-scale_factor.md) while the analog value can be read from the [ANALOG:SCALE](./required/analog-scale.md) Parameter.

## Structure 

### Header

| Name | Byte | Description                                                                                     |
|------|------|-------------------------------------------------------------------------------------------------|
| Unused/Ignored | 1    | Reserved and unused.                                                                             |
| Unused/Ignored | 2    | Reserved and unused.                                                                             |
| Length | 3    | Number of 512-byte blocks composing the Parameter Section.                                      |
|Processor Type | 4    | Processor type:<br>- 0x54: Intel<br>- 0x55: DEC (VAX, PDP-11)<br>- 0x56: MIPS (SGI/MIPS)          |

### Group

| Name | Position (byte) | Length (bytes) | Type            | Description                                                                                     |
|-----|-----------------|----------------|-----------------|-------------------------------------------------------------------------------------------------|
| Name Length | 1               | 1              | Signed byte     | Number of characters in the Group name (1-127). If negative, the Group is advertised as locked.              |
| ID | 2               | 1              | Signed byte     | The Group Id (-1 to -127). Always negative.                                                      |
| Name | 3               | n              | ASCII           | Group name. Only uppercase A-Z, 0-9, and "_" are supported.                                     |
|Pointer to next | 3 + n           | 2              | Unsigned int    | Number of bytes till the next Parameter Structure (starting at position 3+n, includes pointer).|
| Description Length | 3 + n + 2       | 1              | Unsigned byte   | Number of bytes in the Group description.                                                       |
| Description | 3 + n + 3       | m              | UTF-8           | Group description.                                                                              |

### Parameter

| Name | Position (byte)       | Length (bytes) | Type            | Description                                                                                     |
|-|-----------------------|----------------|-----------------|-------------------------------------------------------------------------------------------------|
| Name Length | 1                     | 1              | Signed byte     | Number of characters in the Parameter name (1-127). If negative, the Group is advertised as locked.              |
| ID | 2                     | 1              | Signed byte     | The Parameter Id (1 to 127). Always positive.                                                      |
| Name | 3                     | n              | ASCII           | Parameter name. Only uppercase A-Z, 0-9, and "_" are supported.                                     |
| Pointer to next | 3 + n                 | 2              | Unsigned int    | Number of bytes till the next Parameter Structure (starting at position 3+n, includes pointer).|
| Data Type | 3 + n + 2             | 1              | Unsigned byte   | Length in bytes of each data element:<br>- -1: Char<br>- 1: Byte<br>- 2: Int16<br>- 4: Float32   |
| Dimensions Number | 3 + n + 3             | 1              | Unsigned byte   | Number of dimensions of the Parameter Data. 0 for scalar.                                       |
| Dimensions Length | 3 + n + 4             | d              | Unsigned byte   | Length of each Parameter Data dimension.                                                        |
| Data | 3 + n + 4 + d         | t              | -               | Parameter Data.                                                                                 |
| Description Length | 3 + n + 4 + d + t     | 1              | Unsigned byte   | Number of bytes in the Parameter description.                                                       |
| Description | 3 + n + 4 + d + t + 1 | m              | UTF-8           | Parameter description.  

## Details

### Parameter Section Header

The Parameter Section Header is composed of 4 bytes and records the endian format that defines the storage method for all values stored in the C3D file: Int16 or Float32. So the Parameter Section must be located and read before most of the C3D file [Header](../c3d-header.md) values can be interpreted.

Then follows the Parameter Structures, listed in a random order. They can be either Groups or Parameters. Groups regroup the Parameters inside them.

#### Byte 1-2: Unused/Ignored

The first two bytes of the Parameter Section Header are to be ignored in the C3D Standard.

These two bytes are remainder from previous file format from which the C3D seems to be base upon: [ADTECH file format](https://ignazioa.github.io/c3d-test-v0/overview.html). 

The original ADTECH file format required
the first byte of a file to point to the first parameter block and the second byte to define the data format of the file.

#### Byte 3: Parameter Section Size

The third byte of the parameter header contains a count of the number of 512-byte blocks within the parameter section, counting the block that contains the parameter header record as block 1. 
This sets the maximum size of the parameter section storage allocation within the C3D file. 

The Parameter Block Count field is maintained to allow applications reading the C3D file to quickly determine the size of the Parameter Section, instead of having to calculate its size
by adding up the size of every individual Parameter within the C3D file.

If the parameters are added, edited, or deleted then the parameter storage block count must be verified and updated when the file is closed.

#### Byte 4: Processor Type and File Endianness

It enables any program accessing the parameter and data files to determine the endian format of the floating-point and integer numbers within the C3D file. 

This comes from a time where CPU architecture was a lot more varied than it is now. Today, most of the files you might encounter, if not all, will be of Intel type endianness.

The following three processor are the one documented by the C3D Standard:

|Value | Processor |
|------|-----------|
| 0x54 | Intel     |
| 0x55 | DEC (VAX, PDP-11) |
| 0x56 | MIPS (SGI/MIPS) |

You can add more processor to suits your needs, but other applications probably won't be able to read your file.

Supporting other processor types is more a matter of backward compatibility with older files, or working with specific system encountered in some niche applications.

### Groups and Parameters Details

The Groups and Parameters are stored starting at byte 5 of the Parameter Section. They are stored in random order providing flexibility when parameters need to be edited, deleted or added. 

> Each Parameter belongs to a Group. 

#### Common fields

##### Byte 1: Name Length

The number of character in the name of the Group/Parameter. A name can contains between 1 to 127 characters. If the value is set to a negative number, this means that the Group/Parameter is [Locked](#locked-flag).

> Always use the absolute value of Name Length to compute the actual Name Length.

Although the capability exists, in practice parameter Groups are not locked. Locking is only used for individual Parameters within Groups to flag items that that contain critical values within the C3D file structure.

##### Byte 2: ID

The Group/Parameter ID is a value in the range [1-127]. 

The value is negative for Groups, and positive for Parameters.

The ID values serve as link between Groups and Parameters: the Parameters with the same absolute ID values as a Group, belongs to this Group.

The actual value chosen for a Group ID number is not fixed and may vary from one C3D file to another. It is not required that group ID numbers are assigned in a contiguous sequence.

##### Byte 3: Name

All Group/Parameter names consist of 7-bit ASCII characters, letters A through Z,
the numerals 0 through 9, and the underscore character “_”. All Group/Parameter names must start with a letter: from A through Z.

Other punctuation or printable characters may not be used, and UTF-8 encoding is not permitted in group or parameter names.

When applications read Group/Parameter names, the case of the parameter or group name is not significant and if punctuation characters have been added by mistake, those should be ignored. For example the label h00b00m2 must be read as H00B00M2 and the labels L.Post.Fem. and Acc_2:X must be read as LPOSTFEM and ACC_2X to guarantee universal access to the data.

>All Group/Parameter names must be stored in a C3D file as standard 7-bit ASCII values to comply with the C3D standard for universal compatibility ([A-Z, 0-9, _ ]).

The same names may only be used for two Parameters if they occur in different groups. For example, both [POINT:SCALE](./required/point-scale_factor.md) and [ANALOG:SCALE](./required/analog-scale.md) parameters are permitted. 

>It is essential that all Group names, and all Parameters names within each Group, are unique. 

If you are working with older file, or older applications, keep in mind that the original C3D specification stated that when a parameter or group name is interpreted then only the first six characters of the group name and the first six characters of the parameter name are used. 
For example POINT:MARKER_UNITS and POINT:MARKER_ID may cause problems because the first six characters in both parameter names are identical.

> The Name field is simply a “name” that is used to reference the Group or Parameter. It does not have to be long and descriptive. For this, use the [Description field](#description).

Name length cannot exceed 127 character, and must have a bare minimum of 1 character, although 4 characters should generally be considered the actual a minimum.

Always make sure that Group/Parameter Names are spelled correctly: a software application that expects to read data from a Parameter called OFFSET will probably fail to find it if the Parameter has been spelt incorrectly as OFFSETS. Although the original C3D specification stated that the first six characters must be unique, the specification does not require that applications treat similar Parameter Names in the same way

##### Word 3+n: Pointer to next Group/Parameter

A word pointer to the next parameter data structure follows the Group/Parameter name string. Its value is the number of bytes to the next structure.

>The value of the pointer account for its own size. 

If this is the last Group/Parameter of the Parameter Section the pointer is supposed to be set as 0x0000 to indicate that this is the last one of the section. But in practice, this is rarely implemented as such in C3D files. Therefore, that behaviour cannot be relied on when trying to determine when to stop reading the Parameter Section.

>Due to the C3D Standard allowing Group/Parameter to not have [Description Length](#description-length) and [Description](#description) fields, the pointer value is your main source of truth as to when a Group/Parameter definition is over.

##### Description Length

As it is advertised in the official C3D User Guide, this stores the length of the Group/Parameter description string (0-255 characters) that immediately follows this byte.

In practice, it seems to be the length in bytes of the Description. Indeed Description support ASCII and UTF-8 character. UTF-8 characters have variable length between 1 and 4 bytes. The Description Length value is usually safe as a source of truth to determine when the Description actual length.

>This field is optional as per C3D Standard. But in order to keep Group/Parameter consistent everywhere, we vividly recommend applications to implement it even if no Description is to be set for the parameter data structure. 

##### Description

It stores the Description of the Group/Parameter. This field support ASCII and UTF-8 character formatting.

While Group/Parameter Name must be stored as ASCII characters to guarantee universal file access the parameter description, stored as UTF-8 characters, enables the parameter to be presented to users in their local language and characters.

While a Group description is not required, if you are creating a new group or parameter then it is recommended that you describe it so that other users who open the file will understand its function.

>This field is optional as per C3D Standard. But in order to keep Group/Parameter consistent everywhere, we vividly recommend applications to implement it even if no Description is to be set for the parameter data structure. 

#### Parameter fields

##### Byte 3+n+2: Data type

A parameter’s Data Type determines the type data that may be stored in the parameter. Four
parameter types are available; Signed Int16, Float32, Char (8bits), and Byte.

An integer is normally a signed 16-bit Integer number with a range of -32767 to +32767 although some Parameters will store an Unsigned 16-bit Integer. In the present C3D Standard, there is no way to know if a Parameter's Data is supposed to be Signed or Unisged without knowing about the Parameter beforehand.

> [SHARP3D integrate support for Unisgned Int16](https://tss-22.github.io/SHARP3D/api/SHARP3D.Utils.Enum.DataType.html). This way of doing might not be supported by other applications. 

32-bit floating-point numbers are written in scientific exponential representation. 

Characters are symbols such as a letter entered from the keyboard, they are mainly used as ASCII character for human readable parameters. 

Byte can contain a one’s complement 8-bit signed integer in the range -127 to +127 or an unsigned integer with a range of 0 to +255. As for Int16, The C3D Standard doesn't offer any way to know how the Parameter is supposed to be read if you don't have prior knowledge. 

> SHARP3D don't support Signed Byte as no such Parameter were found during testing but this could be implemented in the future if needed.

> Int16 default: Signed (-32767; +32767). Byte default: Unsiged (0; +255) 

Note that the interpretation of the data values is controlled by the [processor type](#byte-4-processor-type-and-file-endianness) which is usually determined by the hardware that originally generated the C3D file.

##### Byte 3+n+3: Dimensions Number

It stores the Parameter's data dimensions number. Its value can range from 0 to 7 ([scalar](https://en.wikipedia.org/wiki/Scalar_processor#Scalar_data_type), 1D, 2D, 3D, etc...). The actual values of each dimensions of the Parameter's data (e.g. 2 by 3, 6 by 6 etc...) are stored in the next two bytes.

The terminology "dimensions" is used here as its computing equivalent: it denotes the Rank of the Data: 0D, 1D, 2D, 3D, etc... 

A Parameter's data with 0 dimensions is a [scalar](https://en.wikipedia.org/wiki/Scalar_processor#Scalar_data_type), for example a single number or a signle character.

If the Parameter's data as a value of 1, it is a vector.

If the Parameter's data as a value of 2 or more, it is a matrix.

##### Byte 3+n+4: Dimensions Length

The actual length of each dimensions of the Parameter's Data.

##### Byte 3+n+4+d: Data

The Parameter's data as a vector of [data type](#byte-3n2-data-type). This data vector length is equal to the product of the Dimensions values or 1 if the Parameter's Data is a [scalar](https://en.wikipedia.org/wiki/Scalar_processor#Scalar_data_type) and has 0 [dimensions](#byte-3n3-dimensions-number).

> The parameter section of the C3D file follows [FORTRAN convention](https://fortran-lang.org/learn/best_practices/multidim_arrays/) and stores array in column order.

The storage order of multi-dimensioned array parameters follows the FORTRAN convention. In this format, the dimension that changes more
rapidly appears first.

Software applications reading and processing data in C3D arrays must ensure that the elements in the array are used correctly. If care if not taken then confusion can arise in the way matrices are processed due to the differences between the default FORTRAN column order of the array and the row based order assumed by C or other C++ based languages.

> SHARP3D takes care of this for you for the Parameters described in our C3D Documentation, see the Supported Parameter section of the documentation. If you would like other Parameter to be supported, [please contact us](https://github.com/TSS-22/SHARP3D/issues). Our goal is to be as exhaustive and easy to use as possible.

## Locked Flag

A locking mechanism is implemented to provide a mechanism to limit the ability of casual users to change parameters using Parameter examination and editing programs that might cause data corruption. A locked parameter can be edited and changed if necessary, the locking feature is simply present to limit the chance of anyone accidentally changing a parameter that will affect the data interpretation, or the file integrity.

This is accomplished by setting the first byte, [Name Length](#byte-1-name-length) of the Group/Parameter, to be negative (the absolute value remains unchanged). 

Although the capability exists, in practice parameter Groups are not locked. Locking is only used for individual parameters within Groups to flag items that that contain critical values within the C3D file structure.

> All parameters that have a negative [Name Length](#byte-1-name-length) are considered locked and should not be casually changed by the user. 

Any program that allows the user to modify Parameters must respect the locking mechanism. Applications that allow the user to edit Parameters should respect the Locked Flag status and either refuse to change the locked parameters, or restrict this feature to prevent an inexperienced user from damaging the integrity of the C3D file data. Unless there are special circumstances, any application that accesses a C3D file should not modify locked parameters.

For example an application that resamples the C3D point data will need to update the locked POINT:RATE parameter to record the new data rate but users occasionally think that they change the C3D file sample rate by simply editing the POINT:RATE parameter - a simple edit that would corrupt the C3D file.  

The effectiveness of the locking mechanism depends on the degree to which locking is supported and the consistency with which applications that create C3D files apply the locking property. The fact that a parameter has been locked by one application does not prevent any other application from changing it. Locking simply provides a flag that may be utilized by other locking aware applications.

>All locked C3D parameters (defined by the sign of the [Name Length](#byte-1-name-length)) should not be casually changed, as the stored values are normally critical to the integrity of
the C3D file data.

While it is recommended that all C3D applications respect the Parameter Lock Flag and follow procedures to preserve C3D file contents from casual changes that might corrupt the contents, this is not a strict requirement in the C3D file format. Many modern applications set the Parameter Lock Flag incorrectly, either leaving every Parameter unlocked or occasionally locking every Parameter.

## Notes

> Applications that modify C3D files must take care to preserve all groups and parameters from the original input file even if the application does not use or understand the parameters

- Unused bytes at the end of the parameter section are normally filled with 0x00.

- Initially C3D files stored the number of Parameters in the third byte, a factor that was changed as users started creating additional Parameters. No sample file ever made it to us showcasing such structure.

- The Pointer to Next Structure is an important value as some applications have non standard behavior and discard the Description Length and Description all together from Group/Parameter definition. You therefore need to rely on it to know if you came to the end of the present Group/Parameter definition. 

- There is no count stored for the number of Parameters in each Group and all Group and Parameter records can appear in any order. This means that it is permissible for a Parameter to appear in the Parameter Section before the Group it belongs to. Software accessing a C3D file should be prepared to deal with this situation.

- It is good practice to not create Parameters that depends on another Parameters to be intepreted, due to the random order of appearance. Some applications do such thing so you must be prepared to encounter this behavior, but can not encourage such ways.

- Do not assume that just because a parameter exists and has the name that you expect, that it will contain the same type of data. The parameter structure provides a means to determine the type of the parameter (floating- point, signed integer, character etc.) before you read it. The consequences of reading an integer value by default, when the parameter data structure turns out to be floating-point may cause applications to fail. The C3D format was designed to be flexible and applications reading C3D files must always determine the parameter type before reading the data from the parameter.

