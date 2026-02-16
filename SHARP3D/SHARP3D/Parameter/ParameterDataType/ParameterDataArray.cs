using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterDataType
{
    // Single dimension data

    // TODO: How to do the doc for this (I think it will be straight forward once implemented
    // TODO: Do I need to implement a processing function to make it prettier the handling of that weird ass fortran matrix shit ? If so, It will be nice to put an interface to force the implementation of said function
    // TODO: 
    internal class MultiCharParameterData : ParameterData<char[]>
    {
        public override MultiCharParameterData FromByte(byte[] data, int[]? dimension, ProcessorType? _ = null)
        {
            if (dimension == null)
            {
                throw new ArgumentNullException(nameof(dimension), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else
            {
                for (int i = 0; i < dimension.Length; i++)
                {

                }
                //return new MultiCharParameterData();
                return new MultiCharParameterData();
            }
        }
    }
    internal class MultiByteParameterData : ParameterData<byte[]>
    {
        public override MultiByteParameterData FromByte(byte[] data, int[]? dimension, ProcessorType? _ = null)
        {
            if (dimension == null)
            {
                throw new ArgumentNullException(nameof(dimension), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else
            {
                return new MultiByteParameterData
                {
                }
            }
        }
    }
    internal class MultiIntParameterData : ParameterData<int[]>
    {
        public override MultiIntParameterData FromByte(byte[] data, int[]? dimension, ProcessorType? processor = null)
        {
            if (dimension == null)
            {
                throw new ArgumentNullException(nameof(dimension), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for int parameter data.");
            }
            else 
            {
                return new MultiIntParameterData
                {
                }
            }
        }
    }

    internal class MultiFloatParameterData : ParameterData<float[]>
    {
        public override MultiFloatParameterData FromByte(byte[] data, int[]? dimension, ProcessorType? processor = null)
        {
            if (dimension == null)
            {
                throw new ArgumentNullException(nameof(dimension), "Dimensions must be provided for multi-dimensional char parameter data.");
            }
            else if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for int parameter data.");
            }
            else
            {
                return new MultiFloatParameterData
            }
        }
    }
}