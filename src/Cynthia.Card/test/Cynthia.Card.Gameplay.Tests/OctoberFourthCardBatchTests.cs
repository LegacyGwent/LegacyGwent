using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class OctoberFourthCardBatchTests
    {
        [Fact]
        public async Task GarkainOnlyTargetsTheOppositeRowAcrossAllFiveHits()
        {
            var fixture = new HeadlessGameFixture();
            var garkain = fixture.AddCard(fixture.Game.Player1Index, CardId.Garkain, RowPosition.MyRow2);
            var opposite = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 20);
            var other = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(() => garkain.Effect.CardPlayEffect(false, false));

            Assert.Equal(-5, opposite.Status.HealthStatus);
            Assert.Equal(0, other.Status.HealthStatus);
        }

        [Fact]
        public async Task ArtisCanTransformAnAlliedUnitWithoutSoldierCategory()
        {
            var fixture = new HeadlessGameFixture();
            var ally = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            var artis = fixture.AddCard(fixture.Game.Player1Index, CardId.Artis, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(() => artis.Effect.CardPlayEffect(false, false));

            Assert.Equal(CardId.SvalblodFanatic, ally.Status.CardId);
        }

        [Fact]
        public async Task DopplerFallsBackToFarmerWhenHandCountHasNoEligiblePower()
        {
            var fixture = new HeadlessGameFixture();
            var doppler = fixture.AddCard(fixture.Game.Player1Index, CardId.Doppler, RowPosition.MyStay);
            for (var i = 0; i < 30; i++)
            {
                fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            }
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(() => doppler.Effect.CardUseEffect());

            Assert.Contains(fixture.Game.RowToList(fixture.Game.Player1Index, RowPosition.MyStay),
                card => card.Status.CardId == "15011");
        }

        [Fact]
        public async Task DopplerOffersOnlyMatchingPowerFactionBronzesOutsideStartingDeck()
        {
            var fixture = new HeadlessGameFixture();
            var doppler = fixture.AddCard(fixture.Game.Player1Index, CardId.Doppler, RowPosition.MyStay);
            var startingIds = doppler.GetMyBaseDeck().Select(card => card.CardId).ToHashSet();
            var faction = fixture.Game.PlayersFaction[fixture.Game.Player1Index];
            var handCount = Enumerable.Range(1, 12).First(count =>
                GwentMap.GetGenerateCardsId(
                    card => card.Faction == faction &&
                        card.Is(Group.Copper, CardType.Unit) &&
                        card.Strength == count,
                    startingIds, isHasAgent: true).Any());
            for (var i = 0; i < handCount; i++)
            {
                fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            }
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(() => doppler.Effect.CardUseEffect());

            var menu = fixture.FirstPlayer.MenuRequests.Last();
            Assert.NotEmpty(menu.SelectList);
            Assert.All(menu.SelectList, card =>
            {
                var definition = GwentMap.CardMap[card.CardId];
                Assert.Equal(faction, definition.Faction);
                Assert.Equal(Group.Copper, definition.Group);
                Assert.Equal(CardType.Unit, definition.CardType);
                Assert.Equal(handCount, definition.Strength);
                Assert.DoesNotContain(card.CardId, startingIds);
            });
        }
    }
}
