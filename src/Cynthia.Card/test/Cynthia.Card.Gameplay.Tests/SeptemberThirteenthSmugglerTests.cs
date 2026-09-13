using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberThirteenthSmugglerTests
    {
        [Fact]
        public async Task SmugglerResurrectsTheSelectedOwnBronzeUnitIntoDeckWithoutPlayingIt()
        {
            var f = new HeadlessGameFixture();
            var smuggler = f.AddCard(f.Game.Player1Index, CardId.DimunSmuggler, RowPosition.MyHand);
            var otherBronze = f.AddCard(
                f.Game.Player1Index, CardId.AnCraiteMarauder, RowPosition.MyCemetery);
            var target = f.AddCard(
                f.Game.Player1Index, CardId.TuirseachSkirmisher, RowPosition.MyCemetery);
            target.Status.IsLock = true;
            var originalStrength = target.Status.Strength;
            var observer = new ResurrectionObserver(target);
            target.Effects.Add(observer);
            var special = f.AddCard(f.Game.Player1Index, CardId.BitingFrost, RowPosition.MyCemetery);
            var silver = f.AddCard(f.Game.Player1Index, CardId.Sigrdrifa, RowPosition.MyCemetery);
            var enemyBronze = f.AddCard(
                f.Game.Player2Index, CardId.TuirseachSkirmisher, RowPosition.MyCemetery);
            var ownCerys = f.AddCard(f.Game.Player1Index, CardId.Cerys, RowPosition.MyCemetery);
            var enemyCerys = f.AddCard(f.Game.Player2Index, CardId.Cerys, RowPosition.MyCemetery);
            ownCerys.Status.Countdown = enemyCerys.Status.Countdown = 2;
            f.FirstPlayer.QueueMenuCardIds(CardId.TuirseachSkirmisher);
            await f.SynchronizeClientsAsync();

            await smuggler.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(RowPosition.MyDeck, target.Status.CardRow);
            Assert.Contains(target, f.Game.PlayersDeck[f.Game.Player1Index]);
            Assert.DoesNotContain(target, f.Game.PlayersCemetery[f.Game.Player1Index]);
            Assert.False(target.Status.IsLock);
            Assert.Equal(originalStrength + 3, target.Status.Strength);
            Assert.Equal(1, ownCerys.Status.Countdown);
            Assert.Equal(2, enemyCerys.Status.Countdown);
            Assert.Equal(1, observer.Resurrections);
            Assert.Equal(RowPosition.MyDeck, observer.RowOnResurrection);
            Assert.False(observer.LockedOnResurrection);
            Assert.Equal(0, observer.Deployments);
            Assert.Equal(0, observer.Landings);
            Assert.DoesNotContain(f.Game.HistoryList, entry => entry.Item2 == target);
            Assert.All(new[] { otherBronze, special, silver, enemyBronze, ownCerys, enemyCerys },
                card => Assert.Equal(RowPosition.MyCemetery, card.Status.CardRow));
            var menu = Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.Equal(new[] { CardId.AnCraiteMarauder, CardId.TuirseachSkirmisher },
                menu.SelectList.Select(card => card.CardId));
            Assert.Empty(f.Game.PlayersStay[f.Game.Player1Index]);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SmugglerResolvesWithoutSelectionWhenNoOwnBronzeUnitsAreInCemetery(
            bool addIneligibleCards)
        {
            var f = new HeadlessGameFixture();
            var smuggler = f.AddCard(f.Game.Player1Index, CardId.DimunSmuggler, RowPosition.MyHand);
            var ineligible = new List<GameCard>();
            if (addIneligibleCards)
            {
                ineligible.Add(f.AddCard(
                    f.Game.Player1Index, CardId.BitingFrost, RowPosition.MyCemetery));
                ineligible.Add(f.AddCard(
                    f.Game.Player1Index, CardId.Sigrdrifa, RowPosition.MyCemetery));
                ineligible.Add(f.AddCard(
                    f.Game.Player2Index, CardId.TuirseachSkirmisher, RowPosition.MyCemetery));
            }
            var observer = new ResurrectionObserver(smuggler);
            smuggler.Effects.Add(observer);
            var originalDeckCount = f.Game.PlayersDeck[f.Game.Player1Index].Count;
            await f.SynchronizeClientsAsync();

            await smuggler.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(smuggler.Status.CardRow.IsOnPlace());
            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.Equal(originalDeckCount, f.Game.PlayersDeck[f.Game.Player1Index].Count);
            Assert.All(ineligible, card => Assert.Equal(RowPosition.MyCemetery, card.Status.CardRow));
            Assert.Equal(0, observer.Resurrections);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task DecliningSmugglerSelectionLeavesTheUnitLockedInCemetery()
        {
            var f = new HeadlessGameFixture();
            var smuggler = f.AddCard(f.Game.Player1Index, CardId.DimunSmuggler, RowPosition.MyHand);
            var target = f.AddCard(
                f.Game.Player1Index, CardId.TuirseachSkirmisher, RowPosition.MyCemetery);
            target.Status.IsLock = true;
            var originalStrength = target.Status.Strength;
            var cerys = f.AddCard(f.Game.Player1Index, CardId.Cerys, RowPosition.MyCemetery);
            cerys.Status.Countdown = 2;
            f.FirstPlayer.MenuSelectionOverride = _ => new List<int>();
            await f.SynchronizeClientsAsync();

            await smuggler.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(RowPosition.MyCemetery, target.Status.CardRow);
            Assert.True(target.Status.IsLock);
            Assert.Equal(originalStrength, target.Status.Strength);
            Assert.Equal(2, cerys.Status.Countdown);
            Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.DoesNotContain(target, f.Game.PlayersDeck[f.Game.Player1Index]);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        private sealed class ResurrectionObserver : CardEffect,
            IHandlesEvent<AfterCardResurrect>, IHandlesEvent<AfterUnitDown>
        {
            public ResurrectionObserver(GameCard card) : base(card) { }

            public int Resurrections { get; private set; }
            public RowPosition RowOnResurrection { get; private set; }
            public bool LockedOnResurrection { get; private set; }
            public int Deployments { get; private set; }
            public int Landings { get; private set; }

            public override Task<int> CardPlayEffect(bool isSpying, bool isReveal)
            {
                Deployments++;
                return Task.FromResult(0);
            }

            public Task HandleEvent(AfterCardResurrect @event)
            {
                if (@event.Target == Card)
                {
                    Resurrections++;
                    RowOnResurrection = Card.Status.CardRow;
                    LockedOnResurrection = Card.Status.IsLock;
                }
                return Task.CompletedTask;
            }

            public Task HandleEvent(AfterUnitDown @event)
            {
                if (@event.Target == Card) Landings++;
                return Task.CompletedTask;
            }
        }
    }
}
