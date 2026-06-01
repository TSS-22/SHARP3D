# Application Parameters

It is common to find additional parameters in C3D files besides the basic parameters that describe the data contained within the file. Additional C3D parameters do not exist in every file because they normally record values that are either associated with the data stored in the file (subject weight, leg length etc.,) or have been added by an application that has processed the data. Many application parameters are often created by users or programmers to solve an issue and provide information needed in particular circumstances however the author may be working in unique environment fail to considered problems that may be created by their additions to the C3D file.

While all parameters reserve space in each parameter header for a description, it is not uncommon to find manufacturers and users creating parameters without any documentation which can lead to confusion. Since applications change over time, many application specific parameters exist that are not documented here, while some of the parameters described may be obsolete or non-functional.

## Overview

While the parameters described in this chapter are optional, all applications that read and write C3D files should preserve all the parameters that are found when the C3D file is initially opened, regardless of any application specific processing, and then write the original parameters to the new file to preserve information stored in the original C3D file.

> Ask your C3D application vendor for documentation of any C3D parameters that they have created.

This chapter describes some of the many application parameters found in C3D files and provides some comments about them. The list here is not exhaustive but simply a selection to demonstrate the flexibility of the C3D parameters and describe some of the more common manufacturer or hardware specific parameters that exist. Documentation on the precise use of application specific C3D parameters is usually available on request from the software application developer or hardware manufacturer who added the additional parameters to their files.

The intention here is document some additional parameters but not to single out specific manufacturers or application developers for praise or criticism. These are simply examples of parameters that demonstrate the good and bad points in the choice of implementation, or documentation etc. Please read this section as a brief background to the art of group and parameter creation.

The information presented in this chapter is based on the examination of C3D files from various manufacturers C3D applications. As a result, any questions regarding the exact interpretation of the parameters and groups described, need to be resolved by the application manufacturer who created the additional parameters in your C3D files. It is recommended that any manufacturer that creates application specific C3D parameters and groups provides public documentation to assist all users interpreting and accessing the information. Basic information about each item should always be recorded in each parameter and group description entries.

## Application Parameters List

### ANALOG

- [ANALOG:GAIN](./application/analog/analog-gain.md)

### ANALYSIS

No specific parameter provided by the C3D User Guide.

### MANUFACTURER

- [MANUFACTURER:COMPANY](./application/manufacturer/manufacturer-company.md)
- [MANUFACTURER:EDITED](./application/manufacturer/manufacturer-edited.md)
- [MANUFACTURER:SOFTWARE](./application/manufacturer/manufacturer-software.md)
- [MANUFACTURER:VERSION](./application/manufacturer/manufacturer-version.md)
- [MANUFACTURER:VERSION_label](./application/manufacturer/manufacturer-version_label.md)

### POINT

- [POINT:X_SCREEN](./application/point/point-x_screen.md)
- [POINT:Y_SCREEN](./application/point/point-y_screen.md)

### SEG

- [SEG:ACC_FACTOR](./application/seg/seg-acc_factor.md)
- [SEG:DATA_LIMITS](./application/seg/seg-data_limits.md)
- [SEG:INTERSECTION_LIMIT](./application/seg/seg-intersection_limit.md)
- [SEG:MARKER_DIAMETER](./application/seg/seg-marker_diameter.md)
- [SEG:NOISE_FACTOR](./application/seg/seg-noise_factor.md)
- [SEG:RESIDUAL_ERROR_FACTOR](./application/seg/seg-residual_error_factor.md)

### SUBJECTS

No specific parameter provided by the C3D User Guide.