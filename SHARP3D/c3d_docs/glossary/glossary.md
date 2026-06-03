# Glossary of Terms

This glossary contains definitions of terms used in the C3D documentation. In some cases, terms such as record, blocks, and section, are used in ways that may appear unconventional to many users with a traditional programming background. The use of these terms in this manual is an attempt to describe the C3D format in a coherent fashion as a vehicle for the accurate storage of universally accessible data in the 3D biomechanics motion capture environment.

## 2's complement of the positive value

This is a standard way to represent signed integers (both positive and negative) in binary. For a negative number, its 2's complement is calculated by:

1. Inverting all the bits of its positive counterpart (1's complement).
2. Adding 1 to the result.

Example:
The 8-bit 2's complement of +5 (00000101) is -5 (11111011).

See https://en.wikipedia.org/wiki/Two's_complement for more details.

## 3D Data

The C3D file format was created to provide a standard method of storing [3D data](../data/3d-point.md) as coordinates, referenced to a single origin. All 3D data locations consist of three dimensions, recording the $X$, $Y$ and $Z$ distance from a single fixed origin that is used to define the recording environment co-ordinate system. Typically the $+Z$ axis rises vertically from the floor with the direction of progression for motion within the coordinate system in the $X$ and $Y$ axes but this is only a convention. In simpler term, the typical convention is one of a right handed coordinate system.

See https://en.wikipedia.org/wiki/Right-hand_rule. 

## [3D Frame](../data/3d-point.md)

Each [3D data frame](../data/3d-point.md) consists of one or more 3D data points and analog data samples that can be considered to be the values of the measurement variables at a single instant of time. This avoids the misunderstandings that can be caused by the use of the terms “Video Frame” and Video Field” since C3D files are normally created by motion capture systems that sample camera and sensors directly. All 3D frames are recorded in sequence, at intervals defined by the parameter [POINT:RATE](../parameters/required/point/point-rate.md), which is
written as a frequency value in Hertz (cycles per second).

A [3D data frame](../data/3d-point.md) may contain zero or more 3D points as recorded in the parameter [POINT:USED](../parameters/required/point/point-used.md). Since the C3D format is a general format intended for biomechanical data storage, it is also possible to create C3D files that contain only [analog data](../data/analog.md) values without any associated [3D data](../data/3d-point.md) values. Note that although a C3D file only contains analog data with no [3D data points](../data/3d-point.md), the [analog data](../data/analog.md) will be stored as a fixed
number of analog samples per [3D Frame](../data/3d-point.md).

## 3D Point

A 3D point is a single measurement of a point in space as an offset from the origin of the measurement system. In its most basic form this consists of three coordinate measurements ($X$, $Y$, and $Z$) although it is possible to record fewer dimensions by setting any unused coordinates to zero.

In addition to the $X$, $Y$ and $Z$ coordinates, the C3D format supports additional information stored with each 3D point to describe the coordinate measurement properties. See [the descriptions](../data/3d-point.md#camera-mask-and-residual) of the [Residual](../data/3d-points-residuals.md) and [Camera Contribution](../data/camera-mask.md).

## ADTECH File Format

To best meet all of the above requirements it was decided that the format should incorporate the following:

- The files will be binary format.
- Each file will have a "primary" parameter section where parameter information is stored according to a specified format.
- Data and their parameters will be in the same file, with a specified scheme to tell programs where the "primary" parameter section is located.
- Parameters may have the standard numerical types of byte, integer, float, and character.
- Parameters may have "dimensions" to also allow the definition of vector and matrix quantities.
- Parameters may have associated names and descriptions, and will be classified into groups for ease of use and access by the user.
- The way the data are stored and their location(s) within the file may be flexibly specified through the parameters.

Hence the basic ADTech file format, of which the C3D format is an example, has the following specifications:

- The first byte of the file contains the number of the 1st parameter record in the file (all records are considered to be 512 bytes long).
- The 2nd byte is a key byte and must contain decimal 80.
- The first 2 bytes of the first parameter record are the first two bytes of the file or are not used.
- Byte 3 of the 1st parameter record contains the number of parameter records.
- Byte 4 of the 1st parameter record codes the processor data type, i.e., 84 for PC (Intel), 85 for DEC, and 86 for Sun/MIPS systems.
- The parameters are stored starting at byte 5 of the first parameter record according to the parameter format specification described in the documentation. The parameters are stored in random order providing for much flexibility at the expense of some programming complexity.
- Each parameter has a data type, optional dimensions, a name, a description, and belongs to a group. Each group also has a name and a description.

This is a shameless copy-paste from [Ignazio Aleo works](https://ignazioa.github.io/c3d-test-v0/overview.html). Just to make sure we have a copy in case the website goes down.

## ASCII

The [ASCII standard (American Standard Code for Information Interchange)](https://en.wikipedia.org/wiki/ASCII) was created in 1960 to define standard numerical representations for printable characters and functions in the information transfer environment. The C3D format supports the standard “printable” 7-bit ASCII characters with no support for formatting such as tabs, bold, underscoring, carriage return, or controls such as SYN, DEL, or ESC.

## ADC

An [Analog to Digital Converter](https://en.wikipedia.org/wiki/Analog-to-digital_converter), also named ADC (A/D or A-to-D), is a hardware component that converts analog voltages into digital values that can be recorded in temporal synchronization with 3D measurements, typically enabling force and moment information, together with other biomechanics data such as electromyography and acceleration, to be stored in a C3D file as analog data samples.

The analog data samples generated by an ADC will normally have a fixed digital resolution (typically 12, 14, or 16-bits) and are generated repetitively at a sample rate that defines the bandwidth of the sampled data. The analog ADC data environment is defined by the [ANALOG:SCALE](../parameters/required/analog/analog-scale.md), [OFFSET](../parameters/required/analog/analog-offset.md), and [GEN_SCALE](../parameters/required/analog/analog-gen_scale.md) parameters in each C3D file containing analog data.

## Analog Data Sample

[Analog data](../data/analog.md) stored in a C3D normally consists of a number of analog measurements that have all been recorded at a single instant of time from each analog channel that is being sampled. All analog data samples are recorded in sequence at regular intervals defined by the parameter [ANALOG:RATE](../parameters/required/analog/analog-rate.md), which is written as a frequency value in Hertz. It is required that every analog data sample must contain the same number of analog measurements defined by the parameter [ANALOG:USED](../parameters/required/analog/analog-used.md).

Additional critical factors in recording accurate analog samples are the [ADC](#adc) input
range settings, the analog sample rate, and the scaling calculations that convert each
data sample into real-world values. Both the individual [ADC](#adc) input range settings
and the [ADC](#adc) sample rate are controlled by the data collection system and any
changes that affect the sampled signal must be recorded in the appropriate [C3D analog parameters](../parameters/groups/group-analog.md) so that the analog data can be accurately reconstructed.

## Analog Sample Format

The C3D format expects that the format of the stored analog sample from the [ADC](#adc) will be an unsigned 16-bit binary code defined by the resolution of the [ADC](#adc). The real-world value of the [ADC](#adc) sample is determined by the voltage range of the [ADC](#adc) channel which must be configured to match the range of the applied analog signal.

The stored binary analog samples are converted into real-world values by the scaling calculations using the [ANALOG:SCALE](../parameters/required/analog/analog-scale.md), [ANALOG:GEN_SCALE](../parameters/required/analog/analog-gen_scale.md) and [ANALOG:OFFSET](../parameters/required/analog/analog-offset.md)
parameters.

## Analog Sample Rate

The [Nyquist sampling theorem](https://en.wikipedia.org/wiki/Nyquist%E2%80%93Shannon_sampling_theorem) indicates that a minimum of two samples per cycle of the data bandwidth are required to reproduce the sampled signal with no data loss.

Essentially this eliminates the possibility of introducing an aliasing component into the sampled data but does not guarantee that an accurate signal waveform will be recorded and can be reconstructed post-collection. Accurate data reconstruction of biomedical signals normally requires at least five data samples per maximum data cycle bandwidth.

## Arrays

In [FORTRAN](https://fortran-lang.org/) and in the parameter section of the C3D file, arrays are stored in
column order, i.e. the array:

$$
\begin{matrix}
C_{11} & C_{12} & C_{13}\\
C_{21} & C_{22} & C_{23}\\
\end{matrix}
$$ 

is stored serially in the order $C_{11}, C_{21}, C_{12}, C_{22}, C_{13}, C_{23}$. In [FORTRAN](https://fortran-lang.org/) and
[C3D parameter](../parameters/c3d-parameter-section.md) notation these elements are written as:

`C(1,1), C(2,1), C(1,2), C(22),C(1,3), C(2,3)`

And the array is dimensioned as `C(2,3)`.

In programming environments derived from [C](https://en.wikipedia.org/wiki/C_(programming_language)) and [C++](https://en.wikipedia.org/wiki/C%2B%2B), an array storing the elements in the same serial order is defined as `c[3] [2]`, with the 2nd subscript
varying most rapidly.

## Block

This manual describes the C3D file as being composed of a number of 512-byte blocks of information. Various data sections within the C3D file are aligned on multiples of 512 bytes and pointers to sections within the C3D file structure are generally stored as block counts. The choice of a 512-byte block size for the low-level structure of the C3D file is a historical artifact due to the use of [FORTRAN](https://fortran-lang.org/) in the original [PDP-11](https://en.wikipedia.org/wiki/PDP-11) programming environment.

The term record is used to describe individual units of information such as parameters and data samples that are stored within various sections in the C3D file. Individual sections and records within the C3D file may cross 512-byte block boundaries.

## Bytes

Many parameters and data values are recorded in the C3D file as integer values. In the original C3D implementation, all 8-bit byte values were signed bytes with a range of –127 to +127.

However, in some cases, the use of signed bytes limited the range available for parameter storage. As a result, it is common to find unsigned bytes used in many C3D files yielding numerical ranges from 0 +255 for an unsigned 8-bit byte counter.

Note that this does not apply to the bytes defining the group and parameter name lengths which are stored and read as signed bytes to record the [locked, or unlocked, status flag](../parameters/c3d-parameter-section.md#locked-flag).

## CAMARC

[Computer Aided Movement Analysis in a Rehabilitation Context (CAMARC)](https://link.springer.com/chapter/10.1007/978-3-642-51659-7_3) was a project funded in 1989 by the EU that developed a public [ASCII](#ascii) file format for the storage and exchange of Clinical Motion Information. The project aimed to establish a European network of clinical and research centers with manufacturers and health care "end-users", and create a standard approach to Clinical Functional Assessment and Clinical Motion Analysis by defining a universally accessible [ASCII](#ascii) data format for the exchange and storage of data.

## Camera Contribution

The camera contribution value is also called [camera mask](../data/camera-mask.md). The calculation of a [3D data location](../data/3d-point.md) requires two or more observers (cameras or sensors). When more than two observers contribute to the calculation of a 3D location, it is useful to record which of the observers contributed to the calculated measurement. The C3D point record allows up to seven observers (generally, but not necessarily, cameras) to record whether or not their data was used to generate the 3D Point measurement.

This is information specific to each data collection environment and can be very useful for debugging and quality control as it allows a user to identify the cameras (or observers) that produced information used by the 3D calculations that generate the 3D locations stored in the C3D file.

## Characters

All characters that are defined in the C3D file format are limited to standard 7-bit [ASCII](#ascii) values from decimal 32 to 126. When characters are used in [C3D parameter and group names](../parameters/c3d-parameter-section.md), only upper case characters A-Z, the underscore “_” and 0-9 are permitted to conform to the C3D standard, and ensure universal compatibility for all software applications.

However, user entered data, stored in the LABELS and DESCRIPTIONS parameters or in application specific groups like [SUBJECTS](../parameters/groups/group-subjects.md) and [EVENT](../parameters/groups/group-event.md), may use alternate [UTF-8](#utf-8) 
character sets, but be aware that applications that do not support [UTF-8](#utf-8) encoding may display these incorrectly. The use of [UTF-8](#utf-8) encoding as specified by [RFC3629](https://www.rfc-editor.org/info/rfc3629/) is permitted but, if [ASCII](#ascii) parameters are edited and converted to [UTF-8](#utf-8) encoding, then applications may need to extend the parameter array storage to handle the larger byte count.

## DEC, Intel, and SGI/MIPS

DEC is the default format for data created in a [Digital Equipment Corporation](https://en.wikipedia.org/wiki/Digital_Equipment_Corporation) environment, typically an [RSX-11M](https://en.wikipedia.org/wiki/RSX-11) or [VAX operating system](https://en.wikipedia.org/wiki/VAX).

Intel is normally the default format for data created in an [MSDOS](https://en.wikipedia.org/wiki/MS-DOS) or Microsoft Windows environment.

SGI/MIPS is the default format for data created in a [Silicon Graphics Inc.](https://en.wikipedia.org/wiki/Silicon_Graphics), or MIPS Technologies environment, typically [RISC](https://en.wikipedia.org/wiki/Reduced_instruction_set_computer) based 3D graphics workstations.

As a result of the implementation of the C3D file format in different computing hardware environments, C3D files can use three different endian representations, DEC, Intel, and SGI/MIPS, each of which stores integer and floating-point values in byte different order: [big endian, or little endian](#endian). These describe the order in which bytes, representing numbers, are stored. Both the DEC and Intel processors use the little endian method for integer storage where the lowest bytes are stored first while the SGI/MIPS processors use the big endian method. The C3D file endian structure information can be retrieved from the parameter header record at the start of the parameter section.

In addition, the floating-point format storage differs between all three processors. The original floating-point format created by DEC was later modified by Intel and then standardized as the [IEEE-754](https://ieeexplore.ieee.org/document/8766229) format used by Intel and SGI/MIPS processors.

The [IEEE-754](https://ieeexplore.ieee.org/document/8766229) format uses a sign-magnitude representation where the difference
between a positive value (e.g. +1) and its negative value (-1) is the [Most Significant Bit (MSB)](https://en.wikipedia.org/wiki/Bit_numbering#Most-significant_versus_least-significant_bit_first) of the [word](https://en.wikipedia.org/wiki/Word_(computer_architecture)), thus zero can have two values, one positive and one negative. The DEC floating-point format has the same [mantissa](https://en.wikipedia.org/wiki/Significand) with a "hidden 1 bit", offset binary exponent to the left of the [mantissa](https://en.wikipedia.org/wiki/Significand). But when the numbers are negative, the DEC format stores the value as the [2's complement of the positive value](#2s-complement-of-the-positive-value). So there is no negative zero representation, the DEC format only supports one unsigned zero value. All formats need to be supported for compatibility and data exchange.

## Endian

This describes the order in which bytes representing a value are stored in computer memory and is either big or little. Big endian means that most significant value is stored first at the lowest storage address, while little endian stores the least significant value first. Note that within both big endian and little endian byte orders, the individual bits within each byte are always big-endian so bytes are unaffected.

Most [RISC-based](https://en.wikipedia.org/wiki/Reduced_instruction_set_computer) computers and Motorola microprocessors use the big endian approach while Intel processors and DEC processors are usually little endian by default. The C3D format can use both little endian and big endian orders, and applications supporting the C3D format may see either format when a file is opened. The processor type and endian format of a C3D file can be determined by reading the parameter section header record when a file is opened.

Both DEC and Intel processors use the little endian method where the lowest bytes are stored first in memory. MIPS processors use the big endian method, reversing the storage order.

## Floating-point

The C3D format supports a single-precision floating-point format stored in 32 bits ([two words](https://en.wikipedia.org/wiki/Word_(computer_architecture))) in the C3D file. Each C3D file processor type (DEC, SGI/MIPS and Intel) defines a slightly different internal floating-point format. Intel and SGI/MPIS use the [IEEE-754 format](https://ieeexplore.ieee.org/document/8766229), stored in little endian for Intel and big endian format for SGI/MIPS processors.

The DEC floating-point format has the same [mantissa](https://en.wikipedia.org/wiki/Significand) with "hidden 1 bit", an offset
binary exponent to the left of the [mantissa](https://en.wikipedia.org/wiki/Significand), but when the numbers are negative, DEC
stores the value as the [2's complement of the positive value](#2s-complement-of-the-positive-value). This means that the DEC format can only store a zero with no sign associated because, unlike the Intel format, there is no ability to store both positive and negative zero representations.

Floating-point are also referred as [REAL](#real).

## Integer

Many parameters and data values are recorded in the C3D file as 16-bit integer values. In the original C3D implementation, integer values in C3D files were always stored as 16-bit signed integers, that have a the range of –32767 to +32767.

However, the use of signed integers and bytes can reduces the range available for parameter and data storage. As a result, it is common to find unsigned integers and bytes used in many C3D files yielding numerical ranges from 0 to +65535 for unsigned 16-bit integers.

One’s complement signed Integers (–32767 to +32767) remain the default storage
format for analog data and parameters associated with signed analog data.

The C3D format stipulate that the signed integers values are represented as "[One’s complement signed Integers](https://en.wikipedia.org/wiki/Ones%27_complement)". It is very unlikely that this was consistent. Some interpretation problems could arise from this with older files. The ones created using DEC and SIG/MIPS should be the most affected. The ones created with Intel system should be fine. 

## Parameters

The C3D file format defines a method of recording information about, or associated with, the data contained within the file. This information is stored in objects called “parameters” which can be floating-point, signed or unsigned integers and bytes, or [ASCII](#ascii) values. Parameters are kept in collections depending on their use. These collections are called “groups” and every parameter is a member of a group.

Individual parameters have names, and are generally referred to by placing the group name first separated from the parameter name by a colon e.g., GROUP:PARAMETER.

## Raw data

The C3D format considers raw data to be the initial, relatively unprocessed, data sample from the data collection environment. But in a typical 3D photogrammetry environment, the 3D point locations are a result of the motion capture system processing 2D images to calculate a 3D location that is stored in the C3D files as a raw data sample. The C3D format was originally designed to store analog data as raw data binary sample values from an [ADC](#adc) that can be scaled to real-world values.

## REAL

The C3D format supports a single-precision floating-point format stored in 32 bits ([two words](https://en.wikipedia.org/wiki/Word_(computer_architecture))) in the C3D file.

Note that the stored format is affected by the C3D file processor type, DEC, SGI/MIPS, and Intel processors each use a different internal format.

## Records

The sections within a C3D file contain information stored in records. This manual will consistently use the term record to describe a unit of data storage within the C3D format. In this context, the term record should be seen more in the terms of database usage than a file structure.

Thus, all C3D files contain a header record (i.e. the header section), parameter records are stored within the parameter section, and data records (3D and/or analog) are stored within the data section etc.

## Residual

The [3D point residual](../data/3d-points-residuals.md) is generated when the location of the associated [3D point](../data/3d-point.md) recorded in the C3D file is calculated, and records the average accuracy distance of the point, calculated by the photogrammetry software and recorded in [POINT:UNITS](../parameters/required/point/point-units.md) that documents the intersection of the rays used to generate the [3D point](../data/3d-point.md) locations.

Low residual numbers indicate that the [3D point](../data/3d-point.md) locations are more accurate when these numbers are derived from the measured 2D vector data used for the photogrammetry reconstruction and will always be absolute, non-zero values.

Residual values of zero indicate that the point was not directly derived from measurements, i.e. the associated 3D coordinates were estimated by interpolation, affected by filtering, or simulated from a software model. Negative residual values indicate that the stored [3D point](../data/3d-point.md) is probably invalid. It is recommended that all residual calculation methods are fully documented.

## Section

This manual uses the term section to describe the layout of the information within
the C3D file. C3D files are described as being composed of three sections:

- [Header](../c3d-header.md)
- [Parameters](../parameters/c3d-parameter-section.md)
- [Data](../data/c3d-data-section.md)

where each section contains collections of records that store information (parameters, 3D points, analog samples etc.). A section is always at least one, or more, 512-byte blocks in size.

## Trial

A trial is a single motion capture recording while the motion measurement system generates 3D coordinates from specific locations on, or associated with, a subject or subjects. During the 3D data collection additional analog sensors such as force plates, electromyography systems, and accelerometers, generate detailed information related to the subject(s) that must be synchronized temporally with the 3D data samples that record the sampled 3D locations. A typical motion collection session consists of multiple trials recorded under identical conditions.

## UTF-8

Encoding user entered text in [UTF-8](https://en.wikipedia.org/wiki/UTF-8) offers a few advantages over the traditional [ASCII](#ascii) characters expected in a C3D file. The [ASCII](#ascii) character set only supports the Latin alphabet, while [UTF-8](https://en.wikipedia.org/wiki/UTF-8) supports Chinese, Japanese, Hebrew, Arabic, etc., thus [UTF-8](https://en.wikipedia.org/wiki/UTF-8) support makes the C3D file format universally accessible.

[UTF-8](https://en.wikipedia.org/wiki/UTF-8) can encode each of the 1,112,064 valid code points in the Unicode code space while remaining backwards compatible with the [ASCII](#ascii) character set used in the original C3D format definition. Therefore, any application that supports [UTF-8](https://en.wikipedia.org/wiki/UTF-8) will be able to read all C3D files created since the early 1980’s.

All C3D group and parameter names must use 7-bit [ASCII](#ascii) characters to preserve the universally defined C3D format structure. [UTF-8](https://en.wikipedia.org/wiki/UTF-8) is permitted in the individual group and parameter descriptions which may be created in local character sets for localization support. Note that most C3D parameter string lengths are limited to 255 8-bit characters in length.