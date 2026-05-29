# FORCE_PLATFORM:CHANNEL

- **Type**: [Required](../../required.md)

- **Locked**: False

The FORCE_PLATFORM:CHANNEL parameter is an array of signed integer data values that record which analog channels contain specific force platform data. The force platform outputs may be connected to any convenient analog input channels in anyorder that is convenient to the user, provided that the assignment of force platform signals to analog channels is correctly specified in this parameter.

While it is recommended that force plate channels be connected to the analog recording device in a logical fashion it is not essential that they are stored in any fixed order within the C3D file. Any application that reads force plate data must use this parameter to determine the force plate channel to analog channel assignments.

Note that if your data collection environment used several different types of force platforms and any of them are **TYPE-3** then this parameter must contain eight (8,) entries for all plates. If **TYPE-3 force plates** are not used then the dimension may be either (6,) or (8,) as the unused values in the CHANNEL parameter should be set to zero and ignored.

>Channels number:
>- **TYPE-1**: 6 or 8
>- **TYPE-2**: 6 or 8
>- **TYPE-3**: 8
>- **TYPE-4**: 6 or 8

|  | **TYPE-1** | **TYPE-2** | **TYPE-3** | **TYPE-4** |
| --- | --- | --- | --- | --- |
| CHANNEL (1,i) | Forceₓ | Forceₓ | Forceₓ¹,² | Forceₓ |
| CHANNEL (2,i) | Forceᵧ | Forceᵧ | Forceₓ³,⁴ | Forceᵧ |
| CHANNEL (3,i) | Force_z | Force_z | Forceᵧ¹,⁴ | Force_z |
| CHANNEL (4,i) | CoPₓ | Momentₓ | Forceᵧ²,³ | Momentₓ |
| CHANNEL (5,i) | CoPᵧ | Momentᵧ | Force_z¹ | Momentᵧ |
| CHANNEL (6,i) | Free Moment_z | Moment_z | Force_z² | Moment_z |
| CHANNEL (7,i) | 0 | 0 | Force_z³ | 0 |
| CHANNEL (8,i) | 0 | 0 | Force_z⁴ | 0 |

The table above shows the assignment of analog channel numbers to force plate signals within this parameter where i is the force platform number. For instance, if $MZ$ of force platform number 2 is connected to analog channel 15, then CHANNEL(6,2) should contain the entry 15.