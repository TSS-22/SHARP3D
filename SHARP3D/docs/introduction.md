# Introduction

## Samples and Tests

You can find C3D samples on the [C3D website](https://www.c3d.org/sampledata.html).
A large part of their set is used for testing this library and can be found in SHARP3D.Test/SampleFiles and SHARP3D.Test/SampleErrorFiles.

The expected results where extracted from C3D files automatically using [ezc3d](https://github.com/pyomeca/ezc3d), they can therefore be subject to some problems.
The values that were obviously wrongly read using [ezc3d](https://github.com/pyomeca/ezc3d) have been discarded or sanitized. When in doubt, binaries files where manually checked using [Ghidra](https://github.com/NationalSecurityAgency/ghidra), [ImHex](https://imhex.werwolv.net/) and [Qualisys Track Manager](https://www.qualisys.com/software/qualisys-track-manager/). The SHARP3D project is independant and not endorsed by any of them.
You can extract the test values from the C3D samples via the RunTestDataScripts.py (SHARP3D.Test/TestDataExctraction/RunTestDataScripts.py).

The following .JSON file have been corrected by hand due to [ezc3d](https://github.com/pyomeca/ezc3d) encountering some issues.
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


