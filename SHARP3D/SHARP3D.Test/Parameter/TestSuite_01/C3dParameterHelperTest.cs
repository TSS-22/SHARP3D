using SHARP3D.Parameter;

namespace SHARP3D.Test.Parameter.TestSuite_01
{
    public class C3dParameterHelperTest
    {

        [Fact]
        public void Reset_ShouldNotCrash_Test()
        {
            Exception ex = Record.Exception(() => C3dParameterHelper.Reset());
            Assert.Null(ex);
        }
    }
}
