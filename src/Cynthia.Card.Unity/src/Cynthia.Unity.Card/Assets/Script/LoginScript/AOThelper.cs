using Newtonsoft.Json.Utilities;
using UnityEngine.Scripting;
using Cynthia.Card;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
public class AotTypeEnforcer
{
    [Preserve]
    private void EnsureTypes()
    {
        AotHelper.EnsureList<HideTag>();
    }
}

public class BoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert,
                            JsonSerializerOptions options) => reader.GetBoolean();
    public override void Write(Utf8JsonWriter writer, bool value,
            JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, options);
}