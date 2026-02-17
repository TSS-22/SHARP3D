namespace SHARP3D.Test.Utils
{
    internal class FortranMatrixTests
    {
        public static readonly int[] idxVectorByte = {0,12,3,15,6,18,9,21,1,13,4,16,7,19,10,22,2,14,5,17,8,20,11,23};
        public static readonly int[] idxVectorInt = { 0, 24, 6, 30, 12, 36, 18, 42, 2, 26, 8, 32, 14, 38, 20, 44, 4, 28, 10, 34, 16, 40, 22, 46, };
        public static readonly int[] idxVectorFloat = { 0, 48, 12, 60, 24, 72, 36, 84, 4, 52, 16, 64, 28, 76, 40, 88, 8, 56, 20, 68, 32, 80, 44, 92 };
        public static readonly int[] idxMatrixDim = { 3, 4, 2 };
        public static readonly int[][] idxMatrix = { 
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
        
    }
}
