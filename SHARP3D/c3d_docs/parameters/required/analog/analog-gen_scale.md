# ANALOG:GEN_SCALE

- **Type**: [Required](../../required.md)

- **Locked**: True

The C3D file format was designed to accurately store 3D data, together with analog sampled data, and provide the user with as much information as possible about each of the signals recorded in the file. The C3D format was designed to store the analog samples recorded directly from the ADC subsystem and consequently supports one universal scale factor that describes the ADC system and individual scale factors for each analog channel that translates the sampled analog data into physical world values, e.g. typically volts, degrees, newtons, etc.

The ANALOG:GEN_SCALE parameter is a single floating-point value that is a universal common analog scaling factor for all analog channels. Its original function was to define the ADC data collection system resolution when the analog data was sampled and written to a C3D files. [Each analog channel has an associated individual scaling factor in the ANALOG:SCALE array which is individually applied, together with the common GEN_SCALE value, to all analog signals in the C3D file to generate physical world values when the file is read](../../../data/analog.md). This method of storing the directly sampled data values has significant advantages in guaranteeing data accuracy and enables verification of the sampled data post-collection as the measurements can be verified by manually checking the scaling calculations and then correcting any errors without losing the original data.

Some motion capture systems simply record scaled numbers instead of data sample values; this makes post-collection data verification and debugging impossible. Data collection systems that pre-scale and process the analog data as scaled floating-point values set the ANALOG:GEN_SCALE parameter and the individual [ANALOG:SCALE](./analog-scale.md) array values to 1.0, which removes the option of determining the original data sample values.

Common values for the ANALOG:GEN_SCALE parameter are:

- **1.0**: this effectively removes the GEN_SCALE contribution from all scaling
math.

- **0.0003052**: the value of a single bit of data from a 16-bit ADC that is measuring a ±10V input range. Each channel [ANALOG:SCALE](./analog-scale.md) value would then be 1.00 to obtain the analog data scaled in Volts with a ±10V input range or 0.5 to set the analog data input range to a ±5V.

- **0.0048828**: the value of a single bit of data from a 12-bit ADC that is measuring a ±10V input range. Each channel [ANALOG:SCALE](./analog-scale.md) value would then be 1.00 to obtain the analog data scaled in Volts.

> If the stored analog data is pre-scaled and stored in a floating-point C3D file then setting both scaling factors to 1.00 effectively deletes the scaling information and may lead to problems during analysis in the future.

Since the value of the ANALOG:GEN_SCALE parameter is used with each of the individual [ANALOG:SCALE](./analog-scale.md) values to calculate the correct value of each analog channel signal, it is critically important that the ANALOG:GEN_SCALE value is not changed without considering its effect on the individual [ANALOG:SCALE](./analog-scale.md) values.

It is important to take into account the possible scaling ranges when selecting scaling values. For example, a C3D files using an ANALOG:GEN_SCALE value of 1.000 will require individual [ANALOG:SCALE](./analog-scale.md) values of 0.0048828 to scale 12-bit resolution data samples in Volts, an EMG application might require scaling in microvolts with corresponding [ANALOG:SCALE](./analog-scale.md) value in the range of 0.0000048828 to 0.0000000048828, while the force plate, scaled in newtons would use values of 100 – 300. 

