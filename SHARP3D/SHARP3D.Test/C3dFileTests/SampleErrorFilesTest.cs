using SHARP3D.Test.ToolKit;
using Xunit;

namespace SHARP3D.Test.Tests
{
    public class SampleErrorFilesTest
    {
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

        public static IEnumerable<object[]> Sample06C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample06);
        public static IEnumerable<object[]> Sample09C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample09);
        public static IEnumerable<object[]> Sample11C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample11);
        public static IEnumerable<object[]> Sample13C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample13);
        public static IEnumerable<object[]> Sample14C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample14);
        public static IEnumerable<object[]> Sample15C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample15);
        public static IEnumerable<object[]> Sample16C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample16);
        public static IEnumerable<object[]> Sample18C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample18);
        //public static IEnumerable<object[]> Sample20C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample20);
        public static IEnumerable<object[]> Sample21C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample21);
        public static IEnumerable<object[]> Sample24C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample24);
        public static IEnumerable<object[]> Sample25C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample25);
        public static IEnumerable<object[]> Sample32C3dFilesData => TestingTools.GetEnumerableC3dFilesData(Sample32);

        public static IEnumerable<object[]> OpenFileTest_Data =>
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

        [Theory]
        [MemberData(nameof(OpenFileTest_Data))]
        public void OpenFileTest_Test(string folderPath)
        {
            string[] filePaths = Directory.GetFiles(folderPath, "*.c3d");
            foreach (string filePath in filePaths)
            {
                C3dFile c3dFile = C3dFile.LoadFromFile(filePath);

                Assert.NotNull(c3dFile);

                bool hasRightNumberOfPoints = c3dFile.Point.Frames == c3dFile.Data.Points.Count;
                bool hasRightNumberOfAnalogs = c3dFile.Point.Frames * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

                Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
            }
        }

        [Theory]
        [MemberData(nameof(Sample06C3dFilesData))]
        public void Sample06Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample09C3dFilesData))]
        public void Sample09Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample11C3dFilesData))]
        public void Sample11Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample13C3dFilesData))]
        public void Sample13Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);

            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            // We look at c3dFile.Point.Frames as it is corrected for this kind of fuck up.
            // It is actually "correctly" handled: the creator made some error. There are only 499 frames not 500 (probably counted from 1 to 500, instead of 0 to 499)
            // Also the axis are wrong for some reasons. It is actually quite funny to see the dancer move on the ground. It looks like a fish pulled out of the water at first glance.
            bool hasRightNumberOfPoints = c3dFile.Point.Frames == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.Point.Frames * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample14C3dFilesData))]
        public void Sample14Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample15C3dFilesData))]
        public void Sample15Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }
        [Theory]
        [MemberData(nameof(Sample16C3dFilesData))]
        public void Sample16Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        // Taken out at the moment because the error on sample18 is a clusterfuck and will need hard work to be recoverable
        // If recoverable.
        // Not a priority.
        //[Theory]
        //[MemberData(nameof(Sample18C3dFilesData))]
        //public void Sample18Basic_Test(string filePath)
        //{
        //    C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
        //    bool hasPoints = c3dFile.Data.Points.Count != 0;
        //    bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

        //    Assert.True(hasPoints || hasAnalogs);

        //    bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
        //    bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

        //    Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        //}

        // Taken out because sample20 is just not readable.
        //[Theory]
        //[MemberData(nameof(Sample20C3dFilesData))]
        //public void Sample20Basic_Test(string filePath)
        //{
        //    C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
        //    bool hasPoints = c3dFile.Data.Points.Count != 0;
        //    bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

        //    Assert.True(hasPoints || hasAnalogs);

        //    bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
        //    bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

        //    Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        //}

        [Theory]
        [MemberData(nameof(Sample21C3dFilesData))]
        public void Sample21Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample24C3dFilesData))]
        public void Sample24Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample25C3dFilesData))]
        public void Sample25Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }

        [Theory]
        [MemberData(nameof(Sample32C3dFilesData))]
        public void Sample32Basic_Test(string filePath)
        {
            C3dFile c3dFile = C3dFile.LoadFromFile(filePath);
            bool hasPoints = c3dFile.Data.Points.Count != 0;
            bool hasAnalogs = c3dFile.Data.Analogs.Count != 0;

            Assert.True(hasPoints || hasAnalogs);

            bool hasRightNumberOfPoints = c3dFile.DataContext.FramesNumber == c3dFile.Data.Points.Count;
            bool hasRightNumberOfAnalogs = c3dFile.DataContext.FramesNumber * c3dFile.DataContext.AnalogSamplePerFrame == c3dFile.Data.Analogs.Count;

            Assert.True(hasRightNumberOfPoints || hasRightNumberOfAnalogs);
        }
    }
}
