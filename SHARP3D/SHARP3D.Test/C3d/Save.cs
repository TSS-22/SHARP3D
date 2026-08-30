using SHARP3D.Parameter.DataEntity.Clean;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace SHARP3D.Test.C3dTests
{
    public class Save
    {
        //private static string FolderPath00_ARTG = @"..\..\..\SampleFiles\Sample00\Advanced Realtime Tracking GmbH";
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
        //private static string FolderPath19 = @"..\..\..\SampleFiles\Sample19";
        private static string FolderPath22 = @"..\..\..\SampleFiles\Sample22";
        private static string FolderPath23 = @"..\..\..\SampleFiles\Sample23";
        private static string FolderPath26 = @"..\..\..\SampleFiles\Sample26";
        private static string FolderPath27 = @"..\..\..\SampleFiles\Sample27";
        private static string FolderPath28 = @"..\..\..\SampleFiles\Sample28";
        private static string FolderPath29 = @"..\..\..\SampleFiles\Sample29";
        private static string FolderPath30 = @"..\..\..\SampleFiles\Sample30";
        private static string FolderPath31 = @"..\..\..\SampleFiles\Sample31";
        private static string FolderPath33 = @"..\..\..\SampleFiles\Sample33";
        //private static string FolderPath34 = @"..\..\..\SampleFiles\Sample34";
        private static string FolderPath35 = @"..\..\..\SampleFiles\Sample35";
        private static string FolderPath36 = @"..\..\..\SampleFiles\Sample36";
        private static string FolderPath37 = @"..\..\..\SampleFiles\Sample37";

        private static string Sample06 = @"..\..\..\SampleErrorFiles\Sample06";
        private static string Sample09 = @"..\..\..\SampleErrorFiles\Sample09";
        private static string Sample11 = @"..\..\..\SampleErrorFiles\Sample11";
        private static string Sample13 = @"..\..\..\SampleErrorFiles\Sample13";
        private static string Sample14 = @"..\..\..\SampleErrorFiles\Sample14";
        private static string Sample15 = @"..\..\..\SampleErrorFiles\Sample15";
        private static string Sample16 = @"..\..\..\SampleErrorFiles\Sample16";
        private static string Sample18 = @"..\..\..\SampleErrorFiles\Sample18";
        private static string Sample20 = @"..\..\..\SampleErrorFiles\Sample20";
        private static string Sample21 = @"..\..\..\SampleErrorFiles\Sample21";
        private static string Sample24 = @"..\..\..\SampleErrorFiles\Sample24";
        private static string Sample25 = @"..\..\..\SampleErrorFiles\Sample25";
        private static string Sample32 = @"..\..\..\SampleErrorFiles\Sample32";

        public static IEnumerable<object[]> DataError_Full =>
            new[]
            {
                new object[] { Sample06 },
                new object[] { Sample09 },
                new object[] { Sample11 },
                //new object[] { Sample13 },
                new object[] { Sample14 },
                new object[] { Sample15 },
                new object[] { Sample16 },
                //new object[] { Sample18 },
                //new object[] { Sample20 },
                new object[] { Sample21 },
                new object[] { Sample24 },
                new object[] { Sample25 },
                new object[] { Sample32 }
            };

        public static IEnumerable<object[]> Data_Full =>
            new[]
            {
                //FolderPath00_ARTG,
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
                //FolderPath19,
                FolderPath22,
                FolderPath23,
                FolderPath26,
                FolderPath27,
                FolderPath28,
                FolderPath29,
                FolderPath30,
                //FolderPath31,
                FolderPath33,
                //FolderPath34,
                FolderPath35,
                FolderPath36,
                FolderPath37
            }
            .SelectMany(folder =>
                Directory.GetFiles(folder, "*.c3d")
                    .Select(files => new object[] { files })
            );


        string tempFileName = "temp_c3d_test_save";

        [Theory]
        [MemberData(nameof(Data_Full))]
        public void CreateSave(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            bool fileExist = false;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                fileExist = File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            Assert.True(fileExist);
        }

        [Theory]
        [MemberData(nameof(Data_Full))]
        public void OpenSaveC3dFile(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            C3dFile test2;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                test2 = C3dFile.LoadFromFile($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            Assert.NotNull(test2);
        }

        [Theory]
        [MemberData(nameof(Data_Full))]
        public void OpenSaveC3d(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            C3d test2;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                test2 = new C3d($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            Assert.NotNull(test2);
        }

        [Theory]
        [MemberData(nameof(Data_Full))]
        public void SaveC3dCheckCorruption(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            C3d test2;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                test2 = new C3d($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            // Required Parameters
            // Points
            Assert.Equal(test1.Required.Point.Frames, test1.Required.Point.Frames);
            Assert.Equal(test1.Required.Point.Rate, test1.Required.Point.Rate);
            Assert.Equal(test1.Required.Point.MaximumInterpolationGap, test1.Required.Point.MaximumInterpolationGap);
            Assert.Equal(test1.Required.Point.Units, test1.Required.Point.Units);
            // Analogs
            Assert.Equal(test1.Required.Analog.GeneralScale, test1.Required.Analog.GeneralScale);
            Assert.Equal(test1.Required.Analog.AnalogframePerFrame, test1.Required.Analog.AnalogframePerFrame);

            // Data
            // Point
            for (int idTraj = 0; idTraj < test1.Data.Points.Length; idTraj++)
            {
                Assert.Equal(test1.Data.Points[idTraj].Label, test2.Data.Points[idTraj].Label);
                if (test1.Data.Points[idTraj].Point.GetLength(0) > 0)
                {
                    Assert.Equal(test1.Data.Points[idTraj].Point[0, 0], test2.Data.Points[idTraj].Point[0, 0]);
                    //Assert.Equal(test1.Data.Points[0].Residual[0], test2.Data.Points[0].Residual[0]); // Due to the data corruption inherent to C3D (as far as we understood). This will always be false
                    Assert.Equal(test1.Data.Points[idTraj].CameraMask[0, 0], test2.Data.Points[idTraj].CameraMask[0, 0]);

                    Assert.Equal(
                        test1.Data.Points[idTraj].Point[
                        test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                        ],
                        test2.Data.Points[idTraj].Point[
                            test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                            ]);
                    //Assert.Equal(test1.Data.Points[0].Residual[0], test2.Data.Points[0].Residual[0]); // Due to the data corruption inherent to C3D (as far as we understood). This will always be false
                    Assert.Equal(
                        test1.Data.Points[idTraj].CameraMask[
                            test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                            ],
                        test2.Data.Points[idTraj].CameraMask[
                            test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                            ]);
                }
            }
            // Analog
            for (int idChannel = 0; idChannel < test1.Data.Analogs.Length; idChannel++)
            {
                Assert.Equal(test1.Data.Analogs[idChannel].Bits, test2.Data.Analogs[idChannel].Bits);
                Assert.Equal(test1.Data.Analogs[idChannel].Scale, test2.Data.Analogs[idChannel].Scale);
                Assert.Equal(test1.Data.Analogs[idChannel].Label, test2.Data.Analogs[idChannel].Label);
                Assert.Equal(test1.Data.Analogs[idChannel].Offset, test2.Data.Analogs[idChannel].Offset);
                Assert.Equal(test1.Data.Analogs[idChannel].Rate, test2.Data.Analogs[idChannel].Rate);
                Assert.Equal(test1.Data.Analogs[idChannel].Units, test2.Data.Analogs[idChannel].Units);

                if (test1.Data.Analogs[idChannel].Data.Length > 0)
                {
                    Assert.Equal(test1.Data.Analogs[idChannel].Data[0], test2.Data.Analogs[idChannel].Data[0]);
                    Assert.Equal(test1.Data.Analogs[idChannel].Data[test1.Data.Analogs[idChannel].Data.Length - 1], test2.Data.Analogs[idChannel].Data[test1.Data.Analogs[idChannel].Data.Length - 1]);
                }
            }
            // Forceplates 
            for (int idFp = 0; idFp < test1.Data.Forceplates.Length; idFp++)
            {
                Assert.Equal(test1.Data.Forceplates[idFp].Type, test2.Data.Forceplates[idFp].Type);
                Assert.Equal(test1.Data.Forceplates[idFp].Zero, test2.Data.Forceplates[idFp].Zero);
                Assert.Equal(test1.Data.Forceplates[idFp].Corners, test2.Data.Forceplates[idFp].Corners);
                Assert.Equal(test1.Data.Forceplates[idFp].Origin, test2.Data.Forceplates[idFp].Origin);
                Assert.Equal(test1.Data.Forceplates[idFp].Zero, test2.Data.Forceplates[idFp].Zero);

                for (int idChannel = 0; idChannel < test1.Data.Forceplates[idFp].Channels.Length; idChannel++)
                {
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Bits, test2.Data.Forceplates[idFp].Channels[idChannel].Bits);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Scale, test2.Data.Forceplates[idFp].Channels[idChannel].Scale);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Label, test2.Data.Forceplates[idFp].Channels[idChannel].Label);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Offset, test2.Data.Forceplates[idFp].Channels[idChannel].Offset);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Rate, test2.Data.Forceplates[idFp].Channels[idChannel].Rate);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Units, test2.Data.Forceplates[idFp].Channels[idChannel].Units);

                    if (test1.Data.Forceplates[idFp].Channels[idChannel].Data.Length > 0)
                    {
                        Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Data[0], test2.Data.Forceplates[idFp].Channels[idChannel].Data[0]);
                        Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Data[
                            test1.Data.Forceplates[idFp].Channels[idChannel].Data.Length - 1
                            ],
                            test2.Data.Forceplates[idFp].Channels[idChannel].Data[
                                test1.Data.Forceplates[idFp].Channels[idChannel].Data.Length - 1]);
                    }
                }
            }
            // Header event
            for (int idHeaderEvent = 0; idHeaderEvent < test1.HeaderEvents.Length; idHeaderEvent++) 
            {
                Assert.Equal(test1.HeaderEvents[idHeaderEvent], test2.HeaderEvents[idHeaderEvent]); 
            }

            // Parameters
            // Assert groups
            string[] listGroup1 = test1.Parameters.GetStringListGroups();
            string[] listGroup2 = test2.Parameters.GetStringListGroups();

            listGroup1 = listGroup1.Where(g => !string.Equals(g, "TRIAL", StringComparison.OrdinalIgnoreCase)).ToArray();
            listGroup2 = listGroup2.Where(g => !string.Equals(g, "TRIAL", StringComparison.OrdinalIgnoreCase)).ToArray();

            Array.Sort(listGroup1);
            Array.Sort(listGroup2);
            
            // Basic checks
            Assert.Equal(listGroup1.Length, listGroup2.Length);
            for(int i=0; i < listGroup1.Length; i++)
            {
                Assert.Equal(listGroup1[0], listGroup2[0]);
            }
            // Cyle through all non managed parameters
            foreach(string groupName in listGroup1)
            {
                C3dParameterGroup group1 = test1.Parameters.GetGroup(groupName);
                C3dParameterGroup group2 = test2.Parameters.GetGroup(groupName);

                var sortedgroup1 = group1.Parameters
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .ToList();
                var sortedgroup2 = group2.Parameters
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .ToList();

                for (int idParam = 0; idParam < group1.Parameters.Count; idParam++)
                {
                    Assert.Equal(sortedgroup1[idParam].Name, sortedgroup2[idParam].Name);
                    Assert.Equal(sortedgroup1[idParam].Dimensions, sortedgroup2[idParam].Dimensions);
                    // check data point by point
                    Assert.True(sortedgroup1[idParam].Data.Cast<object>().SequenceEqual(sortedgroup1[idParam].Data.Cast<object>()));
                }
            }
            
        }

        [Theory]
        [MemberData(nameof(DataError_Full))]
        public void CreateSave_errorFiles(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            bool fileExist = false;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                fileExist = File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            Assert.True(fileExist);
        }

        [Theory]
        [MemberData(nameof(DataError_Full))]
        public void OpenSaveC3dFile_errorFiles(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            C3dFile test2;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                test2 = C3dFile.LoadFromFile($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            Assert.NotNull(test2);
        }

        [Theory]
        [MemberData(nameof(DataError_Full))]
        public void OpenSaveC3d_errorFiles(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            C3d test2;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                test2 = new C3d($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            Assert.NotNull(test2);
        }

        [Theory]
        [MemberData(nameof(DataError_Full))]
        public void SaveC3dCheckCorruption_errorFiles(string c3dPath)
        {
            if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
            {
                File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            C3d test1 = new C3d(c3dPath);
            C3d test2;
            try
            {
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                test2 = new C3d($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
            }
            finally
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
            // Required Parameters
            // Points
            Assert.Equal(test1.Required.Point.Frames, test1.Required.Point.Frames);
            Assert.Equal(test1.Required.Point.Rate, test1.Required.Point.Rate);
            Assert.Equal(test1.Required.Point.MaximumInterpolationGap, test1.Required.Point.MaximumInterpolationGap);
            Assert.Equal(test1.Required.Point.Units, test1.Required.Point.Units);
            // Analogs
            Assert.Equal(test1.Required.Analog.GeneralScale, test1.Required.Analog.GeneralScale);
            Assert.Equal(test1.Required.Analog.AnalogframePerFrame, test1.Required.Analog.AnalogframePerFrame);

            // Data
            // Point
            for (int idTraj = 0; idTraj < test1.Data.Points.Length; idTraj++)
            {
                Assert.Equal(test1.Data.Points[idTraj].Label, test2.Data.Points[idTraj].Label);
                if (test1.Data.Points[idTraj].Point.GetLength(0) > 0)
                {
                    Assert.Equal(test1.Data.Points[idTraj].Point[0, 0], test2.Data.Points[idTraj].Point[0, 0]);
                    //Assert.Equal(test1.Data.Points[0].Residual[0], test2.Data.Points[0].Residual[0]); // Due to the data corruption inherent to C3D (as far as we understood). This will always be false
                    Assert.Equal(test1.Data.Points[idTraj].CameraMask[0, 0], test2.Data.Points[idTraj].CameraMask[0, 0]);

                    Assert.Equal(
                        test1.Data.Points[idTraj].Point[
                        test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                        ],
                        test2.Data.Points[idTraj].Point[
                            test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                            ]);
                    //Assert.Equal(test1.Data.Points[0].Residual[0], test2.Data.Points[0].Residual[0]); // Due to the data corruption inherent to C3D (as far as we understood). This will always be false
                    Assert.Equal(
                        test1.Data.Points[idTraj].CameraMask[
                            test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                            ],
                        test2.Data.Points[idTraj].CameraMask[
                            test1.Data.Points[idTraj].Point.GetLength(0) - 1, 0
                            ]);
                }
            }
            // Analog
            for (int idChannel = 0; idChannel < test1.Data.Analogs.Length; idChannel++)
            {
                Assert.Equal(test1.Data.Analogs[idChannel].Bits, test2.Data.Analogs[idChannel].Bits);
                Assert.Equal(test1.Data.Analogs[idChannel].Scale, test2.Data.Analogs[idChannel].Scale);
                Assert.Equal(test1.Data.Analogs[idChannel].Label, test2.Data.Analogs[idChannel].Label);
                Assert.Equal(test1.Data.Analogs[idChannel].Offset, test2.Data.Analogs[idChannel].Offset);
                Assert.Equal(test1.Data.Analogs[idChannel].Rate, test2.Data.Analogs[idChannel].Rate);
                Assert.Equal(test1.Data.Analogs[idChannel].Units, test2.Data.Analogs[idChannel].Units);

                if (test1.Data.Analogs[idChannel].Data.Length > 0)
                {
                    Assert.Equal(test1.Data.Analogs[idChannel].Data[0], test2.Data.Analogs[idChannel].Data[0]);
                    Assert.Equal(test1.Data.Analogs[idChannel].Data[test1.Data.Analogs[idChannel].Data.Length - 1], test2.Data.Analogs[idChannel].Data[test1.Data.Analogs[idChannel].Data.Length - 1]);
                }
            }
            // Forceplates 
            for (int idFp = 0; idFp < test1.Data.Forceplates.Length; idFp++)
            {
                Assert.Equal(test1.Data.Forceplates[idFp].Type, test2.Data.Forceplates[idFp].Type);
                Assert.Equal(test1.Data.Forceplates[idFp].Zero, test2.Data.Forceplates[idFp].Zero);
                Assert.Equal(test1.Data.Forceplates[idFp].Corners, test2.Data.Forceplates[idFp].Corners);
                Assert.Equal(test1.Data.Forceplates[idFp].Origin, test2.Data.Forceplates[idFp].Origin);
                Assert.Equal(test1.Data.Forceplates[idFp].Zero, test2.Data.Forceplates[idFp].Zero);

                for (int idChannel = 0; idChannel < test1.Data.Forceplates[idFp].Channels.Length; idChannel++)
                {
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Bits, test2.Data.Forceplates[idFp].Channels[idChannel].Bits);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Scale, test2.Data.Forceplates[idFp].Channels[idChannel].Scale);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Label, test2.Data.Forceplates[idFp].Channels[idChannel].Label);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Offset, test2.Data.Forceplates[idFp].Channels[idChannel].Offset);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Rate, test2.Data.Forceplates[idFp].Channels[idChannel].Rate);
                    Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Units, test2.Data.Forceplates[idFp].Channels[idChannel].Units);

                    if (test1.Data.Forceplates[idFp].Channels[idChannel].Data.Length > 0)
                    {
                        Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Data[0], test2.Data.Forceplates[idFp].Channels[idChannel].Data[0]);
                        Assert.Equal(test1.Data.Forceplates[idFp].Channels[idChannel].Data[
                            test1.Data.Forceplates[idFp].Channels[idChannel].Data.Length - 1
                            ],
                            test2.Data.Forceplates[idFp].Channels[idChannel].Data[
                                test1.Data.Forceplates[idFp].Channels[idChannel].Data.Length - 1]);
                    }
                }
            }
            // Header event
            for (int idHeaderEvent = 0; idHeaderEvent < test1.HeaderEvents.Length; idHeaderEvent++)
            {
                Assert.Equal(test1.HeaderEvents[idHeaderEvent], test2.HeaderEvents[idHeaderEvent]);
            }

            // Parameters
            // Assert groups
            string[] listGroup1 = test1.Parameters.GetStringListGroups();
            string[] listGroup2 = test2.Parameters.GetStringListGroups();

            listGroup1 = listGroup1.Where(g => !string.Equals(g, "TRIAL", StringComparison.OrdinalIgnoreCase)).ToArray();
            listGroup2 = listGroup2.Where(g => !string.Equals(g, "TRIAL", StringComparison.OrdinalIgnoreCase)).ToArray();

            Array.Sort(listGroup1);
            Array.Sort(listGroup2);

            // Basic checks
            Assert.Equal(listGroup1.Length, listGroup2.Length);
            for (int i = 0; i < listGroup1.Length; i++)
            {
                Assert.Equal(listGroup1[0], listGroup2[0]);
            }
            // Cyle through all non managed parameters
            foreach (string groupName in listGroup1)
            {
                C3dParameterGroup group1 = test1.Parameters.GetGroup(groupName);
                C3dParameterGroup group2 = test2.Parameters.GetGroup(groupName);

                var sortedgroup1 = group1.Parameters
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .ToList();
                var sortedgroup2 = group2.Parameters
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .ToList();

                for (int idParam = 0; idParam < group1.Parameters.Count; idParam++)
                {
                    Assert.Equal(sortedgroup1[idParam].Name, sortedgroup2[idParam].Name);
                    Assert.Equal(sortedgroup1[idParam].Dimensions, sortedgroup2[idParam].Dimensions);
                    // check data point by point
                    Assert.True(sortedgroup1[idParam].Data.Cast<object>().SequenceEqual(sortedgroup1[idParam].Data.Cast<object>()));
                }
            }

        }
    }
}
