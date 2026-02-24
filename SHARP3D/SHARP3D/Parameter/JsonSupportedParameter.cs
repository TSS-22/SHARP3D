using SHARP3D.Utils.Enum;
using System.Text.Json.Serialization;

namespace SHARP3D.Parameter
{
    public struct JsonSupportedParameter
    {
        [JsonPropertyName("group")]
        public string Group { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public int ParameterType { get; set; }
        [JsonPropertyName("general_description")]
        public string GeneralDescription { get; set; }
        [JsonPropertyName("dimension_description")]
        public string[] DimensionDescription { get; set; }
    }
}
