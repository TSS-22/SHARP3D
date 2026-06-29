namespace SHARP3D.Utils
{
    public static class ArrayUtils
    {
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

        /// <summary>
        /// Converts a <see cref="List{T[]}"/> (jagged list) into a rectangular 2D array (<see cref="T[,]"/>).
        /// </summary>
        /// <typeparam name="T">The type of elements in the arrays.</typeparam>
        /// <param name="jaggedList">The list of arrays to convert. All inner arrays must have the same length.</param>
        /// <returns>A rectangular 2D array containing the elements of the jagged list.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="jaggedList"/> is null, empty, or if any inner array has a different length than the first.
        /// </exception>
        public static T[,] To2DArray<T>(this List<T[]> jaggedList)
        {
            if (jaggedList == null || jaggedList.Count == 0)
                throw new ArgumentException("List is empty or null.");

            int rows = jaggedList.Count;
            int cols = jaggedList[0].Length;

            var rectangularArray = new T[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                if (jaggedList[i].Length != cols)
                    throw new ArgumentException("All inner arrays must have the same length.");

                for (int j = 0; j < cols; j++)
                {
                    rectangularArray[i, j] = jaggedList[i][j];
                }
            }
            return rectangularArray;
        }

        /// <summary>
        /// Converts a jagged array of 2D arrays (<see cref="T[][,]"/>) into a 3D rectangular array (<see cref="T[,,]"/>).
        /// </summary>
        /// <typeparam name="T">The type of elements in the arrays.</typeparam>
        /// <param name="jagged3DArray">The jagged array of 2D arrays to convert. All inner 2D arrays must have the same dimensions.</param>
        /// <returns>A 3D rectangular array containing the elements of the jagged array.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="jagged3DArray"/> is null, empty, or if any inner 2D array has different dimensions than the first.
        /// </exception>
        public static T[,,] To3DArray<T>(this List<T[,]> jagged3DArray)
        {
            if (jagged3DArray == null || jagged3DArray.Count == 0)
                throw new ArgumentException("Jagged array is empty or null.", nameof(jagged3DArray));

            // Get dimensions from the first 2D array
            int depth = jagged3DArray.Count;
            int rows = jagged3DArray[0].GetLength(0);
            int cols = jagged3DArray[0].GetLength(1);

            var rectangular3DArray = new T[depth, rows, cols];

            for (int k = 0; k < depth; k++)
            {
                // Check if the current 2D array has the same dimensions as the first
                if (jagged3DArray[k].GetLength(0) != rows || jagged3DArray[k].GetLength(1) != cols)
                    throw new ArgumentException("All inner 2D arrays must have the same dimensions.", nameof(jagged3DArray));

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        rectangular3DArray[k, i, j] = jagged3DArray[k][i, j];
                    }
                }
            }

            return rectangular3DArray;
        }
    }
}

