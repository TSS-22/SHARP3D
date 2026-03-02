using SHARP3D.Utils.Enum;

namespace SHARP3D.Test.TestSuites
{
    /// <summary>
    /// Test for basic functionnality and if all processor and INT16 and FLOAT32 data are supporter
    /// </summary>
    internal class TestSuite
    {

        // Test:
        // - Parameter name
        // - Group name
        // - First Frame values
        // - Last Frame values

        public static readonly string PathEb015pi = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d";
        public static readonly string PathEb015pr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pr.c3d";
        public static readonly string PathEb015si = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015si.c3d";
        public static readonly string PathEb015sr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015sr.c3d";
        public static readonly string PathEb015vi = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vi.c3d";
        public static readonly string PathEb015vr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vr.c3d";

        public static IEnumerable<object[]> FileStreamData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, ProcessorType.INTEL},
                new object[] { PathEb015pr, ProcessorType.INTEL},
                new object[] { PathEb015si, ProcessorType.SIG_MIPS},
                new object[] { PathEb015sr, ProcessorType.SIG_MIPS},
                new object[] { PathEb015vi, ProcessorType.DEC},
                new object[] { PathEb015vr, ProcessorType.DEC},
            };
    }
}
