# General informations

The following is our attempt to edit the [C3D User Guide](https://www.c3d.org/docs/C3D_User_Guide.pdf) to provide a documentation that is a lot more accessible to everybody.

This editing is the fruit of not just reorganisation of the original document, but also amendment made from the experience gained from the implementation of the C3D Standard into a library and its testing against all the [sample files](https://www.c3d.org/sampledata.html) provided by the C3D organization.

We hope it will provide a better and more accessible experience for the reader interested in learning about the in and out of the C3D Standard.

If you encounter errors, typo, inconsistencies, needs of examples, or simply foggy section: [please do let us know](https://github.com/TSS-22/SHARP3D/issues). Our goal is to make this documentation as clear and straight forward as can be. 

> Some recommendations and judgments have been added. They should be interpreted as our view and the C3D organization can not be held responsible for them.

## Reading process of a C3D File

The endian format used in the C3D file must be determined in order to read the C3D file contents because the endian format affects the interpretation of all 16-bit integer and 32-bit floating-point formats. All applications opening a C3D file must determine the endian type before reading any integer or floating-point values.