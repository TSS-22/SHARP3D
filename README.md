At the moment the project is in a hiatus till january 2027. We are producing a C3D trajectory visualizer, which will be the basis of a MOCAP data cleaning software. We don't plan on making the code source open for those onesin the near future, but the visualizer will be provided for free. In the early 2027, we will work to clean up the code of SHARP3D and make a clear documentation and explain our goal with this library regarding the C3D format.

We will also plan to implement a `.JSON` and `.HDF5` exporter/importer at that moment, probably as an outside library (the architecture of it has not been decided yet). This is in order to get away of the (too) many problems with the C3D format.


/!\ WORK IN PROGRESS /!\

At the time, the library open and save C3D files as best as it can. 
To read a C3D file use

`C3d myC3d = new C3d(@"my/path/to/c3dFile.c3d");`

The `C3d` object try to recover a lot of the errors and formatting issues.

If you want the "pure" C3D file use:

`C3dFile myC3dAsIs = C3dFile.LoadFromFile(@"my/path/to/c3dFile.c3d");`

The C3dFile objects are used for debugging and having a trace of the unchanged C3D file. We don't recommend using it. 

The goal is to use the `C3d` object to handle most of the tricks of the C3D format to allow user to actually be able to be productive with this format, and guarantee file opening in other applications.

At the moment EZC3D doesn't like our formatting for some reasons, we will look into it, and potentially create a specific output format for them.

Qualisys read the C3D file created with SHARP3D.

We expect to potentially have some issue with Vicon as we don't use the TRIAL format for frames.

![](asset/img/rampage-rampagejackson.gif)

# SHARP3D
Implementation of the C3D file format in C#, as per the definition and guidelines from https://www.c3d.org/

![](asset/img/c3d/C3DIconv3.png)

# Documentation

https://tss-22.github.io/SHARP3D/index.html

# Read C3D files

Working. Beta level: Files are read as good as it gets if they have been formatted correclty, or present recoverable error.

The API is getting worked on.

If you encounter Reading Error with some files, feel free to open an issue and provide the faulty files and as much information about them as you can and the error. We will try to help.

# Save C3D files

Saving from `C3d` object is working.

# Managing C3D files
