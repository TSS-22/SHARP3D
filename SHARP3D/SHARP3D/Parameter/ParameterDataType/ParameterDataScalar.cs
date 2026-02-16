using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterDataType
{
    // Single dimension data

    // TODO: How to do the doc for this
    internal class CharParameterData : ParameterData
    {
        char Data { set; get; }
        public CharParameterData(byte[] data, int[]? _ = null, ProcessorType? __ = null) 
        {
            Data = (char)data[0];
        }
    }

    internal class ByteParameterData : ParameterData
    {
        byte Data { set; get; }

        public ByteParameterData(byte[] data, int[]? _ = null, ProcessorType? __ = null)
        {
            Data = data[0];
        }
    }
    internal class IntParameterData : ParameterData
    {
        int Data { set; get; }

        public IntParameterData(byte[] data, int[]? _ = null, ProcessorType? processor = null)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for int parameter data.");
            }
            else
            {
                Data = C3dBytesConvertor.ToInt(data, (ProcessorType)processor);
            }
        }
    }

    internal class FloatParameterData : ParameterData
    {
        float Data { set; get; }

        public FloatParameterData(byte[] data, int[]? _ = null, ProcessorType? processor = null)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for float parameter data.");
            }
            else
            {
                Data = C3dBytesConvertor.ToFloat(data, (ProcessorType)processor);
            }
        }
    }
}