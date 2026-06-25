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

        /// <summary>
        /// Multiplies a row vector (1xN) by a matrix (NxM), resulting in a row vector (1xM).
        /// </summary>
        /// <param name="vector">Row vector (1xN).</param>
        /// <param name="matrix">Matrix (NxM).</param>
        /// <returns>Resulting row vector (1xM).</returns>
        public static float[] VecMatMultiplication(float[] vector, float[,] matrix)
        {
            int vectorLength = vector.Length;
            int matrixRows = matrix.GetLength(0);
            int matrixCols = matrix.GetLength(1);

            // Validate dimensions
            if (vectorLength != matrixRows)
            {
                throw new ArgumentException(
                    $"Vector length ({vectorLength}) must match the number of matrix rows ({matrixRows}).");
            }

            float[] result = new float[matrixCols];

            // Perform multiplication: result[j] = sum(vector[i] * matrix[i, j] for all i)
            for (int j = 0; j < matrixCols; j++)
            {
                float sum = 0.0f;
                for (int i = 0; i < vectorLength; i++)
                {
                    sum += vector[i] * matrix[i, j];
                }
                result[j] = sum;
            }

            return result;
        }
    }
}
