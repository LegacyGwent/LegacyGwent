using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Cynthia.Card.Server
{
    public sealed class InitialPowderOptions
    {
        public bool Enabled { get; }
        public long Amount { get; }

        public InitialPowderOptions(long amount) : this(amount > 0, amount) { }

        public InitialPowderOptions(bool enabled, long amount)
        {
            if (amount < 0 || amount > 1000000000 || (enabled && amount == 0))
                throw new InvalidDataException("Initial powder Amount must be 1..1000000000 when enabled, or 0..1000000000 when disabled.");
            Enabled = enabled;
            Amount = amount;
        }

        public static InitialPowderOptions Load(string path = null)
        {
            var json = JObject.Parse(File.ReadAllText(path ?? Path.Combine(AppContext.BaseDirectory, "InitialPowder.json")));
            var enabled = json["Enabled"];
            var amount = json["Amount"];
            if (enabled == null || enabled.Type != JTokenType.Boolean || amount == null || amount.Type != JTokenType.Integer)
                throw new InvalidDataException("InitialPowder.json requires a boolean Enabled and an integer Amount.");
            return new InitialPowderOptions(enabled.Value<bool>(), amount.Value<long>());
        }
    }
}
