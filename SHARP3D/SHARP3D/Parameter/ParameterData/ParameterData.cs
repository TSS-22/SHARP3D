using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterData
{
    // TODO: The whole ParameterData class
    public abstract class ParameterData<T>
    {
        public T Data { set; get; }
        public abstract ParameterData<T> FromByte(byte[] data, ProcessorType? processor = null, int[]? dimension = null);

    }

}
