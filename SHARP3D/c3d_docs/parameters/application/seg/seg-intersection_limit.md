# SEG:INTERSECTION_LIMIT

- **Type**: [Application](../../application.md)

- **Locked**: False

This is a single floating-point value that sets the limit for the intersection of photogrammetric rays to reconstruct a 3D point.

Its nominal value, in terms of [POINT:UNITS](../../required/point/point-units.md) is 7mm or less.

## From Mistral

### Ray Intersection Threshold

This value defines the maximum allowed distance between two or more photogrammetric rays (projected from cameras) for them to be considered as intersecting at a single 3D point.

### How it works

Cameras capture 2D images of markers (or features) from different angles. The system traces rays from each camera through the detected 2D marker positions into 3D space. If the rays from multiple cameras pass within this threshold distance of each other, the system assumes they intersect at a valid 3D point (the marker’s position). If the rays are too far apart (beyond the threshold), the system may reject the point as unreliable or noisy.

- A threshold of 7mm means that rays must come within 7mm of each other in 3D space to be considered a valid intersection.
- Smaller values (e.g., 1–3mm) increase precision but may reject valid points due to minor noise or calibration errors.
- Larger values (e.g., 10mm+) make the system more forgiving but may introduce inaccuracies by merging distinct points.

### Why It Matters

Accuracy vs. Robustness: A tighter threshold (e.g., 2mm) improves accuracy but risks losing data if cameras or markers are noisy. A looser threshold (e.g., 10mm) captures more points but may include errors.

Calibration Quality: Poorly calibrated cameras or lens distortion can cause rays to misalign, requiring a higher threshold to compensate.

Marker Size/Environment: Larger markers or longer baseline distances between cameras may need adjusted thresholds.

### Practical Example

If two cameras detect the same marker but their rays pass 8mm apart in 3D space, and your threshold is 7mm, the system won’t reconstruct that point. Lowering the threshold to 10mm would allow it, but at the cost of potential precision loss.
