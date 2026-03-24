# Header

The first 512-bytes block at the beginning of every C3D file is referred as the Header. It is partially composed of copy of values from the Parameter section, in order for applications to be able to read C3D file 3D Point Data quickly.

## Structure

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

## Details

the C3D header contains a number of areas that are marked as "Currently not used". Any application that copies, or edits a C3D file, must preserve these areas to guarantee future compatibility while all applications creating new C3D files must set these values as zero (0x00h).

### Word 1

The first word in the C3D file must be read as two bytes, so that the values are unaffected by the C3D file endian format.

#### First byte

The first byte of the header gives the position, with unit being 512-bytes block, of the First Parameter Section block. Or more simply, the number + 1 of 512-bytes block from the beginning of the file to the Parameter Section first block.

#### Second byte

The second byte in the C3D file is an identification allowing applications to verify the data section format. This byte is usually set as 0x50 (ASCII: ‘P’, decimal: 80) when a C3D file uses the standard Frame setup, with a Frame being composed of:
- 3D Point Data
- Synchronized Analogs Data

> In practice, most if not all C3D files follow this standard. But this does not mean that this byte will be set as 0x50 nonetheless.

