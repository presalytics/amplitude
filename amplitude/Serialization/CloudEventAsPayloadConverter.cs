namespace amplitude.Serialization
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using CloudNative.CloudEvents;
    using System.Collections.Generic;
    using System;

    // This is, admittedly, pretty hacky.. could be much simplier and strongly typed
    public class CloudEventAsPayloadConverter : JsonConverter<CloudEvent>
    {
        public override CloudEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("This method really shouldn't be called.  Receive a CloudEvent per the docs instead.");
        }
        public override void Write(
            Utf8JsonWriter writer,
            CloudEvent cloudEvent,
            JsonSerializerOptions options)
        {

            string serializedData = JsonSerializer.Serialize(cloudEvent.Data, options);
            JsonDocument data = JsonDocument.Parse(serializedData);
            if (!data.RootElement.TryGetProperty("userId", out JsonElement userIdProperty))
            {
                userIdProperty = data.RootElement.GetProperty("user_id");
            }
            string userId = Object.Equals(userIdProperty, default(JsonElement)) ? Guid.Empty.ToString() : userIdProperty.GetString();
            if (!data.RootElement.TryGetProperty("deviceId", out JsonElement deviceIdProperty))
            {
                deviceIdProperty = data.RootElement.GetProperty("device_id");
            }
            string deviceId = Object.Equals(deviceIdProperty, default(JsonElement)) ? Guid.Empty.ToString() : deviceIdProperty.GetString();

            Dictionary<string, object> eventProperties = JsonSerializer.Deserialize<Dictionary<string, object>>(data.RootElement.ToString());
            eventProperties.Remove("userId");
            eventProperties.Remove("user_id");
            eventProperties.Remove("device_id");
            eventProperties.Remove("deviceId");


            writer.WriteStartObject();
            writer.WriteString("user_id", userId);
            writer.WriteString("device_id", deviceId);
            List<string> _removeKeys = new List<string>();
            foreach (KeyValuePair<string, object> entry in eventProperties)
            {
                if (_topLevelKeys.Contains(entry.Key))
                {
                    JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(entry.Value));
                    foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                    {
                        property.WriteTo(writer);
                    }
                    _removeKeys.Add(entry.Key);
                }
            }
            _removeKeys.ForEach(x => eventProperties.Remove(x));
            if (cloudEvent.Type != null) writer.WriteString("event_type", cloudEvent.Type.ToString());
            if (cloudEvent.Time == null) 
            {
                writer.WriteNumber("time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            } else {
                writer.WriteNumber("time", ((DateTimeOffset)cloudEvent.Time).ToUnixTimeMilliseconds());
            }
            if (cloudEvent.Id != null) {
                writer.WriteString("insert_id", cloudEvent.Id);
            }
            if (cloudEvent.Source != null) eventProperties.Add("source", cloudEvent.Source.ToString());
            if (cloudEvent.Subject != null) eventProperties.Add("subject", cloudEvent.Subject);
            //writer.WriteString("event_properties", JsonSerializer.Serialize(eventProperties).Replace("/",""));

            JsonDocument props = JsonDocument.Parse(JsonSerializer.Serialize(eventProperties));
            writer.WritePropertyName("event_properties");
            writer.WriteStartObject();
            foreach (JsonProperty property in props.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        private static List<string> _topLevelKeys = new List<string> {    
            "user_properties",
            "groups",
            "app_version",
            "platform",
            "os_name",
            "os_version",
            "device_brand",
            "device_manufacturer",
            "device_model",
            "carrier",
            "country",
            "region",
            "city",
            "dma",
            "language",
            "price",
            "quantity",
            "revenue",
            "productId",
            "revenueType",
            "location_lat",
            "location_lng",
            "ip",
            "idfa",
            "idfv",
            "adid",
            "android_id",
            "event_id",
            "session_id",
            "insert_id"
        };
    }
}