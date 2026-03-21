namespace SHARP3D.Test.ToolKit
{
    public static class TestingTools
    {
        /// <summary>
        /// Return the JSON and corresponding C3D files from a folder.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>
        ///     <list type="number">
        ///         <item>JSON file list</item>
        ///         <item>C3D file list</item>
        ///     </list>
        /// </returns>
        public static IEnumerable<(string jsonFile, string c3dFile)> GetJsonAndC3dFileList(string folderPath)
        {
            string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

            return jsonFiles.Select(jsonFile => (jsonFile, Path.ChangeExtension(jsonFile, ".c3d")));
        }

        /// <summary>
        /// Retrieves a collection of C3D file paths from a specified folder.
        /// </summary>
        /// <param name="folderPath">
        /// The path to the folder containing the C3D files.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="object"/> arrays,
        /// where each array contains a single string representing the full path to a C3D file.
        /// This format is compatible with xUnit's <see cref="MemberDataAttribute"/> for use in data-driven tests.
        /// </returns>
        /// <remarks>
        /// This method searches the specified folder for files with the ".c3d" extension
        /// and returns their paths as an enumerable of object arrays.
        /// </remarks>
        /// <example>
        /// Usage in an xUnit test class:
        /// <code>
        /// public static IEnumerable<object[]> C3dFilesData => GetC3dFilesData(@"C:\path\to\c3d\files");
        ///
        /// [Theory]
        /// [MemberData(nameof(C3dFilesData))]
        /// public void TestC3dFile(string c3dFilePath)
        /// {
        ///     // Test logic using the provided C3D file path
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<object[]> GetEnumerableC3dFilesData(string folderPath)
        {
            string[] fileList = Directory.GetFiles(folderPath, "*.c3d");
            return fileList.Select(file => new object[] { file });
        }
    }
}
