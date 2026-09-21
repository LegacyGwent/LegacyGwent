using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberTwentiethEffectsTests
    {
        [Fact]
        public async Task AvallachSageInspectsTheTopUnitOfEachQualityAndSpawnsTheSelectedBaseCopy()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player2Index].Clear();
            f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck);
            f.AddCard(f.Game.Player2Index, CardId.Nekker, RowPosition.MyDeck);
            f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyDeck);
            f.AddCard(f.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            var sage = f.AddCard(f.Game.Player1Index, CardId.AvallacHTheSage, RowPosition.MyRow1);
            f.FirstPlayer.QueueMenuCardIds(CardId.Eskel);
            await f.SynchronizeClientsAsync();

            var created = await sage.Effect.CardPlayEffect(false, false);

            Assert.Equal(1, created);
            Assert.Equal(
                new[] { CardId.Wolf, CardId.Eskel, CardId.GeraltOfRivia },
                f.FirstPlayer.MenuRequests.Single().SelectList.Select(card => card.CardId));
            Assert.Equal(CardId.Eskel,
                Assert.Single(f.Game.PlayersStay[f.Game.Player1Index]).Status.CardId);
        }

        [Fact]
        public async Task FiendMovesAnAlliedBeastToItsRow()
        {
            var f = new HeadlessGameFixture();
            var beast = f.AddCard(f.Game.Player1Index, CardId.Crow, RowPosition.MyRow1);
            var fiend = f.AddCard(f.Game.Player1Index, CardId.Fiend, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await fiend.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));

            Assert.Equal(RowPosition.MyRow2, beast.Status.CardRow);
        }

        [Fact]
        public async Task SvalblodPriestStrengthensAfterEveryTwoAdjacentDamageEvents()
        {
            var f = new HeadlessGameFixture();
            var left = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            var priest = f.AddCard(f.Game.Player1Index, CardId.SvalblodPriest, RowPosition.MyHand);
            var distant = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2, 20);
            var source = f.AddCard(f.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();
            await priest.Effect.Play(new CardLocation(RowPosition.MyRow1, 1));
            var initialStrength = priest.Status.Strength;

            await distant.Effect.Damage(1, source);
            Assert.Equal(2, priest.Status.Countdown);
            await left.Effect.Damage(1, source);
            Assert.Equal(1, priest.Status.Countdown);
            await left.Effect.Damage(1, source);

            Assert.Equal(initialStrength + 1, priest.Status.Strength);
            Assert.Equal(2, priest.Status.Countdown);
        }

        [Fact]
        public async Task SvalblodFanaticTakesOnlyThePowerActuallyLostByItsTarget()
        {
            var f = new HeadlessGameFixture();
            var fanatic = f.AddCard(f.Game.Player1Index, CardId.SvalblodFanatic, RowPosition.MyRow1);
            var target = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 5);
            target.Status.Armor = 2;
            await f.SynchronizeClientsAsync();

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));

            Assert.Equal(0, target.Status.Armor);
            Assert.Equal(-1, target.Status.HealthStatus);
            Assert.Equal(-1, fanatic.Status.HealthStatus);
        }

        [Fact]
        public async Task TrissDealsTenAndPhilippaDoublesHerFirstHitAgainstALockedTarget()
        {
            var trissFixture = new HeadlessGameFixture();
            var triss = trissFixture.AddCard(
                trissFixture.Game.Player1Index, CardId.TrissMerigold, RowPosition.MyRow1);
            var trissTarget = trissFixture.AddCard(
                trissFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            await trissFixture.SynchronizeClientsAsync();
            await triss.Effect.CardPlayEffect(false, false);
            Assert.Equal(-10, trissTarget.Status.HealthStatus);

            var philippaFixture = new HeadlessGameFixture();
            var philippa = philippaFixture.AddCard(
                philippaFixture.Game.Player1Index, CardId.PhilippaEilhart, RowPosition.MyRow1);
            var locked = philippaFixture.AddCard(
                philippaFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            locked.Status.IsLock = true;
            await philippaFixture.SynchronizeClientsAsync();
            await philippa.Effect.CardPlayEffect(false, false);
            Assert.Equal(-10, locked.Status.HealthStatus);
        }
    }
}
