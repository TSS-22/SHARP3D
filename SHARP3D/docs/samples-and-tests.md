# Samples and Tests

## Samples

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

## Tests

Some tests are bound to fail due to problems in the file that can't be recovered or circumvented.

### Expected failing test

At the moment 12 tests are failing:

1. [SHARP3D.Test.Tests.BasicTest.Basics](#sharp3dtesttestsbasictestbasics)
    1. Sample31\large01.c3d
    2. Sample31\large02.c3d

2. [SHARP3D.Test.Tests.BasicTest.ReadsFloatingFrameNumber](#sharp3dtesttestsbasictestreadsfloatingframenumber)
    1. SampleFiles\Sample36\72610framesf.c3

3. [SHARP3D.Test.Tests.SampleErrorFilesTest.OpenFileTest_Test](#sharp3dtesttestssampleerrorfilestestopenfiletest_test)
    1. SampleErrorFiles\Sample13
    2. SampleErrorFiles\Sample18
    3. SampleErrorFiles\Sample20

4. [SHARP3D.Test.Tests.SampleErrorFilesTest.Sample13Basic_Test](#sharp3dtesttestssampleerrorfilestestsample13basic_test)
    1. SampleErrorFiles\Sample13\Dance.c3d
    2. SampleErrorFiles\Sample13\Dance1.c3d
    3. SampleErrorFiles\Sample13\golfswing.c3d
    4. SampleErrorFiles\Sample13\golfswing1.c3d

5. [SHARP3D.Test.Tests.SampleErrorFilesTest.Sample18Basic_Test](#sharp3dtesttestssampleerrorfilestestsample18basic_test)
    1. SampleErrorFiles\Sample18\bad_parameter_section.c3d

6. [SHARP3D.Test.Tests.SampleErrorFilesTest.Sample20Basic_Test](#sharp3dtesttestssampleerrorfilestestsample20basic_test)
    1. SampleErrorFiles\Sample20\phasespace_sample

### Explanation

#### SHARP3D.Test.Tests.BasicTest.Basics

**Status**: Under investigation

This is unusal behavior as per manual binaries inspection. Not all frame produce a wrong data point error. The last one at least. EZC3D and Qualisys both have similar error, but don't give back the same wrong data point value. The error is inconsistent between solutions, but always wrong.

#### SHARP3D.Test.Tests.BasicTest.ReadsFloatingFrameNumber

**Status**: Solved

The file is incorrectly build and is missing data to account for the expected amount of frame. This is confirmed by manual inspection and Qualisys.

#### SHARP3D.Test.Tests.SampleErrorFilesTest.OpenFileTest_Test

**Status**: Under investigation 

#### SHARP3D.Test.Tests.SampleErrorFilesTest.Sample13Basic_Test

**Status**: Under investigation

#### SHARP3D.Test.Tests.SampleErrorFilesTest.Sample18Basic_Test

**Status**: Under investigation

#### SHARP3D.Test.Tests.SampleErrorFilesTest.Sample20Basic_Test

**Status**: Under investigation