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
            JsonSerializerOptions options) => writer.WriteStringValue(value ? "true" : "false");
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