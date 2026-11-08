using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class OctoberTwentyFourthFilavandrelTests
    {
        [Fact]
        public async Task GeneratesOnlyScoiaTaelSilverSpecialsOutsideStartingDeck()
        {
            var f = new HeadlessGameFixture();
            var leader = f.AddCard(f.Game.Player1Index, CardId.Filavandrel, RowPosition.MyRow1);
            var eligible = GwentMap.GetGenerateCardsId(card =>
                card.Faction == Faction.ScoiaTael && card.Is(Group.Silver, CardType.Special)).ToArray();
            Assert.True(eligible.Length > 1);
            var excluded = eligible[0];
            f.Game.PlayerBaseDeck[f.Game.Player1Index].Deck = new List<GwentCard>
            {
                GwentMap.CardMap[excluded]
            };
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => leader.Effect.CardPlayEffect(false, false));

            var options = f.FirstPlayer.MenuRequests.Single().SelectList;
            Assert.Equal(eligible.Skip(1).OrderBy(id => id), options.Select(card => card.CardId).OrderBy(id => id));
            Assert.All(options, card =>
            {
                var definition = GwentMap.CardMap[card.CardId];
                Assert.Equal(Faction.ScoiaTael, definition.Faction);
                Assert.Equal(Group.Silver, definition.Group);
                Assert.Equal(CardType.Special, definition.CardType);
                Assert.NotEqual(excluded, card.CardId);
            });
            Assert.Single(f.Game.PlayersStay[f.Game.Player1Index]);
            Assert.Contains(f.Game.PlayersStay[f.Game.Player1Index].Single().Status.CardId,
                options.Select(card => card.CardId));
        }

        [Fact]
        public async Task GeneratesNothingWhenAllEligibleCardsAreInStartingDeck()
        {
            var f = new HeadlessGameFixture();
            var leader = f.AddCard(f.Game.Player1Index, CardId.Filavandrel, RowPosition.MyRow1);
            f.Game.PlayerBaseDeck[f.Game.Player1Index].Deck = GwentMap.GetGenerateCardsId(card =>
                card.Faction == Faction.ScoiaTael && card.Is(Group.Silver, CardType.Special))
                .Select(id => GwentMap.CardMap[id]).ToList();
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() => leader.Effect.CardPlayEffect(false, false));

            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.Empty(f.Game.PlayersStay[f.Game.Player1Index]);
        }
    }
}
