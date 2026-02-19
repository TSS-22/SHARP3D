using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Test.Utils
{
    public class FortranMatrixTests
    {
        public static readonly int[] Mat3d_vecByteChar_index = {0,12,3,15,6,18,9,21,1,13,4,16,7,19,10,22,2,14,5,17,8,20,11,23};
        public static readonly int[] Mat3d_vecInt_index = { 0, 24, 6, 30, 12, 36, 18, 42, 2, 26, 8, 32, 14, 38, 20, 44, 4, 28, 10, 34, 16, 40, 22, 46, };
        public static readonly int[] Mat3d_vecFloat_index = { 0, 48, 12, 60, 24, 72, 36, 84, 4, 52, 16, 64, 28, 76, 40, 88, 8, 56, 20, 68, 32, 80, 44, 92 };
        public static readonly int[] Mat3d_dimensions = { 3, 4, 2 };
        public static readonly int[][] Mat3d_index = { 
            new int[] {0, 0, 0},
            new int[] {0, 0, 1},
            new int[] {0, 1, 0},
            new int[] {0, 1, 1},
            new int[] {0, 2, 0},
            new int[] {0, 2, 1},
            new int[] {0, 3, 0},
            new int[] {0, 3, 1},
            new int[] {1, 0, 0},
            new int[] {1, 0, 1},
            new int[] {1, 1, 0},
            new int[] {1, 1, 1},
            new int[] {1, 2, 0},
            new int[] {1, 2, 1},
            new int[] {1, 3, 0},
            new int[] {1, 3, 1},
            new int[] {2, 0, 0},
            new int[] {2, 0, 1},
            new int[] {2, 1, 0},
            new int[] {2, 1, 1},
            new int[] {2, 2, 0},
            new int[] {2, 2, 1},
            new int[] {2, 3, 0},
            new int[] {2, 3, 1}
        };
        public static readonly float[] Mat3d_vecFloat_val = {
            520.045104980469f,
            1242.16943359375f,
            0.621867537498474f,
            57.0462799072266f,
            1243.19958496094f,
            0.621107697486877f,
            58.1764984130859f,
            1751.1962890625f,
            2.08121299743652f,
            521.17529296875f,
            1750.16613769531f,
            2.08197283744812f,
            53.6554870605469f,
            1139.998046875f,
            1.92042636871338f,
            516.643188476563f,
            1143.31591796875f,
            1.28802752494812f,
            520.282470703125f,
            635.330139160156f,
            0.181368887424469f,
            57.2947692871094f,
            632.011840820313f,
            0.813767731189728f
            };
        public static readonly byte[] Mat3d_vecFloat_bytes =
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

        

        //public static IEnumerable<object[]> Data3D =>
        //    new List<object[]>
        //    {
        //        new object[] { floatVector, floatVectorBytes},
        //    };

        public static IEnumerable<object[]> Float3dTestData => 
            new List<object[]>
        {
            new object[]
            {
                Mat3d_vecFloat_bytes,
                Mat3d_vecFloat_val,
                Mat3d_vecFloat_index,
                Mat3d_dimensions,
                Mat3d_index,
                DataLength.FLOAT32,
                ProcessorType.INTEL
            }
        };


        public static IEnumerable<object[]> IndexTestData =>
            new List<Object[]>
            {
                new object[] {

                }
            };
        [Theory]
        [MemberData(nameof(Float3dTestData))]
        public static void float3d_Tests(
            byte[ ] mat3d_vecFloat_bytes,
            float[] mat3d_vecFloat_val,
            int[] mat3d_vecFloat_index,
            int[] mat3d_dimensions,
            int[][] mat3d_index,
            DataLength dataLength,
            ProcessorType processor
            )
        {
            float[,,] matrix = new float[3, 4, 2];

            // Assign values based on indices
            matrix[0, 0, 0] = 520.045105f;
            matrix[1, 0, 0] = 1242.169434f;
            matrix[2, 0, 0] = 0.621867537f;

            matrix[0, 1, 0] = 57.04628f;
            matrix[1, 1, 0] = 1243.199585f;
            matrix[2, 1, 0] = 0.6211077f;

            matrix[0, 2, 0] = 58.176498f;
            matrix[1, 2, 0] = 1751.19629f;
            matrix[2, 2, 0] = 2.081213f;

            matrix[0, 3, 0] = 521.17529f;
            matrix[1, 3, 0] = 1750.16614f;
            matrix[2, 3, 0] = 2.081973f;

            matrix[0, 0, 1] = 53.655487f;
            matrix[1, 0, 1] = 1139.998047f;
            matrix[2, 0, 1] = 1.9204264f;

            matrix[0, 1, 1] = 516.643188f;
            matrix[1, 1, 1] = 1143.315918f;
            matrix[2, 1, 1] = 1.2880275f;

            matrix[0, 2, 1] = 520.28247f;
            matrix[1, 2, 1] = 635.33014f;
            matrix[2, 2, 1] = 0.1813689f;

            matrix[0, 3, 1] = 57.29477f;
            matrix[1, 3, 1] = 632.01184f;
            matrix[2, 3, 1] = 0.8137677f;

            float[,,] result = (float[,,])FortranMatrix.FortranVectorToNDMatrix<float>(
                mat3d_vecFloat_bytes,
                mat3d_dimensions,
                dataLength,
                processor
                );
            for (int i = 0; i < mat3d_dimensions[0]; i++)
            {
                for (int j = 0; j < mat3d_dimensions[1]; j++)
                {
                    for (int k = 0; k < mat3d_dimensions[2]; k++)
                    {
                        Assert.Equal(matrix[i,j,k], result[i,j,k], 5f);
                    }
                }
            }
        }

        [Fact]
        public static void Index3dTest()
        {
            int[] dimensions = { 3, 4, 2 };
            DataLength[] dataLengths = { DataLength.CHAR, DataLength.BYTE, DataLength.INT16, DataLength.FLOAT32};
            for (int h=0; h< dataLengths.Length; h++)
            {
                int[] expectedVectorResult = Mat3d_vecByteChar_index.Select(x => x * Math.Abs((int)dataLengths[h])).ToArray();
                int idExpected = 0;
                for (int i = 0; i < dimensions[0]; i++)
                {
                    for (int j = 0; j < dimensions[1]; j++)
                    {
                        for (int k = 0; k < dimensions[2]; k++)
                        {
                            Assert.Equal(expectedVectorResult[idExpected], FortranMatrix.ComputeFortranIndex(dimensions, new int[] {i,j,k}, dataLengths[h]));
                            idExpected++;
                        }
                    }
                }
            }
            
        }

    }
}
