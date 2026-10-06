using Newtonsoft.Json.Utilities;
using UnityEngine.Scripting;
using Cynthia.Card;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
public class AotTypeEnforcer
{
    [Preserve]
    private void EnsureTypes()
    {
        AotHelper.EnsureList<HideTag>();
        AotHelper.EnsureList<RowPosition>();
    }
}

public class BoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert,
                            JsonSerializerOptions options) => reader.GetBoolean();
    public override void Write(Utf8JsonWriter writer, bool value,
            JsonSerializerOptions options) => writer.WriteBooleanValue(value);
}

public class NullableBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        return reader.GetBoolean();
    }

    public override void Write(Utf8JsonWriter writer, bool? value,
        JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteBooleanValue(value.Value);
        else
            writer.WriteNullValue();
    }
}

public class IntArrayConverter : JsonConverter<int[]>
{
    public override int[] Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected a JSON array of integers.");

        var values = new List<int>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return values.ToArray();
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException("Expected an integer array element.");
            values.Add(reader.GetInt32());
        }

        throw new JsonException("Incomplete integer array.");
    }

    public override void Write(Utf8JsonWriter writer, int[] value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
            writer.WriteNumberValue(item);
        writer.WriteEndArray();
    }
}

// System.Text.Json creates its default Dictionary<string, int> converter through
// reflection. Unity 2019 IL2CPP cannot compile that closed generic constructor
// when it is only discovered at runtime while parsing a SignalR response.
public class StringIntDictionaryConverter : JsonConverter<Dictionary<string, int>>
{
    public override Dictionary<string, int> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected a JSON object for Dictionary<string, int>.");

        var result = new Dictionary<string, int>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return result;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected a dictionary key.");

            var key = reader.GetString();
            if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                throw new JsonException("Expected an integer dictionary value.");
            result[key] = reader.GetInt32();
        }

        throw new JsonException("Incomplete Dictionary<string, int> JSON object.");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, int> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var entry in value)
            writer.WriteNumber(entry.Key, entry.Value);
        writer.WriteEndObject();
    }
}

public class PremiumDeckSelectionDictionaryConverter : JsonConverter<Dictionary<string, PremiumDeckSelection>>
{
    public override Dictionary<string, PremiumDeckSelection> Read(ref Utf8JsonReader reader,
        Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected a JSON object for premium deck selections.");

        var result = new Dictionary<string, PremiumDeckSelection>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return result;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected a premium deck key.");

            var key = reader.GetString();
            if (!reader.Read())
                throw new JsonException("Expected a premium deck selection.");
            result[key] = JsonSerializer.Deserialize<PremiumDeckSelection>(ref reader, options);
        }

        throw new JsonException("Incomplete premium deck selections object.");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, PremiumDeckSelection> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var entry in value)
        {
            writer.WritePropertyName(entry.Key);
            JsonSerializer.Serialize(writer, entry.Value, options);
        }
        writer.WriteEndObject();
    }
}

public class ListOperationConverter : JsonConverter<IList<Operation<int>>>
{
    public override IList<Operation<int>> Read(ref Utf8JsonReader reader, Type typeToConvert,
                    JsonSerializerOptions options)
    {
        Debug.Log(DateTime.Now.ToString("h:mm:ss tt") + $" 开始反序列化Operations");
        var list = new List<Operation<int>>();
        var currArguments = new List<string>();
        bool receivingArguments = false;
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    list.Add(new Operation<int>());
                    break;
                case JsonTokenType.EndObject: break;
                case JsonTokenType.StartArray:
                    receivingArguments = true;
                    break;
                case JsonTokenType.EndArray:
                    if (receivingArguments)
                    {
                        list[list.Count - 1].Arguments = currArguments.ToArray();
                        currArguments.Clear();
                        receivingArguments = false;
                    }
                    break;
                case JsonTokenType.String:
                    currArguments.Add(reader.GetString());
                    break;
                case JsonTokenType.Number:
                    list[list.Count - 1].OperationType = reader.GetInt32();
                    break;
                case JsonTokenType.PropertyName:
                    break;
            }
        }
        Debug.Log(DateTime.Now.ToString("h:mm:ss tt") + $" 结束反序列化Operations");
        return list;
    }
    public override void Write(Utf8JsonWriter writer, IList<Operation<int>> value,
            JsonSerializerOptions options)
    {
        Debug.Log(DateTime.Now.ToString("h:mm:ss tt") + " start writing");
        JsonSerializer.Serialize(writer, value);
        Debug.Log(DateTime.Now.ToString("h:mm:ss tt") + " end writing");

    }
}
