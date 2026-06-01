# POINT:Y_SCREEN

- **Type**: [Application](../../application.md)

- **Locked**: False

Like the X_SCREEN above, this is a 7-bit ASCII string containing a sign together with
a single character:

- $+X$
- $+Y$
- $+Z$
- $-X$
- $-Y$
- $-Z$. 

This is used by software applications to indicate which axis of the reference coordinate system should be displayed bottom-to-top up the screen when the application initially opens the file.

This parameter is a companion to the [X_SCREEN](./point-x_screen.md) parameter, and is also compatible
with the C3D file format. 

Note that the programmer could have chosen to implement the parameter as an array, e.g., SCREEN(1,2). However, this might not have been as intuitive for a casual user to edit or use. Creating two separate parameters was a good decision as it makes the function of both values clear.