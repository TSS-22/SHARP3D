namespace SHARP3D.Test.Utils
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
    }
}
