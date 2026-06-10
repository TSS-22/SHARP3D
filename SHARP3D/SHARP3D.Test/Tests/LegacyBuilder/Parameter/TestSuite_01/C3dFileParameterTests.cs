using SHARP3D.C3d;

namespace SHARP3D.Test.Tests.BuilderTests.Parameter.TestSuite_01
{
    public class C3dFileParameterTests
    {

        public static readonly string PathEb015pi = @"..\..\..\SampleFiles\Sample01\Eb015pi.c3d";
        public static readonly string PathEb015pr = @"..\..\..\SampleFiles\Sample01\Eb015pr.c3d";
        public static readonly string PathEb015si = @"..\..\..\SampleFiles\Sample01\Eb015si.c3d";
        public static readonly string PathEb015sr = @"..\..\..\SampleFiles\Sample01\Eb015sr.c3d";
        public static readonly string PathEb015vi = @"..\..\..\SampleFiles\Sample01\Eb015vi.c3d";
        public static readonly string PathEb015vr = @"..\..\..\SampleFiles\Sample01\Eb015vr.c3d";

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

        internal C3dFile GetC3dFileWithparameter(string filePath)
        {
            FileStream fileStream = C3dFile.OpenC3dFile(filePath);
            C3dFile c3dFile = new C3dFile();
            c3dFile.ProcessorFile = C3dFile.ReadProcessorByte(fileStream);
            
            c3dFile.Parameters = c3dFile.GetParameters(fileStream, c3dFile.ProcessorFile, 512, 5120);
            return c3dFile;
        }

        [Theory]
        [MemberData(nameof(FileStreamData))]
        public void RunningParameter_Test(string filepath)
        {
            C3dFile test = GetC3dFileWithparameter(filepath);
            Assert.NotNull(test);
        }
    }
}