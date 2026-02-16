using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterDataType
{
    // Single dimension data

    // TODO: How to do the doc for this (I think it will be straight forward once implemented
    // TODO: Do I need to implement a processing function to make it prettier the handling of that weird ass fortran matrix shit ? If so, It will be nice to put an interface to force the implementation of said function
    // TODO: Check if that works with List<List<T>> and so on. If not I am good to do another abstract class lol
    internal class MultiCharParameterData : ParameterData
    {
        List<char> Data { set; get; }
         
        public MultiCharParameterData(byte[] data, int[]? dimensions, ProcessorType? _ = null)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException(nameof(dimensions), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else
            {
                Data = FortranMatrix.FVectorToFMatrix<char>(data, dimensions, DataLength.CHAR);

            }
        }
    }
    internal class MultiByteParameterData : ParameterData
    {
        List<byte> Data { set; get; }
        public MultiByteParameterData(byte[] data, int[]? dimensions, ProcessorType? _ = null)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException(nameof(dimensions), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else
            {
                Data = FortranMatrix.FVectorToFMatrix<byte>(data, dimensions, DataLength.BYTE);
            }
        }
    }
    internal class MultiIntParameterData : ParameterData
    {
        List<int> Data { set; get; }

        public MultiIntParameterData(byte[] data, int[]? dimensions, ProcessorType? processor = null)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException(nameof(dimensions), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for int parameter data.");
            }
            else 
            {
                Data = FortranMatrix.FVectorToFMatrix<int>(data, dimensions, DataLength.INT16);
            }
        }
    }

    internal class MultiFloatParameterData : ParameterData
    {
        List<float> Data { set; get; }
        public MultiFloatParameterData(byte[] data, int[]? dimensions, ProcessorType? processor = null)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException(nameof(dimensions), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for int parameter data.");
            }
            else
            {
                Data = FortranMatrix.FVectorToFMatrix<float>(data, dimensions, DataLength.INT16);
            }
        }
    }
}