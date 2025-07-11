﻿namespace BlazorCanvas2d.Serialization;

internal sealed class MouseScrollEventJsonConverter : JsonConverter<MouseScrollEvent>
{
    public override MouseScrollEvent Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        double deltaX = 0,
            deltaY = 0,
            clientX = 0,
            clientY = 0;

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
                    case "deltax":
                        deltaX = reader.GetDouble();
                        break;

                    case "deltay":
                        deltaY = reader.GetDouble();
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

        return new MouseScrollEvent(clientX, clientY, deltaX, deltaY);
    }

    public override void Write(
        Utf8JsonWriter writer,
        MouseScrollEvent value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        writer.WriteNumber("deltaX", value.DeltaX);
        writer.WriteNumber("deltaY", value.DeltaY);
        writer.WriteNumber("clientX", value.ClientX);
        writer.WriteNumber("clientY", value.ClientY);
        writer.WriteEndObject();
    }
}
