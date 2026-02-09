namespace SHARP3D.Test
{
    public class C3dFileTests
    {
        public static readonly byte[] ParameterSectionPointer = { 0x02, 0x00 };

        /// <summary>
        /// 
        /// </summary>
        /// <para>
        /// <list type="number">
        ///   <item>Path to test file</item>
        ///   <item>Processor used to create the file</item>
        ///   <item>Parameter section pointer</item>
        ///   <item>Flag data format</item>
        ///   <item></item>
        ///   <item></item>
        /// </list>
        /// </para>
        public static IEnumerable<object[]> ProcessorMakerData =>
            new List<object[]>
            {
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d", ProcessorType.INTEL, ParameterSectionPointer, 'P'},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pr.c3d", ProcessorType.INTEL, ParameterSectionPointer, 'P'},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015si.c3d", ProcessorType.SIG_MIPS, ParameterSectionPointer, 'P'},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015sr.c3d", ProcessorType.SIG_MIPS, ParameterSectionPointer, 'P'},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vi.c3d", ProcessorType.DEC, ParameterSectionPointer, 'P'},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vr.c3d", ProcessorType.DEC, ParameterSectionPointer, 'P'},
            };
  
    }
}