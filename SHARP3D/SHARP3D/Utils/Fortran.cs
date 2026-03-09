using SHARP3D.Utils.Enum;
namespace SHARP3D.Utils
{
    /// <summary>
    /// Provides utility methods for converting Fortran-style vectors to C# matrices.
    /// </summary>
    /// <remarks>
    /// This class handles the conversion of Fortran-style column-major vectors to C# row-major matrices.
    /// </remarks>
    internal class Fortran
    {
        /// <summary>
        /// Converts a Fortran-style vector to a C# multi-dimensional matrix.
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix.</typeparam>
        /// <param name="vector">The byte array representing the Fortran-style vector.</param>
        /// <param name="dimensions">The dimensions of the resulting matrix.</param>
        /// <param name="dataLength">The data type of the elements in the vector.</param>
        /// <param name="processor">The processor type used for data conversion (required for INT16 and FLOAT32 data types).</param>
        /// <returns>A multi-dimensional array representing the matrix.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// <list type="bullet">
        ///   <item>The data vector is empty.</item>
        ///   <item>The data vector length doesn't match the expected size based on dimensions and data type.</item>
        ///   <item>The dimensions array is empty or invalid.</item>
        ///   <item>An unsupported data type is provided.</item>
        ///   <item>The processor type is unknown for INT16 or FLOAT32 data types.</item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code>
        /// byte[] vector = new byte[] { 1, 0, 2, 0, 3, 0, 4, 0 };
        /// int[] dimensions = new int[] { 2, 2 };
        /// Array matrix = Fortran.VectorToMatrix&lt;short&gt;(vector, dimensions, DataType.INT16, ProcessorType.INTEL);
        /// </code>
        /// </example>
        public static Array VectorToMatrix<T>(
            byte[] vector,
            int[] dimensions,
            DataType dataLength,
            ProcessorType processor = ProcessorType.UNKOWN
            )
        {
            int lengthData = dimensions.Aggregate((acc, val) => acc * val) * Math.Abs((int)dataLength);
            if (vector.Length == 0)
            {
                throw new ArgumentException("Data vector can't be empty.");
            }
            if (vector.Length != lengthData)
            {
                throw new ArgumentException("Data vector length must be a multiple of data length.");
            }
            if (dimensions.Length < 1)
            {
                throw new ArgumentException("Dimensions must be greater than 0.");
            }
            if ((processor == ProcessorType.UNKOWN) && ((dataLength == DataType.INT16) || (dataLength == DataType.FLOAT32)))
            {
                throw new ArgumentException("Choose supported processor to convert C3D binaries to INT16 or FLOAT32.");
            }

            int elementSize = Math.Abs((int)dataLength);
            int totalElements = 1;
            foreach (int dim in dimensions)
                totalElements *= dim;
            totalElements *= elementSize;

            if (vector.Length != totalElements)
                throw new ArgumentException("Vector length must match total elements in the matrix.");
            
            Array matrix = Array.CreateInstance(typeof(T), dimensions);
            int[] indices = new int[dimensions.Length];

            for (int i = 0; i < vector.Length; i += elementSize)
            {
                int remaining = i / elementSize; ;
                // Reverse the order of dimensions for Fortran to C# conversion
                for (int d = 0; d < dimensions.Length; d++)
                {
                    indices[d] = remaining % dimensions[d];
                    remaining /= dimensions[d];
                }
                // Reverse the indices array to match C# row-major order
                //Array.Reverse(indices);
                switch (dataLength)
                {
                    case DataType.CHAR:
                        matrix.SetValue((char)vector[i], indices);
                        break;
                    case DataType.BYTE:
                        matrix.SetValue(vector[i], indices);
                        break;
                    case DataType.INT16:
                        matrix.SetValue(C3dBytesConvertor.ToInt(vector.Skip(i).Take(elementSize).ToArray(), processor), indices);
                        break;
                    case DataType.FLOAT32:
                        matrix.SetValue(C3dBytesConvertor.ToFloat(vector.Skip(i).Take(elementSize).ToArray(), processor), indices);
                        break;
                    default:
                        throw new ArgumentException("Unsupported datatype.");
                }
                
            }
            return matrix;
        }
    }
}
