namespace BlazorCanvas2d.Serialization;

internal sealed class KeyboardModifierJsonConverter : JsonConverter<KeyboardModifierState>
{
    public override KeyboardModifierState Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        bool shift = false,
            ctrl = false,
            alt = false,
            meta = false;

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
                    case "shift":
                        shift = reader.GetBoolean();
                        break;

                    case "ctrl":
                        ctrl = reader.GetBoolean();
                        break;

                    case "alt":
                        alt = reader.GetBoolean();
                        break;

                    case "meta":
                        meta = reader.GetBoolean();
                        break;
                }
            }
        }

        return new KeyboardModifierState(shift, ctrl, alt, meta);
    }

    public override void Write(
        Utf8JsonWriter writer,
        KeyboardModifierState value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        writer.WriteBoolean("shift", value.Shift);
        writer.WriteBoolean("ctrl", value.Ctrl);
        writer.WriteBoolean("alt", value.Alt);
        writer.WriteBoolean("meta", value.Meta);
        writer.WriteEndObject();
    }
}
