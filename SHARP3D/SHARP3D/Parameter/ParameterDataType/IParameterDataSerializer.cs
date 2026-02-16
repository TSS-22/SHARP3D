using SHARP3D.Utils.Enum;

namespace SHARP3D.Parameter.ParameterData
{
    internal interface IParameterDataSerializer
    {
        internal DeserializeData(
            byte[] data,
            
            ProcessorType? processor = null
            );
    }
}
