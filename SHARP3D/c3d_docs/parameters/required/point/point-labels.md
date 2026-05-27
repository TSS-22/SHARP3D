# POINT:LABELS

- **Type**: Required

- **Locked**: False

> The POINT:LABELS name must be 7-bit ASCII. UTF-8 encoding is only permitted in the stored array values. (sic. This is unclear.)

By convention, the LABELS array values are usually four characters of upper-case ASCII text (A-Z, underscore, and 0-9) although longer labels and UTF-8 encoding are permitted. The original C3D file format defined the POINT:LABELS parameter as a character data array that consisted of one unique four-character ASCII value for each 3D data point contained within the C3D file.

Each label is referred to as the point label and is used to provide a unique reference each 3D point contained within the C3D file data section. This allows software applications to identify and process data based on the unique label identification.

> Unless the C3D file contains several hundred valid points in each frame of data **the POINT:LABELS strings should not normally exceed 16 characters**, **its function is to provide a unique point identification, not a description**.

The purpose of the LABELS parameter is to allow applications reading data from the C3D file to search for a specific 3D point or trajectory by referencing its LABELS value instead of looking for a specific trajectory number in a fixed list of trajectories. This allows applications to be written in a general manner so that they can process data by reference. For example, calculate the mouvement of a participant pelvis, by assessing the direction of progression from the 3D points identified as the defining standard biomechanical landmarksuch of the pelvis: LASI, RASI and SACR. An application written in this way will work in any environment, as it does not require that the 3D data is stored in any specific order within the C3D file.

Note that while the labels stored in POINT:LABELS are typically four upper case characters, many applications may create labels with more characters. When longer labels are used **it is recommended that the first six characters of each label are unique**.

> Always create labels that are unique and easy to read, e.g. LASI and RASI. Do not create labels with names like MARKER0001, MARKER0002 etc.

Individual labels must always be unique to identify each point in the file but there is no need to make them excessively descriptive as the [POINT:DESCRIPTIONS](./point-descriptions.md) parameter is provided for human intelligible descriptions. It is recommended that POINT:LABELS are always no more than 16 characters in length.

> Labels must descriptive but not a description.

Note that a C3D file may contain more or less than the number of trajectories described by this parameter.  The parameter [POINT:USED](./point-used.md) determine the actual number of trajectories stored in the 3D Point data section. If the C3D file contains more trajectories than are described by POINT:LABELS parameters, then the additional trajectories must be either referenced by number or can be defined by creating additional POINT:LABELS. Those additional POINT:LABELS parameters must be named POINT:LABELSX, X being an integer from 2 onward. For example POINT:LABELS2 and POINT:LABELS3, each supporting up to an additional 255 labels. These new POINT:LABELS parameters must still name trajectories in regard to their index in the [POINT:USED](./point-used.md) parameter.

> 3D data points are stored in the [3D Point data section](../../../data/3d-point.md) in the same order recorded in the POINT:LABELS parameter.

It is strongly recommended that the POINT:LABELS used are consistent within any set of data files collected for a specific analysis environment to ease subsequent data analysis and processing. This parameter is not normally locked and may be edited if necessary. Editing any of the labels only changes the ASCII reference that identifies a specific trajectory and does not affect the C3D file structure.