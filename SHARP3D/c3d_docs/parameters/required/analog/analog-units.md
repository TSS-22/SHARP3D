# ANALOG:UNITS

- **Type**: [Required](../../required.md)

- **Locked**: True

The ANALOG:UNITS parameter is an array of character data values, normally 4 characters in length each. This parameter stores the analog measurement units for each channel (e.g. V, N, Nm). The units should describe the quantities after the scaling factors are applied. As a result, there should always be one ANALOG:UNITS entry for a total of ANALOG:USED channels.

Note that changing the ANALOG:UNITS parameter does not automatically affect the calculated analog values, as it is not used in the analog scaling calculations. You must change the [ANALOG:SCALE](./analog-scale.md) parameter to re-scale the analog data.