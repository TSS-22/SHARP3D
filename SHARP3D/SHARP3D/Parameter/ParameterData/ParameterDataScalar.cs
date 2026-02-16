using SHARP3D.Utils;
using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterData
{
    // Single dimension data

    // TODO: How to do the doc for this
    internal class CharParameterData : ParameterData<char>
    {
        public override CharParameterData FromByte(byte[] data, ProcessorType? _ = null, int[]? __ = null)
        {
            return new CharParameterData { Data = (char)data[0] };
        }
    }

    internal class ByteParameterData : ParameterData<byte>
    {
        public override ByteParameterData FromByte(byte[] data, ProcessorType? _ = null, int[] ? __ = null)
        {
            return new ByteParameterData { Data = data[0] };
        }
    }
    internal class IntParameterData : ParameterData<int>
    {
        public override IntParameterData FromByte(byte[] data, ProcessorType? processor = null, int[]? _ = null)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for int parameter data.");
            }
            else
            {
                return new IntParameterData { Data = C3dBytesConvertor.ToInt(data, (ProcessorType)processor) };
            }
        }
    }

    internal class FloatParameterData : ParameterData<float>
    {
        public override FloatParameterData FromByte(byte[] data, ProcessorType? processor = null, int[]? _ = null)
        {
            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor), "Processor type must be provided for float parameter data.");
            }
            else
            {
                return new FloatParameterData { Data = C3dBytesConvertor.ToFloat(data, (ProcessorType)processor) };
            }
        }
    }
}