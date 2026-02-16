using SHARP3D.Utils;
using SHARP3D.Utils.Enum;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SHARP3D.Parameter.ParameterDataType
{
    // Single dimension data

    // TODO: How to do the doc for this
    internal class CharParameterData : ParameterData<char>
    {
        public CharParameterData() { }
        public CharParameterData(byte[] data, int[]? _ = null, ProcessorType? __ = null) 
        {
            Data = FromByte(data);
        }
        
        //public override CharParameterData FromByte(byte[] data, int[]? _ = null, ProcessorType ? __ = null)
        public override char FromByte(byte[] data, int[]? _ = null, ProcessorType ? __ = null)
        {
            //CharParameterData result = new CharParameterData();
            //return new CharParameterData { Data = (char)data[0] };
            return (char)data[0];
        }
    }

    internal class ByteParameterData : ParameterData<byte>
    {
        public ByteParameterData() { }

        public ByteParameterData(byte[] data, int[]? _ = null, ProcessorType? __ = null)
        {
            FromByte(data);
        }

        public override ByteParameterData FromByte(byte[] data, int[]? _ = null, ProcessorType ? __ = null)
        {
            return new ByteParameterData { Data = data[0] };
        }
    }
    internal class IntParameterData : ParameterData<int>
    {
        public IntParameterData() { }

        public IntParameterData(byte[] data, int[]? _ = null, ProcessorType? processor = null)
        {
            FromByte(data, _, processor);
        }

        public override IntParameterData FromByte(byte[] data, int[]? _ = null, ProcessorType? processor = null)
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
        public FloatParameterData() { }

        public FloatParameterData(byte[] data, int[]? _ = null, ProcessorType? processor = null)
        {
            FromByte(data, _, processor);
        }

        public override FloatParameterData FromByte(byte[] data, int[]? _ = null, ProcessorType? processor = null)
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