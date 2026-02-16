namespace SHARP3D.Explorer
{
    internal static class Program
    {
        private static int Main()
        {
            //var path = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d";
            //if (!File.Exists(path))
            //{
            //    Console.Error.WriteLine($"File not found: {path}");
            //    return 1;
            //}
            //try
            //{
            //    Console.WriteLine($"Opening C3D file: {path}");
            //    C3dFile c3dFile = C3dFile.LoadFromFile(path);
            //    Console.WriteLine($"Processor type: {c3dFile.ProcessorHostType}");
            //    return 0;
            //}
            //catch (Exception ex) 
            //{ 
            //    Console.Error.WriteLine($"Error: {ex.Message}");
            //    return 1;
            //}
            int[] dimensions =  { 3, 4, 2 };
            int[,,] vector = new int[dimensions[0], dimensions[1], dimensions[2]];
            int[] indices = new int[dimensions.Length];

            for (int linearIndex = 0; linearIndex < vector.Length; linearIndex++)
            {
                int remaining = linearIndex;
                for (int d = dimensions.Length - 1; d >= 0; d--)
                {
                    indices[d] = remaining % dimensions[d];
                    remaining /= dimensions[d];
                }
                Console.WriteLine($"Linear index: {linearIndex}, Indices: [{string.Join(", ", indices)}]");
            }

            return 0;

        }
    }
}