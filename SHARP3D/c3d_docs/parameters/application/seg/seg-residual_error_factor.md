# SEG:RESIDUAL_ERROR _FACTOR

- **Type**: [Application](../../application.md)

- **Locked**: False

The SEG:RESIDUAL_ERROR _FACTOR parameter is a single floating-point value that controls the inclusion of rays during marker reconstruction. It has a nominal value of 2.0 to 3.0 for most gait analysis applications.

> No other information is given as to what it actually means. In a motion capture system, rays are the camera view of a marker. You need at least 2 to interpolate a 3D position for the marker, but 3 is the minimum to have a trusted position, as explained [here](../../../data/3d-points-residuals.md). SEG:RESIDUAL_ERROR _FACTOR could be referring to the minimum number of rays user for marker reconstruction. A way to ensure that at least SEG:RESIDUAL_ERROR _FACTOR number of camera saw the point. This could be useful in system with more than 7 camera, where the [camera mask](../../../data/camera-mask.md) can't cover the whole set of camera and would leave up to interpretation how many camera, at least, saw the marker.

### From Mistral

### Purpose

This parameter scales the acceptable residual error for a ray to be included in the reconstruction of a 3D marker point. It acts as a multiplier for the system’s internal residual error threshold.

### How it works

For each detected 2D marker in a camera image, the system projects a ray into 3D space.
The system calculates the residual error for each ray—this is the difference between the observed 2D marker position (in the camera image) and the projected 2D position of the reconstructed 3D point.

If the residual error for a ray exceeds $\text{RESIDUAL\_ERROR\_FACTOR} \times (\text{internal threshold})$, that ray is excluded from the reconstruction for that marker.

The internal threshold is often derived from the system’s calibration accuracy or noise estimates.

A value of 2.0 means rays with residual errors twice the internal threshold are still included. A value of 3.0 is more permissive, allowing rays with errors up to three times the threshold.

### Effect of Adjusting:

Lower values (e.g., 1.5): Stricter filtering. Only rays with very low residual errors are included, improving precision but potentially dropping valid data if noise is present.

Higher values (e.g., 4.0): More rays are included, making the system more tolerant to noise or minor misalignments, but risking the inclusion of outliers.


### Why It Matters

Noise Handling: In real-world motion capture, noise (e.g., from camera sensors, marker occlusion, or calibration errors) can cause residual errors. This factor helps the system decide which rays to trust.

Marker Occlusion: If a marker is partially occluded in one camera, its residual error may be higher. A higher factor allows the system to still use that ray.

Application-Specific Tuning: Gait analysis often uses 2.0–3.0 because it involves dynamic movement with some expected noise, but other applications (e.g., static object scanning) might use tighter values.

### Practical Example

If the internal residual error threshold is 0.5 pixels, and SEG:RESIDUAL_ERROR_FACTOR is set to 2.0, rays with residual errors up to 1.0 pixels will be included. If a ray has a residual error of 1.2 pixels, it will be excluded unless the factor is increased (e.g., to 2.4)
