using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cynthia.Card.Server
{
    // Publish the client contract without exposing persisted idempotency history.
    // MongoDB serialization and the Unity model's dependencies remain independent.
    public sealed class DailyQuestProgressJsonConverter : JsonConverter<DailyQuestProgress>
    {
        public override DailyQuestProgress Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) =>
            throw new NotSupportedException("Daily quest progress is server-owned and output-only.");

        public override void Write(Utf8JsonWriter writer, DailyQuestProgress value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, new {
                value.Day, value.LoginGranted, value.Crowns, value.PowderGranted,
                value.GGReceived, value.GGPowderGranted, value.RoundIds
            }, options);
    }
}
