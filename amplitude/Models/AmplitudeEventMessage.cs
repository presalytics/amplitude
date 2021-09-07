namespace amplitude.Models
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using CloudNative.CloudEvents;
    using System.Collections.Generic;
    public class AmplitudeEventMessage
    {
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set;}
        [JsonPropertyName("events")]
        public List<CloudEvent> Events { get; set;}
    }
}