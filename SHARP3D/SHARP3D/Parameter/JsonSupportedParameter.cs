using System.Text.Json.Serialization;

namespace SHARP3D.Parameter
{
    /// <summary>
    /// Represents a supported parameter in JSON format, used for serialization and deserialization.
    /// </summary>
    public struct JsonSupportedParameter
    {
        /// <summary>
        /// Gets or sets the group name of the parameter.
        /// </summary>
        [JsonPropertyName("group")]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        [JsonPropertyName("type")]
        public int ParameterType { get; set; }

        /// <summary>
        /// Gets or sets the general description of the parameter.
        /// </summary>
        [JsonPropertyName("general_description")]
        public string GeneralDescription { get; set; }

        /// <summary>
        /// Gets or sets the description of the parameter's dimensions.
        /// </summary>
        [JsonPropertyName("dimension_description")]
        public string[] DimensionDescription { get; set; }
    }
}
