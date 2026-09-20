using System.Collections.Generic;
using System.Text.Json;
using Cynthia.Card;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public sealed class PremiumCompatibilityTests
    {
        [Fact]
        public void PremiumFieldsAreAdditiveForLegacyWireReaders()
        {
            var status = new CardStatus { CardId = "legacy-card", CardInfo = new GwentCard(), IsPremium = true };
            var json = JsonSerializer.Serialize(status);
            var legacy = JsonSerializer.Deserialize<LegacyCardStatus>(json);

            Assert.Equal(status.CardId, legacy.CardId);
            Assert.True(JsonDocument.Parse(json).RootElement.TryGetProperty("IsPremium", out _));

            var legacyDeck = JsonSerializer.Deserialize<DeckModel>(
                "{\"Id\":\"7c439ffd-487e-4933-86e5-e66a7a796bd4\",\"Name\":\"old\",\"Leader\":\"11001\",\"Deck\":[]}");
            Assert.Null(legacyDeck.PremiumCards);
            Assert.Null(legacyDeck.PremiumLeader);
        }

        [Fact]
        public void UserDocumentsKeepTheLegacyDeckSchema()
        {
            Program.ConfigureMongoMappings();
            var user = new UserInfo
            {
                Decks = new List<DeckModel>
                {
                    new DeckModel
                    {
                        Leader = "11001",
                        Deck = new List<string> { "12001" },
                        PremiumCards = new Dictionary<string, int> { ["12001"] = 1 },
                        PremiumLeader = true
                    }
                }
            };

            var document = user.ToBsonDocument();
            var storedDeck = document["Decks"].AsBsonArray[0].AsBsonDocument;
            Assert.False(storedDeck.Contains("PremiumCards"));
            Assert.False(storedDeck.Contains("PremiumLeader"));

            storedDeck["PremiumCards"] = new BsonDocument("12001", 1);
            storedDeck["PremiumLeader"] = true;
            var restored = BsonSerializer.Deserialize<UserInfo>(document);
            Assert.Null(restored.Decks[0].PremiumCards);
            Assert.Null(restored.Decks[0].PremiumLeader);
        }

        private sealed class LegacyCardStatus
        {
            public string CardId { get; set; }
        }
    }
}
