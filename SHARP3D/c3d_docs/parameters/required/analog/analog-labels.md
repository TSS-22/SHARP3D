# ANALOG:LABELS

- **Type**: [Required](../../required.md)

- **Locked**: True

The C3D file format defines the ANALOG:LABELS parameter as a character data array that consists of a unique four-character ASCII (A-Z, 0-9) string for each analog channel contained within the C3D file. This is referred to as the analog channel label and is used to reference each channel of data contained within the C3D file data section in the order in which the channels are stored. Labels are typically 4-16 characters long (4 upper case characters is the default).

The purpose of the LABELS parameter is to allow applications to search for a specific channel of data by referencing its LABELS value instead of looking for a specific analog channel number. This allows applications to be written in a general manner so that they can process data by reference e.g. Analyze all the EMG channels where they are identified as channels EM01 through EM32. An application written in this way will work in any environment, as it does not require that the EMG signals be stored on specific numbered ADC channels.

Note that while ANALOG:LABELS are typically four upper case characters, many applications may create labels that are longer and contain upper and lower case characters. The LABELS parameter is defined to allow software applications to uniquely identify analog channels, as such each label must be unique and need not be descriptive. The ANALOG:DESCRIPTIONS parameter is available for documentation, so creating ANALOG:LABELS like “Moment.Mz1” and “Voltage.Right Rectus Femoris” is a waste of space when “MZ1” and “RRF” are all that an application needs to identify individual channels.

Note that a C3D file may contain more or less analog channels than described by this parameter. If the C3D file contains more analog channels than are described by ANALOG:LABELS parameters then the additional analog channels must be referenced by the channel index number

## Disclaimer

Users occasionally create files with different numbers of [LABELS](./analog-labels.md) and [DESCRIPTIONS](./analog-descriptions.md) parameters. This can create unwanted behavior using SHARP3D or any other C3D tool in general due to the inconsistencies it produce. **SHARP3D consider this a malpractice**. There should always be a one to one relationship between the number of [LABELS](./analog-labels.md), the number of [DESCRIPTIONS](./analog-descriptions.md), and [POINT:USED](./analog-used.md). 