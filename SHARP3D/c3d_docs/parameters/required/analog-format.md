# ANALOG:FORMAT

If the [ANALOG:FORMAT](./analog-format.md) parameter is `UNSIGNED` then the [ANALOG:OFFSET](./analog-offset.md) parameter must be interpreted as an Unsigned Int6.

If the [ANALOG:FORMAT](./analog-format.md) parameter does not exists then assume that the Analog Data is stored as positive value as a signed 16-bit integer. This will be correct most of the time.