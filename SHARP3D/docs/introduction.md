# Introduction

# Samples and Tests

You can find C3D samples on the [C3D website](https://www.c3d.org/sampledata.html).
A large part of their set is used for testing this library and can befound in SHARP3D.Test.SampleFiles.

The expected results where extracted from C3D files automatically using [**ezc3d**](https://github.com/pyomeca/ezc3d), they can therefore be subject to some problems.
The values that were obviously wrongly read using [**ezc3d**](https://github.com/pyomeca/ezc3d) have been discarded or sanitized. When in doubt, binaries files where manually checked using [Ghidra](https://github.com/NationalSecurityAgency/ghidra), [ImHex](https://imhex.werwolv.net/) and [Qualisys Track Manager](https://www.qualisys.com/software/qualisys-track-manager/).
You can extract the test values from the C3D samples via the RunTestDataScripts.py (SHARP3D.Test/TestDataExctraction/RunTestDataScripts.py).

The following .JSON file have been corrected by hand due to [**ezc3d**](https://github.com/pyomeca/ezc3d) encountering some issues.
Bad formatting:
- "kyowadengyo.json" (Expected path: "SHARP3D.Test/SampleFiles/Sample27/kyowadengyo.json")  

Bin to Float Conversion disagreement on the last frame (off by around +/- 0.5:
- "large01.json" (Expected path: "SHARP3D.Test/TestFiles/Sample31/large01.c3d")
- "large02.json" (Expected path: "SHARP3D.Test/TestFiles/Sample31/large02.c3d")

The SHARP3D project is independant from and not endorssed by any of the aformentionned projects.

Only two files are missing as they are above the 100 MB limit from Github:
- "c24089 13.c3d" (Expected path: "SHARP3D.Test/SampleFiles/Sample12/c24089 13.c3d")
- "large02.c3d" (Expected path: "SHARP3D.Test/TestFiles/Sample31/large02.c3d")

The corresponding .JSON files are still in the respective folders so you can simply add the missing .C3D back manually to be able to run all our test sets.

The file 72610framesf.c3d (Expected path: "SHARP3D.Test/SampleFiles/Sample36/72610framesf.c3d") fail to open but this could be due to an incorrect file: the file is too short for the amount of data it is supposed to have by around 2 MB.


