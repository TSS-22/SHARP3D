# ANALOG:OFFSET

The possibility of 16-bit integer overflow exists when applying the [ANALOG:OFFSET](./analog-offset.md) Parameter to the sampled 16-bit analog data. It is recommended that all applications perform internal scaling calculations with more than 16-bits of resolution (either 32-bit or floating-point) and check the results to ensure that internal math overflow has not occurred.