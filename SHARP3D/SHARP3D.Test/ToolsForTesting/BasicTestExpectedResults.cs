using System.Text.Json.Serialization;

namespace SHARP3D.Test.Utils
{
    public record struct BasicTestExpectedResults
    {
            [JsonPropertyName("groups_parameter")]
            public string[] GroupsParameter { get; set; }

            [JsonPropertyName("parameters")]
            public string[][] Parameters { get; set; }

            [JsonPropertyName("point_first_0")]
            public float[] PointFirst0 { get; set; }

            [JsonPropertyName("point_last_0")]
            public float[] PointLast0 { get; set; }

            [JsonPropertyName("analog_first_0")]
            public float AnalogFirst0 { get; set; }

            [JsonPropertyName("analog_last_0")]
            public float AnalogLast0 { get; set; }

            [JsonPropertyName("point_frames")]
            public int PointFrames { get; set; }

            [JsonPropertyName("analog_frames")]
            public int AnalogFrames { get; set; }

    }
}
