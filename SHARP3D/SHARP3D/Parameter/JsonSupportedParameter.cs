using SHARP3D.Utils.Enum;
using System.Text.Json.Serialization;

namespace SHARP3D.Parameter
{
    public struct JsonSupportedParameter
    {
        [JsonPropertyName("group")]
        public string Group;
        [JsonPropertyName("name")]
        public string Name;
        [JsonPropertyName("type")]
        public ParameterType ParameterType;
        [JsonPropertyName("general_description")]
        public string GeneralDescription;
        [JsonPropertyName("dimension_description")]
        public string[] DimensionDescription;
    }
}
