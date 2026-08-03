using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SaesenthessisBlazeTests
    {
        [Fact]
        public async Task EmptyDeckResurrectsEligibleCemeteryUnitsAndContinuesDrawing()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();

            var blaze = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.SaesenthessisBlaze,
                RowPosition.MyHand);
            var banishedHand = Enumerable.Range(0, 4)
                .Select(_ => fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.GeraltOfRivia,
                    RowPosition.MyHand))
                .ToArray();
            var originalDeck = new[]
            {
                fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.GeraltOfRivia,
                    RowPosition.MyDeck),
                fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.IceTroll,
                    RowPosition.MyDeck)
            };
            var skirmisher = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.TuirseachSkirmisher,
                RowPosition.MyCemetery);
            var cemeteryUnits = new[]
            {
                skirmisher,
                fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.GeraltOfRivia,
                    RowPosition.MyCemetery),
                fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.IceTroll,
                    RowPosition.MyCemetery)
            };
            var cemeterySpecial = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.BitingFrost,
                RowPosition.MyCemetery);
            var cemeteryLeader = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.WhisperingHillock,
                RowPosition.MyCemetery);
            var cemeterySpy = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Cantarella,
                RowPosition.MyCemetery);
            var skirmisherBaseStrength = skirmisher.Status.Strength;
            await fixture.SynchronizeClientsAsync();

            await blaze.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.All(banishedHand, card => Assert.Equal(RowPosition.Banish, card.Status.CardRow));
            Assert.Equal(4, fixture.Game.PlayersHandCard[fixture.Game.Player1Index].Count);
            Assert.Single(fixture.Game.PlayersDeck[fixture.Game.Player1Index]);
            Assert.All(originalDeck, card =>
                Assert.Contains(card, fixture.Game.PlayersHandCard[fixture.Game.Player1Index]));
            Assert.Equal(
                2,
                cemeteryUnits.Count(card => card.Status.CardRow.IsInHand()));
            Assert.Single(cemeteryUnits, card => card.Status.CardRow.IsInDeck());
            Assert.DoesNotContain(
                cemeteryUnits,
                card => card.Status.CardRow.IsInCemetery());
            Assert.Equal(skirmisherBaseStrength + 3, skirmisher.Status.Strength);
            Assert.Equal(RowPosition.MyCemetery, cemeterySpecial.Status.CardRow);
            Assert.Equal(RowPosition.MyCemetery, cemeteryLeader.Status.CardRow);
            Assert.Equal(RowPosition.MyCemetery, cemeterySpy.Status.CardRow);
            Assert.Equal(3, fixture.Game.PlayersCemetery[fixture.Game.Player1Index].Count);
        }

        [Fact]
        public async Task DrawingStopsCleanlyWhenNoCemeteryUnitsCanRefillTheDeck()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();

            var blaze = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.SaesenthessisBlaze,
                RowPosition.MyHand);
            var banishedHand = Enumerable.Range(0, 2)
                .Select(_ => fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.GeraltOfRivia,
                    RowPosition.MyHand))
                .ToArray();
            fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyDeck);
            var cemeterySpecial = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.BitingFrost,
                RowPosition.MyCemetery);
            var cemeteryLeader = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.WhisperingHillock,
                RowPosition.MyCemetery);
            var cemeterySpy = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Cantarella,
                RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();

            await blaze.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.All(banishedHand, card => Assert.Equal(RowPosition.Banish, card.Status.CardRow));
            Assert.Single(fixture.Game.PlayersHandCard[fixture.Game.Player1Index]);
            Assert.Empty(fixture.Game.PlayersDeck[fixture.Game.Player1Index]);
            Assert.Equal(RowPosition.MyCemetery, cemeterySpecial.Status.CardRow);
            Assert.Equal(RowPosition.MyCemetery, cemeteryLeader.Status.CardRow);
            Assert.Equal(RowPosition.MyCemetery, cemeterySpy.Status.CardRow);
        }
    }
}
