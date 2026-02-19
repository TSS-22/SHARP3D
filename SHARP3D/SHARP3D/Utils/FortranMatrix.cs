using SHARP3D.Utils.Enum;
using SHARP3D.Utils.Matrix;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

[assembly: InternalsVisibleTo("SHARP3D.Test")]
namespace SHARP3D.Utils
{
    internal class FortranMatrix
    {
        // TODO: FortranMatrix reader. So all the code is stored in one place and is easier to sort/debug.
        // TODO: Mention that it is not to be use with extremely large matrices.
        public static C3dMatrix FVectorToFMatrix<T>(byte[] vector, int[] dimensions, DataLength dataLength, ProcessorType processor = ProcessorType.UNKOWN)
        {
            int lengthData = dimensions.Aggregate((acc, val) => acc * val) * (int)dataLength;
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

            C3dMatrix data = InitMatrix(dimensions, dataLength);
            List<T> dataBuffer = new List<T>();
            
            Span<byte> span = vector.AsSpan();




            //int[] idxArray = new int[dimensions.Length];
            //if ((processor == ProcessorType.UNKOWN) && (Math.Abs((int)dataLength) == 1))
            //{
            //    processor = ProcessorType.INTEL;
            //    data = RecursiveFill<T>(vector, dimensions, idxArray, 0, dataLength);
            //} else if (processor == ProcessorType.UNKOWN && (Math.Abs((int)dataLength) > 1))
            //{
            //    throw new ArgumentException("Processor type must be specified for data types other than BYTE and CHAR.");
            //}else if (processor != ProcessorType.UNKOWN)
            //{
            //    data = RecursiveFill<T>(vector, dimensions, idxArray, 0, dataLength, processor);
            //}
                
            return data;
        }

        internal static Array FortranVectorToNDMatrix<T>(byte[] vector, int[] dimensions, DataLength dataLength, ProcessorType processor)
        {

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
            return matrix;
        }

        internal static C3dMatrix InitMatrix(int[] dimensions, DataLength dataLength)
        {
            switch (dimensions.Length) 
            {
                case 0:
                    throw new ArgumentException("Invalid dimension: Scalar.");
                case 1:
                    switch (dataLength)
                    {
                        case DataLength.CHAR:
                            return new C3dMatrix1D<char>();
                        case DataLength.BYTE:
                            return new C3dMatrix1D<byte>();
                        case DataLength.INT16:
                            return new C3dMatrix1D<int>();
                        case DataLength.FLOAT32:
                            return new C3dMatrix1D<float>();
                        default:
                            throw new ArgumentException("Unkown type of data.");
                    }
                case 2:
                    switch (dataLength)
                    {
                        case DataLength.CHAR:
                            return new C3dMatrix2D<char>();
                        case DataLength.BYTE:
                            return new C3dMatrix2D<byte>();
                        case DataLength.INT16:
                            return new C3dMatrix2D<int>();
                        case DataLength.FLOAT32:
                            return new C3dMatrix2D<float>();
                        default:
                            throw new ArgumentException("Unkown type of data.");
                    }
                case 3:
                    switch (dataLength)
                    {
                        case DataLength.CHAR:
                            return new C3dMatrix3D<char>();
                        case DataLength.BYTE:
                            return new C3dMatrix3D<byte>();
                        case DataLength.INT16:
                            return new C3dMatrix3D<int>();
                        case DataLength.FLOAT32:
                            return new C3dMatrix3D<float>();
                        default:
                            throw new ArgumentException("Unkown type of data.");
                    }
                case 4:
                    switch (dataLength)
                    {
                        case DataLength.CHAR:
                            return new C3dMatrix4D<char>();
                        case DataLength.BYTE:
                            return new C3dMatrix4D<byte>();
                        case DataLength.INT16:
                            return new C3dMatrix4D<int>();
                        case DataLength.FLOAT32:
                            return new C3dMatrix4D<float>();
                        default:
                            throw new ArgumentException("Unkown type of data.");
                    }
                case 5:
                    switch (dataLength)
                    {
                        case DataLength.CHAR:
                            return new C3dMatrix5D<char>();
                        case DataLength.BYTE:
                            return new C3dMatrix5D<byte>();
                        case DataLength.INT16:
                            return new C3dMatrix5D<int>();
                        case DataLength.FLOAT32:
                            return new C3dMatrix5D<float>();
                        default:
                            throw new ArgumentException("Unkown type of data.");
                    }
                case 6:
                    switch (dataLength)
                    {
                        case DataLength.CHAR:
                            return new C3dMatrix6D<char>();
                        case DataLength.BYTE:
                            return new C3dMatrix6D<byte>();
                        case DataLength.INT16:
                            return new C3dMatrix6D<int>();
                        case DataLength.FLOAT32:
                            return new C3dMatrix6D<float>();
                        default:
                            throw new ArgumentException("Unkown type of data.");
                    }
                case 7:
                    switch (dataLength)
                    {
                        case DataLength.CHAR:
                            return new C3dMatrix7D<char>();
                        case DataLength.BYTE:
                            return new C3dMatrix7D<byte>();
                        case DataLength.INT16:
                            return new C3dMatrix7D<int>();
                        case DataLength.FLOAT32:
                            return new C3dMatrix7D<float>();
                        default:
                            throw new ArgumentException("Unkown type of data.");
                    }
                default:
                    throw new ArgumentException("Dimensions number not supported");
            }
            
        }

