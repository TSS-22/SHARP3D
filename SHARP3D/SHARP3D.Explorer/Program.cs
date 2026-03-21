using SHARP3D.Utils.Enum;

namespace SHARP3D.Explorer
{
    internal class Program
    {
        public static readonly string PathEb015pi = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample01\Eb015pi.c3d";
        public static readonly string PathEb015pr = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample01\Eb015pr.c3d";
        public static readonly string PathEb015si = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample01\Eb015si.c3d";
        public static readonly string PathEb015sr = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample01\Eb015sr.c3d";
        public static readonly string PathEb015vi = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample01\Eb015vi.c3d";
        public static readonly string PathEb015vr = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample01\Eb015vr.c3d";
        
        public static readonly string PathSample27 = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample27\kyowadengyo.c3d";
        
        public static readonly string PathSample33 = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample33\bigparlove.c3d";
        
        public static readonly string PathSample31 = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample31\large01.c3d";

        public static readonly string PathSample29facial = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample29\Facial-Sing.c3d";
        public static readonly string PathSample29Opti = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample29\OptiTrack-IITSEC2007.c3d";

        public static readonly string PathFileSample02 = @"..\..\..\..\SHARP3D.Test\SampleFiles\Sample02\dec_int.c3d";

        public static readonly string PathFileSampleError13 = @"..\..\..\..\SHARP3D.Test\SampleErrorFiles\Sample13\Dance.c3d";

        public static readonly int[] dimensions = { 3, 4, 2 };
        public static readonly byte[] vector =
            {
            0xE3, 0x02, 0x02, 0x44,
            0x6C, 0x45, 0x9B, 0x44,
            0xB6, 0x32, 0x1F, 0x3F,
            0x64, 0x2F, 0x64, 0x42,
            0x63, 0x66, 0x9B, 0x44,
            0xEA, 0x00, 0x1F, 0x3F,
            0xBC, 0xB4, 0x68, 0x42,
            0x48, 0xE6, 0xDA, 0x44,
            0x98, 0x32, 0x05, 0x40,
            0x38, 0x4B, 0x02, 0x44,
            0x51, 0xC5, 0xDA, 0x44,
            0x0B, 0x3F, 0x05, 0x40,
            0x38, 0x9F, 0x56, 0x42,
            0xF0, 0x7F, 0x8E, 0x44,
            0x88 , 0xD0 , 0xF5 , 0x3F,
            0x2A , 0x29 , 0x01 , 0x44,
            0x1C , 0xEA , 0x8E , 0x44,
            0x16 , 0xDE , 0xA4 , 0x3F,
            0x14 , 0x12 , 0x02 , 0x44,
            0x21 , 0xD5 , 0x1E , 0x44,
            0xC4 , 0xB8 , 0x39 , 0x3E,
            0xD8 , 0x2D , 0x65 , 0x42,
            0xC2 , 0x00 , 0x1E , 0x44,
            0x15 , 0x53 , 0x50 , 0x3F
        };
        public static DataType dataLength = DataType.FLOAT32;
        public static ProcessorType processor = ProcessorType.INTEL;
        private static int Main()
        { 
            C3dFile c3dFile = C3dFile.LoadFromFile(PathFileSampleError13);

            return 0;
        }

        internal static C3dFile GetC3dFileWithparameter(string filePath)
        {
            FileStream fileStream = C3dFile.OpenC3dFile(filePath);
            C3dFile c3dFile = new C3dFile();
            c3dFile.ProcessorFile = C3dFile.ReadProcessorByte(fileStream);
            c3dFile.Header = c3dFile.GetHeader(fileStream, c3dFile.ProcessorFile);
            c3dFile.Parameters = c3dFile.GetParameters(fileStream, c3dFile.ProcessorFile, c3dFile.Header.PointerParameterSection, c3dFile.Header.PointerDataSection);
            return c3dFile;
        }

        internal static C3dFile GetC3dFileWithHeader(string filePath)
        {

            FileStream fileStream = C3dFile.OpenC3dFile(filePath);
            C3dFile c3dFile = new C3dFile();
            c3dFile.ProcessorFile = C3dFile.ReadProcessorByte(fileStream);
            c3dFile.Header = c3dFile.GetHeader(fileStream, c3dFile.ProcessorFile);
            return c3dFile;
        }

    }
}

