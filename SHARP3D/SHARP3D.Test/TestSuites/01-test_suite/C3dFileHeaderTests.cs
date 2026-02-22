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

        public static readonly float ScaleFactor = -0.0833333358f;
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
                new object[] { PathEb015pi, StorageFormat.ORIGINAL},
                new object[] { PathEb015pr, StorageFormat.ORIGINAL},
                new object[] { PathEb015si, StorageFormat.ORIGINAL},
                new object[] { PathEb015sr, StorageFormat.ORIGINAL},
                new object[] { PathEb015vi, StorageFormat.ORIGINAL},
                new object[] { PathEb015vr, StorageFormat.ORIGINAL},
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
                new object[] { PathEb015pi, ScaleFactor},
                new object[] { PathEb015pr, ScaleFactor},
                new object[] { PathEb015si, ScaleFactor},
                new object[] { PathEb015sr, ScaleFactor},
                new object[] { PathEb015vi, ScaleFactor},
                new object[] { PathEb015vr, ScaleFactor},
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

        public static IEnumerable<object[]> DataPointerData =>
            new List<object[]>
            {
                new object[] { PathEb015pi, 11},
                new object[] { PathEb015pr, 11},
                new object[] { PathEb015si, 11},
                new object[] { PathEb015sr, 11},
                new object[] { PathEb015vi, 11},
                new object[] { PathEb015vr, 11},
            };

        internal C3dFile GetC3dFileWithHeader(string filePath)
        {

            FileStream fileStream = C3dFile.OpenC3dFile(filePath);
            C3dFile c3dFile = new C3dFile();
            c3dFile.C3dStream = fileStream;
            c3dFile.ProcessorFileType = C3dFile.ReadProcessorByte(fileStream);
            c3dFile.Header = c3dFile.GetHeader(fileStream, c3dFile.ProcessorFileType);
            return c3dFile;
        }

        [Theory]
        [MemberData(nameof(FileStreamData))]
        public void FileStream_Test(string filepath)
        {
            FileStream fileStream = C3dFile.OpenC3dFile(filepath);
            Assert.NotNull(fileStream);
        }

        [Theory]
        [MemberData(nameof(ProcessorMakerData))]
        public void ProcessorMakerType_Test(string filepath, ProcessorType expectedProcessorType)
        {
            FileStream fileStream = C3dFile.OpenC3dFile(filepath);
            ProcessorType processor = C3dFile.ReadProcessorByte(fileStream);
            Assert.Equal(expectedProcessorType, processor);
            fileStream.Close();
        }

        [Theory]
        [MemberData(nameof(ParameterPointerData))]
        public void ParameterSectionPointer_Test(string filepath, int expectedParameterSectionPointerValue)
        {
            C3dFile c3dFile = GetC3dFileWithHeader(filepath);
            Assert.Equal(expectedParameterSectionPointerValue, c3dFile.Header.PointerParameterSection);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(DataFlagFormatData))]
        public void DataFlag_Test(string filepath, StorageFormat expectedDataFormat)
        {
            C3dFile c3dFile = GetC3dFileWithHeader(filepath);
            Assert.Equal(expectedDataFormat, c3dFile.Header.StorageFormat);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(MarkerPerFrameData))]
        public void NbMarkerPerFrame_Test(string filepath, int expectedNumber)
        {
            C3dFile c3dFile = GetC3dFileWithHeader(filepath);
            Assert.Equal(expectedNumber, c3dFile.Header.MarkersPerFrame);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(PointScaleData))]
        public void PointScaleValue_Test(string filepath, float expectedPointScale)
        {
            C3dFile c3dFile = GetC3dFileWithHeader(filepath);
            Assert.Equal(expectedPointScale, c3dFile.Header.ScaleFactor);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(AcquisitionRate3dData))]
        public void AcquisitionRate3dValue_Test(string filepath, float expectedAcquisitionRate)
        {
            C3dFile c3dFile = GetC3dFileWithHeader(filepath);
            Assert.Equal(expectedAcquisitionRate, c3dFile.Header.Rate3dFrame);
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(EventTimeData))]
        public void EventTimeValue_Test(string filepath, float[] expectedEventTimes)
        {
            C3dFile c3dFile = GetC3dFileWithHeader(filepath);
            for (int i=0; i < c3dFile.Header.Events.Length;i++)
            {
                Assert.Equal(expectedEventTimes[i], c3dFile.Header.Events[i].EventTime);
            }
            c3dFile.CloseFileStream();
        }

        [Theory]
        [MemberData(nameof(DataPointerData))]
        public void DataPointer_Tests(string filepath, int expectedValuePointer)
        {
            C3dFile c3dFile = GetC3dFileWithHeader(filepath);
            ProcessorType processor = C3dFile.ReadProcessorByte(c3dFile.C3dStream);
            int pointerDataSection = C3dFile.GetDataSectionPointer(c3dFile.C3dStream, processor);
            Assert.Equal(expectedValuePointer, c3dFile.Header.PointerDataSection);
            c3dFile.CloseFileStream();
            c3dFile.CloseFileStream();
        }
    }
}