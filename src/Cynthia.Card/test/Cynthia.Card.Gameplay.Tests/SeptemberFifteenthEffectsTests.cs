using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberFifteenthEffectsTests
    {
        [Fact]
        public async Task GasconSpendsTwoBoostPerMoveAndStopsWhenLessThanTwoRemain()
        {
            var f = new HeadlessGameFixture();
            var gascon = f.AddCard(f.Game.Player1Index, "70032", RowPosition.MyRow1, 1);
            gascon.Status.HealthStatus = 5;
            var firstAlly = f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var secondAlly = f.AddCard(f.Game.Player1Index, CardId.Lambert, RowPosition.MyRow1);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1);
            f.FirstPlayer.QueueRows(RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            await gascon.Effect.CardPlayEffect(false, false);

            Assert.Equal(RowPosition.MyRow1, gascon.Status.CardRow);
            var movable = new[] { firstAlly, secondAlly, enemy };
            Assert.Equal(2, movable.Count(card => card.Status.CardRow != RowPosition.MyRow1));
            Assert.Single(movable, card => card.Status.CardRow == RowPosition.MyRow1);
            Assert.Equal(1, gascon.Status.HealthStatus);
            Assert.Equal(2, gascon.CardPoint());
            Assert.False(gascon.IsDead);

            var lowBoostFixture = new HeadlessGameFixture();
            var lowBoostGascon = lowBoostFixture.AddCard(
                lowBoostFixture.Game.Player1Index, "70032", RowPosition.MyRow1, 1);
            lowBoostGascon.Status.HealthStatus = 1;
            var unmoved = lowBoostFixture.AddCard(
                lowBoostFixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            lowBoostFixture.FirstPlayer.QueueRows(RowPosition.MyRow1);
            await lowBoostFixture.SynchronizeClientsAsync();

            await lowBoostGascon.Effect.CardPlayEffect(false, false);

            Assert.Equal(RowPosition.MyRow1, unmoved.Status.CardRow);
            Assert.Equal(1, lowBoostGascon.Status.HealthStatus);
        }

        [Fact]
        public async Task GasconInHandOrDeckBoostsOnlyForOwnTurnBronzeOrSilverUnitMoves()
        {
            var f = new HeadlessGameFixture();
            var gascon = f.AddCard(f.Game.Player1Index, "70032", RowPosition.MyHand);
            var copperUnit = f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            copperUnit.Status.Group = Group.Copper;
            var silverUnit = f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1);
            silverUnit.Status.Group = Group.Silver;
            var goldUnit = f.AddCard(f.Game.Player1Index, CardId.Lambert, RowPosition.MyRow1);
            goldUnit.Status.Group = Group.Gold;
            await f.SynchronizeClientsAsync();

            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.Game.SendEvent(new AfterCardMove(copperUnit, goldUnit));
            await f.Game.SendEvent(new AfterCardMove(silverUnit, goldUnit));
            await f.Game.SendEvent(new AfterCardMove(goldUnit, copperUnit));
            await f.Game.SendEvent(new AfterCardMove(gascon, copperUnit));
            Assert.Equal(2, gascon.Status.HealthStatus);

            f.Game.GameRound = (TwoPlayer)f.Game.Player2Index;
            await f.Game.SendEvent(new AfterCardMove(copperUnit, goldUnit));
            Assert.Equal(2, gascon.Status.HealthStatus);
        }

        [Fact]
        public async Task CrowClanDruidCreatesAtTheRightThenRepeatsOnlyWithACrowOnItsOwnTurnEnd()
        {
            var f = new HeadlessGameFixture();
            var druid = f.AddCard(f.Game.Player1Index, CardId.CrowClanDruid, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await druid.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));
            Assert.Equal(new[] { CardId.CrowClanDruid, CardId.Crow },
                f.Game.PlayersPlace[f.Game.Player1Index][1].Select(card => card.Status.CardId));

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player2Index));
            Assert.Equal(2, f.Game.PlayersPlace[f.Game.Player1Index][1].Count);
            Assert.Equal(0, druid.Status.HealthStatus);

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(3, f.Game.PlayersPlace[f.Game.Player1Index][1].Count);
            Assert.Equal(-1, druid.Status.HealthStatus);

            var crows = f.Game.PlayersPlace[f.Game.Player1Index][1]
                .Where(card => card.Status.CardId == CardId.Crow)
                .ToList();
            foreach (var crow in crows)
            {
                await crow.Effect.ToCemetery();
            }

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Single(f.Game.PlayersPlace[f.Game.Player1Index][1]);
            Assert.Equal(-1, druid.Status.HealthStatus);
        }

        [Fact]
        public async Task CrowmotherCountsDestroyedCrowsAcrossTheMatchButNotWhileLockedThenGeneratesThatMany()
        {
            var f = new HeadlessGameFixture();
            var mother = f.AddCard(f.Game.Player1Index, CardId.Crowmother, RowPosition.MyDeck);
            var ownCrow = f.AddCard(f.Game.Player1Index, CardId.Crow, RowPosition.MyRow1);
            var enemyCrow = f.AddCard(f.Game.Player2Index, CardId.Crow, RowPosition.MyRow1);
            var source = f.AddCard(f.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2);
            await f.SynchronizeClientsAsync();

            await ownCrow.Effect.Damage(3, source);
            await enemyCrow.Effect.Damage(3, source);
            Assert.Equal(2, mother.Status.Countdown);
            Assert.True(mother.Status.IsCountdown);

            mother.Status.IsLock = true;
            var lockedCrow = f.AddCard(f.Game.Player1Index, CardId.Crow, RowPosition.MyRow3);
            await f.SynchronizeClientsAsync();
            await lockedCrow.Effect.Damage(3, source);
            Assert.Equal(2, mother.Status.Countdown);
            mother.Status.IsLock = false;

            await mother.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));
            Assert.Single(f.Game.PlayersPlace[f.Game.Player1Index][0],
                card => card.Status.CardId == CardId.Crow);
            Assert.Single(f.Game.PlayersPlace[f.Game.Player1Index][2],
                card => card.Status.CardId == CardId.Crow);
            Assert.Equal(2, f.Game.PlayersPlace[f.Game.Player1Index][1]
                .Count(card => card.Status.CardId == CardId.Crow));
        }

        [Fact]
        public async Task AxelThreeEyesOffersCrowsOrAddsTwoCrowEyesToDeckAndPlaysTheTopCopy()
        {
            var crowFixture = new HeadlessGameFixture();
            var crowAxel = crowFixture.AddCard(
                crowFixture.Game.Player1Index, CardId.AxelThreeEyes, RowPosition.MyHand);
            crowFixture.FirstPlayer.QueueMenuOptionKeys("AxelThreeEyes_1_SpawnCrows");
            await crowFixture.SynchronizeClientsAsync();

            await crowAxel.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.All(crowFixture.Game.PlayersPlace[crowFixture.Game.Player1Index], row =>
                Assert.Single(row, card => card.Status.CardId == CardId.Crow));

            var eyeFixture = new HeadlessGameFixture();
            var eyeAxel = eyeFixture.AddCard(
                eyeFixture.Game.Player1Index, CardId.AxelThreeEyes, RowPosition.MyHand);
            var enemy = eyeFixture.AddCard(
                eyeFixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            eyeFixture.FirstPlayer.QueueMenuOptionKeys("AxelThreeEyes_2_CreateCrowEyes");
            await eyeFixture.SynchronizeClientsAsync();

            await eyeAxel.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Single(eyeFixture.Game.PlayersDeck[eyeFixture.Game.Player1Index],
                card => card.Status.CardId == CardId.CrowSEye);
            Assert.Single(eyeFixture.Game.PlayersCemetery[eyeFixture.Game.Player1Index],
                card => card.Status.CardId == CardId.CrowSEye);
            Assert.Equal(-4, enemy.Status.HealthStatus);
            Assert.DoesNotContain(eyeFixture.Game.GetPlaceCards(eyeFixture.Game.Player1Index),
                card => card.Status.CardId == CardId.Crow);
        }
    }
}
