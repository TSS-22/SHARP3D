# Header Section

The first 512-bytes block at the beginning of every C3D file is referred as the Header. It is partially composed of copy of values from the Parameter section, in order for applications to be able to read C3D file 3D Point Data quickly.

## Structure

| Word(s)   | Data Type               | Description                                                                                     |
|-----------|-------------------------|-------------------------------------------------------------------------------------------------|
| 1         | Byte 1: uint8, Byte 2: char | Byte 1: Number of 512-byte blocks to Parameter Section + 1.<br>Byte 2: Data storage format flag. |
| 2         | uint16                  | Number of markers stored in each Data Frame.                                                    |
| 3         | uint16                  | Total number of analog samples per [Data Frame](./data/c3d-data-section.md#data-frame-structure).                                                  |
| 4         | uint16                  | First frame number of raw data (not used/misleading).                                           |
| 5         | uint16                  | Last frame number of raw data (not used/misleading).                                            |
| 6         | uint16                  | Maximum 3D frame interpolation gap.                                                             |
| 7-8       | float32                 | Data Scale factor.                                                                              |
| 9         | uint16                  | Number of 512-byte blocks to the Data Section + 1.                                              |
| 10        | uint16                  | [Analog Frames](./data/analog.md) per [Data Frame](./data/c3d-data-section.md#data-frame-structure).                                                                   |
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

## Structure Details

> All values, aside from the [Word 1](#word-1-pointer-to-parameter-section-and-file-format) depends on the [file endian format](./parameters/c3d-parameter-section.md). Its value need to be determined first, to allow for the interpretation of any Int16, Uint16 or Float32.

the C3D header contains a number of areas that are marked as "Currently not used". Any application that copies, or edits a C3D file, must preserve these areas to guarantee future compatibility while all applications creating new C3D files must set these values as zero (0x00h).

### Word 1: Pointer to Parameter Section and File Format

The first word in the C3D file must be read as two bytes, so that the values are unaffected by the C3D file endian format.

#### First byte

The first byte of the header gives the position of the First [Parameter Section](./parameters/c3d-parameter-section.md) block, with unit being 512-bytes blocks; Header Section counted as block 1. More simply, it is the number + 1 of 512-bytes block from the beginning of the file to the [Parameter Section](./parameters/c3d-parameter-section.md) first block.

#### Second byte

The second byte in the C3D file is an identification allowing applications to verify the [Data Section](./data/c3d-data-section.md) format. This byte is usually set as 0x50 (ASCII: ‘P’, decimal: 80) when a C3D file uses the standard Frame setup, with a Frame being composed of:
- 3D Point Data
- Synchronized Analogs Data

> In practice, most if not all C3D files follow this standard. But this does not mean that this byte will be set as 0x50 nonetheless.

### Word 2: Number of 3D Markers/Trajectories per Data Frame

It records the number of trajectories stored in each frame of the file as 3D points. this is a copy of the [POINT:USED parameter](./parameters/required/point/point-used.md). 

### Word 3: Total Number of Analog Samples per Data Frames

It contains the total number of analog samples stored with each data frame in the file.  If the third word is zero then the C3D file contains 3D Point samples but does not contain any analog data samples. It is obtained by multiplying the number of Analog Frames in each Data Frames by the number of Analog Channels recorded.

If the C3D file does not contain any analog data then the value of Word 3 will be zero.

### Word 4: First Frame Number of the Raw Data

It contains the number of the first frame of the raw data used to create the C3D file. It does not represent the number of the first frame of the C3D file, as not all the frames from the raw data might have been used to create the C3D file.

> This is a confusing field and is not used in practice due to this, and the fact that its usefulness is "limited". It is not used to read C3D file.

This parameter was originally included for reference only.

Applications occasionally attempt to determine the number of frame in a C3D file by subtracting the Header First Frame number from the [End Frame Number](#word-5-end-frame-number-of-the-raw-data), so these values may need to be maintained when C3D files are created to maintain compatibility with older applications.

### Word 5: End Frame Number of the Raw Data

It contains the number of the End frame of the raw data used to create the C3D file. It does not represent the number of the End frame of the C3D file, as not all the frames from the raw data might have been used to create the C3D file.

> This is a confusing field and is not used in practice due to this, and the fact that its usefulness is "limited". It is not used to read C3D file.

This parameter was originally included for reference only.

Applications occasionally attempt to determine the number of frame in a C3D file by subtracting the Header [First Frame number](#word-4-first-frame-number-of-the-raw-data) from the End Frame Number, so these values may need to be maintained when C3D files are created to maintain compatibility with older applications.

### Word 6: Maximum Interpolation Gap

It stores the maximum interpolation gap length for 3D Point Data. 

The use of this item is not specified in the C3D file description although the maximum interpolation gap length value is usually set to indicate the maximum length of potentially invalid 3D point data samples (in frames) over which 3D point interpolation may be or have been performed.  This may be used by various applications to specify the length of gaps that can be interpolated or gap filled when reading or creating a C3D file.

> The interpretation of the maximum interpolation gap header word is application dependent and its value is normally set when the C3D file is created. Ask your vendor for their definition and use of this item.

> It does not indicate that any 3D data points have been interpolated.

Any application reading the C3D file may override this value and interpolate gaps of any length if desired and record the maximum interpolation length by updating this value.

### Word 7-8: 3D Sacle Factor

It contain the [3D Scale Factor](./parameters/required/point/point-scale_factor.md) value.

This parameter is required when 3D data values are stored using the standard signed integer format because it is used to scale each of the stored 3D point and residual values from signed integer values to physical world values.

When 3D data is stored as scaled floating-point values, it is used to scale the 3D residuals which are recorded as integers.

> Always calculate a valid 3D scale factor.

The sign of the [3D Scale Factor](./parameters/required/point/point-scale_factor.md) is used to determine the 3D point and analog data storage format: 
- Negative Scale Factor: Float32
- Positive Scale Factor: Int16

### Word 9: Pointer to Data Section

It gives the position of the First [Data Section](./data/c3d-data-section.md) block, with unit being 512-bytes blocks; Header Section counted as block 1. More simply, it is the number + 1 of 512-bytes block from the beginning of the file to the [Data Section](./data/c3d-data-section.md) first block.

### Word 10: Number of Analog Frame per Data Frame

It gives the number of Analog Frame per Data Frame. The C3D structure for 3D Point and Analog Data assumes that each Data Frame can have one 3D Point Frame and one or more Analog Frame from each analog channel sampled. Thus this value is the actual analog sample rate measured and recorded in terms of Analog Frame per 3D Point Frame.

While this means that C3D files can only contain data sampled at integer multiples of the 3D frame rate, it means that data storage synchronization is guaranteed and makes it easy to calculate the size and location of individual 3D data frames and their associated analog samples within the C3D file.

If the C3D file does not contain any analog data then the value of Word 10 will be zero.

### Word 11-12: 3D Point Data Acquisition Rate

It is the acquisition rate used to acquire the 3D Point Data, in Hertz. This is a copy of the  [POINT:RATE Parameter](./parameters/required/point/point-rate.md). 

The 3D frame rate parameter is a floating-point value, making it possible to accurately record the 3D frame rate for video based
sampling systems

### Word 13-149: Unused

C3D file header words 13 – 149 are currently not used and may provide additional expansion features in the future. Applications that create new C3D files should fill these words with 0x00h, while applications that copy or edit C3D files must preserve the contents of these words.

### Word 150: Supported Header Event Label Length 

Determine if the Header Event Labels are 2 or 4 ASCII characters long. A value of 0x3039 (decimal: 12345) indicate that the file store the Header Event Labels as 4 ASCII character. Any other value indicate that the file store the Header Event Labels as 2 ASCII character.

>The presence of the 0x3039 only indicates that the C3D file supports labels with four characters – it does not indicate that any events are actually stored

### Word 151: Number of Header Events

The number of Header Events. The value needs to be in the range [0, 18]. 

Value of 0 indicate that no Header Events are stored, value of 1 to 18 indicating that Header Events are present.

### Word 152: Not used

C3D file header word 152 is currently not used and may provides additional expansion features in the future. Applications that create new C3D files should fill this word with 0x00h, while applications that copy or edit C3D files must preserve the contents of this words.

### Words 153-188: Header Events Time

Store the value of the Header Events time at which they happens, in seconds. Each Header Event time is recorded as the number of seconds and fractions of a second that have elapsed since the first 3D Point Frame recorded, designated as 0.0s.

### Words 189-197:

Store the Header Event Flag.

>0x00 = OFF, 0x01 = ON

THERE ARE CONTRADICTING INFORMATIONS FROM THE [C3D USER GUIDE](https://www.c3d.org/docs/C3D_User_Guide.pdf). THE VALUE ADVERTISED HERE ARE THE LOGICAL VALUE FROM HOW LOGIC IS BEING PROCESSED IN COMPUTER SCIENCE AND FROM SOME OF [SAMPLE FILES](https://www.c3d.org/sampledata.html) PROVIDED BY THE C3D WEBSITE.

The ON/OFF status of the event may be interpreted in any convenient way. There is no formal convention for the interpretation or use of the event status. Events are valid within the C3D file regardless of their actual status.

The Header Event being valid whatever his Flag value is, we recommend to not think too hard about it unless you have very specific reasons for it.

### Word 198: Not used

C3D file header word 198 is currently not used and may provides additional expansion features in the future. Applications that create new C3D files should fill this word with 0x00h, while applications that copy or edit C3D files must preserve the contents of this words.

### Word 199 - 234: Header Event Labels

Store the Header Event Labels. The labels are either 2 or 4 ASCII characters length, depending on the [value of Word 150](#word-150-supported-header-event-label-length). 

> Event labels should always use 7-bit ASCII characters (a-Z, 0-9 and space)

>Header Event Labels each needs to be UNIQUE.

In the occurence the Header Event Labels length is less than the maximum length allowed, it needs to be padded with a "space" character (hexadecimal: 0x20, decimal: 32).  

### Word 235-256: Unused

C3D file header words 13 – 149 are currently not used and may provide additional expansion features in the future. Applications that create new C3D files should fill these words with 0x00h, while applications that copy or edit C3D files must preserve the contents of these words.

## Header Events

Header events were added to the C3D format to allow applications to record timing information, relevant to the recorded data, for gait analysis – typically recording events like left/right heel-contact and toe-off information as the subject walked across one or more force plates. This feature ensures that gait analysis and other data processing programs perform their calculations in a repeatable manner using the event times to determine the gait cycle data analysis timing.

A maximum of eighteen (18) of these events can be stored in the C3D header record, each header event has an on/off status flag that can be used by applications to control the display of the event position when the C3D file is processed.

Most C3D files store large numbers of events in the parameter block, thus freeing up the header events for storing specific events that have special significance, e.g. recording the gait cycle that has been selected for analysis when a file contains multiple gait cycles.

>Events can also be independently stored in the EVENT group in the
parameter section of the C3D file – parameter events and header events are independently recorded and there is no requirement that they duplicate each other.

Modern C3D files tend to use the EVENT group but the header event storage feature continues to be valid and may contain unique or duplicate events. Applications should always interpret and process the header events as well as the optional EVENT Parameter Group if it exists.

### Stored order

The Header Events are stored as an unordered list that can be indexed directly by the event count stored in header word 151. Events are always added to the end of the list: if one or more events are deleted from the middle of the list then all higher index events (together with their labels and status flags) are moved down to fill the empty space. 

>Events may be stored in the list in any order so long as the event time, label and status are indexed correctly by the event count in header Word 151.

## Notes

- Applications that create or modify C3D files must always ensure that the C3D header section contains the identical copies of the values stored in the [Parameter Section](./parameters/c3d-parameter-section.md). A C3D file is corrupted if there is a discrepancy between Header Section values and Parameter Section values for the same items.