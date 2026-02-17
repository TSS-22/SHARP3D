using SHARP3D;

namespace SHARP3D.Explorer
{
    internal class Program
    {
        public static readonly string PathEb015pi = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pi.c3d";
        public static readonly string PathEb015pr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015pr.c3d";
        public static readonly string PathEb015si = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015si.c3d";
        public static readonly string PathEb015sr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015sr.c3d";
        public static readonly string PathEb015vi = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vi.c3d";
        public static readonly string PathEb015vr = @"..\..\..\..\..\C3D_sample\TestSuites\01-test_suite\Eb015vr.c3d";


        private static int Main()
        {
            Program test = new Program();
            C3dFile c3dFile = test.GetC3dFileWithparameter(PathEb015pi);
            Console.WriteLine(c3dFile.Parameters.Groups.ToString());
            return 0;
        }

        internal C3dFile GetC3dFileWithparameter(string filePath)
        {
            FileStream fileStream = C3dFile.OpenC3dFile(filePath);
            C3dFile c3dFile = new C3dFile();
            c3dFile.ProcessorFileType = C3dFile.ReadProcessorByte(fileStream);
            c3dFile.Parameters = c3dFile.GetParameters(fileStream, c3dFile.ProcessorFileType);
            return c3dFile;
        }

        internal C3dFile GetC3dFileWithHeader(string filePath)
        {

            FileStream fileStream = C3dFile.OpenC3dFile(filePath);
            C3dFile c3dFile = new C3dFile();
            c3dFile.ProcessorFileType = C3dFile.ReadProcessorByte(fileStream);
            c3dFile.Header = c3dFile.GetHeader(fileStream, c3dFile.ProcessorFileType);
            return c3dFile;
        }

    }
}

