using SHARP3D;
using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics;

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
        public static DataLength dataLength = DataLength.FLOAT32;
        public static ProcessorType processor = ProcessorType.INTEL;
        private static int Main()
        {

            int elementSize = Math.Abs((int)dataLength);
            int totalElements = 1;
            foreach (int dim in dimensions)
                totalElements *= dim;
            totalElements *= elementSize;

            if (vector.Length != totalElements)
                throw new ArgumentException("Vector length must match total elements in the matrix.");

            Array matrix = Array.CreateInstance(typeof(float), dimensions);
            int[] indices = new int[dimensions.Length];

            for (int i = 0; i < vector.Length; i += elementSize)
            {
                int remaining = i/ elementSize;
                // Reverse the order of dimensions for Fortran to C# conversion
                for (int d = 0; d < dimensions.Length; d++)
                {
                    indices[d] = remaining % dimensions[d];
                    remaining /= dimensions[d];
                    Console.WriteLine(d);
                    Console.WriteLine("[" + string.Join(", ", indices) + "]");
                    Console.WriteLine("_________\n");
                }
                switch (dataLength)
                {
                    case DataLength.CHAR:
                        matrix.SetValue((char)vector[i], indices);
                        break;
                    case DataLength.BYTE:
                        matrix.SetValue(vector[i], indices);
                        break;
                    case DataLength.INT16:
                        matrix.SetValue(C3dBytesConvertor.ToInt(vector.Skip(i).Take(elementSize).ToArray(), processor), indices);
                        break;
                    case DataLength.FLOAT32:
                        matrix.SetValue(C3dBytesConvertor.ToFloat(vector.Skip(i).Take(elementSize).ToArray(), processor), indices);
                        break;
                    default:
                        throw new ArgumentException("Unsupported datatype.");
                }

            }
            return 0;
        }

        //internal C3dFile GetC3dFileWithparameter(string filePath)
        //{
        //    FileStream fileStream = C3dFile.OpenC3dFile(filePath);
        //    C3dFile c3dFile = new C3dFile();
        //    c3dFile.ProcessorFileType = C3dFile.ReadProcessorByte(fileStream);
        //    c3dFile.Parameters = c3dFile.GetParameters(fileStream, c3dFile.ProcessorFileType);
        //    return c3dFile;
        //}

        //internal C3dFile GetC3dFileWithHeader(string filePath)
        //{

        //    FileStream fileStream = C3dFile.OpenC3dFile(filePath);
        //    C3dFile c3dFile = new C3dFile();
        //    c3dFile.ProcessorFileType = C3dFile.ReadProcessorByte(fileStream);
        //    c3dFile.Header = c3dFile.GetHeader(fileStream, c3dFile.ProcessorFileType);
        //    return c3dFile;
        //}

    }
}

