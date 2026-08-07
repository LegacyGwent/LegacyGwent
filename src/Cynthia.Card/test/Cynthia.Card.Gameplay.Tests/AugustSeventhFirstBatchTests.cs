using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustSeventhFirstBatchTests
    {
        [Fact]
        public async Task MantletStartsWithSixArmorAndProtectsBothAdjacentAllies()
        {
            var fixture = new HeadlessGameFixture();
            var left = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            var mantlet = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Mantlet, RowPosition.MyRow1);
            var right = fixture.AddCard(
                fixture.Game.Player1Index, CardId.RedanianKnight, RowPosition.MyRow1);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await mantlet.Effect.CardPlayEffect(false, false);
            Assert.Equal(6, mantlet.Status.Armor);

            await left.Effect.Damage(4, enemy);
            Assert.Equal(0, left.Status.HealthStatus);
            Assert.Equal(2, mantlet.Status.Armor);

            await right.Effect.Damage(5, enemy);
            Assert.Equal(-3, right.Status.HealthStatus);
            Assert.Equal(0, mantlet.Status.Armor);
        }

        [Fact]
        public async Task ImmortalCavalryStaysLockedUntilExternallyUnlockedThenRelocksAfterTwoOwnerTurns()
        {
            var fixture = new HeadlessGameFixture();
            var cavalry = fixture.AddCard(
                fixture.Game.Player1Index, CardId.ImmortalCavalry, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new OnGameStart());
            Assert.True(cavalry.Status.IsLock);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(cavalry.Status.IsLock);

            await cavalry.Effect.Lock(cavalry);
            Assert.False(cavalry.Status.IsLock);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player2Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.False(cavalry.Status.IsLock);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(cavalry.Status.IsLock);
        }

        [Theory]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        public async Task LyrianCavalryGainsHalfTheSelectedUnitsBoostRoundedUp(
            int selectedBoost,
            int expectedBoost)
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var selected = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            selected.Status.HealthStatus = selectedBoost;
            var cavalry = fixture.AddCard(
                fixture.Game.Player1Index, CardId.LyrianCavalry, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await cavalry.Effect.CardPlayEffect(false, false);

            Assert.Equal(expectedBoost, cavalry.Status.HealthStatus);
        }

        [Fact]
        public async Task ReynardOdoHasNoThreeUseLimit()
        {
            var fixture = new HeadlessGameFixture();
            var reynard = fixture.AddCard(
                fixture.Game.Player1Index, CardId.ReynardOdo, RowPosition.MyRow1);
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            for (var i = 0; i < 4; i++)
            {
                await reynard.Effect.Boost(3, reynard);
                await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            }

            Assert.Equal(12, reynard.Status.Armor);
            Assert.Equal(8, ally.Status.HealthStatus);
        }

        [Fact]
        public async Task WarElephantUsesItsOwnArmorEvenWithoutAdjacentAllies()
        {
            var fixture = new HeadlessGameFixture();
            var elephant = fixture.AddCard(
                fixture.Game.Player1Index, "70033", RowPosition.MyRow1);
            elephant.Status.Armor = 6;
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await elephant.Effect.CardPlayEffect(false, false);

            Assert.Equal(0, elephant.Status.Armor);
            Assert.Equal(-6, enemy.Status.HealthStatus);
        }
    }
}
