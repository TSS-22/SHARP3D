# SEG:DATA_LIMITS

- **Type**: [Application](../../application.md)

- **Locked**: False

A 3 by 2 array of floating-point values that defines the upper and lower limits of the reconstruction volume, measured in [POINT:UNITS](../../required/point/point-units.md), during the trajectory photogrammetry calculations.

It is presented in the form<sup id="fnref1"><a href="#fn1">1</a></sup>:
- SEG:DATA_LIMITS[0,0]: X<sub>lower limit</sub>
- SEG:DATA_LIMITS[0,1]: X<sub>upper limit</sub>
- SEG:DATA_LIMITS[1,0]: Y<sub>lower limit</sub>
- SEG:DATA_LIMITS[1,1]: Y<sub>upper limit</sub>
- SEG:DATA_LIMITS[2,0]: Z<sub>lower limit</sub>
- SEG:DATA_LIMITS[2,1]: Z<sub>upper limit</sub>

This parameter is generally used by the photogrammetry software to enable it to discard 3D information that strays outside the data collection volume. This helps speed up the intense photogrammetry computations by allowing an application to ignore unwanted data from reflections, camera strobes, lights etc., which might reduce the overall accuracy of data stored using the original integer format by requiring a larger [POINT:SCALE](../../required/point/point-scale_factor.md) value to accommodate spurious points outside the data collation area as a result of reflections or other photogrammetry errors.

If set correctly, the SEG:DATA_LIMITS parameter can also provide useful information to any application that needs to set up a view window as it documents the maximum bounds of the 3D trajectory data.

<sup id="fn1">1</sup>. It could be the other way around, upper limit being given before the lower limit. [C3D User Guide](https://www.c3d.org/docs/C3D_User_Guide.pdf) is unclear. This will be investigated. [↩] (#fnref1)