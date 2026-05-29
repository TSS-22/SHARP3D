# FORCE_PLATFORM:TYPE

- **Type**: [Required](../../required.md)

- **Locked**: False

> Anyone can define a new force plate type to handle a specific configuration. New force plate types must be documented to allow users to read and translate the data stored in the C3D file. [Please fill an issue or contact us to add your force plate definition to SHARP3D supported type](https://github.com/TSS-22/SHARP3D/issues/new).

The FORCE_PLATFORM:TYPE parameter is an array of signed integers that define the type of force platform output expected from each force platform. The TYPE array size must be equal to or greater than the value of the [FORCE_PLATFORM:USED](./force_platform-used.md) parameter, so at least all the used force plate are defined. Initially, the C3D specification supported three force platform types ([Type-1](#type-1), [Type-2](#type-2) and [Type-3](#type-3)), with the [Type-4](#type-4) platform added in the early 90’s to support the inclusion of the full force plate calibration matrix. Over the years since, various manufacturers have occasionally created additional force platform descriptions to define specific force data collection environments but these are not commonly seen.

The analog data from each force platform is stored in the associated analog channels defined by the [FORCE_PLATFORM:CHANNEL](./force_platform-channel.md) parameter. The data stored from each force plate channel is scaled by the [ANALOG:SCALE](../analog/analog-scale.md) parameter. **The default storage method should be to store the unprocessed analog samples from each force plate channel in the associated analog channel**. These values are then scaled using the associated floating-point [ANALOG:SCALE](../analog/analog-scale.md) or [CAL_MATRIX](../../additional/force_platform/force_platform-cal_matrix.md) parameters, which prevents data corruption if the C3D file format is ever changed from floating-point to integer.

Starting in 2007, one manufacturer started storing pre-scaled force plate data in floating-point formatted C3D files, an approach that has since been used by other manufacturers, resulting in much larger file sizes that require more processing power. The pre-scaled processed data is defined as force plate [TYPE-2](#type-2) data with the calculated force and moment data stored in the analog channels defined by the [FORCE_PLATFORM:CHANNEL](./force_platform-channel.md) parameter. The relevant [ANALOG:SCALE](../analog/analog-scale.md) parameters set to a value of 1.00, indicating that the data has already been scaled by the [Nexus software](https://www.vicon.com/software/nexus/) and can be interpreted directly as three forces ($F_x$, $F_y$ and $F_z$) and three moments ($M_x$, $M_y$ and $M_z$).

While this scheme relieves the end-user of the problems of calculating and applying the [SCALE](../analog/analog-scale.md) or [CAL_MATRIX](../../additional/force_platform/force_platform-cal_matrix.md) parameters to the data, it eliminates the ability to review the raw force plate signals in the event of any problems with the force plate. As a result end-users have no way of verifying the data collection conditions or the correct force plate scaling factors during any future review or processing of the force data.

This decision means that when pre-scaled data is stored using the floating-point format with the relevant [ANALOG:SCALE](../analog/analog-scale.md) parameters set to a value of 1.00, the C3D file cannot be converted to the integer format without rescaling the force plate data. This is because integer overflow can occur as the stored force plate data (especially the $M_x$ and $M_y$ moments) can easily exceed the 16-bit integer storage range when the force plate details and scales are not stored in the C3D file. This essentially defeats one of the major features of the C3D format. This is not a result of using floating-point storage but because the application has failed to record the scaling values.
 
An addition effect of storing pre-scaled force data is that the stored values appear to be very accurate (typically storing data values with calculated submicron resolutions) although the actual measurement accuracy does not match the recorded results.

## TYPE-1

The force platform outputs ($F_x$, $F_y$, and $F_z$) are recorded in the first three channels, the locations of the center of pressure ($P_x$, $P_y$) in the next two channels and the free moment about the Z-axis ($M_z$) in the sixth channel. The recommended parameter [ANALOG:LABELs](../analog/analog-labels.md) and [ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) are shown below:

| [ANALOG:LABELs](../analog/analog-labels.md) | [ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) |
| --- | --- |
| nFX | FP<sub>n</sub> $F_x$ force |
| nFY | FP<sub>n</sub> $F_y$ force |
| nFZ | FP<sub>n</sub> $F_z$ force |
| nPX | FP<sub>n</sub> $X$ center of pressure |
| nPY | FP<sub>n</sub> $Y$ center of pressure |
| nMZ | FP<sub>n</sub> $Z$ moment |

If multiple force plates are used, it is important to identify the channels for each plate with the force plate number shown as $n$ in the parameters documented here.

## TYPE-2

The force platform outputs ($F_x$, $F_y$, and $F_z$) are recorded in the first three channels, and the
moments ($M_x$, $M_y$, $M_z$) in the next three channels. This is an arrangement typical
for many [AMTI](https://www.amti.biz/product-line/force-plates/) and [Bertec](https://www.bertec.com/products/force-plates) force plates. The recommended [ANALOG:LABELS](../analog/analog-labels.md) and
[ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) are shown below: 

| [ANALOG:LABELS](../analog/analog-labels.md) | [ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) |
| --- | --- |
| nFX | FP<sub>n</sub> $F_x$ force |
| nFY | FP<sub>n</sub> $F_y$ force |
| nFZ | FP<sub>n</sub> $F_z$ force |
| nMX | FP<sub>n</sub> $M_x$ moment |
| nMY | FP<sub>n</sub> $M_y$ moment |
| nMZ | FP<sub>n</sub> $M_z$ moment |

If multiple force plates are used, it is important to identify the channels for each plate with the force plate number shown as $n$ in the parameters documented here.

when multiple force plates are used, identify each plate with a number ensures that each [ANALOG:LABELS](../analog/analog-labels.md) is unique and make the individual data channels easy to identify. Providing a unique channel [LABELS](../analog/analog-labels.md) and [DESCRIPTIONS](../analog/analog-descriptions.md) parameter takes very little effort when compared to the amount of time that can be spent attempting to identify individual force plate configuration and scaling issues at any time in the future. It is much easier to look at an analog channel identified as 5MY than a channel labeled A047.

It is common to see processed force and moment data stored using a TYPE-2 description in the channels defined by the [FORCE_PLATFORM:CHANNEL](./force_platform-channel.md) parameter.

## TYPE-3

The force platform has eight analog outputs, which are combinations of the $X$, $Y$, and $Z$ forces measured at each of the corners of the force platform, an arrangement typical of [Kistler](https://www.kistler.com/US/en/force-plate/C00000113) force plates.

It is recommended that each analog channel signal is identified with a unique [ANALOG:LABELS](../analog/analog-labels.md) and ANALOG:DESCRIPTION to store information in each C3D file that documents the file contents. Typical [Kistler](https://www.kistler.com/US/en/force-plate/C00000113) specific [ANALOG:LABELS](../analog/analog-labels.md) and [ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) are shown below:

| [ANALOG:LABELS](../analog/analog-labels.md) | [ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) |
| --- | --- |
| nFX12 | FP<sub>n</sub> $F_x$ force 1,2 |
| nFX34 | FP<sub>n</sub> $F_x$ force 3,4 |
| nFY14 | FP<sub>n</sub> $F_y$ force 1,4 |
| nFY23 | FP<sub>n</sub> $F_y$ force 2,3 |
| nFZ1 | FP<sub>n</sub> $F_z$ force 1 |
| nFZ2 | FP<sub>n</sub> $F_z$ force 2 |
| nFZ3 | FP<sub>n</sub> $F_z$ force 3 |
| nFZ4 | FP<sub>n</sub> $F_z$ force 4 |

If multiple force plates are used, it is important to identify the channels for each plate with the force plate number shown as $n$ in the parameters documented here.

When multiple force plates are used, identify each plate with a number to ensure that each [ANALOG:LABELS](../analog/analog-labels.md) is unique. Correctly identifying each force plate channel takes very little effort when compared to the amount of time that can be spent attempting to discover force plate configuration and scaling issues in data.

## TYPE-4

This force platform is the same as a [TYPE-2](#type-2) force platform except that a full calibration matrix is being provided via the [CAL_MATRIX](../../additional/force_platform/force_platform-cal_matrix.md) parameter which includes full crosstalk scaling. For a TYPE-4 force plate the individual channel [SCALE](../analog/analog-scale.md) parameters should convert the analog data to volts only because the calibration matrix is applied in an additional step to convert volts to force and moment units. 

> Do
not use a TYPE-4 force plate type unless the force plate manufacturer provides a complete crosstalk correction matrix with scaling values for all matrix entries. If the supplied matrix only contains the main diagonal elements then define the force plate as a [TYPE-2](#type-2) and store the individual scale values for the analog channels.

The recommended [ANALOG:LABELS](../analog/analog-labels.md) and [ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) values are identical to a [TYPE-2](#type-2) force plate and are shown below:

| [ANALOG:LABELS](../analog/analog-labels.md) | [ANALOG:DESCRIPTIONS](../analog/analog-descriptions.md) |
| --- | --- |
| nFX | FP<sub>n</sub> $F_x$ force |
| nFY | FP<sub>n</sub> $F_y$ force |
| nFZ | FP<sub>n</sub> $F_z$ force |
| nMX | FP<sub>n</sub> $M_x$ moment |
| nMY | FP<sub>n</sub> $M_y$ moment |
| nMZ | FP<sub>n</sub> $M_z$ moment |


When multiple force plates are used,
identify each plate with a number to ensure that each [ANALOG:LABELS](../analog/analog-labels.md) is unique. Correctly identifying each channel takes very little effort when compared to the amount of time that can be spent attempting to discover force plate configuration and scaling issues in data.

> Note that some applications may not recognize TYPE-4 plates correctly. These applications will usually work correctly by specifying the [FORCE_PLATFORM:TYPE](./force_platform-type.md) as a [TYPE-2](#type-2) plate and editing the associated [ANALOG:SCALE](../analog/analog-scale.md) parameters. If in doubt, consult your application and force plate vendor, but note that defining a force plate as a TYPE-4 plate when the manufacturer has not provided a full crosstalk matrix does not improve accuracy and adds a needless complication to force measurements.