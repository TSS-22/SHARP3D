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
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d", ProcessorType.INTEL, ParameterSectionPointer, DataFormat.RIGHT},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pr.c3d", ProcessorType.INTEL, ParameterSectionPointer, DataFormat.RIGHT},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015si.c3d", ProcessorType.SIG_MIPS, ParameterSectionPointer, DataFormat.RIGHT},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015sr.c3d", ProcessorType.SIG_MIPS, ParameterSectionPointer, DataFormat.RIGHT},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vi.c3d", ProcessorType.DEC, ParameterSectionPointer, DataFormat.RIGHT},
                new object[] { @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vr.c3d", ProcessorType.DEC, ParameterSectionPointer, DataFormat.RIGHT},
            };

        [Theory]
        [MemberData(nameof(ProcessorMakerData))]
        public void LoadC3dTestSuite01(string filepath, ProcessorType expectedProcessorType, byte[] expectedParameterSectionPointer, DataFormat expectedFlagDataFormat)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.NotNull(c3dFile);
            Assert.True(c3dFile.IsFileStreamOpen());
            Assert.Equal(expectedProcessorType, c3dFile.ProcessorHostType);
            Assert.Equal(BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS, c3dFile.ProcessorFileType);
            Assert.Equal(expectedFlagDataFormat, c3dFile.C3DHeader.FlagDataFormat);
            c3dFile.CloseFileStream();
        }
    }
}