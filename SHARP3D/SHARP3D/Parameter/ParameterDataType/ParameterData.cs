using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterDataType
{
    // TODO: The whole ParameterData class
    public abstract class ParameterData<T>
    {
        public T Data { set; get; }
        public abstract T FromByte(byte[] data, int[]? dimensions = null, ProcessorType ? processor = null);

    }

}
