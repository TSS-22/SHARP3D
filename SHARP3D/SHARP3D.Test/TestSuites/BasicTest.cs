using SHARP3D.Test.Utils;
using System.Net.Http.Json;
using System.Text.Json;

namespace SHARP3D.Test.TestSuites
{
    /// <summary>
    /// Test for the following values for the files from Sample01:
    /// <list type="bullet">
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.GroupsParameter"/>: Array of parameter group names.</description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.Parameters"/>: Jagged array of parameters names.</description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.PointFirst0"/>: The values (X, Y, Z) of the first channel of the first POINTS frame.</description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.PointLast0"/>: The values (X, Y, Z) of the first channel of the last POINTS frame.</description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.AnalogFirst0"/>: The value of the first channel of the first ANALOGS frame.</description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.AnalogLast0"/>: The value of the first channel of the last ANALOGS frame.</description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.PointFrames"/>: The number of POINTS frames.</description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SHARP3D.Test.Utils.BasicTestExpectedResults.AnalogFrames"/>: The number of ANALOGS frames.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class BasicTest
    {
        private static string FolderPath = @"..\..\..\TestFiles\Sample01"; // Replace with your folder path
        
        public static IEnumerable<object[]> Sample01Test_Basic_Data => 
            TestingTools.GetJsonAndC3dFileList(FolderPath)
                .Select(pair => new object[] { pair.jsonFile, pair.c3dFile });


        private static void AssertParameterGroupsMatch(BasicTestExpectedResults expectedResults, C3dFile c3dFile)
        {
            Assert.Equal(expectedResults.GroupsParameter.Length, c3dFile.ParameterCollection.ListGroups().Length);
            for (int i = 0; i < expectedResults.GroupsParameter.Length; i++)
            {
                Assert.Equal(expectedResults.GroupsParameter[i], c3dFile.ParameterCollection.ListGroups()[i]);
            }
        }

        private static void AssertParameterMatch(BasicTestExpectedResults expectedResults, C3dFile c3dFile)
        {
            Assert.Equal(expectedResults.GroupsParameter.Length, c3dFile.ParameterCollection.ListGroups().Length);
            for (int i = 0; i < expectedResults.GroupsParameter.Length; i++)
            {
                Assert.Equal(expectedResults.Parameters[i].Length, c3dFile.ParameterCollection.ListGroupParameters(i).Length);
                for (int j = 0; j < expectedResults.Parameters[i].Length; j++)
                {
                    Assert.Equal(expectedResults.Parameters[i][j], c3dFile.ParameterCollection.ListGroupParameters(i)[j].Item1);
                }
            }
        }

        [Theory]
        [MemberData(nameof(Sample01Test_Basic_Data))]
        public void BasicTest_Sample01(string jsonPath, string c3dPath)
        {

            string jsonContent = File.ReadAllText(jsonPath);
            BasicTestExpectedResults expectedResults = JsonSerializer.Deserialize<BasicTestExpectedResults>(jsonContent);
            C3dFile c3dFile = C3dFile.LoadFromFile(c3dPath);

            // Group
            AssertParameterGroupsMatch(expectedResults, c3dFile);
            AssertParameterMatch(expectedResults, c3dFile);


            Assert.True(true);
        }

    }
}
