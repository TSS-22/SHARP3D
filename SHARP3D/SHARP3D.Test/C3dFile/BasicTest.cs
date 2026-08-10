using SHARP3D.Test.ToolKit;
using System.Diagnostics;
using System.Text.Json;

namespace SHARP3D.Test.Tests
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
        

        private static string FolderPath00_ARTG = @"..\..\..\SampleFiles\Sample00\Advanced Realtime Tracking GmbH";
        private static string FolderPath00_C = @"..\..\..\SampleFiles\Sample00\Codamotion";
        private static string FolderPath00_CS = @"..\..\..\SampleFiles\Sample00\Cometa Systems";
        private static string FolderPath00_IST = @"..\..\..\SampleFiles\Sample00\Innovative Sports Training";
        private static string FolderPath00_MAC = @"..\..\..\SampleFiles\Sample00\Motion Analysis Corporation";
        private static string FolderPath00_NE = @"..\..\..\SampleFiles\Sample00\NexGen Ergonomics";
        private static string FolderPath00_VMS = @"..\..\..\SampleFiles\Sample00\Vicon Motion Systems";
        private static string FolderPath01 = @"..\..\..\SampleFiles\Sample01";
        private static string FolderPath02 = @"..\..\..\SampleFiles\Sample02";
        private static string FolderPath03 = @"..\..\..\SampleFiles\Sample03";
        private static string FolderPath04 = @"..\..\..\SampleFiles\Sample04";
        private static string FolderPath05 = @"..\..\..\SampleFiles\Sample05";
        private static string FolderPath07 = @"..\..\..\SampleFiles\Sample07";
        private static string FolderPath08 = @"..\..\..\SampleFiles\Sample08";
        private static string FolderPath10 = @"..\..\..\SampleFiles\Sample10";
        private static string FolderPath12 = @"..\..\..\SampleFiles\Sample12";
        private static string FolderPath17 = @"..\..\..\SampleFiles\Sample17";
        private static string FolderPath19 = @"..\..\..\SampleFiles\Sample19";
        private static string FolderPath22 = @"..\..\..\SampleFiles\Sample22";
        private static string FolderPath23 = @"..\..\..\SampleFiles\Sample23";
        private static string FolderPath26 = @"..\..\..\SampleFiles\Sample26";
        private static string FolderPath27 = @"..\..\..\SampleFiles\Sample27";
        private static string FolderPath28 = @"..\..\..\SampleFiles\Sample28";
        private static string FolderPath29 = @"..\..\..\SampleFiles\Sample29";
        private static string FolderPath30 = @"..\..\..\SampleFiles\Sample30";
        private static string FolderPath31 = @"..\..\..\SampleFiles\Sample31";
        private static string FolderPath33 = @"..\..\..\SampleFiles\Sample33";
        private static string FolderPath35 = @"..\..\..\SampleFiles\Sample35";
        private static string FolderPath36 = @"..\..\..\SampleFiles\Sample36";
        private static string FolderPath37 = @"..\..\..\SampleFiles\Sample37";

        public static IEnumerable<object[]> Test_Basic_Data =>
            new[]
            {
                FolderPath00_ARTG,
                FolderPath00_C,
                FolderPath00_CS,
                FolderPath00_IST,
                FolderPath00_MAC,
                FolderPath00_NE,
                FolderPath00_VMS,
                FolderPath01,
                FolderPath02,
                FolderPath03,
                FolderPath04,
                FolderPath05,
                FolderPath07,
                FolderPath08,
                FolderPath10,
                FolderPath12,
                FolderPath17,
                FolderPath19,
                FolderPath22,
                FolderPath23,
                FolderPath26,
                FolderPath27,
                FolderPath28,
                FolderPath29,
                FolderPath30,
                //FolderPath31,
                FolderPath33,
                FolderPath35,
                FolderPath36,
                FolderPath37
            }
            .SelectMany(folder =>
                TestingTools.GetJsonAndC3dFileList(folder)
                    .Select(pair => new object[] { pair.jsonFile, pair.c3dFile })
            );

        public static IEnumerable<object[]> Data_Full =>
            new[]
            {
                FolderPath00_ARTG,
                FolderPath00_C,
                FolderPath00_CS,
                FolderPath00_IST,
                FolderPath00_MAC,
                FolderPath00_NE,
                FolderPath00_VMS,
                FolderPath01,
                FolderPath02,
                FolderPath03,
                FolderPath04,
                FolderPath05,
                FolderPath07,
                FolderPath08,
                FolderPath10,
                FolderPath12,
                FolderPath17,
                FolderPath19,
                FolderPath22,
                FolderPath23,
                FolderPath26,
                FolderPath27,
                FolderPath28,
                FolderPath29,
                FolderPath30,
                //FolderPath31,
                FolderPath33,
                FolderPath35,
                FolderPath36,
                FolderPath37
            }
            .SelectMany(folder =>
                Directory.GetFiles(folder, "*.c3d")
                    .Select(files => new object[] { files })
            );

        public static IEnumerable<object[]> Sample36C3dFilesData => TestingTools.GetEnumerableC3dFilesData(FolderPath36);

        public static IEnumerable<object[]> Sample29C3dFilesData => TestingTools.GetEnumerableC3dFilesData(FolderPath29);

        private static void AssertParameterGroupsMatch(BasicTestExpectedResults expectedResults, C3dFile c3dFile)
        {
            Assert.Equal(expectedResults.GroupsParameter.Length, c3dFile.ParameterCollection.ListGroups().Length);
            // Sort the group because with the empty group from EZC3D, the order is off when that happens.
            string[] sortedExpectedGroupsParameter = expectedResults.GroupsParameter.OrderBy(group => group).ToArray();
            string[] sortedActualGroupsParameter = c3dFile.ParameterCollection.ListGroups().OrderBy(group => group).ToArray();

            for (int i = 0; i < expectedResults.GroupsParameter.Length; i++)
            {
                Assert.Equal(sortedExpectedGroupsParameter[i], sortedActualGroupsParameter[i]);
            }
        }

        private static void AssertParameterMatch(BasicTestExpectedResults expectedResults, C3dFile c3dFile)
        {
            Assert.Equal(expectedResults.GroupsParameter.Length, c3dFile.ParameterCollection.ListGroups().Length);
            string[] sortedExpectedGroupsParameter = expectedResults.GroupsParameter.OrderBy(group => group).ToArray();
            string[] sortedActualGroupsParameter = c3dFile.ParameterCollection.ListGroups().OrderBy(group => group).ToArray();

            // Sort the group because with the empty group from EZC3D, the order is off when that happens.

            // Sort Expected and get corresponding index shuffle
            int[] indicesExpected = Enumerable.Range(0, expectedResults.GroupsParameter.Length).ToArray();
            string[] sortedExpectedGroup = expectedResults.GroupsParameter;
            Array.Sort(sortedExpectedGroup, indicesExpected);

            // Sort Actual and get corresponding index shuffle
            int[] indicesActual = Enumerable.Range(0, c3dFile.ParameterCollection.ListGroups().Length).ToArray();
            string[] sortedActualGroup = c3dFile.ParameterCollection.ListGroups();
            Array.Sort(sortedActualGroup, indicesActual);

            for (int i = 0; i < expectedResults.GroupsParameter.Length; i++)
            {
                // Because some files read calibration matrix with just one zero value. And I discard those zero cal_matrix from my testing pool.
                if ((sortedActualGroup[i] == "FORCE_PLATFORM") && (expectedResults.Parameters[indicesExpected[i]].Length == c3dFile.ParameterCollection.ListGroupParameters(indicesActual[i]).Length - 1))
                {
                    Assert.Equal(expectedResults.Parameters[indicesExpected[i]].Length, c3dFile.ParameterCollection.ListGroupParameters(indicesActual[i]).Length - 1);
                    (string, int)[] filteredKeys = c3dFile.ParameterCollection.ListGroupParameters(indicesActual[i]).Where(k => k.Item1 != "CAL_MATRIX").ToArray();

                    for (int j = 0; j < expectedResults.Parameters[indicesExpected[i]].Length; j++)
                    {
                        Assert.Equal(expectedResults.Parameters[indicesExpected[i]][j], filteredKeys[j].Item1);
                    }
                }
                else 
                {
                    Assert.Equal(expectedResults.Parameters[indicesExpected[i]].Length, c3dFile.ParameterCollection.ListGroupParameters(indicesActual[i]).Length);

                    for (int j = 0; j < expectedResults.Parameters[indicesExpected[i]].Length; j++)
                    {
                        Assert.Equal(expectedResults.Parameters[indicesExpected[i]][j], c3dFile.ParameterCollection.ListGroupParameters(indicesActual[i])[j].Item1);
                    }
                }

                
            }
        }

        private static void AssertPointsDataMatch(BasicTestExpectedResults expectedResults, C3dFile c3dFile)
        {
            // First frame point
            if (c3dFile.Data.Points[0][0].Valid == true)
            {
                for (int i = 0; i < 3; i++)
                {
                    Assert.Equal(expectedResults.PointFirst0[i], c3dFile.Data.Points[0][0].Point[i]);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (expectedResults.PointFirst0[i] == null)
                    {
                        Assert.Null(expectedResults.PointFirst0[i]);
                    }
                    else 
                    {
                        Assert.Equal(expectedResults.PointFirst0[i], c3dFile.Data.Points[0][0].Point[i]);
                    }
                        
                }
            }

            // Last frame point
            if (c3dFile.Data.Points[c3dFile.Data.Points.Count - 1][0].Valid == true)
            {
                for (int i = 0; i < 3; i++)
                {
                    Assert.Equal((double)expectedResults.PointLast0[i], (double)c3dFile.Data.Points[c3dFile.Data.Points.Count - 1][0].Point[i], precision: 2);
                }
            }
            else 
            {
                for (int i = 0; i < 3; i++)
                {
                    if (expectedResults.PointLast0[i] == null)
                    {
                        Assert.Null(expectedResults.PointLast0[i]); // The fuck
                    }
                    else
                    {
                        Assert.Equal(expectedResults.PointLast0[i], c3dFile.Data.Points[c3dFile.Data.Points.Count - 1][0].Point[i]);
                    }
                }
            }
            
        }

        [Theory]
        [MemberData(nameof(Test_Basic_Data))]
        public void AssertAgainstEzc3d_Tests(string jsonPath, string c3dPath)
        {

            string jsonContent = File.ReadAllText(jsonPath);
            BasicTestExpectedResults expectedResults = JsonSerializer.Deserialize<BasicTestExpectedResults>(jsonContent);
            C3dFile c3dFile = C3dFile.LoadFromFile(c3dPath);
            
            Debug.WriteLine(c3dPath);

            // Assert groups
            AssertParameterGroupsMatch(expectedResults, c3dFile);
            // Assert parameters
            AssertParameterMatch(expectedResults, c3dFile);
            // Assert first and last frame value of the first channel
            // POINTS
            AssertPointsDataMatch(expectedResults, c3dFile);
            // ANALOGS
            Assert.Equal(expectedResults.AnalogFirst0, c3dFile.Data.Analogs[0][0][0]);
            //c3dFile.Data.Points.Count - 1
            Assert.Equal(expectedResults.AnalogLast0, c3dFile.Data.Analogs[c3dFile.Data.Analogs.Count - 1][c3dFile.Data.Analogs[c3dFile.Data.Analogs.Count - 1].Length - 1][0]);

            // Assert the Frames count
            // POINTS
            Assert.Equal(expectedResults.PointFrames, c3dFile.Data.Points.Count);
            // ANALOGS
            if (c3dFile.Data.Analogs.Count != 0)
            {
                Assert.Equal(expectedResults.AnalogFrames, c3dFile.Data.Analogs.Count * c3dFile.Data.Analogs[0].Length);
            }
            else
            {
                Assert.Equal(expectedResults.AnalogFrames, c3dFile.Data.Analogs.Count);
            }

        }

        [Theory]
        [MemberData(nameof(Sample36C3dFilesData))]
        public void ReadsFloatingFrameNumber(string filePath)
        {
  
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            // This file is badly built and doesn't have enough data for the amount frame advertised. 
            Assert.Equal(c3dFile.Point.Frames, c3dFile.Data.Points.Count);
        }

        [Theory]
        [MemberData(nameof(Sample29C3dFilesData))]
        public void ReadsNoAnalogFiles(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);

            Assert.Equal(c3dFile.DataContext.FramesNumber, c3dFile.Data.Points.Count);
        }

        [Theory]
        [MemberData(nameof(Data_Full))]
        public void OpenC3dFile_Tests(string c3dPath)
        {
            C3dFile c3DFile = C3dFile.LoadFromFile(c3dPath);
            Assert.NotNull(c3DFile);
        }
    }
}
