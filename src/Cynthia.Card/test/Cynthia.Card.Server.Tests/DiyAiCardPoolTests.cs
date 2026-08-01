using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Cynthia.Card;
using Cynthia.Card.AI;
using Xunit;

namespace Cynthia.Card.Server.Tests
{
    public class DiyAiCardPoolTests
    {
        [Fact]
        public void ResetPoolRetiresOnlyDiyCardsAndPreservesSystemCards()
        {
            Assert.Equal(183, DiyAiCardPool.RetiredCardIds.Count);
            Assert.Equal(10, DiyAiCardPool.SystemCardIds.Count);
            Assert.Empty(DiyAiCardPool.RetiredCardIds.Intersect(DiyAiCardPool.SystemCardIds));

            Assert.All(
                DiyAiCardPool.RetiredCardIds,
                cardId => Assert.True(GwentMap.CardMap[cardId].IsDerive));
            Assert.All(
                DiyAiCardPool.SystemCardIds,
                cardId => Assert.True(GwentMap.CardMap.ContainsKey(cardId)));
            Assert.DoesNotContain(
                GwentMap.CardMap.Values.SelectMany(card => card.LinkedCards ?? Enumerable.Empty<string>()),
                DiyAiCardPool.RetiredCardIds.Contains);
        }

        [Fact]
        public void CardMapOrdinalOrderRemainsHistoricalDecodeCompatible()
        {
            Assert.Equal(new Version(1, 0, 0, 154), GwentMap.CardMapVersion);
            Assert.Equal(709, GwentMap.CardMap.Count);

            var orderedIds = string.Join(",", GwentMap.CardMap.Keys);
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(orderedIds)))
                .ToLowerInvariant();

            Assert.Equal("1d92e39fd29ffd178e2998d5c9bc761cebd37bf5c3adee040505840d9fab84a9", hash);
        }

        [Fact]
        public void OriginalStarterDeckIsPlayableAndRetiredCardsAreRejected()
        {
            var starter = GwentDeck.CreateBasicDeck(0);
            Assert.True(starter.IsBasicDeck());
            Assert.All(starter.Deck, cardId => Assert.True(DiyAiCardPool.IsUserDeckCard(cardId)));

            starter.Deck[0] = "70001";
            Assert.False(starter.IsBasicDeck());
            Assert.False(starter.IsSpecialDeck());
        }

        [Fact]
        public void AiZeroThroughFiveKeepAllRuntimeCardDependencies()
        {
            var players = new AIPlayer[]
            {
                new GeraltNovaAI(),
                new SoldierTrainAI(),
                new MillAI(),
                new AuberonKingAI(),
                new IronFalconAI(),
                new ReaverHunterAI()
            };

            Assert.All(players, player =>
            {
                Assert.True(GwentMap.CardMap.ContainsKey(player.Deck.Leader));
                Assert.All(player.Deck.Deck, cardId =>
                {
                    Assert.True(GwentMap.CardMap.ContainsKey(cardId));
                    Assert.DoesNotContain(cardId, DiyAiCardPool.RetiredCardIds);
                });
            });
        }

        [Fact]
        public void DiyWeatherRulesRemainExplicitExceptions()
        {
            Assert.Equal(10, GwentMap.CardMap["12009"].Strength);
            Assert.Contains("最强单位", GwentMap.CardMap["14019"].Info);
            Assert.Contains("最弱单位", GwentMap.CardMap["14019"].Info);
            Assert.True(typeof(IHandlesEvent<AfterCardMove>).IsAssignableFrom(typeof(AleOfTheAncestors)));
            Assert.True(typeof(IHandlesEvent<AfterCardMove>).IsAssignableFrom(typeof(BloodMoonStatus)));
            Assert.True(typeof(IHandlesEvent<AfterCardMove>).IsAssignableFrom(typeof(PitTrapStatus)));
        }
    }
}