        internal static List<T> RecursiveFill<T>(
            Span<byte> vector,
            int[] dimensions,
            int[] idxArray,
            int idxDimension,
            DataLength dataLength,
            ProcessorType processor = ProcessorType.UNKOWN)
        {
            List<T> data = new List<T>();
            if (idxDimension == dimensions.Length - 1)
            {
                for (int i = 0; i < dimensions[idxDimension]; i++)
                {
                    idxArray[idxDimension] = i;
                    int idx = ComputeFortranIndex(dimensions, idxArray, dataLength);
                    switch (dataLength)
                    {
                        case DataLength.BYTE:
                            data.Add((T)(object)vector[i]);
                            break;
                        case DataLength.INT16:
                            data.Add((T)(object)C3dBytesConvertor.ToInt(vector.Slice(idx, 2).ToArray(), processor));
                            break;
                        case DataLength.FLOAT32:
                            data.Add((T)(object)C3dBytesConvertor.ToFloat(vector.Slice(idx, 4).ToArray(), processor));
                            break;
                        case DataLength.CHAR:
                            data.Add((T)(object)(char)vector.Slice(idx, 1).ToArray()[0]);
                            break;
                        default:
                            throw new ArgumentException("Unsupported data type.");
                    }
                }
            }
            else
            {
                // Recurse
                for (int i = 0; i < dimensions[idxDimension]; i++)
                {
                    idxArray[idxDimension] = i;
                    data.AddRange(RecursiveFill<T>(vector, dimensions, idxArray, idxDimension + 1, dataLength, processor));
                }

            }
            return data;
        }

        // TODO: Put the function it is based on in the doc.
        internal static int ComputeFortranIndex(int[] dimensions, int[] idxArray, DataLength dataLength)
        {
            int idx = 0;
            int[] multiplier = new int[dimensions.Length];
            for (int i = 0; i < idxArray.Length; i++)
            {
                // First dimension
                if (i == 0)
                {
                    multiplier[i + 1] = dimensions[i];
                    idx += idxArray[i];
                }
                // Last dimension
                else if (i == idxArray.Length - 1)
                {
                    idx += idxArray[i] * multiplier[i];
                }
                else
                {
                    multiplier[i + 1] = dimensions[i] * multiplier[i];
                    idx += idxArray[i] * multiplier[i];
                }
            }
            return idx * Math.Abs((int)dataLength);
        }
    }
}
