using SHARP3D.Utils.Enum;
namespace SHARP3D.Utils
{
    internal class Fortran
    {
        public static Array VectorToMatrix<T>(
            byte[] vector,
            int[] dimensions,
            ParameterDataType dataLength,
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
            if ((processor == ProcessorType.UNKOWN) && ((dataLength == ParameterDataType.INT16) || (dataLength == ParameterDataType.FLOAT32)))
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
                    case ParameterDataType.CHAR:
                        matrix.SetValue((char)vector[i], indices);
                        break;
                    case ParameterDataType.BYTE:
                        matrix.SetValue(vector[i], indices);
                        break;
                    case ParameterDataType.INT16:
                        matrix.SetValue(C3dBytesConvertor.ToInt(vector.Skip(i).Take(elementSize).ToArray(), processor), indices);
                        break;
                    case ParameterDataType.FLOAT32:
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
