using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterDataType
{
    // Single dimension data

    // TODO: How to do the doc for this (I think it will be straight forward once implemented
    // TODO: Do I need to implement a processing function to make it prettier the handling of that weird ass fortran matrix shit ? If so, It will be nice to put an interface to force the implementation of said function
    // TODO: 
    internal class MultiCharParameterData : ParameterData<List<char>>
    {
        public override MultiCharParameterData FromByte(byte[] data, int[]? dimensions, ProcessorType? _ = null)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException(nameof(dimensions), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else
            {
                MultiCharParameterData result = new MultiCharParameterData();
                result.Data = FortranMatrix.FVectorToFMatrix<char>(data, dimensions, DataLength.CHAR);
                return result;
            }
        }
    }
    internal class MultiByteParameterData : ParameterData<List<byte>>
    {
        public override MultiByteParameterData FromByte(byte[] data, int[]? dimensions, ProcessorType? _ = null)
        {
            if (dimensions == null)
            {
                throw new ArgumentNullException(nameof(dimensions), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else
            {
                MultiByteParameterData result = new MultiByteParameterData();
                result.Data = FortranMatrix.FVectorToFMatrix<byte>(data, dimensions, DataLength.BYTE);
                return result;
            }
        }
    }
    internal class MultiIntParameterData : ParameterData<List<int>>
    {
        public override MultiIntParameterData FromByte(byte[] data, int[]? dimensions, ProcessorType? processor = null)
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
                MultiIntParameterData result = new MultiIntParameterData();
                result.Data = FortranMatrix.FVectorToFMatrix<int>(data, dimensions, DataLength.INT16);
                return result;
            }
        }
    }

    internal class MultiFloatParameterData : ParameterData<List<float>>
    {
        public override MultiFloatParameterData FromByte(byte[] data, int[]? dimensions, ProcessorType? processor = null)
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
                MultiFloatParameterData result = new MultiFloatParameterData();
                result.Data = FortranMatrix.FVectorToFMatrix<float>(data, dimensions, DataLength.INT16);
                return result;
            }
        }
    }
}