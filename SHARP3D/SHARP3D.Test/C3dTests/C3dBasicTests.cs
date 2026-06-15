namespace SHARP3D.Test.C3dTests
{
    public class C3dBasicTests
    {
        public static readonly string PathEb015pi = @"..\..\..\SampleFiles\Sample01\Eb015pi.c3d";

        [Fact]
        public void C3dFileConstructor_Test()
        {
            C3d c3d = new C3d(PathEb015pi);
            Assert.NotNull(c3d);
        }

        [Fact]
        public void C3dEmptyConstructor_Test()
        {
            C3d c3d = new C3d();
            Assert.NotNull(c3d);
        }


    }
}
