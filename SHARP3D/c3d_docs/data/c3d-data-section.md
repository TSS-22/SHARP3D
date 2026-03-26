# Data Section

The C3D
structure for 3D Point and Analog Data assumes that each Data Frame can have one 3D Point Frame and one or more Analog Frame from each analog channel sampled.

While this means that C3D files can only contain data sampled at integer multiples of the 3D frame rate, it means that data storage synchronization is guaranteed and makes it easy to calculate the size and location of individual 3D data frames and their associated analog samples within the C3D file.