# Troubleshooting C3D Files

Occasionally users experience problems reading C3D files, in general these problems are due to one or more issues:

- The file may not be a C3D standard biomechanics formatted file. Several companies have created [Computer Aided Design (CAD) applications](https://en.wikipedia.org/wiki/Computer-aided_design) that store three dimensional drawings of mechanical and architectural objects in files, misusing the `.C3D` file-type. their formats are unrelated.

- If a C3D file is missing parameters or contains incorrect parameters then applications may experience problems displaying or processing the data.

- If the C3D file contains more than 32767 frames of data but the application does not support files with more than 32767 frames then it may be unable to access the C3D file.

- If the C3D file contains more than 65535 frames of data but the application does not support files with more than 65535 frames then it may be unable to access the C3D file.

- If the C3D file data section format is identified containing as 3D Point data but has been written in a unique, non-coordinate format, then applications may report that the file is empty and does not contain any data as a result of the file is storing data in a private data section format that has not been documented and is not supported by a users C3D application, resulting in a file that appears to be empty.

- An application opening a C3D may report “an error” because it incorrectly assumes that the existence of a parameter is defining data, even though the parameter value, e.g. [FORCE_PLATFORM:USED](../../parameters/required/force_platform/force_platform-used.md), is 0 indicating that there is no data. As a result some “C3D file errors” are actually application bugs.

If the C3D file can be opened, but does not appear to have any marker or point data then the problems may be due to the creation of a file that is missing parameters or contains incorrect parameters. This might be because the original data was collected, or exported to C3D, scaled in meters, not millimeters so the application is plotting a human subject only 1.8288mm high walking across the data collection volume.

If you get an error message from your application that the C3D file cannot be opened for any reason then the application may believe that the file has been modified and extended in some way that it cannot handle, or the file is corrupted or damaged. The easiest path is to attempt to open the C3D file with another application. Several free applications are available from the [C3D web site](https://www.c3d.org/c3dsupport.html) that allow a user to view and display the contents of reasonably formatted C3D files, even if the file contains some minor errors. 

At the time of this writing, a lot of applications can't be found anymore and/or don't support a wide range of C3D requirements. As far as we know, at present, your best bet is using [EZC3D](https://github.com/pyomeca/ezc3d), SHARP3D, or any of the main motion capture company software: [Motiv](https://optitrack.com/software/motive), [Nexus](https://www.vicon.com/software/nexus/) or [Qualysis Track Manager](https://www.qualisys.com/software/qualisys-track-manager/).

If your application can open the C3D file but appears to be unable to process the data
then it is usually because some parameters that are expected to be stored in the C3D
file are missing or stored incorrectly.

If the C3D file cannot be opened then check the source of the file because, while the C3D file type has been in common use since 1987, a number of companies with no connections to the biomechanics or motion capture industries in recent years have started misusing the C3D file-type so the C3D file might be a [CAD](https://en.wikipedia.org/wiki/Computer-aided_design) format file used in architectural and machine building industries.

The [C3D web site](https://www.c3d.org/c3dsupport.html) site has a [collection of C3D files from different sources](https://www.c3d.org/sampledata.html) if you want to verify the ability of an application to open and process a variety of different C3D files, even if the files contain formatting errors. 

> Applications may attempt to discover and internally repair errors when opening files to display and access the data, as a result, the ability to open a C3D file by an application may not guarantee that the C3D file is error free.(sic)