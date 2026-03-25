# C3D File structure

## Header

The header are a 512-bytes block present at the beginning of each C3D Files.

| Word(s)   | Data Type               | Description                                                                                     |
|-----------|-------------------------|-------------------------------------------------------------------------------------------------|
| 1         | Byte 1: uint8, Byte 2: char | Byte 1: Number of 512-byte blocks to Parameter Section + 1.<br>Byte 2: Data storage format flag. |
| 2         | uint16                  | Number of markers stored in each Data Frame.                                                    |
| 3         | uint16                  | Total number of analog samples per Data Frame.                                                  |
| 4         | uint16                  | First frame number of raw data (not used/misleading).                                           |
| 5         | uint16                  | Last frame number of raw data (not used/misleading).                                            |
| 6         | uint16                  | Maximum 3D frame interpolation gap.                                                             |
| 7-8       | float32                 | Data Scale factor.                                                                              |
| 9         | uint16                  | Number of 512-byte blocks to the Data Section + 1.                                              |
| 10        | uint16                  | Analog Frames per Data Frame.                                                                   |
| 11-12     | float32                 | 3D Point Data acquisition rate in Hertz.                                                        |
| 13-149    | —                       | Not used.                                                                                       |
| 150       | uint16                  | Indicates support for 2 or 4-character Header Event labels.                                    |
| 151       | uint16                  | Number of Header Events (0-18).                                                                 |
| 152       | —                       | Not used.                                                                                       |
| 153-188   | float32                 | Header Event times in seconds.                                                                  |
| 189-197   | uint8                   | Header Event flag (0x00 = ON, 0x01 = OFF).                                                       |
| 198       | —                       | Not used.                                                                                       |
| 199-234   | ASCII                   | Header Event labels (2 or 4 characters, depending on Word 150).                                 |
| 235-256   | —                       | Not used.                                                                                       |

## Parameter		

All [Parameter Section](./c3d-parameter-section.md) start with the following 4 bytes:

| Byte | Description                                                                                     |
|------|-------------------------------------------------------------------------------------------------|
| 1    | Reserved and unused.                                                                             |
| 2    | Reserved and unused.                                                                             |
| 3    | Number of 512-byte blocks composing the Parameter Section.                                      |
| 4    | Processor type:<br>- 0x54: Intel<br>- 0x55: DEC (VAX, PDP-11)<br>- 0x56: MIPS (SGI/MIPS)          |

Then follows the Parameter Structures, listed in a random order. They can be either Groups or Parameters. Groups regroup the Parameters inside them.

### Group

| Position (byte) | Length (bytes) | Type            | Description                                                                                     |
|-----------------|----------------|-----------------|-------------------------------------------------------------------------------------------------|
| 1               | 1              | Signed byte     | Number of characters in the Group name (1-127). If negative, the Group is advertised as locked.              |
| 2               | 1              | Signed byte     | The Group Id (-1 to -127). Always negative.                                                      |
| 3               | n              | ASCII           | Group name. Only uppercase A-Z, 0-9, and "_" are supported.                                     |
| 3 + n           | 2              | Unsigned int    | Number of bytes till the next Parameter Structure (starting at position 3+n, includes pointer).|
| 3 + n + 2       | 1              | Unsigned byte   | Number of bytes in the Group description.                                                       |
| 3 + n + 3       | m              | UTF-8           | Group description.                                                                              |


### Parameter

| Position (byte)       | Length (bytes) | Type            | Description                                                                                     |
|-----------------------|----------------|-----------------|-------------------------------------------------------------------------------------------------|
| 1                     | 1              | Signed byte     | Number of characters in the Parameter name (1-127). If negative, the Group is advertised as locked.              |
| 2                     | 1              | Signed byte     | The Parameter Id (1 to 127). Always positive.                                                      |
| 3                     | n              | ASCII           | Parameter name. Only uppercase A-Z, 0-9, and "_" are supported.                                     |
| 3 + n                 | 2              | Unsigned int    | Number of bytes till the next Parameter Structure (starting at position 3+n, includes pointer).|
| 3 + n                 | 2              | Unsigned int    | Number of bytes till the next Parameter Structure (starting at position 3+n, includes pointer).|
| 3 + n + 2             | 1              | Unsigned byte   | Length in bytes of each data element:<br>- -1: Char<br>- 1: Byte<br>- 2: Int16<br>- 4: Float32   |
| 3 + n + 3             | 1              | Unsigned byte   | Number of dimensions of the Parameter Data. 0 for scalar.                                       |
| 3 + n + 4             | d              | Unsigned byte   | Length of each Parameter Data dimension.                                                        |
| 3 + n + 4 + d         | t              | -               | Parameter Data.                                                                                 |
| 3 + n + 4 + d + t     | 1              | Unsigned byte   | Number of bytes in the Parameter description.                                                       |
| 3 + n + 4 + d + t + 1 | m              | UTF-8           | Parameter description.     

## Data

The Data Section is made of Frames that follow each other. Each Frame is made of two parts:

	1. (3D) Points Frame
	2. Analogs Frame

### 3D Points

The 3D Points values are either Signed Int16 or Float32 dependings on the Scale Factor value (Header:Word 7-8).

#### Int16 Version

| Length (bytes) | Type   | Description                                                                                     |
|----------------|--------|-------------------------------------------------------------------------------------------------|
| 2              | Signed Int16  | X value divided by SCALE:FACTOR.                                                                |
| 2              | Signed Int16  | Y value divided by SCALE:FACTOR.                                                                |
| 2              | Signed Int16  | Z value divided by SCALE:FACTOR.                                                                |
| 2              | Signed Int16  | Byte 1: `abcd efgh`.<br>`a`: Residual sign.<br>`b-g`: Camera Mask.<br>Byte 2: Average residual divided by SCALE:FACTOR. |


#### Float32 Version

| Length (bytes) | Type   | Description                                                                                     |
|----------------|--------|-------------------------------------------------------------------------------------------------|
| 4              | Float32| X value.                                                                |
| 4              | Float32| Y value.                                                                |
| 4              | Float32| Z value.                                                                |
| 4              | Float32| After converting its value to Signed Int16. Byte 1: `abcd efgh`.<br>`a`: Residual sign.<br>`b-g`: Camera Mask.<br>Byte 2: Average residual divided by SCALE:FACTOR. |

### Analogs

Each Analog section of a Data Frames is made of NumberOfAnalogChanels \* AnalogFramesPerDataFrames.

The Analog values are either Signed Int16 or Float32 dependings on the Scale Factor value (Header:Word 7-8).