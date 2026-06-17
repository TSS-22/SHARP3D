namespace SHARP3D.Utils
{
    public static class ArrayUtils
    {
        public static float[,] ConvertTo2DArray(List<float[]> list)
        {
            int rows = list.Count;
            int cols = list[0].Length; // Assume all inner arrays have the same length

            float[,] array2D = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array2D[i, j] = list[i][j];
                }
            }

            return array2D;
        }
    }
}
