namespace BlazorCanvas2d.Serialization;

internal sealed class MouseClickEventJsonConverter : JsonConverter<MouseClickEvent>
{
    public override MouseClickEvent Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        double clientX = 0,
            clientY = 0;
        int button = 0;

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();

                if (string.IsNullOrEmpty(propertyName))
                {
                    // If the property name is null or empty, skip to the next property.
                    reader.Skip();
                    continue;
                }

                reader.Read();

                switch (propertyName.ToLower())
                {
                    case "button":
                        button = reader.GetInt32();
                        break;

                    case "clientx":
                        clientX = reader.GetDouble();
                        break;

                    case "clienty":
                        clientY = reader.GetDouble();
                        break;
                }
            }
        }

        return new MouseClickEvent(clientX, clientY, button);
    }

    public override void Write(
        Utf8JsonWriter writer,
        MouseClickEvent value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        writer.WriteNumber("button", value.Button);
        writer.WriteNumber("clientX", value.ClientX);
        writer.WriteNumber("clientY", value.ClientY);
        writer.WriteEndObject();
    }
}
