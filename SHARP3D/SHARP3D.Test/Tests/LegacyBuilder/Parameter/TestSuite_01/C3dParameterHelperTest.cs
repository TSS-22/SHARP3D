using SHARP3D.Parameter;

namespace SHARP3D.Test.Tests.BuilderTests.Parameter.TestSuite_01
{
    public class C3dParameterHelperTest
    {
        public static readonly string RequiredParameters = @"..\..\..\..\SHARP3D\Resources\RequiredParameters.json";
        public static readonly string AdditionalParameters= @"..\..\..\..\SHARP3D\Resources\AdditionalParameters.json";
        public static readonly string ApplicationParameters = @"..\..\..\..\SHARP3D\Resources\ApplicationParameters.json";
        public static readonly string UserDefinedParameters = @"..\..\..\..\SHARP3D\Resources\UserDefinedParameters.json";

        public static IEnumerable<object[]> JsonFiles =>
            new List<object[]>
            {
                new object[] { RequiredParameters },
                new object[] { AdditionalParameters },
                new object[] { ApplicationParameters },
                new object[] { UserDefinedParameters },
            };

        [Fact]
        public void Reset_ShouldNotCrash_Test()
        {
            Exception ex = Record.Exception(() => C3dParameterHelper.Reset());
            Assert.Null(ex);
        }

        [Theory]
        [MemberData(nameof(JsonFiles))]
        public void LoadJson_ShouldReturnJson(string filePath)
        {
            SupportedParameter[] result = C3dParameterHelper.LoadJson(filePath);
            Assert.IsType<SupportedParameter[]?>(result);
        }
    }
}
