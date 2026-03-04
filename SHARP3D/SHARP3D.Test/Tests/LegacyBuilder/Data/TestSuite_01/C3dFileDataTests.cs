namespace SHARP3D.Test.Tests.BuilderTests.Data.TestSuite_01
{

    public class C3dFileDataTests
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

        [Theory]
        [MemberData(nameof(FileStreamData))]
        public void ReadDataWithoutCrashing_Test(string filepath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filepath);
            Assert.NotNull(c3dFile);
        }
    }
}