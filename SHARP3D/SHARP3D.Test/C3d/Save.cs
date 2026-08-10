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
        //private static string FolderPath34 = @"..\..\..\SampleFiles\Sample34";
        private static string FolderPath35 = @"..\..\..\SampleFiles\Sample35";
        private static string FolderPath36 = @"..\..\..\SampleFiles\Sample36";
        private static string FolderPath37 = @"..\..\..\SampleFiles\Sample37";

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
            try
            {
                C3d test1 = new C3d(c3dPath);
                test1.Save(Path.GetDirectoryName(c3dPath), tempFileName);
                Assert.True(File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"));
            }
            finally 
            {
                if (File.Exists($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d"))
                {
                    File.Delete($"{Path.GetDirectoryName(c3dPath)}\\{tempFileName}.c3d");
                }
            }
        }
    }
}
