using SHARP3D.Utils.Enum;

namespace SHARP3D.Test
{
    public class C3dFileHeaderTests
    {
        public static readonly string PathEb015pi = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d";
        public static readonly string PathEb015pr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pr.c3d";
        public static readonly string PathEb015si = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015si.c3d";
        public static readonly string PathEb015sr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015sr.c3d";
        public static readonly string PathEb015vi = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vi.c3d";
        public static readonly string PathEb015vr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vr.c3d";

        public static readonly byte[] ParameterSectionPointer = { 0x02, 0x00 };
        public static readonly int ParameterSectionPointerValue = 512;

        public static readonly float[] EventTimes = { 2.720f, 5.400f, 7.320f};
        public static IEnumerable<object[]> FileStreamData =>
            new List<object[]>
            {
                new object[] { PathEb015pi},
                new object[] { PathEb015pr},
                new object[] { PathEb015si},
                new object[] { PathEb015sr},
                new object[] { PathEb015vi},
                new object[] { PathEb015vr},
            };

        public static IEnumerable<object[]> ProcessorMakerData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, ProcessorType.INTEL},
                new object[] { PathEb015pr, ProcessorType.INTEL},
                new object[] { PathEb015si, ProcessorType.SIG_MIPS},
                new object[] { PathEb015sr, ProcessorType.SIG_MIPS},
                new object[] { PathEb015vi, ProcessorType.DEC},
                new object[] { PathEb015vr, ProcessorType.DEC},
            };

        public static IEnumerable<object[]> ParameterPointerData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, ParameterSectionPointerValue},
                new object[] { PathEb015pr, ParameterSectionPointerValue},
                new object[] { PathEb015si, ParameterSectionPointerValue},
                new object[] { PathEb015sr, ParameterSectionPointerValue},
                new object[] { PathEb015vi, ParameterSectionPointerValue},
                new object[] { PathEb015vr, ParameterSectionPointerValue},
            };

        public static IEnumerable<object[]> DataFlagFormatData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, DataFormat.RIGHT},
                new object[] { PathEb015pr, DataFormat.RIGHT},
                new object[] { PathEb015si, DataFormat.RIGHT},
                new object[] { PathEb015sr, DataFormat.RIGHT},
                new object[] { PathEb015vi, DataFormat.RIGHT},
                new object[] { PathEb015vr, DataFormat.RIGHT},
            };

        public static IEnumerable<object[]> MarkerPerFrameData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, 26},
                new object[] { PathEb015pr, 26},
                new object[] { PathEb015si, 26},
                new object[] { PathEb015sr, 26},
                new object[] { PathEb015vi, 26},
                new object[] { PathEb015vr, 26},
            };

        // TODO: The value is inconsistent from hex to what I can see in qualisys.
        public static IEnumerable<object[]> AnalogSamplePerFrameData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, 64},
                new object[] { PathEb015pr, 64},
                new object[] { PathEb015si, 64},
                new object[] { PathEb015sr, 64},
                new object[] { PathEb015vi, 64},
                new object[] { PathEb015vr, 64},
            };

        // TODO: The hex inconsistent with the spreadsheet value.
        public static IEnumerable<object[]> PointScaleData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, 0.077625f},
                new object[] { PathEb015pr, 0.077625f},
                new object[] { PathEb015si, 0.077625f},
                new object[] { PathEb015sr, 0.077625f},
                new object[] { PathEb015vi, 0.077625f},
                new object[] { PathEb015vr, 0.077625f},
            };

        public static IEnumerable<object[]> AcquisitionRate3dData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, 50.0f},
                new object[] { PathEb015pr, 50.0f},
                new object[] { PathEb015si, 50.0f},
                new object[] { PathEb015sr, 50.0f},
                new object[] { PathEb015vi, 50.0f},
                new object[] { PathEb015vr, 50.0f},
            };


        public static IEnumerable<object[]> EventTimeData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, EventTimes},
                new object[] { PathEb015pr, EventTimes},
                new object[] { PathEb015si, EventTimes},
                new object[] { PathEb015sr, EventTimes},
                new object[] { PathEb015vi, EventTimes},
                new object[] { PathEb015vr, EventTimes},
            };



        [Theory]
        [MemberData(nameof(FileStreamData))]
        public void FileStream_Test(string filepath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.NotNull(c3dFile);
            //c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(ProcessorMakerData))]
        public void ProcessorMakerType_Test(string filepath, ProcessorType expectedProcessorType)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.Equal(expectedProcessorType, c3dFile.ProcessorFileType);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(FileStreamData))]
        public void ProcessorHostType_Test(string filepath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.Equal(BitConverter.IsLittleEndian ? ProcessorType.INTEL : ProcessorType.SIG_MIPS, c3dFile.ProcessorHostType);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(ParameterPointerData))]
        public void ParameterSectionPointer_Test(string filepath, int expectedParameterSectionPointerValue)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.Equal(expectedParameterSectionPointerValue, c3dFile.C3DHeader.PointerParameterSection);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(DataFlagFormatData))]
        public void DataFlag_Test(string filepath, DataFormat expectedDataFormat)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.Equal(expectedDataFormat, c3dFile.C3DHeader.FlagDataFormat);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(MarkerPerFrameData))]
        public void NbMarkerPerFrame_Test(string filepath, int expectedNumber)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.Equal(expectedNumber, c3dFile.C3DHeader.MarkersPerFrame);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(PointScaleData))]
        public void PointScaleValue_Test(string filepath, float expectedPointScale)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.Equal(expectedPointScale, c3dFile.C3DHeader.ScaleFactor);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(AcquisitionRate3dData))]
        public void AcquisitionRate3dValue_Test(string filepath, float expectedAcquisitionRate)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.Equal(expectedAcquisitionRate, c3dFile.C3DHeader.Rate3dFrame);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(EventTimeData))]
        public void EventTimeValue_Test(string filepath, float[] expectedEventTimes)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            for(int i=0; i < c3dFile.C3DHeader.Events.Length;i++)
            {
                Assert.Equal(expectedEventTimes[i], c3dFile.C3DHeader.Events[i].EventTime);
            }
            c3dFile.CloseFileStream();
        }
        

    }
}