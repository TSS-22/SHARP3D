using SHARP3D.Utils.Enum;

namespace SHARP3D.Utils
{
    internal class FortranMatrix
    {
        // TODO: FortranMatrix reader. So all the code is stored in one place and is easier to sort/debug.
        // TODO: Mention that it is not to be use with extremely large matrices.
        public static List<T> FVectorToFMatrix<T>(byte[] vector, int[] dimensions, ProcessorType processor)
        {
            int totalData = dimensions.Aggregate((acc, val) => acc * val);
            List<T> data = new List<T> { };
            if (vector.Length == 0)
            {
                return data;
            }
            if (vector.Length != totalData)
            {
                throw new ArgumentException("Data vector length must be a multiple of data length.");
            }
            if (dimensions.Length < 1)
            {
                throw new ArgumentException("Dimensions must be greater than 0.");
            }
            Span<byte> span = vector.AsSpan();
            int[] idxArray = new int[dimensions.Length];
            data = RecursiveFill<T>(vector, dimensions, idxArray, 0, DataLength.BYTE, processor);
            return data;
        }

        private static List<T> RecursiveFill<T>(
            Span<byte> vector,
            int[]dimensions,
            int[] idxArray,
            int idxDimension,
            DataLength dataLength,
            ProcessorType processor)
        { 
            List<T> data = new List<T>();
            if (idxDimension == dimensions.Length)
            {
                
                for(int i=0; i<dimensions[idxDimension];i++)
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
                for (int i=0; i<dimensions[idxDimension]; i++)
                {
                    idxArray[idxDimension] = i;
                    data.AddRange(RecursiveFill<T>(vector, dimensions, idxArray, idxDimension + 1, dataLength, processor));
                }

            }
            return data;
        }

        // TODO: Put the function it is based on in the doc.
        private static int ComputeFortranIndex(int[] dimensions, int[] idxArray, DataLength dataLength)
        { 
            int idx = 0;
            int[] multiplier = new int[dimensions.Length];
            for (int i = 0; i <= idxArray.Length; i++)
            {
                if (i == 0)
                {
                    multiplier[i + 1] = dimensions[i] * (int)dataLength;
                    idx += idxArray[i];
                }
                else
                {
                    multiplier[i + 1] = dimensions[i] * multiplier[i] * (int)dataLength;
                    idx += idxArray[i] * multiplier[i];
                }     
            }
            // I think I can make it happen in one loop.
            //for (int i = 0; i < idxArray.Length; i++)
            //{
            //    if (i == 0)
            //    {
            //        idx += idxArray[i];
            //    }
            //    else 
            //    {
            //        idx += idxArray[i] * multiplier[i];
            //    } 
            //}
            return idx;
        }

        
    }
}
