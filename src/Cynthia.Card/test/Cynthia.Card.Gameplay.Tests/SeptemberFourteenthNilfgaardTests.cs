using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberFourteenthNilfgaardTests
    {
        [Fact]
        public async Task HunterUsesCeilingHalfAndDealsOnlyTheDeckUnitsActualLoss()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var hunter = f.AddCard(f.Game.Player1Index, CardId.VanMoorleheHunter, RowPosition.MyRow1);
            var deckUnit = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck, 5);
            deckUnit.Status.Armor = 1;
            var boardTarget = f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, 20);
            f.FirstPlayer.QueueMenuCardIds(CardId.Wolf);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await hunter.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(3, deckUnit.CardPoint());
            Assert.Equal(0, deckUnit.Status.Armor);
            Assert.Equal(18, boardTarget.CardPoint());
            Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task HunterFiltersDeckTargetsAndStopsWhenShieldPreventsAllPowerLoss()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var hunter = f.AddCard(f.Game.Player1Index, CardId.VanMoorleheHunter, RowPosition.MyRow1);
            var eligible = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck, 5);
            eligible.Status.IsShield = true;
            var gold = f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            var special = f.AddCard(f.Game.Player1Index, CardId.BitingFrost, RowPosition.MyDeck);
            f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, 20);
            f.FirstPlayer.QueueMenuCardIds(CardId.Wolf);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await hunter.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            var menu = Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.Equal(new[] { CardId.Wolf }, menu.SelectList.Select(card => card.CardId));
            Assert.False(eligible.Status.IsShield);
            Assert.Equal(5, eligible.CardPoint());
            Assert.Empty(f.FirstPlayer.PlaceSelectionSources);
            Assert.Contains(gold, f.Game.PlayersDeck[f.Game.Player1Index]);
            Assert.Contains(special, f.Game.PlayersDeck[f.Game.Player1Index]);
        }

        [Fact]
        public async Task HunterUsesPreCleanupPowerLossWhenTheDamagedDeckUnitDies()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var hunter = f.AddCard(f.Game.Player1Index, CardId.VanMoorleheHunter, RowPosition.MyRow1);
            var deckUnit = f.AddCard(
                f.Game.Player1Index, CardId.AnCraiteMarauder, RowPosition.MyDeck, 1);
            var boardTarget = f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, 20);
            f.FirstPlayer.QueueMenuCardIds(CardId.AnCraiteMarauder);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await hunter.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Contains(deckUnit, f.Game.PlayersCemetery[f.Game.Player1Index]);
            Assert.Equal(19, boardTarget.CardPoint());
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task HunterDoesNothingForAnEmptyOrDeclinedDeckSelection(bool decline)
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var hunter = f.AddCard(f.Game.Player1Index, CardId.VanMoorleheHunter, RowPosition.MyRow1);
            if (decline)
            {
                f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
                f.FirstPlayer.MenuSelectionOverride = _ => new List<int>();
            }
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await hunter.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(decline ? 1 : 0, f.FirstPlayer.MenuRequests.Count);
            Assert.Empty(f.FirstPlayer.PlaceSelectionSources);
            Assert.True(hunter.Status.CardRow.IsOnPlace());
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task PhilippeAcceptsAnyOwnDeckUnitAndUsesActualLossAfterArmor()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var philippe = f.AddCard(f.Game.Player1Index, "70151", RowPosition.MyRow1);
            var deckUnit = f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck, 5);
            deckUnit.Status.Armor = 1;
            var boardTarget = f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, 20);
            f.FirstPlayer.QueueMenuCardIds(CardId.GeraltOfRivia);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await philippe.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(3, deckUnit.CardPoint());
            Assert.Equal(0, deckUnit.Status.Armor);
            Assert.Equal(18, boardTarget.CardPoint());
            Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);
        }

        [Fact]
        public async Task VincentInspectsThreeVisibleEligibleEnemyDeckUnitsWithoutChangingDeckOrder()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player2Index].Clear();
            var vincent = f.AddCard(f.Game.Player1Index, "70150", RowPosition.MyRow1);
            var first = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck, 5);
            var second = f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyDeck, 5);
            var third = f.AddCard(f.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyDeck, 5);
            first.Status.Group = Group.Copper;
            second.Status.Group = Group.Silver;
            third.Status.Group = Group.Copper;
            first.Status.Armor = 3;
            first.Status.IsShield = true;
            var deckOrder = f.Game.PlayersDeck[f.Game.Player2Index].ToList();
            var boardTarget = f.AddCard(f.Game.Player1Index, CardId.Lambert, RowPosition.MyRow2, 20);
            f.FirstPlayer.QueueMenuCardIds(CardId.Wolf);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await vincent.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            var menu = Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.Equal(3, menu.SelectList.Count);
            Assert.Contains(menu.SelectList, card => card.CardId == CardId.Wolf);
            Assert.Equal(deckOrder, f.Game.PlayersDeck[f.Game.Player2Index]);
            Assert.Equal(1, first.CardPoint());
            Assert.Equal(3, first.Status.Armor);
            Assert.True(first.Status.IsShield);
            Assert.Equal(16, boardTarget.CardPoint());
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);
        }

        [Fact]
        public async Task VincentExcludesIntrinsicSpiesAndIneligibleDeckCardsWithoutOpeningSelection()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player2Index].Clear();
            var vincent = f.AddCard(f.Game.Player1Index, "70150", RowPosition.MyRow1);
            var intrinsicSpy = f.AddCard(f.Game.Player2Index, CardId.Cantarella, RowPosition.MyDeck);
            f.AddCard(f.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            f.AddCard(f.Game.Player2Index, CardId.BitingFrost, RowPosition.MyDeck);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await vincent.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.Empty(f.FirstPlayer.PlaceSelectionSources);
            Assert.False(intrinsicSpy.Status.IsSpying);
            Assert.Equal(CardUseInfo.EnemyRow, intrinsicSpy.CardInfo().CardUseInfo);
            Assert.True(intrinsicSpy.Status.CardRow.IsInDeck());
        }

        [Fact]
        public async Task RienceSnapshotsAdjacentUnitsBeforeDestroyingTheEnemyUnit()
        {
            var f = new HeadlessGameFixture();
            var rience = f.AddCard(f.Game.Player1Index, CardId.Rience, RowPosition.MyRow1);
            var concealedLeft = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 6);
            concealedLeft.Status.Conceal = true;
            var target = f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1, 5);
            var right = f.AddCard(f.Game.Player2Index, CardId.Lambert, RowPosition.MyRow1, 6);
            right.Status.IsImmue = true;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await rience.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Contains(target, f.Game.PlayersCemetery[f.Game.Player2Index]);
            Assert.Equal(0, concealedLeft.Status.HealthStatus);
            Assert.Equal(3, right.Status.HealthStatus);
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);
            Assert.False(f.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task RienceRequiresAnEnemyUnitTarget()
        {
            var f = new HeadlessGameFixture();
            var rience = f.AddCard(f.Game.Player1Index, CardId.Rience, RowPosition.MyRow1);
            f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2);
            f.AddCard(f.Game.Player2Index, CardId.BitingFrost, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await rience.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Empty(f.FirstPlayer.PlaceSelectionSources);
            Assert.True(rience.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task RamonDealsFourWhenPlayedAgainstAnEnemyUnit()
        {
            var f = new HeadlessGameFixture();
            var ramon = f.AddCard(f.Game.Player1Index, CardId.RamonTyrconnel, RowPosition.MyHand);
            var target = f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1, 10);
            await f.SynchronizeClientsAsync();

            await ramon.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(6, target.CardPoint());
            Assert.True(ramon.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task RamonRevealsFromHandAtOwnerTurnEndAndRepeatsWithItselfAsSource()
        {
            var f = new HeadlessGameFixture();
            var ramon = f.AddCard(f.Game.Player1Index, CardId.RamonTyrconnel, RowPosition.MyHand);
            var observer = new RevealObserver(ramon);
            ramon.Effects.Add(observer);
            var target = f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1, 10);
            await f.SynchronizeClientsAsync();

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player2Index));
            Assert.False(ramon.Status.IsReveal);
            Assert.Equal(10, target.CardPoint());

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));

            Assert.True(ramon.Status.IsReveal);
            Assert.Equal(6, target.CardPoint());
            Assert.Same(ramon, observer.Source);
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);

            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));

            Assert.Equal(6, target.CardPoint());
            Assert.Single(f.FirstPlayer.PlaceSelectionSources);
        }

        [Fact]
        public async Task RamonIgnoresNullAndEnemyRevealSourcesWithoutOpeningATargetMenu()
        {
            var f = new HeadlessGameFixture();
            var ramon = f.AddCard(f.Game.Player1Index, CardId.RamonTyrconnel, RowPosition.MyHand);
            ramon.Status.IsReveal = true;
            var enemySource = f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1);
            var enemyTarget = f.AddCard(f.Game.Player2Index, CardId.Lambert, RowPosition.MyRow2, 10);
            await f.SynchronizeClientsAsync();

            await f.Game.SendEvent(new AfterCardReveal(ramon, null));
            await f.Game.SendEvent(new AfterCardReveal(ramon, enemySource));

            Assert.Empty(f.FirstPlayer.PlaceSelectionSources);
            Assert.Equal(10, enemyTarget.CardPoint());
        }

        private sealed class RevealObserver : CardEffect, IHandlesEvent<AfterCardReveal>
        {
            public RevealObserver(GameCard card) : base(card) { }

            public GameCard Source { get; private set; }

            public Task HandleEvent(AfterCardReveal @event)
            {
                if (@event.Target == Card)
                {
                    Source = @event.Source;
                }
                return Task.CompletedTask;
            }
        }
    }
}
