using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberFourteenthIrisSupportTests
    {
        [Fact]
        public async Task IrisShadeCreatesOneCompanionForEachPlayerOnItsFirstTwoOwnerTurnEnds()
        {
            var f = new HeadlessGameFixture();
            var shade = f.AddCard(f.Game.Player1Index, CardId.IrisShade, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await shade.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(2, shade.Status.Countdown);
            Assert.Empty(f.Game.PlayersHandCard[f.Game.Player1Index]);
            Assert.Empty(f.Game.PlayersHandCard[f.Game.Player2Index]);

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player2Index));
            Assert.Equal(2, shade.Status.Countdown);
            Assert.Empty(f.Game.PlayersHandCard[f.Game.Player1Index]);
            Assert.Empty(f.Game.PlayersHandCard[f.Game.Player2Index]);

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(1, shade.Status.Countdown);
            Assert.Single(f.Game.PlayersHandCard[f.Game.Player1Index],
                card => card.Status.CardId == CardId.IrisCompanions);
            Assert.Single(f.Game.PlayersHandCard[f.Game.Player2Index],
                card => card.Status.CardId == CardId.IrisCompanions);

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(0, shade.Status.Countdown);
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player1Index]
                .Count(card => card.Status.CardId == CardId.IrisCompanions));
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player2Index]
                .Count(card => card.Status.CardId == CardId.IrisCompanions));

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(0, shade.Status.Countdown);
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player1Index].Count);
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player2Index].Count);

            await shade.Effect.ToCemetery();
            await shade.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));
            Assert.Equal(2, shade.Status.Countdown);
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player1Index].Count);
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player2Index].Count);
        }

        [Fact]
        public async Task IrisShadeRechecksTruceWithoutSpendingAnActivationWhenOpponentHasPassed()
        {
            var f = new HeadlessGameFixture();
            var shade = f.AddCard(f.Game.Player1Index, CardId.IrisShade, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await shade.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(1, shade.Status.Countdown);

            f.Game.IsPlayersPass[f.Game.Player2Index] = true;
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(1, shade.Status.Countdown);
            Assert.Single(f.Game.PlayersHandCard[f.Game.Player1Index]);
            Assert.Single(f.Game.PlayersHandCard[f.Game.Player2Index]);

            f.Game.IsPlayersPass[f.Game.Player2Index] = false;
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(0, shade.Status.Countdown);
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player1Index].Count);
            Assert.Equal(2, f.Game.PlayersHandCard[f.Game.Player2Index].Count);
        }

        [Fact]
        public async Task DiscardingAnOpponentCardDoesNotCountAsPassingForIrisShadeTruce()
        {
            var f = new HeadlessGameFixture();
            var shade = f.AddCard(f.Game.Player1Index, CardId.IrisShade, RowPosition.MyHand);
            var opponentCard = f.AddCard(
                f.Game.Player2Index, CardId.Wolf, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();

            await shade.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));
            await f.Game.AddTask(() => opponentCard.Effect.Discard(shade));

            Assert.False(f.Game.IsPlayersPass[f.Game.Player2Index]);
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(1, shade.Status.Countdown);
            Assert.Single(f.Game.PlayersHandCard[f.Game.Player1Index]);
            Assert.Single(f.Game.PlayersHandCard[f.Game.Player2Index]);
        }

        [Theory]
        [InlineData(RowPosition.MyRow1, true)]
        [InlineData(RowPosition.MyHand, false)]
        public async Task LockedOrOffBoardIrisShadeCannotCreateCompanions(
            RowPosition shadeRow, bool locked)
        {
            var f = new HeadlessGameFixture();
            var shade = f.AddCard(f.Game.Player1Index, CardId.IrisShade, shadeRow);
            shade.Status.Countdown = 2;
            shade.Status.IsLock = locked;
            await f.SynchronizeClientsAsync();

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));

            Assert.Equal(2, shade.Status.Countdown);
            Assert.DoesNotContain(f.Game.PlayersHandCard[f.Game.Player1Index],
                card => card.Status.CardId == CardId.IrisCompanions);
            Assert.Empty(f.Game.PlayersHandCard[f.Game.Player2Index]);
        }

        [Fact]
        public async Task IrisCompanionsStillAllowsChosenDiscardAfterShadeExpiresAndOpponentPasses()
        {
            var f = CreateCompanionsScenario(out var companions, out var shade,
                out var chosenDiscard);
            shade.Status.Countdown = 0;
            f.Game.IsPlayersPass[f.Game.Player2Index] = true;
            f.FirstPlayer.QueueMenuCardIds(CardId.Eskel, CardId.GeraltOfRivia);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() =>
                companions.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowPosition.MyCemetery, chosenDiscard.Status.CardRow);
            Assert.Equal(2, f.FirstPlayer.MenuRequests.Count);
        }

        [Theory]
        [InlineData(RowPosition.MyRow2, true)]
        [InlineData(RowPosition.MyHand, false)]
        public async Task LockedOrOffBoardIrisShadeDoesNotGrantChosenDiscard(
            RowPosition shadeRow, bool locked)
        {
            var f = CreateCompanionsScenario(out var companions, out var shade, out _);
            if (shade.Status.CardRow != shadeRow)
            {
                f.Game.RowToList(shade.PlayerIndex, shade.Status.CardRow).Remove(shade);
                shade.Status.CardRow = shadeRow;
                f.Game.RowToList(shade.PlayerIndex, shadeRow).Add(shade);
            }
            shade.Status.IsLock = locked;
            f.FirstPlayer.QueueMenuCardIds(CardId.Eskel);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(() =>
                companions.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Single(f.FirstPlayer.MenuRequests);
        }

        [Fact]
        public async Task NennekeResurrectsThreeEligibleUnitsIntoDeckAndRaisesNativeListeners()
        {
            var f = new HeadlessGameFixture();
            var nenneke = f.AddCard(f.Game.Player1Index, CardId.Nenneke, RowPosition.MyHand);
            var skirmisher = f.AddCard(
                f.Game.Player1Index, CardId.TuirseachSkirmisher, RowPosition.MyCemetery);
            var wolf = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            var sigrdrifa = f.AddCard(
                f.Game.Player1Index, CardId.Sigrdrifa, RowPosition.MyCemetery);
            skirmisher.Status.IsLock = true;
            var skirmisherStrength = skirmisher.Status.Strength;
            var cerys = f.AddCard(f.Game.Player1Index, CardId.Cerys, RowPosition.MyCemetery);
            cerys.Status.Countdown = 4;
            var gold = f.AddCard(
                f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyCemetery);
            var leader = f.AddCard(
                f.Game.Player1Index, CardId.BranTuirseach, RowPosition.MyCemetery);
            var special = f.AddCard(
                f.Game.Player1Index, CardId.BitingFrost, RowPosition.MyCemetery);
            var enemyUnit = f.AddCard(
                f.Game.Player2Index, CardId.TuirseachSkirmisher, RowPosition.MyCemetery);
            f.FirstPlayer.QueueMenuCardIds(
                CardId.TuirseachSkirmisher, CardId.Wolf, CardId.Sigrdrifa);
            await f.SynchronizeClientsAsync();

            await nenneke.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.All(new[] { skirmisher, wolf, sigrdrifa }, card =>
            {
                Assert.Equal(RowPosition.MyDeck, card.Status.CardRow);
                Assert.Contains(card, f.Game.PlayersDeck[f.Game.Player1Index]);
            });
            Assert.False(skirmisher.Status.IsLock);
            Assert.Equal(skirmisherStrength + 3, skirmisher.Status.Strength);
            Assert.Equal(1, cerys.Status.Countdown);
            Assert.All(new[] { cerys, gold, leader, special, enemyUnit },
                card => Assert.Equal(RowPosition.MyCemetery, card.Status.CardRow));
            var menu = Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.Equal(3, menu.SelectCount);
            Assert.Equal(new[] { CardId.TuirseachSkirmisher, CardId.Wolf, CardId.Sigrdrifa },
                menu.SelectList.Select(card => card.CardId));
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task NennekeDoesNothingWhenNoEligibleCemeteryUnitsExist()
        {
            var f = new HeadlessGameFixture();
            var nenneke = f.AddCard(f.Game.Player1Index, CardId.Nenneke, RowPosition.MyHand);
            var gold = f.AddCard(
                f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyCemetery);
            var leader = f.AddCard(
                f.Game.Player1Index, CardId.BranTuirseach, RowPosition.MyCemetery);
            var special = f.AddCard(
                f.Game.Player1Index, CardId.BitingFrost, RowPosition.MyCemetery);
            await f.SynchronizeClientsAsync();

            await nenneke.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.All(new[] { gold, leader, special },
                card => Assert.Equal(RowPosition.MyCemetery, card.Status.CardRow));
        }

        [Fact]
        public async Task CancellingNennekeSelectionLeavesCemeteryUnitsUnchanged()
        {
            var f = new HeadlessGameFixture();
            var nenneke = f.AddCard(f.Game.Player1Index, CardId.Nenneke, RowPosition.MyHand);
            var target = f.AddCard(
                f.Game.Player1Index, CardId.TuirseachSkirmisher, RowPosition.MyCemetery);
            target.Status.IsLock = true;
            var originalStrength = target.Status.Strength;
            var cerys = f.AddCard(f.Game.Player1Index, CardId.Cerys, RowPosition.MyCemetery);
            cerys.Status.Countdown = 4;
            f.FirstPlayer.MenuSelectionOverride = _ => new List<int>();
            await f.SynchronizeClientsAsync();

            await nenneke.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(RowPosition.MyCemetery, target.Status.CardRow);
            Assert.True(target.Status.IsLock);
            Assert.Equal(originalStrength, target.Status.Strength);
            Assert.Equal(4, cerys.Status.Countdown);
            Assert.Single(f.FirstPlayer.MenuRequests);
        }

        [Fact]
        public async Task TreantBoarDealsThreeDamageWithoutRepeatingWhenTargetSurvives()
        {
            var f = new HeadlessGameFixture();
            var boar = f.AddCard(f.Game.Player1Index, CardId.TreantBoar, RowPosition.MyHand);
            var target = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 5);
            await f.SynchronizeClientsAsync();

            await boar.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(-3, target.Status.HealthStatus);
            Assert.True(target.Status.CardRow.IsOnPlace());
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);
        }

        [Fact]
        public async Task TreantBoarRepeatsThreeDamageOnceAfterDestroyingItsFirstTarget()
        {
            var f = new HeadlessGameFixture();
            var boar = f.AddCard(f.Game.Player1Index, CardId.TreantBoar, RowPosition.MyHand);
            var lethalTarget = f.AddCard(
                f.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 3);
            var repeatedTarget = f.AddCard(
                f.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2, 10);
            await f.SynchronizeClientsAsync();

            await boar.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(RowPosition.MyCemetery, lethalTarget.Status.CardRow);
            Assert.Equal(-3, repeatedTarget.Status.HealthStatus);
            Assert.Equal(2, f.FirstPlayer.PlaceSelectionSources.Count);
        }

        private static HeadlessGameFixture CreateCompanionsScenario(
            out GameCard companions, out GameCard shade, out GameCard chosenDiscard)
        {
            var f = new HeadlessGameFixture();
            companions = f.AddCard(
                f.Game.Player1Index, CardId.IrisCompanions, RowPosition.MyRow1);
            shade = f.AddCard(f.Game.Player1Index, CardId.IrisShade, RowPosition.MyRow2);
            f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            chosenDiscard = f.AddCard(
                f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck);
            return f;
        }
    }
}
